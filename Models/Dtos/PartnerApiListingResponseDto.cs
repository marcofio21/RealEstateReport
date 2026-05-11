namespace RealEstateReport.Models.Dtos
{

    public class PartnerApiListingResponseDto
    {
        public required List<PartnerApiListingDto> Objects { get; init; }
        public required PartnerApiPagingInfoDto Paging { get; init; }
    }

    public class PartnerApiListingDto
    {
        public required Guid Id { get; init; }
        public required int GlobalId { get; init; }
        public required int MakelaarId { get; init; }
        public required string MakelaarNaam { get; init; }
    }

    public class PartnerApiPagingInfoDto
    {
        /// <summary>
        /// total pages
        /// </summary>
        public required int AantalPaginas { get; init; }
        /// <summary>
        /// current page
        /// </summary>
        public required int HuidigePagina { get; init; }
    }
}
