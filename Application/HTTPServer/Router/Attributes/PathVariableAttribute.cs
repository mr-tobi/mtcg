namespace Application.HTTPServer.Router.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class PathVariableAttribute : Attribute
{
    public string? Name { get; }

    public PathVariableAttribute()
    {
    }

    public PathVariableAttribute(string name)
    {
        Name = name;
    }
}