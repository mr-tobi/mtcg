namespace Domain.Cards;

public class Package : IAggregateRoot
{
    private UUID _id;
    public List<Card> Cards { get; }

    public Package(string id, List<Card> cards)
    {
        _id = new UUID(id);
        Cards = cards;
    }

    private Package(List<Card> cards)
    {
        _id = new UUID();
        Cards = cards;
    }

    public string Id()
    {
        return _id.Id;
    }

    public static Package WithCards(List<Card> cards)
    {
        return new Package(cards);
    }
}
