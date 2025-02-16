using System.Data.Common;
using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Models.Domain.Users;
using Npgsql;

namespace AvitoCoinShop.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT user_id, name, balance, password_hash, created_at
                           FROM users
                           WHERE name = :username;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("username", username),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new User(
                reader.GetInt64(reader.GetOrdinal("user_id")),
                reader.GetString(reader.GetOrdinal("name")),
                reader.GetInt32(reader.GetOrdinal("balance")),
                reader.GetString(reader.GetOrdinal("password_hash")),
                reader.GetDateTime(reader.GetOrdinal("created_at"))
            );
        }

        return null;
    }
    
    public async Task<long> CreateUserAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
                           INSERT INTO users (name, balance, password_hash, created_at)
                           VALUES (:name, :balance, :password_hash, :created_at)
                           RETURNING user_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("name", user.Name),
                new NpgsqlParameter("balance", user.Balance),
                new NpgsqlParameter("password_hash", user.PasswordHash),
                new NpgsqlParameter("created_at", user.CreatedAt),
            },
        };

        object? generatedId = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt64(generatedId);
    }

}