using System.Data.Common;
using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Models.Domain.Merch;
using Npgsql;

namespace AvitoCoinShop.Infrastructure.Persistence.Repositories;

public class MerchRepository : IMerchRepository
{
    private readonly NpgsqlDataSource _dataSource;
    
    public MerchRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
    public async Task<long> BuyMerchAsync(long userId, long merchId, CancellationToken cancellationToken)
    {
        const string sql = @"
        INSERT INTO user_merch (user_id, merch_id, amount)
        VALUES (:user_id, :merch_id, 1)
        ON CONFLICT (user_id, merch_id) 
        DO UPDATE SET amount = user_merch.amount + 1
        RETURNING id;
    ";

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        await using DbCommand command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add(new NpgsqlParameter("user_id", userId));
        command.Parameters.Add(new NpgsqlParameter("merch_id", merchId));

        var result = await command.ExecuteScalarAsync(cancellationToken);
    
        return result is long merchItemId ? merchItemId : throw new Exception("Не удалось получить id купленного мерча.");
    }


    public async Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT user_id, merch_id, amount
                           FROM user_merch
                           WHERE user_id = :user_id;
                           """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("user_id", userId),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var userMerchList = new List<UserMerchItem>();
        while (await reader.ReadAsync(cancellationToken))
        {
            userMerchList.Add(new UserMerchItem(
                reader.GetInt64(reader.GetOrdinal("merch_id")),
                reader.GetInt32(reader.GetOrdinal("amount"))
            ));
        }

        return userMerchList;
    }


    public async Task<int> GetMerchPriceAsync(long merchItemId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT price FROM merch_items WHERE id = :merch_id";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("merch_id", merchItemId),
            },
        };
        
        
        object? price = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(price);
    }

    public async Task<long?> GetMerchIdByNameAsync(string merchName, CancellationToken cancellationToken)
    {
        const string sql = "SELECT id FROM merch_items WHERE name = :merch_name";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("merch_name", merchName),
            },
        };

        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return result is long merchId ? merchId : null;
    }

}