using Application.User.Exceptions;
using Domain.Cards;
using Domain.Users;

namespace Application.User;

public class DeckUseCase : IDeckPort
{
    private IUserRepository _userRepository;
    private ICardRepository _cardRepository;

    public DeckUseCase(IUserRepository userRepository, ICardRepository cardRepository)
    {
        _userRepository = userRepository;
        _cardRepository = cardRepository;
    }

    public List<Card> GetDeck(string username)
    {
        var user = _userRepository.Get(username) ?? throw new InvalidOperationException("User not found");
        return _cardRepository.Get(user.Deck);
    }

    public void SetDeck(string username, string[] cardIds)
    {
        if (cardIds.Length != Domain.Users.User.DeckSize)
        {
            throw new DeckSizeException();
        }

        _userRepository.ExecuteInTransaction(() =>
        {
            var user = _userRepository.Get(username) ?? throw new InvalidOperationException("User not found");
            user.SetDeck(_cardRepository.Get(cardIds.Select(id => new CardId(id)).ToList()));
            _userRepository.Update(user);
        });
    }
}
