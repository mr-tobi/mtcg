using Application.Battle;
using Application.HTTPServer;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Domain.Battles;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("/battles")]
public class BattleController
{
    private readonly IBattlePort _battlePort;

    public BattleController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        var userRepo = new UserRepository(connection);
        _battlePort = new BattleUseCase(
            new BattleService(
                new CardRepository(connection),
                userRepo,
                new BattleRoundLogBuilder()
            ),
            userRepo
        );
    }

    [RequestMapping(RequestMethod.POST)]
    public string[] QueueForBattle(CurrentUser currentUser)
    {
        return _battlePort.QueueForBattle(currentUser.Username);
    }
}