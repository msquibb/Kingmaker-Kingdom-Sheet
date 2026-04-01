using System.Text.RegularExpressions;
using KingmakerKingdomSheet.ApiService.Data;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.ApiService.Services;

internal sealed partial class SqliteKingdomManagementService(KingmakerDbContext dbContext, IKingdomCatalogService catalogService) : IKingdomManagementService
{
    private const short GameMasterRoleTypeId = 1;

    public async Task<KingdomDetailsDto> CreateAsync(Guid createdByUserId, CreateKingdomDto dto, CancellationToken ct = default)
    {
        var slug = GenerateSlug(dto.Name);
        var now = DateTime.UtcNow;

        var kingdom = new Kingdom
        {
            KingdomId = Guid.NewGuid(),
            Name = dto.Name,
            Slug = slug,
            Description = dto.Description,
            CreatedByUserAccountId = createdByUserId,
            IsArchived = false,
            CreatedUtc = now,
            ModifiedUtc = now
        };

        dbContext.Kingdoms.Add(kingdom);

        var participant = new KingdomParticipant
        {
            KingdomParticipantId = Guid.NewGuid(),
            KingdomId = kingdom.KingdomId,
            UserAccountId = createdByUserId,
            IsActive = true,
            ReceivesRealtimeUpdates = true,
            JoinedUtc = now,
            ModifiedUtc = now
        };

        dbContext.KingdomParticipants.Add(participant);

        var gameMasterRole = new KingdomParticipantRole
        {
            KingdomParticipantId = participant.KingdomParticipantId,
            KingdomRoleTypeId = GameMasterRoleTypeId,
            IsPrimaryRole = true,
            AssignedUtc = now
        };

        dbContext.KingdomParticipantRoles.Add(gameMasterRole);

        await dbContext.SaveChangesAsync(ct);

        var result = await catalogService.GetAsync(kingdom.KingdomId, ct);
        return result!;
    }

    public async Task<KingdomDetailsDto?> UpdateAsync(Guid kingdomId, UpdateKingdomDto dto, CancellationToken ct = default)
    {
        var kingdom = await dbContext.Kingdoms
            .SingleOrDefaultAsync(k => k.KingdomId == kingdomId && !k.IsArchived, ct);

        if (kingdom is null)
            return null;

        kingdom.Name = dto.Name;
        kingdom.Description = dto.Description;
        kingdom.Slug = GenerateSlug(dto.Name);
        kingdom.ModifiedUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);

        return await catalogService.GetAsync(kingdomId, ct);
    }

    public async Task<bool> ArchiveAsync(Guid kingdomId, CancellationToken ct = default)
    {
        var kingdom = await dbContext.Kingdoms
            .SingleOrDefaultAsync(k => k.KingdomId == kingdomId && !k.IsArchived, ct);

        if (kingdom is null)
            return false;

        kingdom.IsArchived = true;
        kingdom.ModifiedUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant();
        slug = SlugWhitespaceRegex().Replace(slug, "-");
        slug = SlugInvalidCharsRegex().Replace(slug, "");
        slug = SlugMultipleHyphensRegex().Replace(slug, "-");
        slug = slug.Trim('-');
        
        return string.IsNullOrWhiteSpace(slug) ? "kingdom" : slug;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex SlugWhitespaceRegex();

    [GeneratedRegex(@"[^a-z0-9-]")]
    private static partial Regex SlugInvalidCharsRegex();

    [GeneratedRegex(@"-+")]
    private static partial Regex SlugMultipleHyphensRegex();
}
