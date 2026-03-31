using KingmakerKingdomSheet.Shared.DTOs;

namespace KingmakerKingdomSheet.Web;

public class KingdomApiClient(HttpClient httpClient)
{
    public async Task<List<KingdomSummaryDto>> GetKingdomsAsync(CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<List<KingdomSummaryDto>>("/api/kingdoms", ct) ?? [];

    public async Task<KingdomDetailsDto?> GetKingdomAsync(Guid id, CancellationToken ct = default)
        => await httpClient.GetFromJsonAsync<KingdomDetailsDto>($"/api/kingdoms/{id}", ct);
}
