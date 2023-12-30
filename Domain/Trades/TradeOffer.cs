using Domain.Cards;

namespace Domain.Trades;

public class TradeOffer : IAggregateRoot
{
    public TradeOfferId Id { get; }
    public string OwnerUsername { get; }
    public CardId CardToTrade { get; }
    public CardType CardType { get; }
    public int MinimumDamage { get; }

    public TradeOffer(
        TradeOfferId? id,
        string ownerUsername,
        CardId cardToTrade,
        CardType cardType,
        int minimumDamage)
    {
        Id = id ?? new TradeOfferId();
        OwnerUsername = ownerUsername;
        CardToTrade = cardToTrade;
        CardType = cardType;
        MinimumDamage = minimumDamage;
    }

    public bool CanBeAcceptedFor(Card card)
    {
        return card.Type == CardType && card.Damage >= MinimumDamage;
    }
}
