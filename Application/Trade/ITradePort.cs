using Domain.Cards;
using Domain.Trades;

namespace Application.Trade;

public interface ITradePort
{
    public TradeOffer[] GetAllOpenTradeOffers();

    public void CreateTradeOffer(
        TradeOfferId? tradeOfferId,
        CardId cardToTrade,
        string ownerUsername,
        CardType type,
        int minimumDamage);

    void DeleteTradeOffer(TradeOfferId tradeOfferId, string currentUsername);
    void Trade(TradeOfferId tradeOfferId, CardId cardToOfferId, string acceptingUserUsername);
}
