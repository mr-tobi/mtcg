using System.Text.RegularExpressions;

namespace Domain.Cards;

public class Card : IAggregateRoot
{
    private static readonly Regex ElementRegex = new("(Fire|Water|Normal|Regular)");
    private static readonly Regex ClassRegex = new("(Troll|Goblin|Dragon|Wizard|Ork|Knight|Kraken|Elf|Spell)");

    public CardId Id { get; }
    public string Name { get; }
    public float Damage { get; }
    public CardElement Element { get; }
    public CardType Type { get; }
    public CardClass Class { get; }
    public bool LockedForTrade { get; private set; }

    public Card(CardId id, string name, float damage) : this(id, name, damage, false)
    {
    }

    public Card(CardId id, string name, float damage, bool lockedForTrade)
    {
        Id = id;
        Name = name;
        Damage = damage;
        Element = GetElementFromName(name);
        Type = GetTypeFromName(name);
        Class = GetClassFromName(name);
        LockedForTrade = lockedForTrade;
    }

    public string IdValue()
    {
        return Id.Id();
    }

    public void LockForTrade()
    {
        LockedForTrade = true;
    }

    public void Unlock()
    {
        LockedForTrade = false;
    }

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == typeof(Card) && ((Card)obj).Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    private CardElement GetElementFromName(string name)
    {
        var matches = ElementRegex.Match(name);

        if (!matches.Success)
        {
            return CardElement.Normal;
        }


        var elementValue = matches.Groups.Values.Skip(1).First().Value;
        return elementValue == "Regular" ? CardElement.Normal : Enum.Parse<CardElement>(elementValue);
    }

    private CardType GetTypeFromName(string name)
    {
        return name.Contains("Spell")
            ? CardType.Spell
            : CardType.Monster;
    }

    private CardClass GetClassFromName(string name)
    {
        var matches = ClassRegex.Match(name);

        if (!matches.Success)
        {
            throw new InvalidDataException("Name does not contain a class");
        }

        return Enum.Parse<CardClass>(matches.Groups.Values.Skip(1).First().Value);
    }
}
