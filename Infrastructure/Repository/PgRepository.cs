using System.Data;
using Npgsql;

namespace Infrastructure.Repository;

public abstract class PgRepository
{
    private const string Host = "127.0.0.1";
    private const string Username = "postgres";
    private const string Password = "root";
    private const string Database = "mtcg";

    protected string TableName { get; }
    private readonly NpgsqlConnection _connection;

    protected PgRepository(string tableName, NpgsqlConnection connection)
    {
        // Maybe add this at some point again
        // var tableAttribute = (TableAttribute?)typeof(Table).GetCustomAttribute(typeof(TableAttribute));
        // if (tableAttribute == null)
        // {
        //     throw new TypeAccessException("Table class does not have table attribute");
        // }
        // TableName = tableAttribute.Name;
        TableName = tableName;
        _connection = connection;
    }

    ~PgRepository()
    {
        _connection.Close();
    }

    public TReturnType ExecuteInTransaction<TReturnType>(Func<TReturnType> toExecute)
    {
        var transaction = _connection.BeginTransaction();
        try
        {
            var result = toExecute();
            transaction.Commit();
            return result;
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ExecuteInTransaction(Action toExecute)
    {
        var transaction = _connection.BeginTransaction();
        try
        {
            toExecute();
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    protected NpgsqlCommand CreateCommand(string query)
    {
        var command = _connection.CreateCommand();
        command.CommandText = query;
        return command;
    }

    protected void AddParameter<T>(IDbCommand command, string name, T value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public static NpgsqlConnection CreateAndOpenConnection()
    {
        var connection =
            new NpgsqlConnection($"Host={Host};Username={Username};Password={Password};Database={Database}");
        connection.Open();
        return connection;
    }
}
