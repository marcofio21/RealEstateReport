using RealEstateReport.Enums;

namespace RealEstateReport.Models;

public class RealEstateAgentRankingOptions
{
    public int MaxEntries { get; init; } = 10;
    public required EListingType ListingType { get; init; }
    public bool IsGardenRequired { get; init; } = false;
    public required List<string> Locations { get; init; }
}

public class ListingApiRequestOptions
{
    public required EListingType ListingType { get; init; }
    public bool IsGardenRequired { get; init; } = false;
    public int PageSize { get; } = 25;
    public required List<string> Locations { get; init; }
}