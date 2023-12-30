namespace Infrastructure.Controller.DTO;

public class RegisterDTO
{
    public string Username { get; }
    public string Password { get; }
    public string? Role { get; }

    public RegisterDTO(string username, string password, string? role)
    {
        Username = username;
        Password = password;
        Role = role;
    }
}