using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Domain.Users;

namespace AvitoCoinShop.Application.User;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Models.Domain.Users.User> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        Models.Domain.Users.User? user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user is null)
            throw new Exception("User was not found");
        return user;
    }

    public async Task<long> CreateUserAsync(Models.Domain.Users.User user, CancellationToken cancellationToken)
    {
        Models.Domain.Users.User? existingUser = await GetByUsernameAsync(user.Name, cancellationToken);
        if (user is not null)
            throw new Exception("User with given name already exists");
        long userId = await _userRepository.CreateUserAsync(user, cancellationToken);
        return userId;
    }
}