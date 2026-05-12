using RealEstateReport.Clients.Interfaces;
using RealEstateReport.Models;
using RealEstateReport.Models.Domain;

namespace RealEstateReport.Services.Interfaces;

public interface IRealEstateAgentService{
    public Task<Dictionary<int, RealEstateAgent>> GetTopRealEstateAgentsByListingsAsync(RealEstateAgentRankingOptions options);
}

