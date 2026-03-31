using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Domain.Entities;
using KingmakerKingdomSheet.Domain.Enums;
using KingmakerKingdomSheet.Domain.ValueObjects;
using KingmakerKingdomSheet.Shared.DTOs;

namespace KingmakerKingdomSheet.Application.Services;

internal sealed class PreviewKingdomCatalogService : IKingdomCatalogService
{
    private static readonly IReadOnlyList<Kingdom> Kingdoms = BuildPreviewKingdoms();

    public Task<IReadOnlyList<KingdomSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<KingdomSummaryDto> kingdoms = Kingdoms
            .Select(kingdom => new KingdomSummaryDto(
                kingdom.Id,
                kingdom.Name,
                kingdom.Level,
                kingdom.Size,
                kingdom.ControlDc,
                kingdom.Unrest,
                kingdom.Members.Count,
                kingdom.Hexes.Count(hex => hex.IsClaimed),
                kingdom.Towns.Count))
            .ToArray();

        return Task.FromResult(kingdoms);
    }

    public Task<KingdomDetailsDto?> GetAsync(Guid kingdomId, CancellationToken cancellationToken = default)
    {
        KingdomDetailsDto? kingdom = Kingdoms
            .Where(candidate => candidate.Id == kingdomId)
            .Select(Map)
            .SingleOrDefault();

        return Task.FromResult(kingdom);
    }

    private static KingdomDetailsDto Map(Kingdom kingdom) =>
        new(
            kingdom.Id,
            kingdom.Name,
            kingdom.Level,
            kingdom.Size,
            kingdom.ControlDc,
            kingdom.Unrest,
            kingdom.CreatedAtUtc,
            kingdom.UpdatedAtUtc,
            kingdom.Members
                .Select(member => new KingdomMemberDto(
                    member.UserId,
                    member.DisplayName,
                    member.Role.ToString()))
                .ToArray(),
            kingdom.Hexes
                .Select(hex => new HexDto(
                    hex.Id,
                    hex.Coordinate.Column,
                    hex.Coordinate.Row,
                    hex.Terrain.ToString(),
                    hex.IsClaimed,
                    hex.ClaimedByUserId))
                .ToArray(),
            kingdom.Towns
                .Select(town => new TownDto(
                    town.Id,
                    town.Name,
                    town.Location.Column,
                    town.Location.Row,
                    town.Population))
                .ToArray());

    private static IReadOnlyList<Kingdom> BuildPreviewKingdoms()
    {
        var kingdomId = Guid.Parse("8A74E4D5-05A2-442A-8E7D-756E12F4E9BE");

        return
        [
            new Kingdom(
                kingdomId,
                "Greenbelt Compact",
                3,
                12,
                18,
                1,
                DateTimeOffset.Parse("2026-03-07T00:00:00Z"),
                DateTimeOffset.Parse("2026-03-07T00:00:00Z"),
                [
                    new KingdomMember(kingdomId, "gm-aldric", "Aldric", KingdomMemberRole.Owner),
                    new KingdomMember(kingdomId, "gm-liora", "Liora", KingdomMemberRole.Editor),
                    new KingdomMember(kingdomId, "player-tarin", "Tarin", KingdomMemberRole.Viewer)
                ],
                [
                    new KingdomHex(Guid.Parse("3889C031-8AB6-4351-A9C9-B4FF5CC1136D"), kingdomId, new HexCoordinate(0, 0), HexTerrain.Plains, true, "gm-aldric", DateTimeOffset.Parse("2026-03-07T00:00:00Z")),
                    new KingdomHex(Guid.Parse("A52F54D6-16D5-4E38-B0E0-A483D3CB6D95"), kingdomId, new HexCoordinate(1, 0), HexTerrain.Forest, true, "gm-liora", DateTimeOffset.Parse("2026-03-07T00:00:00Z")),
                    new KingdomHex(Guid.Parse("8593B856-0C56-44F5-BA6A-6F587B7F725B"), kingdomId, new HexCoordinate(1, 1), HexTerrain.Hills, false, null, null)
                ],
                [
                    new Town(Guid.Parse("7FEA60C7-B8E5-4C4E-9B51-A08556A5AB46"), kingdomId, "Oleg's Trading Post", new HexCoordinate(0, 0), 124, DateTimeOffset.Parse("2026-03-07T00:00:00Z"))
                ])
        ];
    }
}
