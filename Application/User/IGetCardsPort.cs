using Domain.Cards;

namespace Application.User;

public interface IGetCardsPort
{
    public List<Card> GetCardsOfUser(string username);
}