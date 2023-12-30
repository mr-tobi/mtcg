using Domain.Cards;
using Domain.Users;

namespace Domain.Battles;

public class Battle : IAggregateRoot
{
    private const int MaxRounds = 100;
    private const int MaxDamage = 9999;

    private readonly User _player1;
    private readonly User _player2;
    private readonly ICardRepository _cardRepository;
    private readonly ElementEffectivenessHolder _elementEffectivenessHolder = new();
    private readonly IBattleRoundLogBuilder _battleRoundLogBuilder;

    public Battle(
        User player1,
        User player2,
        ICardRepository cardRepository,
        IBattleRoundLogBuilder battleRoundLogBuilder)
    {
        _player1 = player1;
        _player2 = player2;
        _cardRepository = cardRepository;
        _battleRoundLogBuilder = battleRoundLogBuilder;
    }

    public BattleResult Start()
    {
        var logs = new List<string>();
        var deckPlayer1 = GetCardsFromDeck(_player1);
        var deckPlayer2 = GetCardsFromDeck(_player2);
        User? winner = null;

        for (var i = 1; i < MaxRounds; i++)
        {
            if (IsBattleOver(deckPlayer1, deckPlayer2))
            {
                winner = GetWinner(deckPlayer1, deckPlayer2);
                break;
            }

            var cardPlayer1 = GetRandomCardFromDeck(deckPlayer1);
            var cardPlayer2 = GetRandomCardFromDeck(deckPlayer2);
            _battleRoundLogBuilder.Append(
                $"PlayerA: {cardPlayer1.Name} ({cardPlayer1.Damage} Damage) vs PlayerB: {cardPlayer2.Name} ({cardPlayer2.Damage} Damage)"
            );

            var roundWinner = ExecuteRound(cardPlayer1, cardPlayer2);
            logs.Add(_battleRoundLogBuilder.Build());

            if (roundWinner == null)
            {
                continue;
            }

            if (roundWinner.Equals(_player1))
            {
                TakeOverCard(_player1, _player2, deckPlayer2, cardPlayer2);
            }
            else
            {
                TakeOverCard(_player2, _player1, deckPlayer1, cardPlayer1);
            }
        }

        WritePostBattleLogs(winner, logs);
        _player1.SetDeck(deckPlayer1);
        _player2.SetDeck(deckPlayer2);
        return new BattleResult(winner, logs);
    }

    private User? ExecuteRound(Card cardPlayer1, Card cardPlayer2)
    {
        var (player1CardDamage, player2CardDamage) = GetDamageOfCards(cardPlayer1, cardPlayer2);
        if (player1CardDamage > player2CardDamage)
        {
            AppendRoundWinnerLog(cardPlayer1, cardPlayer2);
            return _player1;
        }

        if (player1CardDamage < player2CardDamage)
        {
            AppendRoundWinnerLog(cardPlayer2, cardPlayer1);
            return _player2;
        }

        _battleRoundLogBuilder.Append("Draw");
        return null;
    }

    private void AppendRoundWinnerLog(Card winnerCard, Card loserCard)
    {
        if (winnerCard.Type == CardType.Monster && loserCard.Type == CardType.Monster)
        {
            _battleRoundLogBuilder.Append($"{winnerCard.Name} defeats {loserCard.Name}");
        }

        _battleRoundLogBuilder.Append($"{winnerCard.Name} wins");
    }

    private Tuple<float, float> GetDamageOfCards(Card cardPlayer1, Card cardPlayer2)
    {
        var specialDamage = CheckCardSpecialties(cardPlayer1, cardPlayer2);
        if (specialDamage != null)
        {
            return specialDamage;
        }

        if (cardPlayer1.Type == CardType.Monster && cardPlayer2.Type == CardType.Monster)
        {
            return new Tuple<float, float>(cardPlayer1.Damage, cardPlayer2.Damage);
        }


        var damagePlayer1 = cardPlayer1.Damage * GetDamageMultiplier(cardPlayer1, cardPlayer2);
        var damagePlayer2 = cardPlayer2.Damage * GetDamageMultiplier(cardPlayer2, cardPlayer1);
        _battleRoundLogBuilder.Append(
            $"{cardPlayer1.Damage} vs {cardPlayer2.Damage} -> {damagePlayer1} vs {damagePlayer2}");
        return new Tuple<float, float>(damagePlayer1, damagePlayer2);
    }

