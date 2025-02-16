using System.Transactions;
using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Merch;

namespace AvitoCoinShop.Application.Merch;

public class MerchService : IMerchService
{
    private readonly IMerchRepository _merchRepository;
    
    private readonly ITransactionHistoryService _transactionHistoryService;
    
    private readonly IWalletService _walletService;

    public MerchService(IMerchRepository merchRepository, ITransactionHistoryService transactionHistoryService, IWalletService walletService)
    {
        _merchRepository = merchRepository;
        _transactionHistoryService = transactionHistoryService;
        _walletService = walletService;
    }

    public async Task<long> BuyMerchAsync(long userId, long merchId, CancellationToken cancellationToken)
    {
        long balance = await _walletService.GetBalanceAsync(userId, cancellationToken);
        int price = await GetMerchPriceAsync(merchId, cancellationToken);
        if (balance - price < 0)
            throw new Exception("Not enough coins");

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);
        await _walletService.RemoveCoinsAsync(userId, price, cancellationToken);
        long merchItemOfUserId = await _merchRepository.BuyMerchAsync(userId, merchId, cancellationToken);
        
        await _transactionHistoryService.LogPurchaseAsync(merchId, price, 1, DateTime.UtcNow, cancellationToken);
        transaction.Complete();
        
        return merchItemOfUserId;
    }

    public async Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken)
    {
        IEnumerable<UserMerchItem> items = await _merchRepository.GetMerchItemsBoughtByUser(userId, cancellationToken);
        return items;
    }

    public async Task<int> GetMerchPriceAsync(long merchItemId, CancellationToken cancellationToken)
    {
        int price = await _merchRepository.GetMerchPriceAsync(merchItemId, cancellationToken);
        return price;
    }
}