using AvitoCoinShop.Application.Models.Merch;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface IMerchRepository
{
    // TODO. What to return?
    public Task<long> BuyMerchAsync(long userId, long merchId, long amount, CancellationToken cancellationToken);
    
    public Task<List<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken);
    
    public Task<int> GetMerchPriceAsync(long merchItemId, int amount, CancellationToken cancellationToken);
}