using Domain.Cards;
using Domain.Shop.Exceptions;
using Domain.Users;

namespace Domain.Shop;

public class ShopService : IShopService
{
    private readonly IUserRepository _userRepository;
    private readonly ICardRepository _cardRepository;

    private readonly Dictionary<CardClass, int> _cardClassPrices = new()
    {
        { CardClass.Troll, 2 },
        { CardClass.Goblin, 2 },
        { CardClass.Ork, 2 },
        { CardClass.Elf, 3 },
        { CardClass.Spell, 3 },
        { CardClass.Wizard, 5 },
        { CardClass.Knight, 5 },
        { CardClass.Dragon, 10 },
        { CardClass.Kraken, 10 },
    };

    public ShopService(IUserRepository userRepository, ICardRepository cardRepository)
    {
        _userRepository = userRepository;
        _cardRepository = cardRepository;
    }

    public void SellCard(Card card, User seller)
    {
        if (card.LockedForTrade || seller.HasCardInDeck(card.Id) || !seller.OwnsCard(card.Id))
        {
            throw new NotSellableException("Card");
        }

        var cardPrice = _cardClassPrices[card.Class];
        seller.AddCoins(cardPrice);
        seller.RemoveCard(card.Id);
        _userRepository.Update(seller);
        _cardRepository.Remove(card.Id);
    }
}
