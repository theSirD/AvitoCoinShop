using System.Security.Claims;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Presentation.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AvitoCoinShop.Presentation.Http.Tests;

[TestFixture]
public class InfoControllerTests
{
    private Mock<IWalletService> _walletServiceMock;
    private Mock<IMerchService> _merchServiceMock;
    private Mock<ITransactionHistoryService> _transactionHistoryServiceMock;
    private InfoController _infoController;

    [SetUp]
    public void SetUp()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _merchServiceMock = new Mock<IMerchService>();
        _transactionHistoryServiceMock = new Mock<ITransactionHistoryService>();
        _infoController = new InfoController(
            _walletServiceMock.Object, 
            _merchServiceMock.Object, 
            _transactionHistoryServiceMock.Object);
    }

    [Test]
    public async Task GetInfoAsync_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _infoController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        IActionResult result = await _infoController.GetInfoAsync(CancellationToken.None);

        // Assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [Test]
    public async Task GetInfoAsync_ShouldReturnInternalServerError_WhenServiceThrowsException()
    {
        // Arrange
        string userId = "1";
        string errorMessage = "Service failure";

        _infoController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }))
            }
        };

        _walletServiceMock.Setup(service => service.GetBalanceAsync(long.Parse(userId), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new System.Exception(errorMessage));

        // Act
        IActionResult result = await _infoController.GetInfoAsync(CancellationToken.None);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
        Assert.AreEqual(errorMessage, statusCodeResult.Value);
    }
}
