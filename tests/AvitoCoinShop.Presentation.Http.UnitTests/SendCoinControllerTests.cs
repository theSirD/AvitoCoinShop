using System.Security.Claims;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Dto;
using AvitoCoinShop.Presentation.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AvitoCoinShop.Presentation.Http.Tests;

public class SendCoinControllerTests
{
    private Mock<IWalletService> _walletServiceMock;
    private SendCoinController _sendCoinController;

    [SetUp]
    public void SetUp()
    {
        _walletServiceMock = new Mock<IWalletService>();
        _sendCoinController = new SendCoinController(_walletServiceMock.Object);
    }

    [Test]
    public async Task SendCoinsAsync_ShouldReturnOk_WhenTransferIsSuccessful()
    {
        // Arrange
        var request = new SendCoinsRequest("receiverUser", 500);
        long expectedTransactionId = 12345;
        
        _walletServiceMock
            .Setup(service => service.TransferCoinsAsync(1, request.ToUser, request.Amount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTransactionId);

        _sendCoinController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }))
            }
        };

        // Act
        IActionResult result = await _sendCoinController.SendCoinsAsync(request, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
        Assert.AreEqual(200, okResult.StatusCode, "Expected HTTP 200 status code.");
        Assert.AreEqual(expectedTransactionId, okResult.Value);
    }

    [Test]
    public async Task SendCoinsAsync_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        // Arrange
        var request = new SendCoinsRequest("receiverUser", 500);

        _sendCoinController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Act
        IActionResult result = await _sendCoinController.SendCoinsAsync(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult, "Expected UnauthorizedResult but got null.");
        Assert.AreEqual(401, unauthorizedResult.StatusCode, "Expected HTTP 401 Unauthorized.");
    }

    [Test]
    public async Task SendCoinsAsync_ShouldReturnInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var request = new SendCoinsRequest("receiverUser", 500);

        _walletServiceMock
            .Setup(service => service.TransferCoinsAsync(1, request.ToUser, request.Amount, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Exception("Something went wrong"));

        _sendCoinController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }))
            }
        };

        // Act
        IActionResult result = await _sendCoinController.SendCoinsAsync(request, CancellationToken.None);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult, "Expected ObjectResult but got null.");
        Assert.AreEqual(500, objectResult.StatusCode, "Expected HTTP 500 Internal Server Error.");
        Assert.AreEqual("Something went wrong", objectResult.Value);
    }
}
