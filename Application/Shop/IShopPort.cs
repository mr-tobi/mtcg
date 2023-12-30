using Domain.Cards;

namespace Application.Shop;

public interface IShopPort
{
    void SellCard(string username, CardId cardId);
}
