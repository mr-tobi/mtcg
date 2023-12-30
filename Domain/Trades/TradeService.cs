using Domain.Cards;
using Domain.Trades.Exceptions;
using Domain.Users;

namespace Domain.Trades;

public class TradeService : ITradeService
{
    private readonly ITradeOfferRepository _tradeOfferRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;

    public TradeService(
        ITradeOfferRepository tradeOfferRepository,
        ICardRepository cardRepository,
        IUserRepository userRepository)
    {
        _tradeOfferRepository = tradeOfferRepository;
        _cardRepository = cardRepository;
        _userRepository = userRepository;
    }

    public void CreateTrade(
        TradeOfferId? tradeOfferId,
        Card cardToTrade,
        User owner,
        CardType type,
        int minimumDamage)
    {
        if (cardToTrade.LockedForTrade)
        {
            throw new NotTradableException("Card is already in a trade offer");
        }

        if (!owner.OwnsCard(cardToTrade.Id))
        {
            throw new NotTradableException("Card is not owned by user");
        }

        if (owner.HasCardInDeck(cardToTrade.Id))
        {
            throw new NotTradableException("Card is in deck of user");
        }

        var tradeOffer = new TradeOffer(tradeOfferId, owner.Username, cardToTrade.Id, type, minimumDamage);
        cardToTrade.LockForTrade();

        _tradeOfferRepository.Save(tradeOffer);
        _cardRepository.UpdateLockedState(cardToTrade);
    }

    public void Trade(TradeOffer tradeOffer, User acceptingUser, User tradeOfferOwner, Card offeredCard)
    {
        if (tradeOfferOwner.Equals(acceptingUser))
        {
            throw new NotTradableException("Trade cannot be accepted by trade offer owner");
        }

        if (!tradeOffer.CanBeAcceptedFor(offeredCard))
        {
            throw new NotTradableException("Trade cannot be accepted with offered card");
        }

        if (!acceptingUser.OwnsCard(offeredCard.Id))
        {
            throw new NotTradableException("User does not own card");
        }

        if (acceptingUser.HasCardInDeck(offeredCard.Id))
        {
            throw new NotTradableException("Trade cannot be accepted if the offered card is in deck of user");
        }


        var cardOfOfferId = tradeOffer.CardToTrade;
        acceptingUser.RemoveCard(offeredCard.Id);
        acceptingUser.AddCard(cardOfOfferId);
        tradeOfferOwner.RemoveCard(cardOfOfferId);
        tradeOfferOwner.AddCard(offeredCard.Id);

        var cardOfOffer = _cardRepository.GetOrElseThrow(cardOfOfferId);
        cardOfOffer.Unlock();

        _userRepository.Update(tradeOfferOwner);
        _userRepository.Update(acceptingUser);
        _cardRepository.UpdateLockedState(cardOfOffer);
        _tradeOfferRepository.Delete(tradeOffer.Id);
    }

    public void DeleteTrade(TradeOfferId tradeOfferId)
    {
        var tradeOffer = _tradeOfferRepository.GetOrElseThrow(tradeOfferId);
        var offeredCard = _cardRepository.GetOrElseThrow(tradeOffer.CardToTrade);
        offeredCard.Unlock();
        _cardRepository.UpdateLockedState(offeredCard);
        _tradeOfferRepository.Delete(tradeOffer.Id);
    }
}
