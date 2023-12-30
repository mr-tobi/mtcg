using Domain.Cards;
using Domain.Users;

namespace Domain.Battles;

public class BattleService : IBattleService
{
    private const int WinningElo = 5;
    private const int LosingElo = 3;

    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBattleRoundLogBuilder _battleRoundLogBuilder;

    public BattleService(ICardRepository cardRepository,
        IUserRepository userRepository,
        IBattleRoundLogBuilder battleRoundLogBuilder)
    {
        _cardRepository = cardRepository;
        _userRepository = userRepository;
        _battleRoundLogBuilder = battleRoundLogBuilder;
    }

    public List<string> StartBattle(User player1, User player2)
    {
        if (player1.Equals(player2))
        {
            throw new InvalidOperationException("User cannot battle against himself");
        }

        if (player1.Deck.Count != User.DeckSize || player2.Deck.Count != User.DeckSize)
        {
            throw new InvalidOperationException("Deck is not full for both players");
        }
        
        var battle = new Battle(player1, player2, _cardRepository, _battleRoundLogBuilder);
        var battleResult = battle.Start();

        if (battleResult.Winner != null)
        {
            var loser = battleResult.Winner.Equals(player1) ? player2 : player1;
            UpdateStats(battleResult.Winner, loser);
        }

        _userRepository.Update(player1);
        _userRepository.Update(player2);

        return battleResult.Logs;
    }

    private void UpdateStats(User winner, User loser)
    {
        winner.AddWin();
        loser.AddLoss();
        UpdateElo(winner, loser);
    }

    private void UpdateElo(User winner, User loser)
    {
        winner.AddElo(WinningElo);
        loser.SubtractElo(LosingElo);
    }
}
