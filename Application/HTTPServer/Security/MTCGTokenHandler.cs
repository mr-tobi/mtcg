using Application.HTTPServer.Exception;

namespace Application.HTTPServer.Security;

public class MTCGTokenHandler : ITokenHandler
{
    public string GenerateToken(string username, string role)
    {
        return new MTCGToken(username, role).GetTokenString();
    }

    public bool VerifyToken(string token)
    {
        try
        {
            MTCGToken.ToToken(token);
            return true;
        }
        catch (InvalidTokenException)
        {
            return false;
        }
    }

    public IToken GetToken(string token)
    {
        return MTCGToken.ToToken(token);
    }
}
