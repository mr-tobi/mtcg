using Application.HTTPServer.Config;

namespace Application.Security;

public class RegisterData
{
    public string Username { get; }
    public Password Password { get; }

    public string Role { get; }

    public RegisterData(string username, string password, string? role)
    {
        Username = username;
        Password = new Password(password);
        Role = role ?? SecurityConfig.DefaultRoleName;
    }
}