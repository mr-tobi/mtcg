namespace Application.HTTPServer.Router.Util;

internal class PathUtil
{
    public static List<string> SplitPath(string fullPath)
    {
        return fullPath
            .Split("/")
            .Where(element => !String.IsNullOrEmpty(element)).ToList();
    }
}