namespace Application.HTTPServer.Exception;

public class InvalidTokenException : System.Exception
{
    public InvalidTokenException() : base("Authentication token is invalid")
    {
    }
}
