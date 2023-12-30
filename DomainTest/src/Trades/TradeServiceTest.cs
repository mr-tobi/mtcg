using Domain.Cards;
using Domain.Exceptions;
using Domain.Trades;
using Domain.Trades.Exceptions;
using Domain.Users;
using FakeItEasy;

namespace DomainTest.Trades;

public class TradeServiceTest
{
    private TradeService _service;

    private ITradeOfferRepository _tradeOfferRepository;
    private ICardRepository _cardRepository;
    private IUserRepository _userRepository;

    [SetUp]
    public void Setup()
    {
        _tradeOfferRepository = A.Fake<ITradeOfferRepository>();
        _cardRepository = A.Fake<ICardRepository>();
        _userRepository = A.Fake<IUserRepository>();

        _service = new TradeService(_tradeOfferRepository, _cardRepository, _userRepository);
    }

    [Test]
    public void CreateTrade_Create()
    {
        TradeOffer? savedTradeOffer = null;
        A.CallTo(() => _tradeOfferRepository.Save(A<TradeOffer>.Ignored))
            .Invokes((TradeOffer tradeOffer) => savedTradeOffer = tradeOffer);
        var card = GetCard("Dragon", 100);
        var owner = new User("tobi");
        owner.AddCard(card.Id);

        _service.CreateTrade(new TradeOfferId(), card, owner, CardType.Monster, 10);

        Assert.Multiple(() =>
        {
            Assert.That(savedTradeOffer.OwnerUsername, Is.EqualTo(owner.Username));
            Assert.That(savedTradeOffer.CardToTrade, Is.EqualTo(card.Id));
            Assert.That(savedTradeOffer.CardType, Is.EqualTo(CardType.Monster));
            Assert.That(savedTradeOffer.MinimumDamage, Is.EqualTo(10));
            Assert.That(card.LockedForTrade, Is.True);
        });
        A.CallTo(() => _cardRepository.UpdateLockedState(A<Card>.Ignored)).MustHaveHappened();
    }

    [Test]
    public void CreateTrade_OfferingCardLockedForTrade()
    {
        var card = new Card(new CardId(), "Dragon", 100);
        var owner = new User("tobi");
        card.LockForTrade();
        owner.AddCard(card.Id);

        Assert.Throws<NotTradableException>(
            () =>
                _service.CreateTrade(new TradeOfferId(), card, owner, CardType.Monster, 10)
        );
    }

    [Test]
    public void CreateTrade_OfferingCardInDeck()
    {
        var card = new Card(new CardId(), "Dragon", 100);
        var owner = new User("tobi");
        owner.AddCard(card.Id);
        owner.SetDeck(new List<Card> { card });

        Assert.Throws<NotTradableException>(
            () =>
                _service.CreateTrade(new TradeOfferId(), card, owner, CardType.Monster, 10)
        );
    }

    [Test]
    public void CreateTrade_OfferingCardNotOwnedByUser()
    {
        var card = new Card(new CardId(), "Dragon", 100);
        var owner = new User("tobi");

        Assert.Throws<NotTradableException>(
            () =>
                _service.CreateTrade(new TradeOfferId(), card, owner, CardType.Monster, 10)
        );
    }

    [Test]
    public void Trade_TradeCard()
    {
        var cardInOffer = GetCard("Dragon", 100);
        var cardToOffer = GetCard("Dragon", 100);
        var tradeOfferOwner = new User("tobi");
        var acceptingUser = new User("tobi2");
        var tradeOffer = new TradeOffer(
            new TradeOfferId(),
            tradeOfferOwner.Username,
            cardInOffer.Id,
            CardType.Monster,
            10
        );
        tradeOfferOwner.AddCard(cardInOffer.Id);
        acceptingUser.AddCard(cardToOffer.Id);
        cardInOffer.LockForTrade();
        A.CallTo(() => _cardRepository.GetOrElseThrow(A<CardId>.Ignored)).Returns(cardInOffer);

        _service.Trade(tradeOffer, acceptingUser, tradeOfferOwner, cardToOffer);

        Assert.Multiple(() =>
        {
            Assert.That(tradeOfferOwner.Cards, Contains.Item(cardToOffer.Id));
            Assert.That(acceptingUser.Cards, Contains.Item(cardInOffer.Id));

            Assert.That(tradeOfferOwner.Cards, Is.Not.Contain(cardInOffer.Id));
            Assert.That(acceptingUser.Cards, Is.Not.Contain(cardToOffer.Id));

            Assert.That(cardInOffer.LockedForTrade, Is.False);
        });

        A.CallTo(() =>
            _userRepository.Update(
                A<User>.That.Matches(user => user.Username.Equals(tradeOfferOwner.Username))
            )
        ).MustHaveHappened();
        A.CallTo(() =>
            _userRepository.Update(
                A<User>.That.Matches(user => user.Username.Equals(acceptingUser.Username))
            )
        ).MustHaveHappened();
        A.CallTo(() => _cardRepository.UpdateLockedState(A<Card>.Ignored)).MustHaveHappened();
        A.CallTo(() => _tradeOfferRepository.Delete(A<TradeOfferId>.Ignored)).MustHaveHappened();
    }

