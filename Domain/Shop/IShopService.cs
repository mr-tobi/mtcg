using Domain.Cards;
using Domain.Users;

namespace Domain.Shop;

public interface IShopService
{
    void SellCard(Card card, User seller);
}
