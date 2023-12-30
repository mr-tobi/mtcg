using Domain.Cards;
using Domain.Users.Exceptions;

namespace Domain.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPackageRepository _packageRepository;
    private readonly int _packageCost = 5;

    public UserService(IUserRepository userRepository, IPackageRepository packageRepository)
    {
        _userRepository = userRepository;
        _packageRepository = packageRepository;
    }

    public void OpenPackage(User user, Package package)
    {
        if (user.Coins < _packageCost)
        {
            throw new NotEnoughMoneyException();
        }

        user.AcquireCards(package);
        user.RemoveCoins(_packageCost);
        _userRepository.Update(user);
        _packageRepository.Remove(package);
    }
}