using Application.HTTPServer.Exception;

namespace Application.HTTPServer.Router;

public class Request
{
    public RequestMethod Method { get; }
    public string Target { get; }
    public Dictionary<string, string> RequestParameter { get; }
    public string ProtocolVersion { get; }
    public Dictionary<string, string> Header { get; }
    public string? Content { get; }

    public Request(
        RequestMethod method,
        string target,
        Dictionary<string, string> requestParameter,
        string protocolVersion,
        Dictionary<string, string> header,
        string? content = null)
    {
        Method = method;
        Target = target;
        RequestParameter = requestParameter;
        ProtocolVersion = protocolVersion;
        Header = header;
        Content = content;
    }

    public string GetToken()
    {
        Header.TryGetValue("authorization", out var tokenString);
        if (tokenString == null)
        {
            Header.TryGetValue("Authorization", out tokenString);
        }

        if (tokenString == null)
        {
            throw new UnauthorizedException();
        }

        return tokenString;
    }

    public override string ToString()
    {
        string[] rows =
        {
            $"{Method} {Target} {ProtocolVersion}",
            $"{string.Join("\r\n", Header.Select(x => $"{x.Key}: {x.Value}"))}",
            "",
            $"{Content}",
        };
        return String.Join("\r\n", rows);
    }
}
