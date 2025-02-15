using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.TransactionHistory;

namespace AvitoCoinShop.Application.Transactions;

public class TransactionHistoryService : ITransactionHistoryService
{
    private readonly ITransactionHistoryRepository _transactionHistoryRepository;

    public TransactionHistoryService(ITransactionHistoryRepository transactionHistoryRepository)
    {
        _transactionHistoryRepository = transactionHistoryRepository;
    }

    public async Task<long> LogTransferAsync(long senderId, long receiverId, long amount, DateTime createdAt,
        CancellationToken cancellationToken)
    {
        long entryId =
            await _transactionHistoryRepository.LogTransferAsync(senderId, receiverId, amount, createdAt, cancellationToken);
        return entryId;
    }

    public async Task<long> LogPurchaseAsync(long itemId, int price, int amount, DateTime date, CancellationToken cancellationToken)
    {
        long entryId =
            await _transactionHistoryRepository.LogPurchaseAsync(itemId, price, amount, date, cancellationToken);
        return entryId;
    }

    public async Task<TransactionHistorySummary> GetTransactionHistorySummaryAsync(long userId, CancellationToken cancellationToken)
    {
        var incomingTransfers =
            await _transactionHistoryRepository.GetIncomingTransfersAsync(userId, cancellationToken);
        var outgoingTransfers =
            await _transactionHistoryRepository.GetOutgoingTransfersAsync(userId, cancellationToken);
        var purchaseHistory = 
            await _transactionHistoryRepository.GetPurchaseHistoryAsync(userId, cancellationToken);
        
        var transactionHistorySummary = new TransactionHistorySummary();
        transactionHistorySummary.IncomingTransfers = incomingTransfers;
        transactionHistorySummary.OutgoingTransfers = outgoingTransfers;
        transactionHistorySummary.Purchases = purchaseHistory;

        return transactionHistorySummary;
    }
}