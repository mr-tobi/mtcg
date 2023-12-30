using Domain.Cards;
using Domain.Users;

namespace Application.User;

public class GetCardsUseCase : IGetCardsPort
{
    private IUserRepository _userRepository;
    private ICardRepository _cardRepository;

    public GetCardsUseCase(IUserRepository userRepository, ICardRepository cardRepository)
    {
        _userRepository = userRepository;
        _cardRepository = cardRepository;
    }

    public List<Card> GetCardsOfUser(string username)
    {
        var user = _userRepository.Get(username);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return _cardRepository.Get(user.Cards);
    }
}