    private Tuple<float, float>? CheckCardSpecialties(Card cardPlayer1, Card cardPlayer2)
    {
        if (AreCardsClassPair(cardPlayer1, cardPlayer2, CardClass.Goblin, CardClass.Dragon))
        {
            _battleRoundLogBuilder.Append(
                $"{GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Goblin)} is too afraid to attack {GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Dragon)}"
            );
            return cardPlayer1.Class == CardClass.Goblin
                ? new Tuple<float, float>(0, MaxDamage)
                : new Tuple<float, float>(MaxDamage, 0);
        }

        if (AreCardsClassPair(cardPlayer1, cardPlayer2, CardClass.Ork, CardClass.Wizard))
        {
            _battleRoundLogBuilder.Append(
                $"{GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Wizard)} controls {GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Ork)}"
            );
            return cardPlayer1.Class == CardClass.Ork
                ? new Tuple<float, float>(0, MaxDamage)
                : new Tuple<float, float>(MaxDamage, 0);
        }

        if (AreCardsClassPair(cardPlayer1, cardPlayer2, CardClass.Knight, CardClass.Spell) &&
            IsOneCardClassElementPair(cardPlayer1, cardPlayer2, CardClass.Spell, CardElement.Water))
        {
            _battleRoundLogBuilder.Append(
                $"{GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Spell)} drowned {GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Knight)}"
            );
            return cardPlayer1.Class == CardClass.Knight
                ? new Tuple<float, float>(0, MaxDamage)
                : new Tuple<float, float>(MaxDamage, 0);
        }

        if (AreCardsClassPair(cardPlayer1, cardPlayer2, CardClass.Spell, CardClass.Kraken))
        {
            _battleRoundLogBuilder.Append(
                $"{GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Kraken)} is immune against {GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Spell)}"
            );
            return cardPlayer1.Class == CardClass.Spell
                ? new Tuple<float, float>(0, MaxDamage)
                : new Tuple<float, float>(MaxDamage, 0);
        }

        if (AreCardsClassPair(cardPlayer1, cardPlayer2, CardClass.Dragon, CardClass.Elf) &&
            IsOneCardClassElementPair(cardPlayer1, cardPlayer2, CardClass.Elf, CardElement.Fire))
        {
            _battleRoundLogBuilder.Append(
                $"{GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Elf)} evades every attack from {GetCardNameOfClass(cardPlayer1, cardPlayer2, CardClass.Dragon)}"
            );
            return cardPlayer1.Class == CardClass.Dragon
                ? new Tuple<float, float>(0, MaxDamage)
                : new Tuple<float, float>(MaxDamage, 0);
        }

        return null;
    }

    private bool AreCardsClassPair(Card card1, Card card2, CardClass class1, CardClass class2)
    {
        return (card1.Class == class1 && card2.Class == class2) || (card2.Class == class1 && card1.Class == class2);
    }

    private string GetCardNameOfClass(Card card1, Card card2, CardClass cardClass)
    {
        if (card1.Class == cardClass)
        {
            return card1.Name;
        }

        if (card2.Class == cardClass)
        {
            return card2.Name;
        }

        throw new InvalidDataException("No card has provided class");
    }

    private bool IsOneCardClassElementPair(Card card1, Card card2, CardClass cardClass, CardElement element)
    {
        return (card1.Class == cardClass && card1.Element == element) ||
               (card2.Class == cardClass && card2.Element == element);
    }

    private float GetDamageMultiplier(Card attackingCard, Card defendingCard)
    {
        return _elementEffectivenessHolder.GetDamageMultiplier(attackingCard.Element, defendingCard.Element);
    }

    private List<Card> GetCardsFromDeck(User player)
    {
        return _cardRepository.Get(player.Deck);
    }

    private Card GetRandomCardFromDeck(List<Card> deck)
    {
        return deck[new Random().Next(deck.Count)];
    }

    private void TakeOverCard(User winningPlayer, User losingPlayer, List<Card> losingPlayerDeck, Card card)
    {
        winningPlayer.AddCard(card.Id);
        losingPlayer.RemoveCard(card.Id);
        losingPlayerDeck.Remove(card);
    }

    private bool IsBattleOver(List<Card> deckPlayer1, List<Card> deckPlayer2)
    {
        return DidPlayerLose(deckPlayer1) || DidPlayerLose(deckPlayer2);
    }

    private User? GetWinner(List<Card> deckPlayer1, List<Card> deckPlayer2)
    {
        if (DidPlayerLose(deckPlayer1))
        {
            return _player2;
        }

        if (DidPlayerLose(deckPlayer2))
        {
            return _player1;
        }

        return null;
    }

    private bool DidPlayerLose(List<Card> deck)
    {
        return deck.Count <= 0;
    }

    private void WritePostBattleLogs(User? winner, List<string> logs)
    {
        if (winner == null)
        {
            logs.Add("Draw");
            return;
        }

        logs.Add($"Player {(winner.Equals(_player1) ? "1" : "2")} won");
    }

    public class BattleResult
    {
        public User? Winner { get; }
        public List<string> Logs { get; }

        public BattleResult(User? winner, List<string> logs)
        {
            Winner = winner;
            Logs = logs;
        }
    }
}
