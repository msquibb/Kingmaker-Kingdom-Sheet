using KingmakerKingdomSheet.Domain.Entities;
using KingmakerKingdomSheet.Domain.Enums;
using KingmakerKingdomSheet.Domain.ValueObjects;

namespace KingmakerKingdomSheet.Domain.Tests;

public sealed class KingdomRecordTests
{
    [Fact]
    public void Constructor_PreservesProvidedDomainState()
    {
        var kingdomId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;
        var updatedAt = createdAt.AddMinutes(5);
        IReadOnlyList<KingdomMember> members =
        [
            new KingdomMember(kingdomId, "gm-1", "Aldric", KingdomMemberRole.Owner)
        ];
        IReadOnlyList<KingdomHex> hexes =
        [
            new KingdomHex(Guid.NewGuid(), kingdomId, new HexCoordinate(1, 2), HexTerrain.Plains, true, "gm-1", updatedAt)
        ];
        IReadOnlyList<Town> towns =
        [
            new Town(Guid.NewGuid(), kingdomId, "Oleg's Trading Post", new HexCoordinate(1, 2), 124, createdAt)
        ];

        var kingdom = new Kingdom(
            kingdomId,
            "Greenbelt Compact",
            3,
            12,
            18,
            1,
            createdAt,
            updatedAt,
            members,
            hexes,
            towns);

        Assert.Equal(kingdomId, kingdom.Id);
        Assert.Equal("Greenbelt Compact", kingdom.Name);
        Assert.Equal(3, kingdom.Level);
        Assert.Equal(12, kingdom.Size);
        Assert.Equal(18, kingdom.ControlDc);
        Assert.Equal(1, kingdom.Unrest);
        Assert.Same(members, kingdom.Members);
        Assert.Same(hexes, kingdom.Hexes);
        Assert.Same(towns, kingdom.Towns);
    }
}
