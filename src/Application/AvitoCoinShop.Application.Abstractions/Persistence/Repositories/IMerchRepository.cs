using AvitoCoinShop.Application.Models.Domain.Merch;

namespace AvitoCoinShop.Application.Abstractions.Persistence.Repositories;

public interface IMerchRepository
{
    public Task<long> BuyMerchAsync(long userId, long merchId, CancellationToken cancellationToken);

    public Task BuyThisItemForFirstTimeAsync(long userId, long itemId, CancellationToken cancellationToken);
    
    public Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken);
    
    public Task<int> GetMerchPriceAsync(long merchItemId, CancellationToken cancellationToken);
    
    public Task<long?> GetMerchIdByNameAsync(string merchName, CancellationToken cancellationToken);

    public Task<bool> CheckIfUserBoughtItemAsync(long userId, long itemId, CancellationToken cancellationToken);
}