    [Test]
    public void Trade_AcceptingUserIsTradeOfferOwner()
    {
        var cardInOffer = GetCard("Dragon", 100);
        var cardToOffer = GetCard("Dragon", 100);
        var tradeOfferOwner = new User("tobi");
        var tradeOffer = new TradeOffer(
            new TradeOfferId(),
            tradeOfferOwner.Username,
            cardInOffer.Id,
            CardType.Monster,
            10
        );
        tradeOfferOwner.AddCard(cardInOffer.Id);
        cardInOffer.LockForTrade();
        A.CallTo(() => _cardRepository.GetOrElseThrow(A<CardId>.Ignored)).Returns(cardInOffer);


        Assert.Throws<NotTradableException>(() =>
        {
            _service.Trade(tradeOffer, tradeOfferOwner, tradeOfferOwner, cardToOffer);
        });
    }

    [Test]
    public void Trade_AcceptingCardInDeck()
    {
        var cardInOffer = GetCard("Dragon", 100);
        var cardToOffer = GetCard("Dragon", 100);
        var tradeOffer = new TradeOffer(new TradeOfferId(), "tobi", cardInOffer.Id, CardType.Monster, 10);
        var tradeOfferOwner = new User("tobi");
        var acceptingUser = new User("tobi2");
        tradeOfferOwner.AddCard(cardInOffer.Id);
        acceptingUser.AddCard(cardToOffer.Id);
        acceptingUser.SetDeck(new List<Card> { cardToOffer });
        cardInOffer.LockForTrade();
        A.CallTo(() => _cardRepository.GetOrElseThrow(A<CardId>.Ignored)).Returns(cardInOffer);


        Assert.Throws<NotTradableException>(() =>
        {
            _service.Trade(tradeOffer, acceptingUser, tradeOfferOwner, cardToOffer);
        });
    }

    [Test]
    public void Trade_AcceptingCardNotOwned()
    {
        var cardInOffer = GetCard("Dragon", 100);
        var cardToOffer = GetCard("Dragon", 100);
        var tradeOffer = new TradeOffer(new TradeOfferId(), "tobi", cardInOffer.Id, CardType.Monster, 10);
        var tradeOfferOwner = new User("tobi");
        var acceptingUser = new User("tobi2");
        tradeOfferOwner.AddCard(cardInOffer.Id);
        cardInOffer.LockForTrade();
        A.CallTo(() => _cardRepository.GetOrElseThrow(A<CardId>.Ignored)).Returns(cardInOffer);


        Assert.Throws<NotTradableException>(() =>
        {
            _service.Trade(tradeOffer, acceptingUser, tradeOfferOwner, cardToOffer);
        });
    }

    [Test]
    public void DeleteTrade_Delete()
    {
        var card = GetCard("Dragon", 100);
        var tradeOffer = new TradeOffer(new TradeOfferId(), "tobi", card.Id, CardType.Monster, 10);
        A.CallTo(() => _tradeOfferRepository.GetOrElseThrow(A<TradeOfferId>.Ignored)).Returns(tradeOffer);
        A.CallTo(() => _cardRepository.GetOrElseThrow(A<CardId>.Ignored)).Returns(card);

        _service.DeleteTrade(tradeOffer.Id);

        Assert.That(card.LockedForTrade, Is.False);
        A.CallTo(() => _cardRepository.UpdateLockedState(A<Card>.Ignored)).MustHaveHappened();
        A.CallTo(() => _tradeOfferRepository.Delete(A<TradeOfferId>.Ignored)).MustHaveHappened();
    }

    [Test]
    public void DeleteTrade_NotFound()
    {
        A.CallTo(() => _tradeOfferRepository.GetOrElseThrow(A<TradeOfferId>.Ignored))
            .Throws(new NotFoundException("TradeOffer"));

        Assert.Throws<NotFoundException>(() => { _service.DeleteTrade(new TradeOfferId()); });
    }

    private Card GetCard(string name, int damage)
    {
        return new Card(new CardId(), name, damage);
    }
}
