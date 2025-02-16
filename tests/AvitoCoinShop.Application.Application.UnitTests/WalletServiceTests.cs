using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Wallet;
using Moq;

namespace AvitoCoinShop.Application.Application.Tests;

[TestFixture]
public class WalletServiceTests
{
    private Mock<IWalletRepository> _walletRepositoryMock;
    private Mock<ITransactionHistoryService> _transactionHistoryServiceMock;
    private Mock<IUserService> _userServiceMock;
    private WalletService _walletService;

    [SetUp]
    public void SetUp()
    {
        _walletRepositoryMock = new Mock<IWalletRepository>();
        _transactionHistoryServiceMock = new Mock<ITransactionHistoryService>();
        _userServiceMock = new Mock<IUserService>();

        _walletService = new WalletService(
            _walletRepositoryMock.Object,
            _transactionHistoryServiceMock.Object,
            _userServiceMock.Object);
    }

    [Test]
    public async Task GetBalanceAsync_ShouldReturnBalance()
    {
        // Arrange
        long userId = 1;
        long expectedBalance = 1000;
        _walletRepositoryMock
            .Setup(repo => repo.GetBalanceAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBalance);

        // Act
        long balance = await _walletService.GetBalanceAsync(userId, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedBalance, balance);
        _walletRepositoryMock.Verify(repo => repo.GetBalanceAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task TransferCoinsAsync_ShouldTransferCoins_WhenReceiverExists()
    {
        // Arrange
        long senderId = 1;
        long receiverId = 2;
        string receiverName = "testUser";
        long amount = 500;
        long expectedTransferId = 123;

        var receiver = new Models.Domain.Users.User(
            UserId: receiverId,
            Name: receiverName,
            Balance: 1000,
            PasswordHash: "hashedPassword",
            CreatedAt: DateTime.UtcNow
        );

        _userServiceMock
            .Setup(service => service.GetByUsernameAsync(receiverName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(receiver);

        _walletRepositoryMock
            .Setup(repo => repo.GetBalanceAsync(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1000);

        _walletRepositoryMock
            .Setup(repo => repo.RemoveCoinsAsync(senderId, amount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(500);

        _walletRepositoryMock
            .Setup(repo => repo.AddCoinsAsync(receiverId, amount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1500);

        _transactionHistoryServiceMock
            .Setup(service => service.LogTransferAsync(senderId, receiverId, amount, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTransferId);

        // Act
        long transferId = await _walletService.TransferCoinsAsync(senderId, receiverName, amount, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedTransferId, transferId);

        _walletRepositoryMock.Verify(repo => repo.RemoveCoinsAsync(senderId, amount, It.IsAny<CancellationToken>()), Times.Once);
        _walletRepositoryMock.Verify(repo => repo.AddCoinsAsync(receiverId, amount, It.IsAny<CancellationToken>()), Times.Once);
        _transactionHistoryServiceMock.Verify(service => service.LogTransferAsync(senderId, receiverId, amount, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void TransferCoinsAsync_ShouldThrowException_WhenReceiverNotFound()
    {
        // Arrange
        long senderId = 1;
        string receiverName = "unknownUser";
        long amount = 500;

        _userServiceMock
            .Setup(service => service.GetByUsernameAsync(receiverName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Domain.Users.User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _walletService.TransferCoinsAsync(senderId, receiverName, amount, CancellationToken.None));

        Assert.AreEqual("No user with such a name", ex.Message);
    }

    [Test]
    public async Task RemoveCoinsAsync_ShouldReduceBalance_WhenEnoughFunds()
    {
        // Arrange
        long userId = 1;
        long initialBalance = 1000;
        long amountToRemove = 300;
        long expectedBalance = 700;

        _walletRepositoryMock
            .Setup(repo => repo.GetBalanceAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialBalance);

        _walletRepositoryMock
            .Setup(repo => repo.RemoveCoinsAsync(userId, amountToRemove, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBalance);

        // Act
        long newBalance = await _walletService.RemoveCoinsAsync(userId, amountToRemove, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedBalance, newBalance);
        _walletRepositoryMock.Verify(repo => repo.RemoveCoinsAsync(userId, amountToRemove, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void RemoveCoinsAsync_ShouldThrowException_WhenNotEnoughFunds()
    {
        // Arrange
        long userId = 1;
        long initialBalance = 200;
        long amountToRemove = 300;

        _walletRepositoryMock
            .Setup(repo => repo.GetBalanceAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialBalance);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _walletService.RemoveCoinsAsync(userId, amountToRemove, CancellationToken.None));

        Assert.AreEqual("Not enough coins", ex.Message);
    }

    [Test]
    public async Task AddCoinsAsync_ShouldIncreaseBalance()
    {
        // Arrange
        long userId = 1;
        long initialBalance = 1000;
        long amountToAdd = 500;
        long expectedBalance = 1500;

        _walletRepositoryMock
            .Setup(repo => repo.AddCoinsAsync(userId, amountToAdd, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBalance);

        // Act
        long newBalance = await _walletService.AddCoinsAsync(userId, amountToAdd, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedBalance, newBalance);
        _walletRepositoryMock.Verify(repo => repo.AddCoinsAsync(userId, amountToAdd, It.IsAny<CancellationToken>()), Times.Once);
    }
}
