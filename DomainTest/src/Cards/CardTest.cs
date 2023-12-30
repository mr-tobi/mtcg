using Domain.Cards;

namespace DomainTest.Cards;

public class CardTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    [TestCase("FireElf", CardElement.Fire)]
    [TestCase("WaterElf", CardElement.Water)]
    [TestCase("NormalElf", CardElement.Normal)]
    public void CardElement_GetElement(string name, CardElement expectedElement)
    {
        var result = new Card(new CardId(), name, 100);

        Assert.That(result.Element, Is.EqualTo(expectedElement));
    }

    [Test]
    public void CardElement_NoElement()
    {
        var result = new Card(new CardId(), "Knight", 100);

        Assert.That(result.Element, Is.EqualTo(CardElement.Normal));
    }

    [Test]
    public void CardElement_GetRegularElementAsNormal()
    {
        var result = new Card(new CardId(), "RegularKnight", 100);

        Assert.That(result.Element, Is.EqualTo(CardElement.Normal));
    }

    [Test]
    public void CardType_GetMonsterType()
    {
        var result = new Card(new CardId(), "WaterTroll", 100);

        Assert.That(result.Type, Is.EqualTo(CardType.Monster));
    }

    [Test]
    public void CardType_GetSpellType()
    {
        var result = new Card(new CardId(), "FireSpell", 100);

        Assert.That(result.Type, Is.EqualTo(CardType.Spell));
    }

    [Test]
    [TestCase("Troll", CardClass.Troll)]
    [TestCase("Goblin", CardClass.Goblin)]
    [TestCase("Dragon", CardClass.Dragon)]
    [TestCase("Wizard", CardClass.Wizard)]
    [TestCase("Ork", CardClass.Ork)]
    [TestCase("Knight", CardClass.Knight)]
    [TestCase("Kraken", CardClass.Kraken)]
    [TestCase("Elf", CardClass.Elf)]
    [TestCase("Spell", CardClass.Spell)]
    public void CardClass_GetClass(string name, CardClass expectedClass)
    {
        var result = new Card(new CardId(), name, 100);

        Assert.That(result.Class, Is.EqualTo(expectedClass));
    }    
    
    [Test]
    public void CardClass_GetClassWithElement()
    {
        var result = new Card(new CardId(), "WaterDragon", 100);

        Assert.That(result.Class, Is.EqualTo(CardClass.Dragon));
    }

    [Test]
    public void CardClass_InvalidClass()
    {
        Assert.Throws<InvalidDataException>(() =>
        {
            new Card(new CardId(), "SomethingElse", 100);
        });
    }   


    [Test]
    public void LockedForTrade_Lock()
    {
        var card = new Card(new CardId(), "Dragon", 100);
        
        card.LockForTrade();
        
        Assert.That(card.LockedForTrade, Is.True);
    }

    [Test]
    public void LockedForTrade_Unlock()
    {
        var card = new Card(new CardId(), "Dragon", 100);
        
        card.Unlock();
        
        Assert.That(card.LockedForTrade, Is.False);

    }
}
