using Domain.Cards;
using Domain.Users;

namespace Domain.Trades;

public interface ITradeService
{
    void CreateTrade(
        TradeOfferId? tradeOfferId,
        Card cardToTrade,
        User owner,
        CardType type,
        int minimumDamage);

    void Trade(TradeOffer tradeOffer, User acceptingUser, User tradeOfferOwner, Card offeredCard);
    void DeleteTrade(TradeOfferId tradeOfferId);
}
