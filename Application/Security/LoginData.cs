namespace Application.Security;

public class LoginData
{
    public string Username { get; }
    private readonly Password _password;

    public LoginData(string username, string password)
    {
        Username = username;
        _password = new Password(password);
    }

    public string PasswordHash()
    {
        return _password.PasswordHash();
    }
}