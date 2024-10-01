using BlueCardPortal.Infrastructure.Data;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("insecureClient")
            .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
            });
        services.AddHttpClient();
        services.AddScoped<IClient>(
            sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return ActivatorUtilities.CreateInstance<Client>(sp, clientFactory, config);
            }
            );
        services.AddMemoryCache();
        return services;
    }

    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IRepository, Repository>();
        return services;
    }

   
}
