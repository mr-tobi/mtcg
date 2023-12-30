using Domain;

namespace Application.Security;

public class UserWithAuthentication : IAggregateRoot
{
    public string Password { get; }
    public string Role { get; }
    public Domain.Users.User User { get; }

    public UserWithAuthentication(string password, string role, Domain.Users.User user)
    {
        Password = password;
        Role = role;
        User = user;
    }
}