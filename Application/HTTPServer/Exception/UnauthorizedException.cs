using System.Net;

namespace Application.HTTPServer.Exception;

public class UnauthorizedException : ExceptionWithStatusCode
{
    public UnauthorizedException() : base("Unauthorized", HttpStatusCode.Unauthorized)
    {
    }
}