using AvitoCoinShop.Application.Models.Users;

namespace AvitoCoinShop.Application.Contracts;

public interface IUserService
{
    public Task<string> AuthAsync(string username, string hashedPassword, CancellationToken cancellationToken);
}