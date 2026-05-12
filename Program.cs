using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateReport.Clients;
using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Services;
using RealEstateReport.Services.Interfaces;
using System.Threading.RateLimiting;

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
        services.AddSingleton<RateLimiter>(_ =>
        {
            return new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
            {
                PermitLimit = 80,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = int.MaxValue,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
        });
    })
    .Build();

using var scope = host.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<IRealEstateAgentService>();

//[TODO] get hard coded data from the appsettings
var generalChart = await service.GetMostActiveRealEstateAgentsAsync(
    new RealEstateReport.Models.RealEstateAgentRankingOptions
    {
        IsGardenRequired = false,
        MaxEntries = 10,
        Locations = ["Amsterdam"],
        ListingType = RealEstateReport.Enums.EListingType.koop
    });

var chartWithGarden = await service.GetMostActiveRealEstateAgentsAsync(
    new RealEstateReport.Models.RealEstateAgentRankingOptions
    {
        IsGardenRequired = true,
        MaxEntries = 10,
        Locations = ["Amsterdam"],
        ListingType = RealEstateReport.Enums.EListingType.koop
    });


//[TODO] Improve output
foreach (var agent in chartWithGarden)
{
    Console.WriteLine($"{agent.Value.Name} - {agent.Value.ListingsCount}");
}
Console.WriteLine("\n\n--------------------------------\n\n");
//[TODO] Improve output
foreach (var agent in generalChart)
{
    Console.WriteLine($"{agent.Value.Name} - {agent.Value.ListingsCount}");
}