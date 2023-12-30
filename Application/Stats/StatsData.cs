namespace Application.Stats;

public class StatsData
{
    public string DisplayName { get; }
    public int Elo { get; }
    public int Wins { get; }
    public int Losses { get; }

    public StatsData(string displayName, int elo, int wins, int losses)
    {
        DisplayName = displayName;
        Elo = elo;
        Wins = wins;
        Losses = losses;
    }
}