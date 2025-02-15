namespace AvitoCoinShop.Application.Models.TransactionHistory;

public class TransactionHistorySummary
{
    public IEnumerable<TransferHistoryItem>? IncomingTransfers { get; set; }
    public IEnumerable<TransferHistoryItem>? OutgoingTransfers { get; set; }
    public IEnumerable<PurchaseHistoryItem>? Purchases { get; set; }

    public TransactionHistorySummary()
    {
        IncomingTransfers = new List<TransferHistoryItem>();
        OutgoingTransfers = new List<TransferHistoryItem>();
        Purchases = new List<PurchaseHistoryItem>();
    }
}