using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Users;

namespace AvitoCoinShop.Application.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<string> AuthAsync(string username, string hashedPassword, CancellationToken cancellationToken)
    {
        User? existingUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        
        if (existingUser is null)
        {
            User newUser = new User(
                null,
                username,
                1000,
                hashedPassword,
                DateTime.UtcNow);
            long userId = await _userRepository.CreateUserAsync(newUser, cancellationToken);
            // TODO. Return JWT token
            return userId.ToString();
        }
        // TODO. Add custom exception
        if (existingUser.PasswordHash != hashedPassword)
            throw new Exception("Wrong password");
        // TODO. Return JWT token
        return existingUser.UserId.ToString();
    }
}