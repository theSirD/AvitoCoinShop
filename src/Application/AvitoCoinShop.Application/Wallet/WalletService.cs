using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;

namespace AvitoCoinShop.Application.Wallet;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;

    public WalletService(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<long> RemoveCoinsAsync(long userId, long amount, CancellationToken cancellationToken)
    {
        long balance = await GetBalanceAsync(userId, cancellationToken);
        // TODO. Replace with custom exceptions
        if (balance - amount < 0)
            throw new Exception("Not enough coins");
        
        balance = await _walletRepository.RemoveCoinsAsync(userId, amount, cancellationToken);
        return balance;
    }

    public async Task<long> AddCoinsAsync(long userId, long amount, CancellationToken cancellationToken)
    {
        long balance = await _walletRepository.AddCoinsAsync(userId, amount, cancellationToken);
        return balance;
    }

    public async Task<long> GetBalanceAsync(long userId, CancellationToken cancellationToken)
    {
        long balance = await _walletRepository.GetBalanceAsync(userId, cancellationToken);
        return balance;
    }
}