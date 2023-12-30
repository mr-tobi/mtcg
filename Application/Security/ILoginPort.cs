namespace Application.Security;

public interface ILoginPort
{
    string Login(LoginData loginData);
}