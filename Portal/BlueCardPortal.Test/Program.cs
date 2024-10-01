// See https://aka.ms/new-console-template for more information
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
var configuration = new ConfigurationBuilder()
                          .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                          .AddJsonFile("appsettings.json",  false, true)
                          .AddUserSecrets<Program>()
                          .Build();
var serviceProvider = new ServiceCollection()
                           .AddApplicationDbContext(configuration)
                           .AddApplicationServices(configuration)
                           .BuildServiceProvider();

var client = serviceProvider.GetRequiredService<IClient>();
var body = new GetNomenclatures_input()
{
    ReferenceDataName = "STATUS" // "COUNTRIES"
};
var result = await client.GetNomenclaturesAsync(body);
Console.WriteLine(result);
// var body = new CreateApplication_input();
// await client.CreateApplicationAsync(body);
Console.WriteLine("Hello, World!");
