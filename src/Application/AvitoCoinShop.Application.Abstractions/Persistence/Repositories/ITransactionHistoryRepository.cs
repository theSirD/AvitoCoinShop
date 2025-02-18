using AvitoCoinShop.Application.Models.Domain.TransactionHistory;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface ITransactionHistoryRepository
{
    public Task<long> LogTransferAsync(long senderId, long receiverId, long amount, DateTime createdAt, CancellationToken cancellationToken);
    
    public Task<long> LogPurchaseAsync(long userId, long itemId, int price, DateTime date, CancellationToken cancellationToken);

    public Task<IEnumerable<TransferHistoryItem>> GetIncomingTransfersAsync(long receiverId, CancellationToken cancellationToken);

    public Task<IEnumerable<TransferHistoryItem>> GetOutgoingTransfersAsync(long senderId, CancellationToken cancellationToken);

    public Task<IEnumerable<PurchaseHistoryItem>> GetPurchaseHistoryAsync(long userId, CancellationToken cancellationToken);
}