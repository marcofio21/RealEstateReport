using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Models;
using RealEstateReport.Models.Dtos;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace RealEstateReport.Clients
{
    public class PartnerApiClient(HttpClient _httpClient, RateLimiter _rateLimiter) : IPartnerApiClient
    {

        //[TODO] These values must come from the appsettings.json file. Base URL + Key
        private readonly string _baseUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/";
        private readonly string _key = "76666a29898f491480386d966b75f949";
        //------//

        public async Task<PartnerApiListingResponseDto> GetListingsAsync(int page, ListingApiRequestOptions options)
        {
            using var lease = await _rateLimiter.AcquireAsync(1);
            if (!lease.IsAcquired)
                throw new Exception("Rate limit exceeded");

            // URL-encode each location segment before building the path (e.g. "Den Haag" => "Den%20Haag")
            var locations = string.Join("/", options.Locations.Select(Uri.EscapeDataString));

            var url = $"{_baseUrl}JSON/{_key}/" +
                       $"?type={options.ListingType}" +
                       $"&zo=/{locations}/" +
                       (options.IsGardenRequired ? "tuin/" : "") +
                       $"&page={page}&pagesize={options.PageSize}";

            try {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json))
                    throw new InvalidOperationException("Empty response from Partner API");

                return JsonSerializer.Deserialize<PartnerApiListingResponseDto>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? throw new InvalidOperationException("Failed to deserialize Partner API response");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize Partner API response. URL: {url}",
                    ex
                );
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(
                    $"HTTP error calling Partner API. URL: {url}",
                    ex
                );
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpRequestException(
                    $"Timeout calling Partner API. URL: {url}",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling API. URL: {url}", ex);
            }
        }
    }
}
