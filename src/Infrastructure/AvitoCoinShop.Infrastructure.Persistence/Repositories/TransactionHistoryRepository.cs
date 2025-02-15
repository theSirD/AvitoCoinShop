using System.Data.Common;
using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Models.TransactionHistory;
using Npgsql;

namespace AvitoCoinShop.Infrastructure.Persistence.Repositories;

public class TransactionHistoryRepository : ITransactionHistoryRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public TransactionHistoryRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    
   public async Task<long> LogTransferAsync(long senderId, long receiverId, long amount, DateTime createdAt,
            CancellationToken cancellationToken)
        {
            const string sql = """
            INSERT INTO transaction_history (sender_id, receiver_id, amount, created_at)
            VALUES (:senderId, :receiverId, :amount, :createdAt)
            RETURNING transaction_id;
            """;

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using DbCommand command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("senderId", senderId),
                    new NpgsqlParameter("receiverId", receiverId),
                    new NpgsqlParameter("amount", amount),
                    new NpgsqlParameter("createdAt", createdAt)
                },
            };

            object? transactionId = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt64(transactionId);
        }

        public async Task<long> LogPurchaseAsync(long itemId, int price, int amount, DateTime date,
            CancellationToken cancellationToken)
        {
            const string sql = """
            INSERT INTO transaction_history (item_id, price, amount, created_at)
            VALUES (:itemId, :price, :amount, :createdAt)
            RETURNING transaction_id;
            """;

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using DbCommand command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("itemId", itemId),
                    new NpgsqlParameter("price", price),
                    new NpgsqlParameter("amount", amount),
                    new NpgsqlParameter("createdAt", date)
                },
            };

            object? transactionId = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt64(transactionId);
        }

        public async Task<IEnumerable<TransferHistoryItem>> GetIncomingTransfersAsync(long receiverId, CancellationToken cancellationToken)
        {
            const string sql = """
                               SELECT id, sender_id, receiver_id, amount, created_at
                               FROM transfer_history
                               WHERE receiver_id = :receiver_id
                               ORDER BY created_at DESC;
                               """;

            await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using DbCommand command = new NpgsqlCommand(sql, connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("receiver_id", receiverId),
                },
            };

            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            var transfers = new List<TransferHistoryItem>();
            while (await reader.ReadAsync(cancellationToken))
            {
                transfers.Add(new TransferHistoryItem(
                    reader.IsDBNull(reader.GetOrdinal("id")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("id")),
                    reader.GetInt64(reader.GetOrdinal("sender_id")),
                    reader.GetInt64(reader.GetOrdinal("receiver_id")),
                    reader.GetInt32(reader.GetOrdinal("amount")),
                    reader.GetDateTime(reader.GetOrdinal("created_at"))
                ));
            }

            return transfers;
        }
        

    public async Task<IEnumerable<TransferHistoryItem>> GetOutgoingTransfersAsync(long senderId, CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT id, sender_id, receiver_id, amount, created_at
        FROM transfer_history
        WHERE sender_id = :sender_id
        ORDER BY created_at DESC;
        """;

        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using DbCommand command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("sender_id", senderId),
            },
        };

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        var transfers = new List<TransferHistoryItem>();
        while (await reader.ReadAsync(cancellationToken))
        {
            transfers.Add(new TransferHistoryItem(
                reader.IsDBNull(reader.GetOrdinal("id")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("id")),
                reader.GetInt64(reader.GetOrdinal("sender_id")),
                reader.GetInt64(reader.GetOrdinal("receiver_id")),
                reader.GetInt32(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("created_at"))
            ));
        }

        return transfers;
    }
    
    public async Task<IEnumerable<PurchaseHistoryItem>> GetPurchaseHistoryAsync(long userId, CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT ph.id, ph.item_id, ph.price, ph.amount, ph.created_at
                           FROM purchase_history ph
                           WHERE ph.user_id = :user_id
                           ORDER BY ph.created_at DESC;
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

        var purchaseHistoryList = new List<PurchaseHistoryItem>();
        while (await reader.ReadAsync(cancellationToken))
        {
            purchaseHistoryList.Add(new PurchaseHistoryItem(
                reader.GetInt64(reader.GetOrdinal("id")),
                reader.GetInt64(reader.GetOrdinal("item_id")),
                reader.GetInt32(reader.GetOrdinal("price")),
                reader.GetInt32(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("created_at"))
            ));
        }

        return purchaseHistoryList;
    }
}