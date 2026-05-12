using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Models;
using RealEstateReport.Models.Domain;
using RealEstateReport.Models.Dtos;
using RealEstateReport.Services.Interfaces;

namespace RealEstateReport.Services
{
    public class RealEstateAgentService(IPartnerApiClient _partnerApiClient) : IRealEstateAgentService
    {
        private readonly IPartnerApiClient _partnerApiClient = _partnerApiClient;

        public async Task<Dictionary<int, RealEstateAgent>> GetMostActiveRealEstateAgentsAsync(RealEstateAgentRankingOptions options)
        {
            var mostActiveRealEstateAgents = new Dictionary<int, RealEstateAgent>();
            var partnerApiConfig = new ListingApiRequestOptions
            {
                IsGardenRequired = options.IsGardenRequired,
                ListingType = options.ListingType,
                Locations = options.Locations
            };
            var totalPages = -1;

            // First call. 
            var listings = await _partnerApiClient.GetListingsAsync(1, partnerApiConfig);
            totalPages = listings.Paging.AantalPaginas;
            AggregateAgents(listings, mostActiveRealEstateAgents);

            // Assuming "AantalPaginas" is used in the UI, default it to 1 when no results are found.
            for (int i = 2; i <= totalPages; i++)
            {
                //requesting the next page to process
                listings = await _partnerApiClient.GetListingsAsync(i, partnerApiConfig);
                //inserting first page entries in the real estate collection
                AggregateAgents(listings, mostActiveRealEstateAgents);
            }

            // Reordering the dictionary in descending order
            return mostActiveRealEstateAgents
                .OrderByDescending(x => x.Value.ListingsCount)
                .Take(options.MaxEntries)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Aggregates listing data by real estate agent into the provided dictionary.
        /// </summary>
        /// <param name="listings">Partner API listing response.</param>
        /// <param name="dictionary">Accumulator of agent statistics.</param>
        private void AggregateAgents(PartnerApiListingResponseDto listings, Dictionary<int, RealEstateAgent> dictionary)
        {
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
