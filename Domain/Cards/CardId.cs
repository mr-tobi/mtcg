namespace Domain.Cards;

public class CardId
{
    private readonly UUID _id;

    public CardId()
    {
        _id = new UUID();
    }

    public CardId(string id)
    {
        _id = new UUID(id);
    }

    public string Id()
    {
        return _id.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == typeof(CardId) && ((CardId)obj)._id.Equals(_id);
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
