using System.Reflection;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router.Util;

namespace Application.HTTPServer.Router;

internal class RoutingTree
{
    private readonly Dictionary<RequestMethod, Node> _routes = new();

    public RoutingTree()
    {
        _routes.Add(RequestMethod.GET, new Node(""));
        _routes.Add(RequestMethod.POST, new Node(""));
        _routes.Add(RequestMethod.PUT, new Node(""));
        _routes.Add(RequestMethod.DELETE, new Node(""));
    }

    public void AddPath(RequestMethod method, string fullPath, MethodInfo callback, string contentType)
    {
        _routes.TryGetValue(method, out var rootNode);
        if (rootNode == null)
        {
            throw new InvalidDataException($"Method \"{method}\" is not supported for path \"{fullPath}\"");
        }

        var currentNode = rootNode;
        foreach (var path in PathUtil.SplitPath(fullPath))
        {
            var childNode = FindChildWithMatchingPath(currentNode, path);
            if (childNode == null)
            {
                childNode = new Node(path);
                currentNode.ChildNodes.Add(childNode);
            }

            currentNode = childNode;
        }

        if (currentNode.CallbackInfo != null)
        {
            throw new InvalidProgramException($"More than one method is pointing to {fullPath}");
        }

        currentNode.CallbackInfo = new PathCallbackInfo(callback, contentType, fullPath);
        _routes[method] = rootNode;
    }

    public PathCallbackInfo GetCallbackInfo(RequestMethod method, string fullPath)
    {
        _routes.TryGetValue(method, out var rootNode);
        if (rootNode == null)
        {
            throw new InvalidDataException("Method is not supported");
        }

        foreach (var path in PathUtil.SplitPath(fullPath))
        {
            rootNode = FindChildWithMatchingPath(rootNode, path);
            if (rootNode == null)
            {
                throw new PathNotFoundException();
            }
        }

        if (rootNode.CallbackInfo == null)
        {
            throw new PathNotFoundException();
        }

        return rootNode.CallbackInfo;
    }

    private static Node? FindChildWithMatchingPath(Node node, string path)
    {
        return node.ChildNodes.Find(child => child.Path.StartsWith(":") || child.Path == path);
    }

    public class Node
    {
        public string Path { get; set; }
        public PathCallbackInfo? CallbackInfo { get; set; }
        public List<Node> ChildNodes { get; set; }

        public Node(string path)
        {
            Path = path;
            CallbackInfo = null;
            ChildNodes = new List<Node>();
        }
    }
}