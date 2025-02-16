namespace AvitoCoinShop.Application.Models.Domain.TransactionHistory;

public record TransactionHistorySummary(
    IEnumerable<TransferHistoryItem> IncomingTransfers,
    IEnumerable<TransferHistoryItem> OutgoingTransfers,
    IEnumerable<PurchaseHistoryItem> Purchases);