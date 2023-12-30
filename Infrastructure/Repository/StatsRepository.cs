using Domain.Users;
using Infrastructure.Repository.Interface;
using Npgsql;

namespace Infrastructure.Repository;

public class StatsRepository : PgRepository, IStatsRepository
{
    public StatsRepository(NpgsqlConnection connection) : base("stats", connection)
    {
    }

    public Statistic Save(User user)
    {
        using var command = CreateCommand(
            $"Insert into {TableName} (username, elo, wins, losses) VALUES (@username, @elo, @wins, @losses) RETURNING elo, wins, losses"
        );
        AddParameter(command, "username", user.Username);
        AddParameter(command, "elo", user.Stats.Elo);
        AddParameter(command, "wins", user.Stats.Wins);
        AddParameter(command, "losses", user.Stats.Losses);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            throw new InvalidOperationException("Stats could not be saved");
        }

        return new Statistic(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
    }

    public void Update(User user)
    {
        using var command = CreateCommand(
            $"UPDATE {TableName} SET elo=@elo, wins=@wins, losses=@losses where username=@username"
        );
        AddParameter(command, "elo", user.Stats.Elo);
        AddParameter(command, "wins", user.Stats.Wins);
        AddParameter(command, "losses", user.Stats.Losses);
        AddParameter(command, "username", user.Username);

        command.ExecuteScalar();
    }

    public Statistic GetOrElseThrow(string username)
    {
        using var command = CreateCommand($"Select elo, wins, losses from {TableName} where username=@username");
        AddParameter(command, "username", username);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            throw new InvalidOperationException("User stats not found");
        }

        return new Statistic(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
    }
}
