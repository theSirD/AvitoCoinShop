using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.User;
using Moq;

namespace AvitoCoinShop.Application.Application.Tests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UserService _userService;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Test]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        string username = "testUser";
        var expectedUser = new Models.Domain.Users.User(1, username, 1000, "hashedPassword", DateTime.UtcNow);

        _userRepositoryMock
            .Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        Models.Domain.Users.User? user = await _userService.GetByUsernameAsync(username, CancellationToken.None);

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual(expectedUser, user);
        _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetByUsernameAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        string username = "unknownUser";

        _userRepositoryMock
            .Setup(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Domain.Users.User?)null);

        // Act
        Models.Domain.Users.User? user = await _userService.GetByUsernameAsync(username, CancellationToken.None);

        // Assert
        Assert.IsNull(user);
        _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(username, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateUserAsync_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Arrange
        var newUser = new Models.Domain.Users.User(null, "newUser", 0, "hashedPassword", DateTime.UtcNow);
        long expectedUserId = 1;

        _userRepositoryMock
            .Setup(repo => repo.GetByUsernameAsync(newUser.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Models.Domain.Users.User?)null);

        _userRepositoryMock
            .Setup(repo => repo.CreateUserAsync(newUser, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUserId);

        // Act
        long userId = await _userService.CreateUserAsync(newUser, CancellationToken.None);

        // Assert
        Assert.AreEqual(expectedUserId, userId);
        _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(newUser.Name, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(newUser, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void CreateUserAsync_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var existingUser = new Models.Domain.Users.User(1, "existingUser", 1000, "hashedPassword", DateTime.UtcNow);

        _userRepositoryMock
            .Setup(repo => repo.GetByUsernameAsync(existingUser.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _userService.CreateUserAsync(existingUser, CancellationToken.None));

        Assert.AreEqual("User with given name already exists", ex.Message);
        _userRepositoryMock.Verify(repo => repo.GetByUsernameAsync(existingUser.Name, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<Models.Domain.Users.User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

