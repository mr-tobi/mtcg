namespace Application.HTTPServer.Security;

public interface IToken
{
    string GetUsername();
    string GetUserRole();
    string GetTokenString();
}