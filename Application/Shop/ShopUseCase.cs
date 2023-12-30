using Domain.Cards;
using Domain.Shop;
using Domain.Users;

namespace Application.Shop;

public class ShopUseCase : IShopPort
{
    private readonly IShopService _shopService;
    private readonly IUserRepository _userRepository;
    private readonly ICardRepository _cardRepository;

    public ShopUseCase(IShopService shopService, IUserRepository userRepository, ICardRepository cardRepository)
    {
        _shopService = shopService;
        _userRepository = userRepository;
        _cardRepository = cardRepository;
    }

    public void SellCard(string username, CardId cardId)
    {
        _userRepository.ExecuteInTransaction(() =>
        {
            _shopService.SellCard(_cardRepository.GetOrElseThrow(cardId), _userRepository.GetOrElseThrow(username));
        });
    }
}
