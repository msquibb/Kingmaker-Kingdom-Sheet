namespace KingmakerKingdomSheet.ApiService.Data;

public sealed class UserAccount
{
    public Guid UserAccountId { get; set; }
    public string IdentityProvider { get; set; } = "local";
    public string ExternalSubject { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public long RowVersion { get; set; }
    public LocalCredential? LocalCredential { get; set; }
    public ICollection<Kingdom> CreatedKingdoms { get; } = new List<Kingdom>();
    public ICollection<KingdomParticipant> KingdomParticipants { get; } = new List<KingdomParticipant>();
}

public sealed class Kingdom
{
    public Guid KingdomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? FoundedOn { get; set; }
    public Guid? CreatedByUserAccountId { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public long RowVersion { get; set; }
    public UserAccount? CreatedByUserAccount { get; set; }
    public ICollection<KingdomParticipant> Participants { get; } = new List<KingdomParticipant>();
    public ICollection<Hex> Hexes { get; } = new List<Hex>();
    public ICollection<Settlement> Settlements { get; } = new List<Settlement>();
}

public sealed class KingdomParticipant
{
    public Guid KingdomParticipantId { get; set; }
    public Guid KingdomId { get; set; }
    public Guid UserAccountId { get; set; }
    public string? DisplayNameOverride { get; set; }
    public bool IsActive { get; set; } = true;
    public bool ReceivesRealtimeUpdates { get; set; } = true;
    public DateTime JoinedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public long RowVersion { get; set; }
    public Kingdom Kingdom { get; set; } = null!;
    public UserAccount UserAccount { get; set; } = null!;
    public ICollection<KingdomParticipantRole> Roles { get; } = new List<KingdomParticipantRole>();
    public ICollection<KingdomParticipantPermission> Permissions { get; } = new List<KingdomParticipantPermission>();
}

public sealed class KingdomParticipantRole
{
    public Guid KingdomParticipantId { get; set; }
    public short KingdomRoleTypeId { get; set; }
    public bool IsPrimaryRole { get; set; }
    public DateTime AssignedUtc { get; set; }
    public KingdomParticipant KingdomParticipant { get; set; } = null!;
    public KingdomRoleType RoleType { get; set; } = null!;
}

public sealed class KingdomParticipantPermission
{
    public Guid KingdomParticipantId { get; set; }
    public short KingdomPermissionTypeId { get; set; }
    public string PermissionState { get; set; } = "G";
    public DateTime AssignedUtc { get; set; }
    public KingdomParticipant KingdomParticipant { get; set; } = null!;
    public KingdomPermissionType PermissionType { get; set; } = null!;
}

public sealed class Hex
{
    public Guid HexId { get; set; }
    public Guid KingdomId { get; set; }
    public int CoordinateX { get; set; }
    public int CoordinateY { get; set; }
    public short HexTerrainTypeId { get; set; }
    public byte ClaimStatusId { get; set; }
    public byte FogStateId { get; set; }
    public DateTime? ExploredUtc { get; set; }
    public DateTime? ClaimedUtc { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public long RowVersion { get; set; }
    public Kingdom Kingdom { get; set; } = null!;
    public HexTerrainType TerrainType { get; set; } = null!;
    public ClaimStatus ClaimStatus { get; set; } = null!;
    public FogState FogState { get; set; } = null!;
    public ICollection<Settlement> Settlements { get; } = new List<Settlement>();
    public ICollection<HexUpgrade> Upgrades { get; } = new List<HexUpgrade>();
}

public sealed class Settlement
{
    public Guid SettlementId { get; set; }
    public Guid KingdomId { get; set; }
    public byte SettlementTypeId { get; set; }
    public Guid? HexId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? Population { get; set; }
    public DateOnly? FoundedOn { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public long RowVersion { get; set; }
    public Kingdom Kingdom { get; set; } = null!;
    public SettlementType SettlementType { get; set; } = null!;
    public Hex? Hex { get; set; }
}

public sealed class HexUpgrade
{
    public Guid HexUpgradeId { get; set; }
    public Guid HexId { get; set; }
    public short HexUpgradeTypeId { get; set; }
    public short Quantity { get; set; } = 1;
    public DateOnly? BuiltOn { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public long RowVersion { get; set; }
    public Hex Hex { get; set; } = null!;
    public HexUpgradeType HexUpgradeType { get; set; } = null!;
}

public sealed class ClaimStatus
{
    public byte ClaimStatusId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsClaimed { get; set; }
    public byte SortOrder { get; set; }
    public ICollection<Hex> Hexes { get; } = new List<Hex>();
}

public sealed class FogState
{
    public byte FogStateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public byte SortOrder { get; set; }
    public ICollection<Hex> Hexes { get; } = new List<Hex>();
}

public sealed class HexTerrainType
{
    public short HexTerrainTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal MovementCost { get; set; }
    public bool IsWater { get; set; }
    public bool SupportsSettlement { get; set; }
    public short SortOrder { get; set; }
    public ICollection<Hex> Hexes { get; } = new List<Hex>();
}

public sealed class HexUpgradeType
{
    public short HexUpgradeTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public short BuildPointCost { get; set; }
    public bool CanStack { get; set; }
    public bool RequiresClaimedHex { get; set; }
    public string? Description { get; set; }
    public short SortOrder { get; set; }
    public ICollection<HexUpgrade> HexUpgrades { get; } = new List<HexUpgrade>();
}

public sealed class SettlementType
{
    public byte SettlementTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public short MinDistricts { get; set; }
    public byte SortOrder { get; set; }
    public ICollection<Settlement> Settlements { get; } = new List<Settlement>();
}

public sealed class KingdomPermissionType
{
    public short KingdomPermissionTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short SortOrder { get; set; }
    public ICollection<KingdomParticipantPermission> ParticipantPermissions { get; } = new List<KingdomParticipantPermission>();
    public ICollection<KingdomRoleDefaultPermission> RoleDefaultPermissions { get; } = new List<KingdomRoleDefaultPermission>();
}

public sealed class KingdomRoleType
{
    public short KingdomRoleTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsGameMasterRole { get; set; }
    public short SortOrder { get; set; }
    public ICollection<KingdomParticipantRole> ParticipantRoles { get; } = new List<KingdomParticipantRole>();
    public ICollection<KingdomRoleDefaultPermission> DefaultPermissions { get; } = new List<KingdomRoleDefaultPermission>();
}

public sealed class KingdomRoleDefaultPermission
{
    public short KingdomRoleTypeId { get; set; }
    public short KingdomPermissionTypeId { get; set; }
    public KingdomRoleType RoleType { get; set; } = null!;
    public KingdomPermissionType PermissionType { get; set; } = null!;
}

public sealed class LocalCredential
{
    public Guid UserAccountId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEndUtc { get; set; }
    public DateTime? LastLoginUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime ModifiedUtc { get; set; }
    public UserAccount UserAccount { get; set; } = null!;
}
