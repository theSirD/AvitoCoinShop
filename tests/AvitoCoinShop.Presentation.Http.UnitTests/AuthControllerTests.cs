using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Dto;
using AvitoCoinShop.Presentation.Http.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AvitoCoinShop.Presentation.Http.Tests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock;
    private AuthController _authController;

    [SetUp]
    public void SetUp()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }
    
    [Test]
    public async Task AuthAsync_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        // Act
        IActionResult result = await _authController.AuthAsync(null, CancellationToken.None);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Username and Password are required", badRequestResult.Value);
    }

    [Test]
    public async Task AuthAsync_ShouldReturnBadRequest_WhenUsernameOrPasswordIsEmpty()
    {
        // Arrange
        var requestWithEmptyUsername = new AuthRequest("", "password123");
        var requestWithEmptyPassword = new AuthRequest("testuser", "");

        // Act
        IActionResult result1 = await _authController.AuthAsync(requestWithEmptyUsername, CancellationToken.None);
        IActionResult result2 = await _authController.AuthAsync(requestWithEmptyPassword, CancellationToken.None);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result1);
        Assert.AreEqual(400, ((BadRequestObjectResult)result1).StatusCode);
        Assert.AreEqual("Username and Password are required", ((BadRequestObjectResult)result1).Value);

        Assert.IsInstanceOf<BadRequestObjectResult>(result2);
        Assert.AreEqual(400, ((BadRequestObjectResult)result2).StatusCode);
        Assert.AreEqual("Username and Password are required", ((BadRequestObjectResult)result2).Value);
    }

    [Test]
    public async Task AuthAsync_ShouldReturnUnauthorized_WhenAuthenticationFails()
    {
        // Arrange
        
        var request = new AuthRequest("testuser", "wrongpassword");

        _authServiceMock
            .Setup(service => service.AuthenticateAsync(request.Username, request.Password, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Wrong password"));

        // Act
        IActionResult result = await _authController.AuthAsync(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }
}