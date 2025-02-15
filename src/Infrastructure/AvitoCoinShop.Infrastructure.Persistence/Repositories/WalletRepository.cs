using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using Npgsql;
using System.Data.Common;

namespace AvitoCoinShop.Infrastructure.Persistence.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public WalletRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> RemoveCoinsAsync(long userId, long amount, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE users
        SET balance = balance - :amount
        WHERE id = :user_id AND balance >= :amount
        RETURNING balance;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("user_id", userId),
                new NpgsqlParameter("amount", amount),
            },
        };

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null)
        {
            throw new InvalidOperationException("Insufficient funds or user not found.");
        }

        return Convert.ToInt64(result);
    }

    public async Task<long> AddCoinsAsync(long userId, long amount, CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE users
        SET balance = balance + :amount
        WHERE id = :user_id
        RETURNING balance;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("user_id", userId),
                new NpgsqlParameter("amount", amount),
            },
        };

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        return Convert.ToInt64(result);
    }

    public async Task<long> GetBalanceAsync(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT balance FROM users WHERE id = :user_id;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("user_id", userId),
            },
        };

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }

        return Convert.ToInt64(result);
    }
}
