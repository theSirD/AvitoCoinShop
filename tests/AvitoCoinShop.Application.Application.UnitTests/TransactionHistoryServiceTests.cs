using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Models.Domain.TransactionHistory;
using AvitoCoinShop.Application.Transactions;
using Moq;

namespace AvitoCoinShop.Application.Application.Tests;

[TestFixture]
public class TransactionHistoryServiceTests
{
    private Mock<ITransactionHistoryRepository> _transactionHistoryRepositoryMock;
    private TransactionHistoryService _transactionHistoryService;

    [SetUp]
    public void SetUp()
    {
        _transactionHistoryRepositoryMock = new Mock<ITransactionHistoryRepository>();
        _transactionHistoryService = new TransactionHistoryService(_transactionHistoryRepositoryMock.Object);
    }

    [Test]
    public async Task LogTransferAsync_ShouldReturnEntryId()
    {
        // Arrange
        long senderId = 1;
        long receiverId = 2;
        long amount = 500;
        DateTime createdAt = DateTime.UtcNow;
        long expectedEntryId = 123;

        _transactionHistoryRepositoryMock
            .Setup(repo => repo.LogTransferAsync(senderId, receiverId, amount, createdAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntryId);

        // Act
        long entryId = await _transactionHistoryService.LogTransferAsync(senderId, receiverId, amount, createdAt, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedEntryId, entryId);
        _transactionHistoryRepositoryMock.Verify(repo => repo.LogTransferAsync(senderId, receiverId, amount, createdAt, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task LogPurchaseAsync_ShouldReturnEntryId()
    {
        // Arrange
        long userId = 1;
        long itemId = 42;
        int price = 150;
        DateTime date = DateTime.UtcNow;
        long expectedEntryId = 456;

        _transactionHistoryRepositoryMock
            .Setup(repo => repo.LogPurchaseAsync(userId, itemId, price, date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEntryId);

        // Act
        long entryId = await _transactionHistoryService.LogPurchaseAsync(userId, itemId, price, date, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedEntryId, entryId);
        _transactionHistoryRepositoryMock.Verify(repo => repo.LogPurchaseAsync(userId, itemId, price, date, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetTransactionHistorySummaryAsync_ShouldReturnTransactionSummary()
    {
        // Arrange
        long userId = 1;
        var incomingTransfers = new List<TransferHistoryItem>
        {
            new TransferHistoryItem(1, 2, userId, 300, DateTime.UtcNow)
        };
        var outgoingTransfers = new List<TransferHistoryItem>
        {
            new TransferHistoryItem(2, userId, 3, 200, DateTime.UtcNow)
        };
        var purchaseHistory = new List<PurchaseHistoryItem>
        {
            new PurchaseHistoryItem(1, 101, 500, DateTime.UtcNow)
        };

        _transactionHistoryRepositoryMock
            .Setup(repo => repo.GetIncomingTransfersAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(incomingTransfers);

        _transactionHistoryRepositoryMock
            .Setup(repo => repo.GetOutgoingTransfersAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(outgoingTransfers);

        _transactionHistoryRepositoryMock
            .Setup(repo => repo.GetPurchaseHistoryAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(purchaseHistory);

        // Act
        TransactionHistorySummary summary = await _transactionHistoryService.GetTransactionHistorySummaryAsync(userId, CancellationToken.None);

        // Assert
        Assert.AreEqual(incomingTransfers, summary.IncomingTransfers);
        Assert.AreEqual(outgoingTransfers, summary.OutgoingTransfers);
        Assert.AreEqual(purchaseHistory, summary.Purchases);

        _transactionHistoryRepositoryMock.Verify(repo => repo.GetIncomingTransfersAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _transactionHistoryRepositoryMock.Verify(repo => repo.GetOutgoingTransfersAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _transactionHistoryRepositoryMock.Verify(repo => repo.GetPurchaseHistoryAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

