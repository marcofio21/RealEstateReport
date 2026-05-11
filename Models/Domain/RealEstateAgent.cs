using RealEstateReport.Models.Domain.Interfaces;

namespace RealEstateReport.Models.Domain;

public class RealEstateAgent : IEntity
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public int ListingsCount { get; set; } = 1;
}