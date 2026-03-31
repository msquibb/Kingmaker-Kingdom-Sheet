using KingmakerKingdomSheet.ApiService.Auth;
using KingmakerKingdomSheet.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.Application.Tests;

public sealed class KingdomAuthorizationServiceTests
{
    private static async Task<KingmakerDbContext> CreateInMemoryContextWithReferenceDataAsync()
    {
        var options = new DbContextOptionsBuilder<KingmakerDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new KingmakerDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        var viewKingdomPermission = new KingdomPermissionType
        {
            KingdomPermissionTypeId = 1,
            Name = "view-kingdom",
            DisplayName = "View Kingdom",
            SortOrder = 10
        };
        var manageKingdomPermission = new KingdomPermissionType
        {
            KingdomPermissionTypeId = 2,
            Name = "manage-kingdom",
            DisplayName = "Manage Kingdom",
            SortOrder = 20
        };
        var editMapPermission = new KingdomPermissionType
        {
            KingdomPermissionTypeId = 4,
            Name = "edit-map",
            DisplayName = "Edit Map",
            SortOrder = 40
        };

        context.KingdomPermissionTypes.AddRange(viewKingdomPermission, manageKingdomPermission, editMapPermission);

        var rulerRole = new KingdomRoleType
        {
            KingdomRoleTypeId = 3,
            Name = "ruler",
            DisplayName = "Ruler",
            IsGameMasterRole = false,
            SortOrder = 30
        };
        var observerRole = new KingdomRoleType
        {
            KingdomRoleTypeId = 12,
            Name = "player-observer",
            DisplayName = "Player Observer",
            IsGameMasterRole = false,
            SortOrder = 120
        };

        context.KingdomRoleTypes.AddRange(rulerRole, observerRole);

        context.KingdomRoleDefaultPermissions.AddRange(
            new KingdomRoleDefaultPermission { KingdomRoleTypeId = 3, KingdomPermissionTypeId = 1 },
            new KingdomRoleDefaultPermission { KingdomRoleTypeId = 3, KingdomPermissionTypeId = 2 },
            new KingdomRoleDefaultPermission { KingdomRoleTypeId = 3, KingdomPermissionTypeId = 4 },
            new KingdomRoleDefaultPermission { KingdomRoleTypeId = 12, KingdomPermissionTypeId = 1 }
        );

        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task GetEffectivePermissionsAsync_ReturnsRoleDefaultPermissions()
    {
        await using var context = await CreateInMemoryContextWithReferenceDataAsync();
        var authzService = new KingdomAuthorizationService(context);

        var user = new UserAccount
        {
            DisplayName = "Test User",
            Email = "test@example.com",
            ExternalSubject = Guid.NewGuid().ToString()
        };
        context.UserAccounts.Add(user);

        var kingdom = new Kingdom
        {
            Name = "Test Kingdom",
            Slug = "test-kingdom"
        };
        context.Kingdoms.Add(kingdom);

        var participant = new KingdomParticipant
        {
            KingdomId = kingdom.KingdomId,
            UserAccountId = user.UserAccountId
        };
        context.KingdomParticipants.Add(participant);

        var participantRole = new KingdomParticipantRole
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomRoleTypeId = 3,
            IsPrimaryRole = true
        };
        context.KingdomParticipantRoles.Add(participantRole);

        await context.SaveChangesAsync();

        var permissions = await authzService.GetEffectivePermissionsAsync(user.UserAccountId, kingdom.KingdomId);

        Assert.NotEmpty(permissions);
        Assert.Contains("view-kingdom", permissions);
        Assert.Contains("manage-kingdom", permissions);
        Assert.Contains("edit-map", permissions);
    }

    [Fact]
    public async Task GetEffectivePermissionsAsync_AppliesExplicitGrants()
    {
        await using var context = await CreateInMemoryContextWithReferenceDataAsync();
        var authzService = new KingdomAuthorizationService(context);

        var user = new UserAccount
        {
            DisplayName = "Test User",
            Email = "test@example.com",
            ExternalSubject = Guid.NewGuid().ToString()
        };
        context.UserAccounts.Add(user);

        var kingdom = new Kingdom
        {
            Name = "Test Kingdom",
            Slug = "test-kingdom"
        };
        context.Kingdoms.Add(kingdom);

        var participant = new KingdomParticipant
        {
            KingdomId = kingdom.KingdomId,
            UserAccountId = user.UserAccountId
        };
        context.KingdomParticipants.Add(participant);

        var participantRole = new KingdomParticipantRole
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomRoleTypeId = 12,
            IsPrimaryRole = true
        };
        context.KingdomParticipantRoles.Add(participantRole);

