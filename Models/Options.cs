using RealEstateReport.Enums;

namespace RealEstateReport.Models;

public class RealEstateAgentRankingOptions
{
    public bool IsGardenRequired { get; init; } = false;
    public int MaxEntries { get; init; } = 10;
}

public class ListingApiRequestOptions
{
    public required EListingType ListingType { get; init; }
    public bool IsGardenRequired { get; init; } = false;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public required List<string> Locations { get; init; }
}
