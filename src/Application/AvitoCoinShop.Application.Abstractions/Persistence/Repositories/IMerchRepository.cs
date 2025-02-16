using AvitoCoinShop.Application.Models.Merch;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface IMerchRepository
{
    public Task<long> BuyMerchAsync(long userId, long merchId, CancellationToken cancellationToken);
    
    public Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken);
    
    public Task<int> GetMerchPriceAsync(long merchItemId, CancellationToken cancellationToken);
}