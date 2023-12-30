using Application.HTTPServer;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Stats;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("")]
public class StatsController
{
    private readonly IStatsPort _statsPort;

    public StatsController()
    {
        _statsPort = new StatsUseCase(new UserRepository(PgRepository.CreateAndOpenConnection()));
    }

    [RequestMapping(RequestMethod.GET, "/stats")]
    public StatsData GetStats(CurrentUser currentUser)
    {
        return _statsPort.GetStats(currentUser.Username);
    }

    [RequestMapping(RequestMethod.GET, "/scoreboard")]
    public StatsData[] GetScoreboard()
    {
        return _statsPort.GetScoreboard();
    }
}