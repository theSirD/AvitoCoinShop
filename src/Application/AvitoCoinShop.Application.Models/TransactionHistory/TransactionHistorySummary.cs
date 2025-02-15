namespace AvitoCoinShop.Application.Models.TransactionHistory;

public record TransactionHistorySummary(
    List<TransferHistoryItem> IncomingTransfers,
    List<TransferHistoryItem> OutgoingTransfers,
    List<PurchaseHistoryItem> Purchases);