        var explicitPermission = new KingdomParticipantPermission
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomPermissionTypeId = 4,
            PermissionState = "G"
        };
        context.KingdomParticipantPermissions.Add(explicitPermission);

        await context.SaveChangesAsync();

        var permissions = await authzService.GetEffectivePermissionsAsync(user.UserAccountId, kingdom.KingdomId);

        Assert.Contains("view-kingdom", permissions);
        Assert.Contains("edit-map", permissions);
    }

    [Fact]
    public async Task GetEffectivePermissionsAsync_AppliesExplicitDenies()
    {
        await using var context = await CreateInMemoryContextWithReferenceDataAsync();
        var authzService = new KingdomAuthorizationService(context);

        var user = new UserAccount
        {
            DisplayName = "Test User",
            Email = "test@example.com",
            ExternalSubject = Guid.NewGuid().ToString()
        };
        context.UserAccounts.Add(user);

        var kingdom = new Kingdom
        {
            Name = "Test Kingdom",
            Slug = "test-kingdom"
        };
        context.Kingdoms.Add(kingdom);

        var participant = new KingdomParticipant
        {
            KingdomId = kingdom.KingdomId,
            UserAccountId = user.UserAccountId
        };
        context.KingdomParticipants.Add(participant);

        var participantRole = new KingdomParticipantRole
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomRoleTypeId = 3,
            IsPrimaryRole = true
        };
        context.KingdomParticipantRoles.Add(participantRole);

        var explicitPermission = new KingdomParticipantPermission
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomPermissionTypeId = 4,
            PermissionState = "D"
        };
        context.KingdomParticipantPermissions.Add(explicitPermission);

        await context.SaveChangesAsync();

        var permissions = await authzService.GetEffectivePermissionsAsync(user.UserAccountId, kingdom.KingdomId);

        Assert.Contains("view-kingdom", permissions);
        Assert.Contains("manage-kingdom", permissions);
        Assert.DoesNotContain("edit-map", permissions);
    }

    [Fact]
    public async Task HasPermissionAsync_ReturnsTrueForGrantedPermission()
    {
        await using var context = await CreateInMemoryContextWithReferenceDataAsync();
        var authzService = new KingdomAuthorizationService(context);

        var user = new UserAccount
        {
            DisplayName = "Test User",
            Email = "test@example.com",
            ExternalSubject = Guid.NewGuid().ToString()
        };
        context.UserAccounts.Add(user);

        var kingdom = new Kingdom
        {
            Name = "Test Kingdom",
            Slug = "test-kingdom"
        };
        context.Kingdoms.Add(kingdom);

        var participant = new KingdomParticipant
        {
            KingdomId = kingdom.KingdomId,
            UserAccountId = user.UserAccountId
        };
        context.KingdomParticipants.Add(participant);

        var roleType = await context.KingdomRoleTypes.FirstAsync(r => r.Name == "ruler");
        var participantRole = new KingdomParticipantRole
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomRoleTypeId = roleType.KingdomRoleTypeId,
            IsPrimaryRole = true
        };
        context.KingdomParticipantRoles.Add(participantRole);

        await context.SaveChangesAsync();

        var hasPermission = await authzService.HasPermissionAsync(user.UserAccountId, kingdom.KingdomId, "view-kingdom");

        Assert.True(hasPermission);
    }

    [Fact]
    public async Task HasPermissionAsync_ReturnsFalseForNonParticipant()
    {
        await using var context = await CreateInMemoryContextWithReferenceDataAsync();
        var authzService = new KingdomAuthorizationService(context);

        var user = new UserAccount
        {
            DisplayName = "Test User",
            Email = "test@example.com",
            ExternalSubject = Guid.NewGuid().ToString()
        };
        context.UserAccounts.Add(user);

        var kingdom = new Kingdom
        {
            Name = "Test Kingdom",
            Slug = "test-kingdom"
        };
        context.Kingdoms.Add(kingdom);

        await context.SaveChangesAsync();

        var hasPermission = await authzService.HasPermissionAsync(user.UserAccountId, kingdom.KingdomId, "view-kingdom");

        Assert.False(hasPermission);
    }
}
