namespace Infrastructure.Controller.DTO;

public class LoginDTO
{
    public string Username { get; }
    public string Password { get; }

    public LoginDTO(string username, string password)
    {
        Username = username;
        Password = password;
    }
}