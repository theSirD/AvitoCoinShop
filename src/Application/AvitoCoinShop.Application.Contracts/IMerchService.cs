using AvitoCoinShop.Application.Models.Domain.Merch;

namespace AvitoCoinShop.Application.Contracts;

public interface IMerchService
{
    public Task<long> BuyMerchAsync(long userId, string merchName, CancellationToken cancellationToken);
    
    public Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken);

    public Task<int> GetMerchPriceAsync(long merchItemId, CancellationToken cancellationToken);

    public Task<long> GetMerchIdByNameAsync(string merchName, CancellationToken cancellationToken);
}