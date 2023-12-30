using Domain.Cards;

namespace Domain.Users;

public interface IUserService
{
    void OpenPackage(User user, Package package);
}
