using Application.HTTPServer;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Shop;
using Domain.Cards;
using Domain.Shop;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("/shop")]
public class ShopController
{
    private readonly IShopPort _shopPort;

    public ShopController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        var userRepo = new UserRepository(connection);
        var cardRepo = new CardRepository(connection);
        _shopPort = new ShopUseCase(new ShopService(userRepo, cardRepo), userRepo, cardRepo);
    }

    [RequestMapping(RequestMethod.POST, "/sell/card")]
    public void SellCard(CurrentUser currentUser, [RequestBody] string cardId)
    {
        _shopPort.SellCard(currentUser.Username, new CardId(cardId));
    }
}
