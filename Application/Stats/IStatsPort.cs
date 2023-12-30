namespace Application.Stats;

public interface IStatsPort
{
    StatsData GetStats(string username);
    StatsData[] GetScoreboard();
}