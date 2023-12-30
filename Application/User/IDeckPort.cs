using Domain.Cards;

namespace Application.User;

public interface IDeckPort
{
    List<Card> GetDeck(string username);
    public void SetDeck(string username, string[] cardIds);
}