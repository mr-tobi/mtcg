using System.Net;

namespace Application.HTTPServer;

public class ResultWithRequestMetadata
{
    public object? Result { get; }
    public HttpStatusCode StatusCode { get; }
    public string ContentType { get; } = "application/json";

    public ResultWithRequestMetadata(object? result, HttpStatusCode statusCode)
    {
        Result = result;
        StatusCode = statusCode;
    }

    public ResultWithRequestMetadata(object? result, HttpStatusCode statusCode, string contentType)
    {
        Result = result;
        StatusCode = statusCode;
        ContentType = contentType;
    }
}
