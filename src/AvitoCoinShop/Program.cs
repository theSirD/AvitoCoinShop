using System.Text;
using AvitoCoinShop.Application.Extensions;
using AvitoCoinShop.Infrastructure.Persistence.Extensions;
using AvitoCoinShop.Presentation.Http.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
        
        // TODO. Hide your_super_secret_key_which_should_be_long_enough
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "AvitoCoinShop",
                    ValidateAudience = true,
                    ValidAudience = "AvitoCoinShopClient",
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_which_should_be_long_enough"))
                };
            });
        
        WebApplication app = builder.Build();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
        
    }
}