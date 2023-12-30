namespace Application.HTTPServer.Security;

public interface ITokenHandler
{
    string GenerateToken(string username, string role);
    bool VerifyToken(string token);
    IToken GetToken(string token);
}
