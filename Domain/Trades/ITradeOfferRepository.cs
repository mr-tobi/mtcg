using Domain.Repositories;

namespace Domain.Trades;

public interface ITradeOfferRepository : IRepository<TradeOffer>
{
    TradeOffer[] GetAllOpenOffers();
    void Save(TradeOffer tradeOffer);
    TradeOffer GetOrElseThrow(TradeOfferId tradeOfferId);
    void Delete(TradeOfferId tradeOfferId);
}
