using KingmakerKingdomSheet.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace KingmakerKingdomSheet.Application.Tests;

public sealed class KingdomCatalogServiceTests
{
    [Fact]
    public async Task ListAsync_ReturnsPreviewSummaryData()
    {
        await using var provider = CreateProvider();
        var service = provider.GetRequiredService<IKingdomCatalogService>();

        var kingdoms = await service.ListAsync();

        var kingdom = Assert.Single(kingdoms);
        Assert.Equal("Greenbelt Compact", kingdom.Name);
        Assert.Equal(3, kingdom.MemberCount);
        Assert.Equal(2, kingdom.ClaimedHexCount);
        Assert.Equal(1, kingdom.TownCount);
    }

    [Fact]
    public async Task GetAsync_ReturnsDetailsForListedKingdom()
    {
        await using var provider = CreateProvider();
        var service = provider.GetRequiredService<IKingdomCatalogService>();
        var summary = Assert.Single(await service.ListAsync());

        var details = await service.GetAsync(summary.Id);

        Assert.NotNull(details);
        Assert.Equal(summary.Id, details.Id);
        Assert.Equal(summary.Name, details.Name);
        Assert.Equal(summary.MemberCount, details.Members.Count);
        Assert.Equal(summary.ClaimedHexCount, details.Hexes.Count(hex => hex.IsClaimed));
        Assert.Equal(summary.TownCount, details.Towns.Count);
    }

    [Fact]
    public async Task GetAsync_ReturnsNullForUnknownKingdom()
    {
        await using var provider = CreateProvider();
        var service = provider.GetRequiredService<IKingdomCatalogService>();

        var details = await service.GetAsync(Guid.NewGuid());

        Assert.Null(details);
    }

    [Fact]
    public void AddKingdomApplication_RegistersSingletonCatalogService()
    {
        using var provider = CreateProvider();

        var first = provider.GetRequiredService<IKingdomCatalogService>();
        var second = provider.GetRequiredService<IKingdomCatalogService>();

        Assert.Same(first, second);
    }

    private static ServiceProvider CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddKingdomApplication();
        return services.BuildServiceProvider();
    }
}
