namespace RealEstateReport.Models.Domain;

public class RealEstateAgent
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public int ListingsCount { get; set; } = 1;
}