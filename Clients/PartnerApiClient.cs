using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Models;
using RealEstateReport.Models.Dtos;
using System.Text.Json;

namespace RealEstateReport.Clients
{
    public class PartnerApiClient(HttpClient httpClient) : IPartnerApiClient
    {
        private readonly HttpClient client = httpClient;

        //[TODO] These values must come from the appsettings.json file. Base URL + Key
        private readonly string _baseUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/";
        private readonly string _key = "76666a29898f491480386d966b75f949";
        //------//

        public async Task<PartnerApiListingResponseDto> GetListingsAsync(ListingApiRequestOptions options)
        {
            //try
            //{
            var url = _baseUrl;

            url += $"JSON/";
            url += _key + $"/";
            url += $"?type={options.ListingType}&zo=/{string.Join("/", options.Locations)}/";
            if (options.IsGardenRequired)
                url += $"tuin/";
            url += $"&page={options.PageNumber}&pagesize={options.PageSize}";

            using HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                throw new Exception("Empty response from API");

            return JsonSerializer.Deserialize<PartnerApiListingResponseDto>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new Exception("Failed to deserialize API response");
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine("\nHttp client failed.\nMessage: {0}", e.Message);
            //}
        }
    }
}
