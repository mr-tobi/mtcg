namespace Application.HTTPServer.Config;

public class Role
{
    public string Name { get; }
    public List<Role> ChildRoles { get; }

    public Role(string name, params Role[] childs)
    {
        Name = name;
        ChildRoles = childs.ToList();
    }

    public Role(string name)
    {
        Name = name;
        ChildRoles = new List<Role>();
    }

    public override bool Equals(object? obj)
    {
        return obj != null && Name.Equals(((Role)obj).Name);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}