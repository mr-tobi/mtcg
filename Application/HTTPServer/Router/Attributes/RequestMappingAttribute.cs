namespace Application.HTTPServer.Router.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RequestMappingAttribute : Attribute
{
    private const string DefaultContentType = "application/json";

    public string Path { get; }
    public RequestMethod Method { get; }
    public string ContentType { get; }

    public RequestMappingAttribute(RequestMethod method, string path = "", string contentType = DefaultContentType)
    {
        Method = method;
        Path = path;
        ContentType = contentType;
    }
}