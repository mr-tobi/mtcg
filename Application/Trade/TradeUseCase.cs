using Application.HTTPServer.Exception;
using Domain.Cards;
using Domain.Trades;
using Domain.Users;

namespace Application.Trade;

public class TradeUseCase : ITradePort
{
    private readonly ITradeOfferRepository _tradeOfferRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;

    private readonly ITradeService _tradeService;

    public TradeUseCase(ITradeOfferRepository tradeOfferRepository, ICardRepository cardRepository,
        IUserRepository userRepository)
    {
        _tradeOfferRepository = tradeOfferRepository;
        _cardRepository = cardRepository;
        _userRepository = userRepository;
        _tradeService = new TradeService(_tradeOfferRepository, _cardRepository, _userRepository);
    }

    public TradeOffer[] GetAllOpenTradeOffers()
    {
        return _tradeOfferRepository.GetAllOpenOffers();
    }


    public void CreateTradeOffer(
        TradeOfferId? tradeOfferId,
        CardId cardToTrade,
        string ownerUsername,
        CardType type,
        int minimumDamage)
    {
        _tradeOfferRepository.ExecuteInTransaction(() =>
        {
            var user = _userRepository.GetOrElseThrow(ownerUsername);
            var card = _cardRepository.GetOrElseThrow(cardToTrade);
            _tradeService.CreateTrade(tradeOfferId, card, user, type, minimumDamage);
        });
    }

    public void DeleteTradeOffer(TradeOfferId tradeOfferId, string currentUsername)
    {
        var tradeOffer = _tradeOfferRepository.GetOrElseThrow(tradeOfferId);
        if (!tradeOffer.OwnerUsername.Equals(currentUsername))
        {
            throw new UnauthorizedException();
        }

        _tradeService.DeleteTrade(tradeOfferId);
    }

    public void Trade(TradeOfferId tradeOfferId, CardId cardToOfferId, string acceptingUserUsername)
    {
        _tradeOfferRepository.ExecuteInTransaction(() =>
        {
            var tradeOffer = _tradeOfferRepository.GetOrElseThrow(tradeOfferId);
            var cardToOffer = _cardRepository.GetOrElseThrow(cardToOfferId);
            var acceptingUser = _userRepository.GetOrElseThrow(acceptingUserUsername);
            var tradeOfferOwner = _userRepository.GetOrElseThrow(tradeOffer.OwnerUsername);
            _tradeService.Trade(tradeOffer, acceptingUser, tradeOfferOwner, cardToOffer);
        });
    }
}
