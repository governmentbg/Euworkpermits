using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Core.Services;
using BlueCardPortal.Infrastructure.Contracts;
using BlueCardPortal.Infrastructure.Data;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Data.Models.UserContext;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Validation;
using EAuthIntegration.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpClient("insecureClient")
            .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                    };
            });
        services.AddHttpClient();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IClient, Client>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<INomenclatureService, NomenclatureService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<ISignerService, SignerService>();
        services.AddSingleton<IValidationAttributeAdapterProvider, BCAttributeAdapterProvider>();
        //services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureModelBindingLocalization>();
        return services;
    }

    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IRepository, Repository>();

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEAuth(configuration, opt =>
        {
            opt.Issuer = configuration["EAuth:Issuer"] ?? string.Empty;
            opt.ServiceName = configuration["EAuth:ServiceName"] ?? string.Empty;
            opt.Service = configuration["EAuth:Service"] ?? string.Empty;
            opt.Provider = configuration["EAuth:Provider"] ?? string.Empty;
            opt.LevelOfAssurance = configuration["EAuth:LevelOfAssurance"] ?? string.Empty;
            opt.IdPMetadata = configuration["EAuth:IdPMetadata"] ?? string.Empty;
            opt.SigningCertificateFile = configuration["EAuth:SigningCertificateFile"] ?? string.Empty;
            opt.SigningCertificatePassword = configuration["EAuth:SigningCertificatePassword"] ?? string.Empty;
            opt.EncriptionCertificateFile = configuration["EAuth:EncryptionCertificateFile"];
            opt.EncriptionCertificatePassword = configuration["EAuth:EncryptionCertificatePassword"];
            opt.SignatureAlgorithm = configuration["EAuth:SignatureAlgorithm"] ?? string.Empty;
            opt.AdministrativeContact = new EAuthAdministrativeContact()
            {
                Company = "Ministry of electronic governance",
                GivenName = "Petya",
                SurName = "Marinova",
                EmailAddress = "p.marinova@egov.government.bg"
            };
            opt.CertificateValidationMode = configuration["EAuth:CertificateValidationMode"] ?? string.Empty;
            opt.RevocationMode = configuration["EAuth:RevocationMode"] ?? string.Empty;
            opt.IgnoreCertificateValidity = configuration.GetValue<bool>("EAuth:IgnoreCertificateValidity");
            opt.TechnicalContact = new EAuthTechnicalContact()
            {
                Company = "Information Services Plc",
                GivenName = "Stamo",
                SurName = "Petkov",
                EmailAddress = "s.g.petkov@is-bg.net"
            };
            opt.RequestedAttributes = new EAuthRequestAttribute[]
            {
                new EAuthRequestAttribute(EAuthRequestAttributesEnum.PersonIdentifier, true),
                new EAuthRequestAttribute(EAuthRequestAttributesEnum.PersonName, true),
                new EAuthRequestAttribute(EAuthRequestAttributesEnum.X509Certificate, true),
                new EAuthRequestAttribute(EAuthRequestAttributesEnum.Email, true),
                new EAuthRequestAttribute(EAuthRequestAttributesEnum.DateOfBirth, false)
            };
            opt.CookieConfiguration = new EAuthCookieConfiguration()
            {
                ExpireTimeSpan = TimeSpan.FromMinutes(120),
                SlidingExpiration = true,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/LogOff")
            };
        });

        return services;
    }

    static Task HandleRemoteFailure(RemoteFailureContext context)
    {
        context.Response.Redirect($"/account/logincerterror?error={context.Failure}");
        context.HandleResponse();

        return Task.FromResult(0);
    }
}
