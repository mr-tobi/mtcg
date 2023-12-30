using Domain.Cards;
using Domain.Users;

namespace Application.Package;

public class OpenPackageUseCase : IOpenPackagePort
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IPackageService _packageService;

    public OpenPackageUseCase(IUserService userService, IUserRepository userRepository, IPackageService packageService)
    {
        _userService = userService;
        _userRepository = userRepository;
        _packageService = packageService;
    }

    public void OpenPackage(string currentUserName)
    {
        _userRepository.ExecuteInTransaction(() =>
        {
            var user = _userRepository.GetOrElseThrow(currentUserName);
            var package = _packageService.GetNextPackage();
            _userService.OpenPackage(user, package);
        });
    }
}
