using AvitoCoinShop.Application.Models.Users;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    public Task<long> CreateUserAsync(User user, CancellationToken cancellationToken);
}