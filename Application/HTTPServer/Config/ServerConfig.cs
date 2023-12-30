namespace Application.HTTPServer.Config;

public class ServerConfig
{
    public int Port { get; }
    public SecurityConfig Security { get; }

    public ServerConfig(int port, SecurityConfig security)
    {
        Security = security;
        Port = port;
    }
}