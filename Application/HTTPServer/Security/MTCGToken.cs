using System.Text.RegularExpressions;
using Application.HTTPServer.Exception;

namespace Application.HTTPServer.Security;

public class MTCGToken : IToken
{
    private const string Prefix = "Bearer ";
    private const string Postfix = "mtcgToken";
    private static readonly Regex _tokenPattern = new($@"^{Prefix}(\w+)-(\w+)-{Postfix}");

    private string _username;
    private string _role;

    public static MTCGToken ToToken(string token)
    {
        var matches = _tokenPattern.Match(token);
        if (!matches.Success)
        {
            throw new InvalidTokenException();
        }

        var values = matches.Groups.Values.Skip(1).Select(group => group.Value).ToArray();
        return new MTCGToken(values[1], values[0]);
    }

    public MTCGToken(string username, string role)
    {
        _username = username;
        _role = role;
    }

    public string GetUsername()
    {
        return _username;
    }

    public string GetUserRole()
    {
        return _role;
    }

    public string GetTokenString()
    {
        return $"{Prefix}{_role}-{_username}-{Postfix}";
    }
}
