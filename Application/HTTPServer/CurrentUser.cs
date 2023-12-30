namespace Application.HTTPServer;

public class CurrentUser
{
    public string Username { get; }

    public CurrentUser(string username)
    {
        Username = username;
    }
}
