using Application.HTTPServer.Config;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;

namespace Application.HTTPServer.Security;

public class SecurityChecker
{
    private List<PathInfo> _paths = new();
    private HashSet<Role> _roles = new();
    private ITokenHandler _tokenHandler;

    public SecurityChecker(SecurityConfig config)
    {
        foreach (var routeInfo in config.GetProtectedRoutes())
        {
            var routeRoles = routeInfo.Roles;
            _paths.Add(new PathInfo(routeInfo.Path, routeRoles));
            foreach (var role in routeRoles)
            {
                _roles.Add(role);
            }
        }

        _tokenHandler = SecurityConfig.TokenHandler;
    }

    public void CheckAllowed(Request request)
    {
        var allowedRoles = GetAllowedRolesForRoute(request.Target);
        if (allowedRoles.Contains(SecurityConfig.AnonymousRole))
        {
            return;
        }

        var token = GetTokenFromRequest(request);
        if (token == null || !_tokenHandler.VerifyToken(token.GetTokenString()))
        {
            throw new UnauthorizedException();
        }

        if (!IsAllowed(token.GetUserRole(), allowedRoles))
        {
            throw new ForbiddenException();
        }
    }

    private bool IsAllowed(string currentUserRole, Role[] allowedRoles)
    {
        if (allowedRoles.Select(role => role.Name).Contains(currentUserRole))
        {
            return true;
        }

        foreach (var childRole in GetChildRoles(currentUserRole))
        {
            if (IsAllowed(childRole.Name, allowedRoles))
            {
                return true;
            }
        }

        return false;
    }

    private Role[] GetChildRoles(string roleName)
    {
        _roles.TryGetValue(new Role(roleName), out var role);

        return role != null ? role.ChildRoles.ToArray() : Array.Empty<Role>();
    }

    private Role[] GetAllowedRolesForRoute(string route)
    {
        foreach (var pathInfo in _paths)
        {
            if (pathInfo.Path == route)
            {
                return pathInfo.Roles;
            }

            if (pathInfo.Path == SecurityConfig.Wildcard)
            {
                return pathInfo.Roles;
            }

            var routeParts = SplitRoute(route);
            var pathParts = SplitRoute(pathInfo.Path);

            if (pathParts.Length > routeParts.Length)
            {
                continue;
            }

            for (var i = 0; i < pathParts.Length; i++)
            {
                if (pathParts[i] == SecurityConfig.Wildcard)
                {
                    return pathInfo.Roles;
                }

                if (pathParts[i] != routeParts[i])
                {
                    break;
                }
            }
        }

        return Array.Empty<Role>();
    }

    private string[] SplitRoute(string route)
    {
        return route.Split("/").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
    }

    private IToken? GetTokenFromRequest(Request request)
    {
        try
        {
            return _tokenHandler.GetToken(request.GetToken());
        }
        catch (NoTokenException)
        {
            return null;
        }
    }

    private class PathInfo
    {
        public string Path { get; }
        public Role[] Roles { get; set; }

        public PathInfo(string path, Role[] roles)
        {
            Path = path;
            Roles = roles;
        }
    }
}
