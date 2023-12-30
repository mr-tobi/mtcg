namespace Application.User;

public interface IGetUserPort
{
    Domain.Users.User? GetUser(string username);
}
