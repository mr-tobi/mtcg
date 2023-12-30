using Domain.Users;

namespace Domain.Battles;

public interface IBattleService
{
    List<string> StartBattle(User player1, User player2);
}