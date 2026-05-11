using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Models;
using RealEstateReport.Models.Domain;
using RealEstateReport.Models.Dtos;
using RealEstateReport.Services.Interfaces;

namespace RealEstateReport.Services
{
    public class RealEstateAgentService(IPartnerApiClient partnerApiClient) : IRealEstateAgentService
    {
        private readonly IPartnerApiClient _partnerApiClient = partnerApiClient;

        public async Task<Dictionary<int, RealEstateAgent>> GetMostActiveRealEstateAgentsAsync(RealEstateAgentRankingOptions options)
        {
            var mostActiveRealEstateAgents = new Dictionary<int, RealEstateAgent>();

            //requesting the first page
            var listings = await _partnerApiClient.GetListingsAsync(new ListingApiRequestOptions
            {
                IsGardenRequired = options.IsGardenRequired,
                ListingType = Enums.EListingType.koop,
                Locations = ["Amsterdam"]
            });
            AggregateAgents(listings, mostActiveRealEstateAgents);

            // Assuming "AantalPaginas" is used in the UI, default it to 1 when no results are found.
            for (int i = 2; i <= listings.Paging.AantalPaginas; i++)
            {
                //requesting new page
                listings = await _partnerApiClient.GetListingsAsync(new ListingApiRequestOptions
                {
                    IsGardenRequired = options.IsGardenRequired,
                    ListingType = Enums.EListingType.koop,
                    Locations = ["Amsterdam"],
                    PageNumber = i
                });
                //inserting first page entries in the real estate collection
                AggregateAgents(listings, mostActiveRealEstateAgents);
            }

            //reordering dictionary in order to have data properlu displayed
            return mostActiveRealEstateAgents
                .OrderByDescending(x => x.Value.ListingsCount)
                .Take(options.MaxEntries)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private void AggregateAgents(PartnerApiListingResponseDto listings, Dictionary<int, RealEstateAgent> dictionary)
        {
            // Extract listings from API response and store them in RealEstateAgentDictionary
            foreach (var listing in listings.Objects)
            {
                if(dictionary.TryGetValue(listing.MakelaarId, out var agent))
                {
                    agent.ListingsCount++;
                }
                else
                {
                    dictionary[listing.MakelaarId] = new RealEstateAgent { Id = listing.MakelaarId, Name = listing.MakelaarNaam}; // ListingsCount is initialized to 1
                }
            }
        }
    }
}
