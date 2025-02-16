using AvitoCoinShop.Application.Abstractions.Persistence.Repositories;
using AvitoCoinShop.Infrastructure.Persistence.Migrations;
using AvitoCoinShop.Infrastructure.Persistence.Repositories;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AvitoCoinShop.Infrastructure.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMerchRepository, MerchRepository>();
        services.AddScoped<ITransactionHistoryRepository, TransactionHistoryRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        return services;
    }

    public static IServiceCollection AddInfrastructureConfigurations(this IServiceCollection services)
    {
        services.AddOptions<PostgresOptions>().BindConfiguration("Infrastructure:Persistence:Postgres");
        return services;
    }
    
    public static IServiceCollection AddNpgsqlDataSource(this IServiceCollection services)
    {
        services.AddScoped<NpgsqlDataSource>(provider =>
        {
            PostgresOptions databaseSettings = provider.GetRequiredService<IOptions<PostgresOptions>>().Value;
            string connectionString = databaseSettings.ConnectionString;

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            return dataSourceBuilder.Build();
        });

        return services;
    }
    
    public static IServiceCollection AddHostedServices(this IServiceCollection collection)
    {
        return collection.AddHostedService<MigrationsBackgroundService>();
    }
    
    public static void AddMigrations(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(provider =>
                {
                    PostgresOptions databaseSettings = provider.GetRequiredService<IOptions<PostgresOptions>>().Value;

                    if (!string.IsNullOrEmpty(databaseSettings.ConnectionString))
                        return databaseSettings.ConnectionString;

                    throw new InvalidOperationException("Connection string was not found.");
                })
                .WithMigrationsIn(typeof(CreateMerchTable).Assembly));
    }
}