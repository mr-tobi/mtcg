namespace Infrastructure.Repository;

public class DatabaseUtil
{
    public static object GetValueOrDBNull(object? value)
    {
        return value ?? DBNull.Value;
    }
}