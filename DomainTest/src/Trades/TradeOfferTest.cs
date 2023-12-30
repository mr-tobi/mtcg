using Domain.Cards;
using Domain.Trades;

namespace DomainTest.Trades;

public class TradeOfferTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CanBeAcceptedFor_Acceptable()
    {
        var tradeOffer = GetNewTradeOffer(CardType.Monster, 15);

        var result = tradeOffer.CanBeAcceptedFor(GetNewCard("FireDragon", 100));

        Assert.That(result, Is.True);
    }

    [Test]
    public void CanBeAcceptedFor_WrongCardType()
    {
        var tradeOffer = GetNewTradeOffer(CardType.Spell, 15);

        var result = tradeOffer.CanBeAcceptedFor(GetNewCard("FireDragon", 100));

        Assert.That(result, Is.False);
    }

    [Test]
    public void CanBeAcceptedFor_TooLessDamage()
    {
        var tradeOffer = GetNewTradeOffer(CardType.Monster, 5000);

        var result = tradeOffer.CanBeAcceptedFor(GetNewCard("FireDragon", 100));

        Assert.That(result, Is.False);
    }

    private TradeOffer GetNewTradeOffer(CardType cardType, int minimumDamage)
    {
        return new TradeOffer(new TradeOfferId(), "tobi", new CardId(), cardType, minimumDamage);
    }

    private Card GetNewCard(string name, int damage)
    {
        return new Card(new CardId(), name, damage);
    }
}
