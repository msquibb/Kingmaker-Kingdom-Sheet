using KingmakerKingdomSheet.ApiService.Auth;
using KingmakerKingdomSheet.Application.Contracts;
using KingmakerKingdomSheet.Shared.DTOs;
using System.Security.Claims;

namespace KingmakerKingdomSheet.ApiService.Endpoints;

public static class SettlementEndpoints
{
    public static void MapSettlementEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/kingdoms/{kingdomId:guid}/settlements");

        group.MapGet("/", ListSettlements);
        group.MapGet("/{id:guid}", GetSettlement);
        group.MapPost("/", CreateSettlement).RequireAuthorization();
        group.MapPut("/{id:guid}", UpdateSettlement).RequireAuthorization();
        group.MapDelete("/{id:guid}", DeleteSettlement).RequireAuthorization();
    }

    private static async Task<IResult> ListSettlements(
        Guid kingdomId,
        ISettlementService settlementService,
        CancellationToken ct)
    {
        var settlements = await settlementService.ListByKingdomAsync(kingdomId, ct);
        return Results.Ok(settlements);
    }

    private static async Task<IResult> GetSettlement(
        Guid kingdomId,
        Guid id,
        ISettlementService settlementService,
        CancellationToken ct)
    {
        var settlement = await settlementService.GetAsync(id, ct);

        if (settlement is null)
            return Results.NotFound(new { error = "Settlement not found" });

        return Results.Ok(settlement);
    }

    private static async Task<IResult> CreateSettlement(
        Guid kingdomId,
        CreateSettlementDto dto,
        HttpContext httpContext,
        ISettlementService settlementService,
        IKingdomAuthorizationService authService,
        CancellationToken ct)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        if (!await authService.HasPermissionAsync(userId, kingdomId, "manage-settlements", ct))
            return Results.Forbid();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Results.BadRequest(new { error = "Settlement name is required" });

        var settlement = await settlementService.CreateAsync(kingdomId, dto, ct);
        return Results.Created($"/api/kingdoms/{kingdomId}/settlements/{settlement.Id}", settlement);
    }

    private static async Task<IResult> UpdateSettlement(
        Guid kingdomId,
        Guid id,
        UpdateSettlementDto dto,
        HttpContext httpContext,
        ISettlementService settlementService,
        IKingdomAuthorizationService authService,
        CancellationToken ct)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        if (!await authService.HasPermissionAsync(userId, kingdomId, "manage-settlements", ct))
            return Results.Forbid();

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Results.BadRequest(new { error = "Settlement name is required" });

        var settlement = await settlementService.UpdateAsync(id, dto, ct);

        if (settlement is null)
            return Results.NotFound(new { error = "Settlement not found" });

        return Results.Ok(settlement);
    }

    private static async Task<IResult> DeleteSettlement(
        Guid kingdomId,
        Guid id,
        HttpContext httpContext,
        ISettlementService settlementService,
        IKingdomAuthorizationService authService,
        CancellationToken ct)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Results.Unauthorized();

        if (!await authService.HasPermissionAsync(userId, kingdomId, "manage-settlements", ct))
            return Results.Forbid();

        var success = await settlementService.DeleteAsync(id, ct);

        if (!success)
            return Results.NotFound(new { error = "Settlement not found" });

        return Results.NoContent();
    }
}
