using BlueCardPortal.Infrastructure.Data;
using BlueCardPortal.ModelBinders;
using BlueCardPortal.Resources;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Globalization;
using System.Reflection;
using static BlueCardPortal.Infrastructure.Constants.FormattingConstant;
using Microsoft.Extensions.Localization;
using BlueCardPortal.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddApplicationIdentity(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

builder.Services.AddDataProtection()
        .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddScoped<IStringLocalizer>((x) =>
     {
         var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName!);
         var factory = x.GetRequiredService<IStringLocalizerFactory>();
         return factory.Create("SharedResource", assemblyName.Name);
     });

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new CultureInfo[]
    {
                    new CultureInfo("bg"),
                    new CultureInfo("en")
    };

    options.DefaultRequestCulture = new RequestCulture("bg");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
       // new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
        options => options.ResourcesPath = "Resources"
    )
    .AddDataAnnotationsLocalization(opt =>
    {
        opt.DataAnnotationLocalizerProvider = (type, factory) =>
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName!);
            return factory.Create(nameof(SharedResource), assemblyName.Name!);
        };
    })
    .AddMvcOptions(config =>
    {
        config.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
        config.ModelBinderProviders.Insert(1, new DoubleModelBinderProvider());
        config.ModelBinderProviders.Insert(2, new DateTimeModelBinderProvider(NormalDateFormat));
        config.ModelBinderProviders.Insert(3, new DateOnlyModelBinderProvider(NormalDateFormat));
    });

builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
      //  app.UseExceptionHandler("/Home/Error");
 app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use((authContext, next) =>
{
    authContext.Request.Scheme = "https";

    return next();
});

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
