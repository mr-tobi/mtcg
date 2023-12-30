namespace Domain.Cards;

public class PackageService : IPackageService
{
    private IPackageRepository _packageRepository;

    public PackageService(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }

    public void CreatePackage(List<Card> cards)
    {
        var package = Package.WithCards(cards);
        _packageRepository.Save(package);
    }

    public Package GetNextPackage()
    {
        return _packageRepository.GetNextPackage();
    }
}
