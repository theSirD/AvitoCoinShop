using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Models.Merch;

namespace AvitoCoinShop.Application.Merch;

public class MerchService : IMerchService
{
    private readonly IMerchRepository _merchRepository;

    public MerchService(IMerchRepository merchRepository)
    {
        _merchRepository = merchRepository;
    }

    public async Task<long> BuyMerchAsync(long userId, long merchId, long amount, CancellationToken cancellationToken)
    {
        long merchItemOfUserId = await _merchRepository.BuyMerchAsync(userId, merchId, amount, cancellationToken);
        return merchItemOfUserId;
    }

    public async Task<IEnumerable<UserMerchItem>> GetMerchItemsBoughtByUser(long userId, CancellationToken cancellationToken)
    {
        IEnumerable<UserMerchItem> items = await _merchRepository.GetMerchItemsBoughtByUser(userId, cancellationToken);
        return items;
    }

    public async Task<int> GetMerchPriceAsync(long merchItemId, int amount, CancellationToken cancellationToken)
    {
        int price = await _merchRepository.GetMerchPriceAsync(merchItemId, amount, cancellationToken);
        return price;
    }
}