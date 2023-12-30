namespace Application.HTTPServer.Router.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    public string Path { get; }

    public ControllerAttribute(string path)
    {
        this.Path = path;
    }
}