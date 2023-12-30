namespace Application.HTTPServer.Exception;

public class NoTokenException : System.Exception
{
    public NoTokenException() : base("No authentication token in request")
    {
    }
}
