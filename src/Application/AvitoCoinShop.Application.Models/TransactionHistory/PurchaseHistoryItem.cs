namespace AvitoCoinShop.Application.Models.TransactionHistory;

public record PurchaseHistoryItem
(
    long PurchaseId,
    long ItemId,
    int Price,
    int Amount,
    DateTime Date
);