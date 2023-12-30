using System.Net;
using Application.HTTPServer;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.User;
using Application.User.Exceptions;
using Domain.Cards;
using Domain.Users.Exceptions;
using Infrastructure.Controller.DTO;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("deck")]
public class DeckController
{
    private IDeckPort _deckPort;

    public DeckController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        _deckPort = new DeckUseCase(new UserRepository(connection), new CardRepository(connection));
    }

    [RequestMapping(RequestMethod.GET)]
    public ResultWithRequestMetadata GetDeck(CurrentUser currentUser, [RequestParam(true)] string? format)
    {
        var deck = _deckPort
            .GetDeck(currentUser.Username)
            .Select(card => GetCardInDeckInFormat(card, format))
            .ToArray();

        return new ResultWithRequestMetadata(
            deck,
            deck.Length > 0 ? HttpStatusCode.OK : HttpStatusCode.NoContent,
            GetContentTypeForFormat(format)
        );
    }

    private object GetCardInDeckInFormat(Card card, string? format)
    {
        return format is "plain" ? card.IdValue() : new CardDTO(card.IdValue(), card.Name, card.Damage);
    }

    private string GetContentTypeForFormat(string? format)
    {
        return format is "plain" ? "text/plain" : "application/json";
    }

    [RequestMapping(RequestMethod.PUT)]
    public void SetDeck([RequestBody] string[] cardIds, CurrentUser currentUser)
    {
        try
        {
            _deckPort.SetDeck(currentUser.Username, cardIds);
        }
        catch (DeckSizeException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.BadRequest);
        }
        catch (MissingOwnershipException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.Forbidden);
        }
    }
}
