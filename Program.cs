using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateReport.Clients;
using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Services;
using RealEstateReport.Services.Interfaces;
using RealEstateReport.Settings;
using System.Threading.RateLimiting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Settings
        services.Configure<PartnerApiSettings>(
            context.Configuration.GetSection("PartnerApi"));

        // HttpClient
        services.AddHttpClient<IPartnerApiClient, PartnerApiClient>();

        // Services
        services.AddScoped<IRealEstateAgentService, RealEstateAgentService>();
        services.AddSingleton<RateLimiter>(_ =>
        {
            return new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = int.MaxValue,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
        });
    })
    .Build();

using var scope = host.Services.CreateScope();
var service = scope.ServiceProvider.GetRequiredService<IRealEstateAgentService>();

var generalChart = await service.GetTopRealEstateAgentsByListingsAsync(
    new RealEstateReport.Models.RealEstateAgentRankingOptions
    {
        IsGardenRequired = false,
        MaxEntries = 10,
        Locations = ["Amsterdam"],
        ListingType = RealEstateReport.Enums.EListingType.koop
    });

var chartWithGarden = await service.GetTopRealEstateAgentsByListingsAsync(
    new RealEstateReport.Models.RealEstateAgentRankingOptions
    {
        IsGardenRequired = true,
        MaxEntries = 10,
        Locations = ["Amsterdam"],
        ListingType = RealEstateReport.Enums.EListingType.koop
    });

Console.WriteLine($"{"Agent",-40} {"Listings",10}");
Console.WriteLine(new string('-', 55));

foreach (var agent in chartWithGarden)
{
    Console.WriteLine($"{agent.Value.Name,-40} {agent.Value.ListingsCount,10}");
}

Console.WriteLine($"{"Agent",-40} {"Listings",10}");
Console.WriteLine(new string('-', 55));

foreach (var agent in generalChart)
{
    Console.WriteLine($"{agent.Value.Name,-40} {agent.Value.ListingsCount,10}");
}
Console.WriteLine("\n\n--------------------------------\n\n");

Console.WriteLine("Execution completed.");
Console.WriteLine("Press any key to close...");

Console.ReadKey(intercept: true);