namespace Infrastructure.Controller.DTO;

public class UpdateUserDTO
{
    public string Name { get; }
    public string? Bio { get; }
    public string? Image { get; }

    public UpdateUserDTO(string name, string? bio, string? image)
    {
        Name = name;
        Bio = bio;
        Image = image;
    }
}
