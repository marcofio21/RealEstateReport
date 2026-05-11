using RealEstateReport.Models;
using RealEstateReport.Models.Dtos;

namespace RealEstateReport.Clients.Interfaces;

public interface IPartnerApiClient
{
    public Task<PartnerApiListingResponseDto> GetListingsAsync(ListingApiRequestOptions options);
}
