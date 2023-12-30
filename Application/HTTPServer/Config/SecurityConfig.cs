using Application.HTTPServer.Security;

namespace Application.HTTPServer.Config;

public class SecurityConfig
{
    public const string Wildcard = "**";
    public static string DefaultRoleName { get; private set; } = "user";
    public static readonly Role AnonymousRole = new("anonymousRole");
    public static ITokenHandler TokenHandler { get; private set; } = new MTCGTokenHandler();

    private List<RouteSecurityInfo> _routes = new();

    public SecurityConfig ProtectRoute(string route, params Role[] roles)
    {
        _routes.Add(new RouteSecurityInfo(route, roles));
        return this;
    }

    public SecurityConfig ProtectAnyRoute(params Role[] roles)
    {
        ProtectRoute(Wildcard, roles);
        return this;
    }

    public SecurityConfig PermitAll(params string[] routes)
    {
        foreach (var route in routes)
        {
            ProtectRoute(route, AnonymousRole);
        }

        return this;
    }

    public SecurityConfig SetTokenHandler(ITokenHandler tokenHandler)
    {
        TokenHandler = tokenHandler;
        return this;
    }

    public SecurityConfig DefaultRole(string role)
    {
        DefaultRoleName = role;
        return this;
    }

    public List<RouteSecurityInfo> GetProtectedRoutes()
    {
        return _routes;
    }

    public class RouteSecurityInfo
    {
        public string Path { get; }
        public Role[] Roles { get; }

        public RouteSecurityInfo(string path, Role[] roles)
        {
            Path = path;
            Roles = roles;
        }
    }
}