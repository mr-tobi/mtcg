using System.Reflection;
using Application.HTTPServer.Router.Attributes;

namespace Application.HTTPServer.Router;

public class Router
{
    private static RoutingTree _routes = new();

    public static void InitRoutes(Assembly assembly)
    {
        var controllerInfos =
            from type in assembly.GetTypes()
            let attributes = (ControllerAttribute[])type.GetCustomAttributes(typeof(ControllerAttribute), true)
            where attributes is { Length: 1 }
            select new { Type = type, attributes[0].Path };

        foreach (var controllerInfo in controllerInfos)
        {
            foreach (var methodInfo in controllerInfo.Type.GetMethods())
            {
                var attributes = (RequestMappingAttribute[])methodInfo.GetCustomAttributes(
                    typeof(RequestMappingAttribute), false
                );

                if (attributes.Length == 0)
                {
                    continue;
                }

                if (attributes.Length > 1)
                {
                    throw new InvalidOperationException("Method cannot have more than one mapping type");
                }

                var attribute = attributes.First();
                _routes.AddPath(
                    attribute.Method,
                    GetPath(controllerInfo.Path, attribute.Path),
                    methodInfo,
                    attribute.ContentType
                );
            }
        }
    }

    public static PathCallbackInfo GetCallbackInfoForRequest(Request request)
    {
        return _routes.GetCallbackInfo(request.Method, request.Target);
    }

    private static string GetPath(string controllerPath, string methodPath)
    {
        return $"{SetTrailingSlash(controllerPath)}{SetTrailingSlash(methodPath)}";
    }

    private static string SetTrailingSlash(string path)
    {
        if (String.IsNullOrEmpty(path))
        {
            return "";
        }

        return path.StartsWith('/') ? path : $"/{path}";
    }
}