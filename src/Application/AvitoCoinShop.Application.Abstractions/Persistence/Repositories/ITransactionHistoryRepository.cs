using AvitoCoinShop.Application.Models.TransactionHistory;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface ITransactionHistoryRepository
{
    // TODO. What to return?
    public Task<long> LogTransferAsync(long senderId, long receiverId, long amount, DateTime createdAt, CancellationToken cancellationToken);
    
    public Task<long> LogPurchaseAsync(long itemId, int price, int amount, DateTime date, CancellationToken cancellationToken);

    public Task<IEnumerable<TransferHistoryItem>> GetIncomingTransfersAsync(long receiverId, CancellationToken cancellationToken);

    public Task<IEnumerable<TransferHistoryItem>> GetOutgoingTransfersAsync(long senderId, CancellationToken cancellationToken);

    public Task<IEnumerable<PurchaseHistoryItem>> GetPurchaseHistoryAsync(long userId, CancellationToken cancellationToken);
}