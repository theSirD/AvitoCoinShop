namespace AvitoCoinShop.Application.Contracts;

public interface IAuthService
{
    public Task<string> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
}