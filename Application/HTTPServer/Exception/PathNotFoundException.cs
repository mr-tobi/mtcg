using System.Net;

namespace Application.HTTPServer.Exception;

public class PathNotFoundException : ExceptionWithStatusCode
{
    public PathNotFoundException() : base("Not found", HttpStatusCode.NotFound)
    {
    }
}