using AvitoCoinShop.Application.Models.Domain.TransactionHistory;

namespace AvitoCoinShop.Application.Contracts;

public interface ITransactionHistoryService
{
    public Task<long> LogTransferAsync(long senderId, long receiverId, long amount, DateTime createdAt, CancellationToken cancellationToken);
    
    public Task<long> LogPurchaseAsync(long itemId, int price, int amount, DateTime date, CancellationToken cancellationToken);

    public Task<TransactionHistorySummary> GetTransactionHistorySummaryAsync(long userId, CancellationToken cancellationToken);
}