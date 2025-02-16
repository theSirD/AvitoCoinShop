using AvitoCoinShop.Application.Auth;
using AvitoCoinShop.Application.Contracts;
using AvitoCoinShop.Application.Merch;
using AvitoCoinShop.Application.Transactions;
using AvitoCoinShop.Application.User;
using AvitoCoinShop.Application.Wallet;
using Microsoft.Extensions.DependencyInjection;

namespace AvitoCoinShop.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddSingleton<IUserService, UserService>();
        collection.AddSingleton<IAuthService, AuthService>();
        collection.AddSingleton<IMerchService, MerchService>();
        collection.AddSingleton<ITransactionHistoryService, TransactionHistoryService>();
        collection.AddSingleton<IWalletService, WalletService>();
        return collection;
    }
    
    public static IServiceCollection AddApplicationConfigurations(this IServiceCollection services)
    {
        services.AddOptions<JwtOptions>().BindConfiguration("JwtSettings");
        return services;
    }
}