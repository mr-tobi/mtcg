using Domain.Users;

namespace Application.User;

public interface IUpdateUserPort
{
    void UpdateUser(string username, UpdatableUserData userData);
}
