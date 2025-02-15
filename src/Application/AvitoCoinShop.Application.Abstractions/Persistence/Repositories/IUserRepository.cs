using AvitoCoinShop.Application.Models.Users;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface IUserRepository
{
    public Task<long> AuthAsync(User user, CancellationToken cancellationToken);
}