namespace Domain.Repositories;

public interface IRepository<T> where T : IAggregateRoot
{
    TReturnType ExecuteInTransaction<TReturnType>(Func<TReturnType> toExecute);
    void ExecuteInTransaction(Action toExecute);
}
