using Domain.Cards;

namespace Domain.Trades;

public class TradeOfferId
{
    private readonly UUID _id;

    public TradeOfferId()
    {
        _id = new UUID();
    }

    public TradeOfferId(string id)
    {
        _id = new UUID(id);
    }

    public string Id()
    {
        return _id.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == typeof(TradeOfferId) && ((TradeOfferId)obj)._id.Equals(_id);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override string ToString()
    {
        return _id.Id;
    }
}
