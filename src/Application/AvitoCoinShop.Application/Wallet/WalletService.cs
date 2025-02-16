using System.Transactions;
using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;

namespace AvitoCoinShop.Application.Wallet;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    
    private readonly ITransactionHistoryService _transactionHistoryService;

    private readonly IUserService _userService;

    public WalletService(IWalletRepository walletRepository, ITransactionHistoryService transactionHistoryService, IUserService userService)
    {
        _walletRepository = walletRepository;
        _transactionHistoryService = transactionHistoryService;
        _userService = userService;
    }

    public async Task<long> GetBalanceAsync(long userId, CancellationToken cancellationToken)
    {
        long balance = await _walletRepository.GetBalanceAsync(userId, cancellationToken);
        return balance;
    }

    public async Task<long> TransferCoinsAsync(long senderId, string receiverName, long amount, CancellationToken cancellationToken)
    {
        Models.Domain.Users.User? receiver = await _userService.GetByUsernameAsync(receiverName, cancellationToken);
        if (receiver is null)
            throw new Exception("No user with such a name");
        if (receiver.UserId is null)
            throw new Exception("User id is null");
        
        long receiverId = receiver.UserId.Value;
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        await RemoveCoinsAsync(senderId, amount, cancellationToken);
        await AddCoinsAsync(receiverId, amount, cancellationToken);
        long transferId = 
            await _transactionHistoryService.LogTransferAsync(senderId, receiverId, amount, DateTime.UtcNow, cancellationToken);
        transaction.Complete();
        
        return transferId;
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
}