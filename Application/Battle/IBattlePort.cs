namespace Application.Battle;

public interface IBattlePort
{
    string[] QueueForBattle(string username);
}