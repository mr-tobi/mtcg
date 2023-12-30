using System.Net;

namespace Application.HTTPServer.Exception;

public class ExceptionWithStatusCode : System.Exception
{
    public HttpStatusCode StatusCode { get; }

    public ExceptionWithStatusCode(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}