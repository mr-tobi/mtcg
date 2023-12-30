namespace Application.HTTPServer.Router.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public class RequestParamAttribute : Attribute
{
    public string? Name { get; }
    public bool Optional { get; }

    public RequestParamAttribute()
    {
    }

    public RequestParamAttribute(string name)
    {
        Name = name;
    }

    public RequestParamAttribute(bool optional)
    {
        Optional = optional;
    }

    public RequestParamAttribute(string? name, bool optional)
    {
        Name = name;
        Optional = optional;
    }
}
