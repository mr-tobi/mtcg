namespace Domain.Users;

public class Statistic
{
    private const int StartElo = 100;

    public int Elo { get; private set; }
    public int Wins { get; private set; }
    public int Losses { get; private set; }

    public Statistic()
    {
        Elo = StartElo;
        Wins = 0;
        Losses = 0;
    }

    public Statistic(int elo, int wins, int losses)
    {
        Elo = elo;
        Wins = wins;
        Losses = losses;
    }

    public void AddElo(int value)
    {
        if (value < 0)
        {
            return;
        }

        Elo += value;
    }

    public void SubtractElo(int value)
    {
        if (value < 0)
        {
            return;
        }

        Elo -= value;
    }

    public void AddWin()
    {
        Wins += 1;
    }

    public void AddLoss()
    {
        Losses += 1;
    }
}