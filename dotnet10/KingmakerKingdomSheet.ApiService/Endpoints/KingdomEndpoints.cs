using KingmakerKingdomSheet.ApiService.Auth;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using System.Security.Claims;

namespace KingmakerKingdomSheet.ApiService.Endpoints;

public static class KingdomEndpoints
{
    public static void MapKingdomEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/kingdoms");

        group.MapGet("/", ListKingdoms);
        group.MapGet("/{id:guid}", GetKingdom);
        group.MapPost("/", CreateKingdom).RequireAuthorization();
        group.MapPut("/{id:guid}", UpdateKingdom).RequireAuthorization();
        group.MapDelete("/{id:guid}", ArchiveKingdom).RequireAuthorization();
    }

    private static async Task<IResult> ListKingdoms(
        IKingdomCatalogService catalogService,
        CancellationToken ct)
    {
        var kingdoms = await catalogService.ListAsync(ct);
        return Results.Ok(kingdoms);
    }

    private static async Task<IResult> GetKingdom(
        Guid id,
        IKingdomCatalogService catalogService,
        CancellationToken ct)
    {
        var kingdom = await catalogService.GetAsync(id, ct);
        
        if (kingdom is null)
            return Results.NotFound(new { error = "Kingdom not found" });

        return Results.Ok(kingdom);
    }

    private static async Task<IResult> CreateKingdom(
        CreateKingdomDto dto,
        HttpContext httpContext,
        IKingdomManagementService managementService,
        CancellationToken ct)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Results.BadRequest(new { error = "Kingdom name is required" });

        var kingdom = await managementService.CreateAsync(userId, dto, ct);
        return Results.Created($"/api/kingdoms/{kingdom.Id}", kingdom);
    }

    private static async Task<IResult> UpdateKingdom(
        Guid id,
        UpdateKingdomDto dto,
        HttpContext httpContext,
        IKingdomManagementService managementService,
        IKingdomAuthorizationService authService,
        CancellationToken ct)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        if (!await authService.HasPermissionAsync(userId, id, "manage-kingdom", ct))
            return Results.Forbid();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Results.BadRequest(new { error = "Kingdom name is required" });

        var kingdom = await managementService.UpdateAsync(id, dto, ct);

        if (kingdom is null)
            return Results.NotFound(new { error = "Kingdom not found" });

        return Results.Ok(kingdom);
    }

    private static async Task<IResult> ArchiveKingdom(
        Guid id,
        HttpContext httpContext,
        IKingdomManagementService managementService,
        IKingdomAuthorizationService authService,
        CancellationToken ct)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        if (!await authService.HasPermissionAsync(userId, id, "manage-kingdom", ct))
            return Results.Forbid();

        var success = await managementService.ArchiveAsync(id, ct);

        if (!success)
            return Results.NotFound(new { error = "Kingdom not found" });

        return Results.NoContent();
    }
}
