using System.Net;
using Application.HTTPServer;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Trade;
using Domain.Cards;
using Domain.Trades;
using Domain.Trades.Exceptions;
using Infrastructure.Controller.DTO;
using Infrastructure.Repository;
using Infrastructure.Repository.Exceptions;

namespace Infrastructure.Controller;

[Controller("/tradings")]
public class TradeController
{
    private readonly ITradePort _tradePort;

    public TradeController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        _tradePort = new TradeUseCase(
            new TradeOfferRepository(connection),
            new CardRepository(connection),
            new UserRepository(connection)
        );
    }

    [RequestMapping(RequestMethod.GET)]
    public ResultWithRequestMetadata GetAllOpenTrades()
    {
        var openTradOffers = _tradePort
            .GetAllOpenTradeOffers()
            .Select(tradeOffer => new TradeOfferDTO(
                tradeOffer.Id.Id(),
                tradeOffer.CardToTrade.Id(),
                tradeOffer.CardType.ToString(),
                tradeOffer.MinimumDamage)
            )
            .ToArray();
        return new ResultWithRequestMetadata(
            openTradOffers,
            openTradOffers.Length > 0 ? HttpStatusCode.OK : HttpStatusCode.NoContent
        );
    }

    [RequestMapping(RequestMethod.POST)]
    public ResultWithRequestMetadata CreateTradeOffer([RequestBody] TradeOfferDTO tradeOfferDto, CurrentUser currentUser)
    {
        try
        {
            _tradePort.CreateTradeOffer(new TradeOfferId(tradeOfferDto.Id),
                new CardId(tradeOfferDto.CardToTrade),
                currentUser.Username,
                Enum.Parse<CardType>(tradeOfferDto.Type),
                tradeOfferDto.MinimumDamage
            );
            return new ResultWithRequestMetadata(null, HttpStatusCode.Created);
        }
        catch (NotTradableException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.Forbidden);
        }
        catch (AlreadyExistsException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.Conflict);
        }
    }

    [RequestMapping(RequestMethod.DELETE, ":id")]
    public void DeleteTradeOffer([PathVariable("id")] string tradeOfferId, CurrentUser currentUser)
    {
        _tradePort.DeleteTradeOffer(new TradeOfferId(tradeOfferId), currentUser.Username);
    }

    [RequestMapping(RequestMethod.POST, ":id")]
    public void AcceptTradeOffer(
        [PathVariable("id")] string tradeOfferId,
        [RequestBody] string cardToTradeId,
        CurrentUser currentUser)
    {
        try
        {
            _tradePort.Trade(new TradeOfferId(tradeOfferId), new CardId(cardToTradeId), currentUser.Username);
        }
        catch (NotTradableException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.Forbidden);
        }
    }
}
