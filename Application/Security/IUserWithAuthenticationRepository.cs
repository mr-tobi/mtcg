using Domain.Repositories;

namespace Application.Security;

public interface IUserWithAuthenticationRepository : IRepository<UserWithAuthentication>
{
    UserWithAuthentication Save(UserWithAuthentication userWithAuthentication);
    bool Exists(string username);
    string? GetPasswordHashForUser(string username);
    string? GetRoleOfUser(string username);
}