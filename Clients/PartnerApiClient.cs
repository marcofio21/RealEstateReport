using Microsoft.Extensions.Options;
using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Models;
using RealEstateReport.Models.Dtos;
using RealEstateReport.Settings;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace RealEstateReport.Clients
{
    public class PartnerApiClient(IOptions<PartnerApiSettings> options, HttpClient httpClient, RateLimiter rateLimiter) : IPartnerApiClient
    {
        private readonly PartnerApiSettings _settings = options.Value;

        /// <summary>
        /// Retrieves a page of real estate listings from the Partner API.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="options">Request options used to filter listings.</param>
        /// <returns>
        /// A <see cref="PartnerApiListingResponseDto"/> containing the paginated listing results.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the API response is empty or cannot be deserialized.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// Thrown when the HTTP request to the Partner API fails.
        /// </exception>
        public async Task<PartnerApiListingResponseDto> GetListingsAsync(int page, ListingApiRequestOptions options)
        {
            using var lease = await rateLimiter.AcquireAsync(1);
            if (!lease.IsAcquired)
                throw new Exception("Rate limit exceeded");

            // URL-encode each location segment before building the path (e.g. "Den Haag" => "Den%20Haag")
            var locations = string.Join("/", options.Locations.Select(Uri.EscapeDataString));

            var url = $"{_settings.BaseUrl}JSON/{_settings.Key}/" +
                       $"?type={options.ListingType}" +
                       $"&zo=/{locations}/" +
                       (options.IsGardenRequired ? "tuin/" : "") +
                       $"&page={page}&pagesize={options.PageSize}";

            try {
                using HttpResponseMessage response = await httpClient.GetAsync(url);
                // The Partner API may occasionally throttle or reject requests even below the documented limit.
                // A conservative rate limiter and basic retry strategy are used to mitigate transient failures.
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
