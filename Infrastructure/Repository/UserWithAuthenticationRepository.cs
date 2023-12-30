using Application.Security;
using Domain.Users;
using Infrastructure.Repository.Interface;
using Npgsql;

namespace Infrastructure.Repository;

public class UserWithAuthenticationRepository : PgRepository, IUserWithAuthenticationRepository
{
    private IStatsRepository _statsRepository;

    public UserWithAuthenticationRepository(NpgsqlConnection connection) : base("users", connection)
    {
        _statsRepository = new StatsRepository(connection);
    }

    public UserWithAuthentication Save(UserWithAuthentication userWithAuthentication)
    {
        var userBuilder = new User.Builder();
        string role;
        string password;
        {
            using var command = CreateCommand(
                $"INSERT INTO {TableName} (username, password, role, coins, deck, display_name, bio, image) VALUES (@username, @password, @role, @coins, @deck, @display_name, @bio, @image) RETURNING username, password, role, coins, deck, display_name, bio, image"
            );
            AddParameter(command, "username", userWithAuthentication.User.Username);
            AddParameter(command, "password", userWithAuthentication.Password);
            AddParameter(command, "role", userWithAuthentication.Role);
            AddParameter(command, "coins", userWithAuthentication.User.Coins);
            AddParameter(command, "deck", Array.Empty<string>());
            AddParameter(command, "display_name", userWithAuthentication.User.DisplayName);
            AddParameter(command, "bio", DatabaseUtil.GetValueOrDBNull(userWithAuthentication.User.Bio));
            AddParameter(command, "image", DatabaseUtil.GetValueOrDBNull(userWithAuthentication.User.Image));

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new InvalidOperationException("User could not be created");
            }

            var bio = reader.GetValue(6);
            var image = reader.GetValue(7);
            userBuilder
                .Username(reader.GetString(0))
                .Coins(reader.GetInt32(3))
                .DisplayName(reader.GetString(5))
                .Bio(bio is DBNull ? null : (string)bio)
                .Image(image is DBNull ? null : (string)image);

            password = reader.GetString(1);
            role = reader.GetString(2);
        }

        userBuilder.Stats(_statsRepository.Save(userWithAuthentication.User));

        return new UserWithAuthentication(password, role, userBuilder.Build());
    }

    public bool Exists(string username)
    {
        using var command = CreateCommand($"SELECT exists(select 1 from {TableName} where username=@username)");
        AddParameter(command, "username", username);

        return (bool)command.ExecuteScalar();
    }

    public string? GetPasswordHashForUser(string username)
    {
        using var command = CreateCommand($"SELECT password from {TableName} where username = @username");
        AddParameter(command, "username", username);

        return (string)command.ExecuteScalar();
    }

    public string? GetRoleOfUser(string username)
    {
        using var command = CreateCommand($"SELECT role from {TableName} where username = @username");
        AddParameter(command, "username", username);

        return (string)command.ExecuteScalar();
    }
}