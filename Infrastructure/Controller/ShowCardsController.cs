using System.Net;
using Application.HTTPServer;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.User;
using Infrastructure.Controller.DTO;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("")]
public class ShowCardsController
{
    private readonly IGetCardsPort _getCardsPort;

    public ShowCardsController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        _getCardsPort = new GetCardsUseCase(new UserRepository(connection), new CardRepository(connection));
    }

    [RequestMapping(RequestMethod.GET, "cards")]
    public ResultWithRequestMetadata GetCardsOfUser(CurrentUser currentUser)
    {
        var cards = _getCardsPort
            .GetCardsOfUser(currentUser.Username)
            .Select(card => new CardDTO(card.IdValue(), card.Name, card.Damage))
            .ToArray();

        return new ResultWithRequestMetadata(cards, cards.Length > 0 ? HttpStatusCode.OK : HttpStatusCode.NoContent);
    }
}
