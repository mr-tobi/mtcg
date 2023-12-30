namespace Infrastructure.Controller.DTO;

public class UserDTO
{
    public string Name { get; }
    public int Coins { get; }
    public string? Bio { get; }
    public string? Image { get; }

    public UserDTO(string name, int coins, string? bio, string? image)
    {
        Name = name;
        Coins = coins;
        Bio = bio;
        Image = image;
    }
}
