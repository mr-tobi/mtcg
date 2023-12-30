using System.Reflection;
using Application.HTTPServer;
using Application.HTTPServer.Config;
using Application.HTTPServer.Router;
using Application.HTTPServer.Security;

namespace Infrastructure;

internal class Program
{
    static void Main(string[] args)
    {
        Router.InitRoutes(Assembly.GetExecutingAssembly());
        var server = new Server(BuildConfig());
        server.Start();
    }

    static ServerConfig BuildConfig()
    {
        var securityConfig = new SecurityConfig();
        var userRole = new Role("user");
        var adminRole = new Role("admin", userRole);
        securityConfig
            .PermitAll("/sessions", "/users")
            .ProtectRoute("/package", adminRole)
            .ProtectAnyRoute(userRole)
            .SetTokenHandler(new MTCGTokenHandler());
        return new ServerConfig(10001, securityConfig);
    }
}
