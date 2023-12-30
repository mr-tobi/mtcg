namespace Domain.Cards;

public interface IPackageService
{
    public void CreatePackage(List<Card> cards);
    Package GetNextPackage();
}
