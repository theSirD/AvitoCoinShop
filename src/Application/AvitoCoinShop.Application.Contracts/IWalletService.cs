namespace AvitoCoinShop.Application.Contracts;

public interface IWalletService
{
    public Task<long> RemoveCoinsAsync(long userId, long amount, CancellationToken cancellationToken);
    
    public Task<long> AddCoinsAsync(long userId, long amount, CancellationToken cancellationToken);
    
    public Task<long> GetBalanceAsync(long userId, CancellationToken cancellationToken);
}