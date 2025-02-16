using AvitoCoinShop.Application.Extensions;
using AvitoCoinShop.Infrastructure.Persistence.Extensions;
using AvitoCoinShop.Presentation.Http.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AvitoCoinShop;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructureConfigurations();
        builder.Services.AddNpgsqlDataSource();
        builder.Services.AddMigrations();
        builder.Services.AddRepositories();
        builder.Services.AddHostedServices();

        builder.Services.AddApplicationConfigurations();
        builder.Services.AddApplication();

        builder.Services.AddControllers().AddApplicationPart(typeof(AuthController).Assembly);
        
        WebApplication app = builder.Build();
        app.UseRouting();
        app.MapControllers();
        app.Run();
        
    }
}