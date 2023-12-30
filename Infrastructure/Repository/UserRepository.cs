using Domain.Cards;
using Domain.Exceptions;
using Domain.Users;
using Infrastructure.Repository.Interface;
using Npgsql;

namespace Infrastructure.Repository;

public class UserRepository : PgRepository, IUserRepository
{
    private ICardRepository _cardRepository;
    private IStatsRepository _statsRepository;

    public UserRepository(NpgsqlConnection connection) : base("users", connection)
    {
        _cardRepository = new CardRepository(connection);
        _statsRepository = new StatsRepository(connection);
    }

    public void Update(User user)
    {
        using var command = CreateCommand(
            $"UPDATE {TableName} SET coins=@coins, deck=@deck, display_name=@display_name, bio=@bio, image=@image where username=@username"
        );
        AddParameter(command, "username", user.Username);
        AddParameter(command, "coins", user.Coins);
        AddParameter(command, "deck", user.Deck.Select(card => card.Id()).ToArray());
        AddParameter(command, "display_name", user.DisplayName);
        AddParameter(command, "bio", DatabaseUtil.GetValueOrDBNull(user.Bio));
        AddParameter(command, "image", DatabaseUtil.GetValueOrDBNull(user.Image));
        command.ExecuteScalar();

        _statsRepository.Update(user);

        foreach (var card in user.Cards)
        {
            _cardRepository.SetOwner(card, user.Username);
        }
    }

    public User GetOrElseThrow(string username)
    {
        return Get(username) ?? throw new NotFoundException("User");
    }

    public User? Get(string username)
    {
        var userBuilder = new User.Builder();
        List<CardId> deckCardIds;
        {
            using var command = CreateCommand(
                $"Select username, coins, deck, display_name, bio, image from {TableName} where username = @username"
            );
            AddParameter(command, "username", username);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            var bio = reader.GetValue(4);
            var image = reader.GetValue(5);
            userBuilder
                .Username(reader.GetString(0))
                .Coins(reader.GetInt32(1))
                .DisplayName(reader.GetString(3))
                .Bio(bio is DBNull ? null : (string)bio)
                .Image(image is DBNull ? null : (string)image);

            deckCardIds = ((string[])reader.GetValue(2)).Select(id => new CardId(id)).ToList();
        }
        userBuilder
            .Cards(_cardRepository.GetCardsOfOwner(username))
            .Deck(_cardRepository.Get(deckCardIds))
            .Stats(_statsRepository.GetOrElseThrow(username));

        return userBuilder.Build();
    }

    public User[] GetUsersWithHighestElo(int count)
    {
        var usernames = new List<string>();
        {
            using var command = CreateCommand(
                "Select users.username " +
                "from users " +
                "inner join stats on (users.username = stats.username) " +
                "order by elo desc " +
                "fetch first @rows rows only"
            );
            AddParameter(command, "rows", count);

            using var reader = command.ExecuteReader();

            for (var i = 0; i < count && reader.Read(); i++)
            {
                usernames.Add(reader.GetString(0));
            }
        }

        return usernames.Select(GetOrElseThrow).ToArray();
    }
}
