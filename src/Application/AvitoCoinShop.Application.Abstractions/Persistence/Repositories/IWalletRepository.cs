namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface IWalletRepository
{
    public Task<long> RemoveCoinsAsync(long userId, long amount, CancellationToken cancellationToken);
    
    public Task<long> AddCoinsAsync(long userId, long amount, CancellationToken cancellationToken);
    
    public Task<long> GetBalanceAsync(long userId, CancellationToken cancellationToken);
}