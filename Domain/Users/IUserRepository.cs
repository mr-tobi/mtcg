using Domain.Repositories;

namespace Domain.Users;

public interface IUserRepository : IRepository<User>
{
    public void Update(User user);

    public User? Get(string username);
    public User GetOrElseThrow(string username);

    User[] GetUsersWithHighestElo(int count);
}
