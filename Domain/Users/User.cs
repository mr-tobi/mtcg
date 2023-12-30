using Domain.Cards;
using Domain.Users.Exceptions;

namespace Domain.Users;

public class User : IAggregateRoot
{
    private const int StartCoins = 20;
    public const int DeckSize = 4;

    public string Username { get; }
    public int Coins { get; private set; }
    public List<CardId> Cards { get; private set; }
    public List<CardId> Deck { get; private set; }
    public Statistic Stats { get; private set; }
    public string DisplayName { get; private set; }
    public string? Bio { get; private set; }
    public string? Image { get; private set; }


    public User(string username) : this(
        username,
        StartCoins,
        new List<CardId>(),
        new List<CardId>(),
        new Statistic(),
        username,
        null,
        null
    )
    {
    }

    public User(
        string username,
        int coins,
        List<CardId> cards,
        List<CardId> deck,
        Statistic stats,
        string displayName,
        string? bio,
        string? image
    )
    {
        Username = username;
        Coins = coins;
        Cards = cards;
        Deck = deck;
        Stats = stats;
        DisplayName = displayName;
        Bio = bio;
        Image = image;
    }

    public void AcquireCards(Package package)
    {
        Cards.AddRange(package.Cards.Select(card => card.Id).ToList());
    }

    public void AddCard(CardId cardId)
    {
        Cards.Add(cardId);
    }

    public void RemoveCard(CardId cardId)
    {
        Cards.Remove(cardId);
    }

    public void AddCoins(int coins)
    {
        if (coins < 1)
        {
            return;
        }

        Coins += coins;
    }

    public void RemoveCoins(int coinsToRemove)
    {
        if (coinsToRemove < 1)
        {
            return;
        }

        var newCoins = Coins - coinsToRemove;
        Coins = newCoins > 0 ? newCoins : 0;
    }

    public void SetDeck(List<Card> deck)
    {
        if (deck.Count > DeckSize)
        {
            throw new InvalidOperationException($"Deck cannot have more than {DeckSize} cards");
        }

        if (deck.Any(card => !Cards.Contains(card.Id)))
        {
            throw new MissingOwnershipException("Card");
        }

        if (deck.Any(card => card.LockedForTrade))
        {
            throw new InvalidOperationException("At least one card is locked for trade");
        }

        Deck = deck.Select(card => card.Id).ToList();
    }

    public bool OwnsCard(CardId card)
    {
        return Cards.Contains(card);
    }

    public bool HasCardInDeck(CardId card)
    {
        return Deck.Contains(card);
    }

    public void UpdateUser(UpdatableUserData userData)
    {
        DisplayName = userData.DisplayName;
        Bio = userData.Bio;
        Image = userData.Image;
    }

    public void AddElo(int value)
    {
        Stats.AddElo(value);
    }

    public void SubtractElo(int value)
    {
        Stats.SubtractElo(value);
    }

    public void AddWin()
    {
        Stats.AddWin();
    }

    public void AddLoss()
    {
        Stats.AddLoss();
    }

    public override bool Equals(object? obj)
    {
        return obj?.GetType() == typeof(User) && ((User)obj).Username.Equals(Username);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public class Builder
    {
        private string? _username;
        private int? _coins;
        private List<CardId> _cards = new();
        private List<CardId> _deck = new();
        private Statistic? _stats;
        private string? _displayName;
        private string? _bio;
        private string? _image;

        public Builder Username(string username)
        {
            _username = username;
            return this;
        }

        public Builder Coins(int coins)
        {
            _coins = coins;
            return this;
        }

        public Builder Cards(List<Card> cards)
        {
            _cards = cards.Select(card => card.Id).ToList();
            return this;
        }

        public Builder Deck(List<Card> deck)
        {
            _deck = deck.Select(card => card.Id).ToList();
            return this;
        }

        public Builder Stats(Statistic stats)
        {
            _stats = stats;
            return this;
        }

        public Builder DisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public Builder Bio(string? bio)
        {
            _bio = bio;
            return this;
        }

        public Builder Image(string? image)
        {
            _image = image;
            return this;
        }

        public User Build()
        {
            ArgumentNullException.ThrowIfNull(_username);
            ArgumentNullException.ThrowIfNull(_coins);
            ArgumentNullException.ThrowIfNull(_stats);
            ArgumentNullException.ThrowIfNull(_displayName);

            return new User(_username, (int)_coins, _cards, _deck, _stats, _displayName, _bio, _image);
        }
    }
}
