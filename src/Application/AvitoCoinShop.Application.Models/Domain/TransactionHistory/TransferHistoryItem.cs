namespace AvitoCoinShop.Application.Models.Domain.TransactionHistory;

public record TransferHistoryItem 
(
    long? Id,
    long SenderId,
    long ReceiverId,
    int Amount,
    DateTime CreatedAt);
