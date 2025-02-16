using System.Security.Claims;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Presentation.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AvitoCoinShop.Presentation.Http.Tests;

[TestFixture]
public class BuyControllerTests
{
    private Mock<IMerchService> _merchServiceMock;
    private BuyController _buyController;

    [SetUp]
    public void SetUp()
    {
        _merchServiceMock = new Mock<IMerchService>();
        _buyController = new BuyController(_merchServiceMock.Object);
    }

    [Test]
    public async Task BuyItemAsync_ShouldReturnOk_WhenPurchaseIsSuccessful()
    {
        // Arrange
        string itemName = "item1";
        long expectedMerchId = 123;

        _buyController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1") 
                }))
            }
        };

        _merchServiceMock.Setup(service => service.BuyMerchAsync(1, itemName, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedMerchId);

        // Act
        IActionResult result = await _buyController.BuyItemAsync(itemName, CancellationToken.None);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var merchId = okResult.Value;
        Assert.AreEqual(expectedMerchId, merchId);
    }
    
    [Test]
    public async Task BuyItemAsync_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        string itemName = "item1"; 

        _buyController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        IActionResult result = await _buyController.BuyItemAsync(itemName, CancellationToken.None);

        // Assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [Test]
    public async Task BuyItemAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        string itemName = "item1";
        string errorMessage = "Something went wrong";

        _buyController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1") 
                }))
            }
        };

        _merchServiceMock.Setup(service => service.BuyMerchAsync(1, itemName, It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception(errorMessage));

        // Act
        IActionResult result = await _buyController.BuyItemAsync(itemName, CancellationToken.None);

        // Assert
        var statusCodeResult = result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
        Assert.AreEqual(errorMessage, statusCodeResult.Value);
    }
}
