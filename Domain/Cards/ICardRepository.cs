using Domain.Repositories;

namespace Domain.Cards;

public interface ICardRepository : IRepository<Card>
{
    Card Save(Card card);
    Card GetOrElseThrow(CardId id);
    List<Card> Get(List<CardId> ids);
    void SetOwner(CardId card, string ownerName);
    List<Card> GetCardsOfOwner(string ownerName);
    void UpdateLockedState(Card card);
    void Remove(CardId cardId);
}
