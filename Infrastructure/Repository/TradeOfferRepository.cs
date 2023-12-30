using Domain.Cards;
using Domain.Exceptions;
using Domain.Trades;
using Infrastructure.Repository.Exceptions;
using Npgsql;

namespace Infrastructure.Repository;

public class TradeOfferRepository : PgRepository, ITradeOfferRepository
{
    public TradeOfferRepository(NpgsqlConnection connection) : base("trade_offers", connection)
    {
    }

    public TradeOffer[] GetAllOpenOffers()
    {
        var tradeOfferIds = new List<TradeOfferId>();
        {
            using var command = CreateCommand($"Select id from {TableName}");

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                tradeOfferIds.Add(new TradeOfferId(reader.GetString(0)));
            }
        }
        return Get(tradeOfferIds.ToArray());
    }

    public void Save(TradeOffer tradeOffer)
    {
        if (Exists(tradeOffer.Id))
        {
            throw new AlreadyExistsException("Trade offer");
        }

        using var command = CreateCommand($"Insert into {TableName} " +
                                          $"(id, owner, card_to_trade, card_type, minimum_damage) " +
                                          $"VALUES (@id, @owner, @card_to_trade, @card_type, @minimum_damage)"
        );
        AddParameter(command, "id", tradeOffer.Id.Id());
        AddParameter(command, "owner", tradeOffer.OwnerUsername);
        AddParameter(command, "card_to_trade", tradeOffer.CardToTrade.Id());
        AddParameter(command, "card_type", tradeOffer.CardType.ToString());
        AddParameter(command, "minimum_damage", tradeOffer.MinimumDamage);

        command.ExecuteScalar();
    }

    public TradeOffer GetOrElseThrow(TradeOfferId tradeOfferId)
    {
        return Get(new[] { tradeOfferId }).First();
    }

    private TradeOffer[] Get(TradeOfferId[] tradeOfferIds)
    {
        using var command =
            CreateCommand(
                $"Select id, owner, card_to_trade, card_type, minimum_damage " +
                $"from {TableName} " +
                $"where id = ANY(:ids)"
            );
        AddParameter(command, "ids", tradeOfferIds.Select(element => element.Id()).ToArray());

        using var reader = command.ExecuteReader();

        var tradeOffers = new List<TradeOffer>();
        while (reader.Read())
        {
            tradeOffers.Add(new TradeOffer(
                new TradeOfferId(reader.GetString(0)),
                reader.GetString(1),
                new CardId(reader.GetString(2)),
                Enum.Parse<CardType>(reader.GetString(3)),
                reader.GetInt32(4)
            ));
        }

        if (tradeOfferIds.Length != tradeOffers.Count)
        {
            throw new NotFoundException("Trade");
        }

        return tradeOffers.ToArray();
    }

    public void Delete(TradeOfferId tradeOfferId)
    {
        using var command = CreateCommand($"DELETE FROM {TableName} where id = @id");
        AddParameter(command, "id", tradeOfferId.Id());

        command.ExecuteScalar();
    }

    private bool Exists(TradeOfferId id)
    {
        using var command = CreateCommand($"SELECT exists(select 1 from {TableName} where id=@id)");
        AddParameter(command, "id", id.Id());

        return (bool)command.ExecuteScalar();
    }
}
