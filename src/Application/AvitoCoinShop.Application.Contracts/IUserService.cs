using AvitoCoinShop.Application.Models.Domain.Users;

namespace AvitoCoinShop.Application.Contracts;

public interface IUserService
{
    public Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    public Task<long> CreateUserAsync(User user, CancellationToken cancellationToken);
}