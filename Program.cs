using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateReport.Clients;
using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Services;
using RealEstateReport.Services.Interfaces;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // HttpClient
        services.AddHttpClient<IPartnerApiClient, PartnerApiClient>();

        // Services
        services.AddScoped<IRealEstateAgentService, RealEstateAgentService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<IRealEstateAgentService>();

//[TODO] get hard coded data from the appsettings

var result = await service.GetMostActiveRealEstateAgentsAsync(
    new RealEstateReport.Models.RealEstateAgentRankingOptions { 
        IsGardenRequired = true,
        MaxEntries = 10
    });

//[TODO] Improve output
foreach (var agent in result)
{
    Console.WriteLine($"{agent.Value.Name} - {agent.Value.ListingsCount}");
}