using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Merch;
using AvitoCoinShop.Application.Models.Domain.Merch;
using Moq;

namespace AvitoCoinShop.Application.Application.Tests;

[TestFixture]
public class MerchServiceTests
{
    private Mock<IMerchRepository> _merchRepositoryMock;
    private Mock<ITransactionHistoryService> _transactionHistoryServiceMock;
    private Mock<IWalletService> _walletServiceMock;
    private MerchService _merchService;

    [SetUp]
    public void SetUp()
    {
        _merchRepositoryMock = new Mock<IMerchRepository>();
        _transactionHistoryServiceMock = new Mock<ITransactionHistoryService>();
        _walletServiceMock = new Mock<IWalletService>();

        _merchService = new MerchService(
            _merchRepositoryMock.Object,
            _transactionHistoryServiceMock.Object,
            _walletServiceMock.Object);
    }

    [Test]
    public async Task BuyMerchAsync_ShouldCompletePurchase_WhenUserHasEnoughBalance()
    {
        // Arrange
        long userId = 1;
        string merchName = "Cool T-Shirt";
        long merchId = 10;
        int price = 500;
        long initialBalance = 1000;
        bool userBoughtBefore = false;

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchIdByNameAsync(merchName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(merchId);

        _walletServiceMock
            .Setup(wallet => wallet.GetBalanceAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialBalance);

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchPriceAsync(merchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(price);

        _merchRepositoryMock
            .Setup(repo => repo.CheckIfUserBoughtItemAsync(userId, merchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userBoughtBefore);

        _walletServiceMock
            .Setup(wallet => wallet.RemoveCoinsAsync(userId, price, It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialBalance - price);

        _merchRepositoryMock
            .Setup(repo => repo.BuyThisItemForFirstTimeAsync(userId, merchId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionHistoryServiceMock
            .Setup(history => history.LogPurchaseAsync(userId, merchId, price, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        long result = await _merchService.BuyMerchAsync(userId, merchName, CancellationToken.None);

        // Assert
        Assert.AreEqual(merchId, result);
        _walletServiceMock.Verify(wallet => wallet.RemoveCoinsAsync(userId, price, It.IsAny<CancellationToken>()), Times.Once);
        _merchRepositoryMock.Verify(repo => repo.BuyThisItemForFirstTimeAsync(userId, merchId, It.IsAny<CancellationToken>()), Times.Once);
        _transactionHistoryServiceMock.Verify(history => history.LogPurchaseAsync(userId, merchId, price, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void BuyMerchAsync_ShouldThrowException_WhenMerchDoesNotExist()
    {
        // Arrange
        long userId = 1;
        string merchName = "Unknown Item";

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchIdByNameAsync(merchName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((long?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _merchService.BuyMerchAsync(userId, merchName, CancellationToken.None));

        Assert.AreEqual("Merch with name given does not exist", ex.Message);
    }

    [Test]
    public void BuyMerchAsync_ShouldThrowException_WhenNotEnoughBalance()
    {
        // Arrange
        long userId = 1;
        string merchName = "Cool T-Shirt";
        long merchId = 10;
        int price = 500;
        long initialBalance = 300;

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchIdByNameAsync(merchName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(merchId);

        _walletServiceMock
            .Setup(wallet => wallet.GetBalanceAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(initialBalance);

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchPriceAsync(merchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(price);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _merchService.BuyMerchAsync(userId, merchName, CancellationToken.None));

        Assert.AreEqual("Not enough coins", ex.Message);
    }

    [Test]
    public async Task GetMerchItemsBoughtByUser_ShouldReturnItems()
    {
        // Arrange
        long userId = 1;
        var expectedItems = new List<UserMerchItem>
        {
            new UserMerchItem(10, 1),
            new UserMerchItem(20, 2)
        };

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchItemsBoughtByUser(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedItems);

        // Act
        IEnumerable<UserMerchItem> items = await _merchService.GetMerchItemsBoughtByUser(userId, CancellationToken.None);

        // Assert
        CollectionAssert.AreEquivalent(expectedItems, items);
        _merchRepositoryMock.Verify(repo => repo.GetMerchItemsBoughtByUser(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetMerchPriceAsync_ShouldReturnPrice()
    {
        // Arrange
        long merchItemId = 10;
        int expectedPrice = 500;

        _merchRepositoryMock
            .Setup(repo => repo.GetMerchPriceAsync(merchItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPrice);

        // Act
        int price = await _merchService.GetMerchPriceAsync(merchItemId, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedPrice, price);
        _merchRepositoryMock.Verify(repo => repo.GetMerchPriceAsync(merchItemId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

