using System.Reflection;

namespace Application.HTTPServer.Router;

public class PathCallbackInfo
{
    public MethodInfo Callback { get; }
    public string ContentType { get; }
    public string Path { get; }

    public PathCallbackInfo(MethodInfo callback, string contentType, string path)
    {
        Callback = callback;
        ContentType = contentType;
        Path = path;
    }
}