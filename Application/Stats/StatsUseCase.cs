using Domain.Users;

namespace Application.Stats;

public class StatsUseCase : IStatsPort
{
    private readonly IUserRepository _userRepository;

    public StatsUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public StatsData GetStats(string username)
    {
        return GetStatsDataFromUser(_userRepository.GetOrElseThrow(username));
    }

    public StatsData[] GetScoreboard()
    {
        return _userRepository.GetUsersWithHighestElo(10).Select(GetStatsDataFromUser).ToArray();
    }

    private StatsData GetStatsDataFromUser(Domain.Users.User user)
    {
        var stats = user.Stats;
        return new StatsData(user.DisplayName, stats.Elo, stats.Wins, stats.Losses);
    }
}