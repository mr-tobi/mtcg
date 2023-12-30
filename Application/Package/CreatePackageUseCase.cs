using Domain.Cards;

namespace Application.Package;

public class CreatePackageUseCase : ICreatePackagePort
{
    private readonly IPackageService _packageService;
    private readonly IPackageRepository _packageRepository;

    public CreatePackageUseCase(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
        _packageService = new PackageService(packageRepository);
    }

    public void CreatePackage(List<CardData> cards)
    {
        _packageRepository.ExecuteInTransaction(() =>
        {
            _packageService.CreatePackage(
                cards.Select(card =>
                        new Card(card.Id == null ? new CardId() : new CardId(card.Id), card.Name, card.Damage)
                    )
                    .ToList()
            );
        });
    }
}
