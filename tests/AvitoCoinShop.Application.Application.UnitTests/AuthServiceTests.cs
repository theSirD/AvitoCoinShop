using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AvitoCoinShop.Application.Auth;
using AvitoCoinShop.Application.Contracts;
using Microsoft.Extensions.Options;
using Moq;

namespace AvitoCoinShop.Application.Application.Tests;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserService> _userServiceMock;
    private IAuthService _authService;
    private JwtOptions _jwtOptions;
    private Mock<IOptions<JwtOptions>> _jwtOptionsMock;

    [SetUp]
    public void SetUp()
    {
        _userServiceMock = new Mock<IUserService>();

        _jwtOptions = new JwtOptions
        {
            Secret = "SuperSecretKey12345SuperSecretKey12345",
            TokenLifetimeDays = 7
        };

        _jwtOptionsMock = new Mock<IOptions<JwtOptions>>();
        _jwtOptionsMock.Setup(o => o.Value).Returns(_jwtOptions);

        _authService = new AuthService(_userServiceMock.Object, _jwtOptionsMock.Object);
    }

    [Test]
    public async Task AuthenticateAsync_ShouldRegisterNewUser_AndReturnJwtToken_WhenUserDoesNotExist()
    {
        // Arrange
        string username = "newUser";
        string password = "password123";
        long userId = 1;

        _userServiceMock
            .Setup(svc => svc.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Domain.Users.User?)null);

        _userServiceMock
            .Setup(svc => svc.CreateUserAsync(It.IsAny<Models.Domain.Users.User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);

        // Act
        string token = await _authService.AuthenticateAsync(username, password, CancellationToken.None);

        // Assert
        Assert.IsNotNull(token);
        Assert.IsNotEmpty(token);
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.AreEqual("AvitoCoinShop", jwtToken.Issuer);
        Assert.AreEqual("AvitoCoinShopClient", jwtToken.Audiences.First());
        Assert.AreEqual(username, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.AreEqual(userId.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }

    [Test]
    public async Task AuthenticateAsync_ShouldReturnJwtToken_WhenUserExists_AndPasswordIsCorrect()
    {
        // Arrange
        string username = "existingUser";
        string password = "securePass";
        long userId = 42;
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var existingUser = new Models.Domain.Users.User(userId, username, 1000, passwordHash, DateTime.UtcNow);

        _userServiceMock
            .Setup(svc => svc.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        string token = await _authService.AuthenticateAsync(username, password, CancellationToken.None);

        // Assert
        Assert.IsNotNull(token);
        Assert.IsNotEmpty(token);
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.AreEqual("AvitoCoinShop", jwtToken.Issuer);
        Assert.AreEqual("AvitoCoinShopClient", jwtToken.Audiences.First());
        Assert.AreEqual(username, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
        Assert.AreEqual(userId.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }

    [Test]
    public void AuthenticateAsync_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        string username = "existingUser";
        string correctPassword = "rightPass";
        string wrongPassword = "wrongPass";
        long userId = 42;
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword);

        var existingUser = new Models.Domain.Users.User(userId, username, 1000, passwordHash, DateTime.UtcNow);

        _userServiceMock
            .Setup(svc => svc.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _authService.AuthenticateAsync(username, wrongPassword, CancellationToken.None));

        Assert.AreEqual("Wrong password", ex.Message);
    }
}
