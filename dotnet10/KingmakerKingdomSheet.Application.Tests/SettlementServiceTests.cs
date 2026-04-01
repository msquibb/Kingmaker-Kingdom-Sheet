using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.ApiService.Services;
using KingmakerKingdomSheet.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.Application.Tests;

public sealed class SettlementServiceTests
{
    private static async Task<KingmakerDbContext> CreateInMemoryContextAsync()
    {
        var options = new DbContextOptionsBuilder<KingmakerDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new KingmakerDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        var settlementType = new SettlementType
        {
            SettlementTypeId = 1,
            Name = "town",
            DisplayName = "Town",
            MinDistricts = 1,
            SortOrder = 10
        };
        context.SettlementTypes.Add(settlementType);

        var kingdom = new Kingdom
        {
            KingdomId = Guid.NewGuid(),
            Name = "Test Kingdom",
            Slug = "test-kingdom",
            IsArchived = false,
            CreatedUtc = DateTime.UtcNow,
            ModifiedUtc = DateTime.UtcNow
        };
        context.Kingdoms.Add(kingdom);

        var terrainType = new HexTerrainType
        {
            HexTerrainTypeId = 1,
            Name = "plains",
            DisplayName = "Plains",
            MovementCost = 1.0m,
            IsWater = false,
            SupportsSettlement = true,
            SortOrder = 10
        };
        context.HexTerrainTypes.Add(terrainType);

        var claimStatus = new ClaimStatus
        {
            ClaimStatusId = 1,
            Name = "claimed",
            DisplayName = "Claimed",
            IsClaimed = true,
            SortOrder = 10
        };
        context.ClaimStatuses.Add(claimStatus);

        var fogState = new FogState
        {
            FogStateId = 1,
            Name = "explored",
            DisplayName = "Explored",
            SortOrder = 10
        };
        context.FogStates.Add(fogState);

        var hex = new Hex
        {
            HexId = Guid.NewGuid(),
            KingdomId = kingdom.KingdomId,
            CoordinateX = 10,
            CoordinateY = 20,
            HexTerrainTypeId = 1,
            ClaimStatusId = 1,
            FogStateId = 1,
            CreatedUtc = DateTime.UtcNow,
            ModifiedUtc = DateTime.UtcNow
        };
        context.Hexes.Add(hex);

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task CreateAsync_CreatesSettlement()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var kingdom = await context.Kingdoms.FirstAsync();
        var hex = await context.Hexes.FirstAsync();
        var dto = new CreateSettlementDto("New Town", 1, hex.HexId);

        var result = await settlementService.CreateAsync(kingdom.KingdomId, dto);

        Assert.NotNull(result);
        Assert.Equal("New Town", result.Name);
        Assert.Equal(10, result.Column);
        Assert.Equal(20, result.Row);

        var settlement = await context.Settlements.FirstOrDefaultAsync(s => s.SettlementId == result.Id);
        Assert.NotNull(settlement);
        Assert.Equal(kingdom.KingdomId, settlement.KingdomId);
        Assert.Equal(hex.HexId, settlement.HexId);
    }

    [Fact]
    public async Task CreateAsync_CreatesSettlementWithoutHex()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var kingdom = await context.Kingdoms.FirstAsync();
        var dto = new CreateSettlementDto("Floating Town", 1, null);

        var result = await settlementService.CreateAsync(kingdom.KingdomId, dto);

        Assert.NotNull(result);
        Assert.Equal("Floating Town", result.Name);
        Assert.Equal(0, result.Column);
        Assert.Equal(0, result.Row);

        var settlement = await context.Settlements.FirstOrDefaultAsync(s => s.SettlementId == result.Id);
        Assert.NotNull(settlement);
        Assert.Null(settlement.HexId);
    }

    [Fact]
    public async Task ListByKingdomAsync_ReturnsAllSettlements()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var kingdom = await context.Kingdoms.FirstAsync();
        var hex = await context.Hexes.FirstAsync();

        await settlementService.CreateAsync(kingdom.KingdomId, new CreateSettlementDto("Town A", 1, hex.HexId));
        await settlementService.CreateAsync(kingdom.KingdomId, new CreateSettlementDto("Town B", 1, null));

        var result = await settlementService.ListByKingdomAsync(kingdom.KingdomId);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Name == "Town A");
        Assert.Contains(result, s => s.Name == "Town B");
    }

    [Fact]
    public async Task GetAsync_ReturnsSettlement()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var kingdom = await context.Kingdoms.FirstAsync();
        var created = await settlementService.CreateAsync(kingdom.KingdomId, new CreateSettlementDto("Test Town", 1, null));

        var result = await settlementService.GetAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Town", result.Name);
    }

    [Fact]
    public async Task GetAsync_ReturnsNullForNonexistent()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var result = await settlementService.GetAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesSettlement()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var kingdom = await context.Kingdoms.FirstAsync();
        var hex = await context.Hexes.FirstAsync();
        var created = await settlementService.CreateAsync(kingdom.KingdomId, new CreateSettlementDto("Original", 1, null));

        var updateDto = new UpdateSettlementDto("Updated Town", 1, hex.HexId, 5000, "Test notes");
        var updated = await settlementService.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(updated);
        Assert.Equal("Updated Town", updated.Name);
        Assert.Equal(5000, updated.Population);
        Assert.Equal(10, updated.Column);
        Assert.Equal(20, updated.Row);

        var settlement = await context.Settlements.FirstAsync(s => s.SettlementId == created.Id);
        Assert.Equal("Test notes", settlement.Notes);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullForNonexistent()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var updateDto = new UpdateSettlementDto("Test", 1, null, null, null);
        var result = await settlementService.UpdateAsync(Guid.NewGuid(), updateDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesSettlement()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var kingdom = await context.Kingdoms.FirstAsync();
        var created = await settlementService.CreateAsync(kingdom.KingdomId, new CreateSettlementDto("To Delete", 1, null));

        var success = await settlementService.DeleteAsync(created.Id);

        Assert.True(success);

        var settlement = await context.Settlements.FirstOrDefaultAsync(s => s.SettlementId == created.Id);
        Assert.Null(settlement);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseForNonexistent()
    {
        await using var context = await CreateInMemoryContextAsync();
        var settlementService = new SqliteSettlementService(context);

        var result = await settlementService.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }
}
