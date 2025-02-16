namespace AvitoCoinShop.Application.Models.Domain.TransactionHistory;

public record PurchaseHistoryItem
(
    long PurchaseId,
    long ItemId,
    int Price,
    DateTime Date
);