using Domain.Users;

namespace Infrastructure.Repository.Interface;

public interface IStatsRepository
{
    Statistic Save(User user);
    void Update(User user);
    Statistic GetOrElseThrow(string username);
}