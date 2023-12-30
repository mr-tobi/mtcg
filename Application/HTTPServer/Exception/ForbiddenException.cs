using System.Net;

namespace Application.HTTPServer.Exception;

public class ForbiddenException : ExceptionWithStatusCode
{
    public ForbiddenException() : base("Forbidden", HttpStatusCode.Forbidden)
    {
    }
}