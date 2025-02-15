namespace AvitoCoinShop.Application.Models.TransactionHistory;

public record TransactionHistorySummary(
    IEnumerable<TransferHistoryItem> IncomingTransfers,
    IEnumerable<TransferHistoryItem> OutgoingTransfers,
    IEnumerable<PurchaseHistoryItem> Purchases);