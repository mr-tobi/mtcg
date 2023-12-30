using Domain.Cards;
using Infrastructure.Repository.Exceptions;
using Npgsql;

namespace Infrastructure.Repository;

public class CardRepository : PgRepository, ICardRepository
{
    public CardRepository(NpgsqlConnection connection) : base("cards", connection)
    {
    }

    public Card Save(Card card)
    {
        if (Exists(card.Id))
        {
            throw new AlreadyExistsException("Card");
        }

        using var command = CreateCommand(
            $"INSERT INTO {TableName} (id, name, damage, owner, locked) VALUES (@id, @name, @damage, @owner, @locked) RETURNING id, name, damage, locked"
        );
        AddParameter(command, "id", card.IdValue());
        AddParameter(command, "name", card.Name);
        AddParameter(command, "damage", card.Damage);
        AddParameter(command, "locked", card.LockedForTrade);
        AddParameter(command, "owner", DBNull.Value);

        using var savedCard = command.ExecuteReader();
        if (!savedCard.Read())
        {
            throw new InvalidOperationException("Card could not be saved");
        }

        return new Card(
            new CardId(savedCard.GetString(0)),
            savedCard.GetString(1),
            savedCard.GetFloat(2),
            savedCard.GetBoolean(3)
        );
    }

    public Card GetOrElseThrow(CardId id)
    {
        var cards = Get(new List<CardId> { id });
        if (cards.Count <= 0)
        {
            throw new InvalidOperationException("Card not found");
        }

        return cards.First();
    }

    public List<Card> Get(List<CardId> ids)
    {
        using var command = CreateCommand($"Select id, name, damage, locked from {TableName} where id = ANY(:ids)");
        AddParameter(command, "ids", ids.Select(id => id.Id()).ToArray());

        var cards = new List<Card>();
        using var cardsResult = command.ExecuteReader();

        while (cardsResult.Read())
        {
            cards.Add(new Card(
                new CardId(cardsResult.GetString(0)),
                cardsResult.GetString(1),
                cardsResult.GetFloat(2),
                cardsResult.GetBoolean(3)
            ));
        }

        if (cards.Count != ids.Count)
        {
            throw new InvalidOperationException("Could not fetch all cards");
        }

        return cards;
    }

    public void SetOwner(CardId cardId, string ownerName)
    {
        using var command = CreateCommand($"UPDATE {TableName} SET owner=@owner where id = @id");
        AddParameter(command, "id", cardId.Id());
        AddParameter(command, "owner", ownerName);

        command.ExecuteScalar();
    }

    public List<Card> GetCardsOfOwner(string ownerName)
    {
        var cardIds = new List<CardId>();
        {
            using var command = CreateCommand($"Select id from {TableName} where owner = @owner");
            AddParameter(command, "owner", ownerName);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                cardIds.Add(new CardId(reader.GetString(0)));
            }
        }

        return Get(cardIds);
    }

    public void UpdateLockedState(Card card)
    {
        using var command = CreateCommand($"UPDATE {TableName} SET locked=@locked where id=@id");
        AddParameter(command, "locked", card.LockedForTrade);
        AddParameter(command, "id", card.Id.Id());

        command.ExecuteScalar();
    }

    public void Remove(CardId cardId)
    {
        using var command = CreateCommand($"DELETE FROM {TableName} where id = @id");
        AddParameter(command, "id", cardId.Id());

        command.ExecuteScalar();
    }

    private bool Exists(CardId cardId)
    {
        using var command = CreateCommand($"SELECT exists(select 1 from {TableName} where id=@id)");
        AddParameter(command, "id", cardId.Id());

        return (bool)command.ExecuteScalar();
    }
}
