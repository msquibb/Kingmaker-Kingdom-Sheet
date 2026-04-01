using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.ApiService.Services;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.Application.Tests;

public sealed class KingdomManagementServiceTests
{
    private static async Task<KingmakerDbContext> CreateInMemoryContextAsync()
    {
        var options = new DbContextOptionsBuilder<KingmakerDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new KingmakerDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        var gameMasterRole = new KingdomRoleType
        {
            KingdomRoleTypeId = 1,
            Name = "game-master",
            DisplayName = "Game Master",
            IsGameMasterRole = true,
            SortOrder = 10
        };
        context.KingdomRoleTypes.Add(gameMasterRole);

        var user = new UserAccount
        {
            UserAccountId = Guid.NewGuid(),
            DisplayName = "Test User",
            Email = "test@example.com",
            NormalizedEmail = "TEST@EXAMPLE.COM",
            IdentityProvider = "local",
            ExternalSubject = "local:test@example.com",
            IsActive = true,
            CreatedUtc = DateTime.UtcNow,
            ModifiedUtc = DateTime.UtcNow
        };
        context.UserAccounts.Add(user);

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task CreateAsync_CreatesKingdomWithGameMasterRole()
    {
        await using var context = await CreateInMemoryContextAsync();
        var catalogService = new SqliteKingdomCatalogService(context);
        var managementService = new SqliteKingdomManagementService(context, catalogService);

        var user = await context.UserAccounts.FirstAsync();
        var dto = new CreateKingdomDto("Test Kingdom", "A test kingdom");

        var result = await managementService.CreateAsync(user.UserAccountId, dto);

        Assert.NotNull(result);
        Assert.Equal("Test Kingdom", result.Name);
        Assert.Equal(user.UserAccountId.ToString("D"), result.Members.Single().UserId);
        Assert.Equal("Game Master", result.Members.Single().Role);

        var kingdom = await context.Kingdoms
            .Include(k => k.Participants)
                .ThenInclude(p => p.Roles)
            .FirstOrDefaultAsync(k => k.KingdomId == result.Id);

        Assert.NotNull(kingdom);
        Assert.Equal("test-kingdom", kingdom.Slug);
        Assert.False(kingdom.IsArchived);
        Assert.Single(kingdom.Participants);
        Assert.Single(kingdom.Participants.First().Roles);
    }

    [Fact]
    public async Task CreateAsync_GeneratesValidSlug()
    {
        await using var context = await CreateInMemoryContextAsync();
        var catalogService = new SqliteKingdomCatalogService(context);
        var managementService = new SqliteKingdomManagementService(context, catalogService);

        var user = await context.UserAccounts.FirstAsync();
        var dto = new CreateKingdomDto("My Test Kingdom! @#$", null);

        var result = await managementService.CreateAsync(user.UserAccountId, dto);

        var kingdom = await context.Kingdoms.FirstAsync(k => k.KingdomId == result.Id);
        Assert.Equal("my-test-kingdom", kingdom.Slug);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesKingdomAndSlug()
    {
        await using var context = await CreateInMemoryContextAsync();
        var catalogService = new SqliteKingdomCatalogService(context);
        var managementService = new SqliteKingdomManagementService(context, catalogService);

        var user = await context.UserAccounts.FirstAsync();
        var createDto = new CreateKingdomDto("Original Name", "Original description");
        var created = await managementService.CreateAsync(user.UserAccountId, createDto);

        var updateDto = new UpdateKingdomDto("Updated Name", "Updated description");
        var updated = await managementService.UpdateAsync(created.Id, updateDto);

        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);

        var kingdom = await context.Kingdoms.FirstAsync(k => k.KingdomId == created.Id);
        Assert.Equal("updated-name", kingdom.Slug);
        Assert.Equal("Updated description", kingdom.Description);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullForNonexistentKingdom()
    {
        await using var context = await CreateInMemoryContextAsync();
        var catalogService = new SqliteKingdomCatalogService(context);
        var managementService = new SqliteKingdomManagementService(context, catalogService);

        var updateDto = new UpdateKingdomDto("Test", null);
        var result = await managementService.UpdateAsync(Guid.NewGuid(), updateDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task ArchiveAsync_SoftDeletesKingdom()
    {
        await using var context = await CreateInMemoryContextAsync();
        var catalogService = new SqliteKingdomCatalogService(context);
        var managementService = new SqliteKingdomManagementService(context, catalogService);

        var user = await context.UserAccounts.FirstAsync();
        var createDto = new CreateKingdomDto("Test Kingdom", null);
        var created = await managementService.CreateAsync(user.UserAccountId, createDto);

        var success = await managementService.ArchiveAsync(created.Id);

        Assert.True(success);

        var kingdom = await context.Kingdoms.FirstAsync(k => k.KingdomId == created.Id);
        Assert.True(kingdom.IsArchived);

        var catalogResult = await catalogService.GetAsync(created.Id);
        Assert.Null(catalogResult);
    }

    [Fact]
    public async Task ArchiveAsync_ReturnsFalseForNonexistentKingdom()
    {
        await using var context = await CreateInMemoryContextAsync();
        var catalogService = new SqliteKingdomCatalogService(context);
        var managementService = new SqliteKingdomManagementService(context, catalogService);

        var result = await managementService.ArchiveAsync(Guid.NewGuid());

        Assert.False(result);
    }
}
