using AvitoCoinShop.Application.Models.Merch;

namespace AvitoCoinShop.Application.Contracts;

public interface IMerchService
{
    public Task<long> BuyMerchAsync(long userId, long merchId, long amount, CancellationToken cancellationToken);
    
    public Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken);
    
    public Task<int> GetMerchPriceAsync(long merchItemId, int amount, CancellationToken cancellationToken);
}