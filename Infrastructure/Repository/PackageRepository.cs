using Domain.Cards;
using Domain.Exceptions;
using Npgsql;

namespace Infrastructure.Repository;

public class PackageRepository : PgRepository, IPackageRepository
{
    private ICardRepository _cardRepository;

    public PackageRepository(NpgsqlConnection connection) : base("packages", connection)
    {
        _cardRepository = new CardRepository(connection);
    }

    public Package Save(Package package)
    {
        var savedCards = package.Cards.Select(card => _cardRepository.Save(card)).ToList();
        using var command = CreateCommand(
            $"INSERT INTO {TableName} (id, card_ids) VALUES (@id, @card_ids) RETURNING id, card_ids"
        );
        AddParameter(command, "id", package.Id());
        AddParameter(command, "card_ids", savedCards.Select(card => card.IdValue()).ToArray());

        using var savedPackage = command.ExecuteReader();
        if (!savedPackage.Read())
        {
            throw new InvalidOperationException("Package could not be saved");
        }

        return new Package(savedPackage.GetString(0), savedCards);
    }

    public void Remove(Package package)
    {
        using var command = CreateCommand($"DELETE FROM {TableName} where id = @id");
        AddParameter(command, "id", package.Id());

        command.ExecuteScalar();
    }

    public Package GetNextPackage()
    {
        using var command = CreateCommand($"Select id from {TableName}");

        string id;
        {
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new NotFoundException("Package");
            }

            id = reader.GetString(0);
        }

        return Get(id) ?? throw new InvalidOperationException("No package found");
    }

    public Package? Get(string id)
    {
        List<CardId> cardIds;
        string packageId;
        {
            using var command = CreateCommand($"Select id, card_ids from {TableName} where id = @id");
            AddParameter(command, "id", id);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            packageId = reader.GetString(0);
            cardIds = ((string[])reader.GetValue(1)).Select(x => new CardId(x)).ToList();
        }

        var cards = _cardRepository.Get(cardIds);
        return new Package(packageId, cards);
    }
}
