namespace AvitoCoinShop.Application.Models.TransactionHistory;

public record TransferHistoryItem
(
    long? Id,
    long SenderId,
    long ReceiverId,
    int Amount,
    DateTime CreatedAt);
