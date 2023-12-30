namespace Domain.Users;

public class UpdatableUserData
{
    public string DisplayName { get; }
    public string? Bio { get; }
    public string? Image { get; }

    public UpdatableUserData(string name, string? bio, string? image)
    {
        DisplayName = name;
        Bio = bio;
        Image = image;
    }
}
