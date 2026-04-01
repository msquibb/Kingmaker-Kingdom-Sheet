using Microsoft.EntityFrameworkCore;

namespace KingmakerKingdomSheet.ApiService.Data;

public sealed class KingmakerDbContext(DbContextOptions<KingmakerDbContext> options) : DbContext(options)
{
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<LocalCredential> LocalCredentials => Set<LocalCredential>();
    public DbSet<Kingdom> Kingdoms => Set<Kingdom>();
    public DbSet<KingdomParticipant> KingdomParticipants => Set<KingdomParticipant>();
    public DbSet<KingdomParticipantRole> KingdomParticipantRoles => Set<KingdomParticipantRole>();
    public DbSet<KingdomParticipantPermission> KingdomParticipantPermissions => Set<KingdomParticipantPermission>();
    public DbSet<Hex> Hexes => Set<Hex>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<HexUpgrade> HexUpgrades => Set<HexUpgrade>();
    public DbSet<ClaimStatus> ClaimStatuses => Set<ClaimStatus>();
    public DbSet<FogState> FogStates => Set<FogState>();
    public DbSet<HexTerrainType> HexTerrainTypes => Set<HexTerrainType>();
    public DbSet<HexUpgradeType> HexUpgradeTypes => Set<HexUpgradeType>();
    public DbSet<SettlementType> SettlementTypes => Set<SettlementType>();
    public DbSet<KingdomPermissionType> KingdomPermissionTypes => Set<KingdomPermissionType>();
    public DbSet<KingdomRoleType> KingdomRoleTypes => Set<KingdomRoleType>();
    public DbSet<KingdomRoleDefaultPermission> KingdomRoleDefaultPermissions => Set<KingdomRoleDefaultPermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.ToTable("app_UserAccount");
            entity.HasKey(item => item.UserAccountId);
            entity.Property(item => item.IdentityProvider).HasMaxLength(100);
            entity.Property(item => item.ExternalSubject).HasMaxLength(200);
            entity.Property(item => item.DisplayName).HasMaxLength(200);
            entity.Property(item => item.Email).HasMaxLength(320);
            entity.Property(item => item.NormalizedEmail).HasMaxLength(320);
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
            entity.HasMany(item => item.CreatedKingdoms)
                .WithOne(item => item.CreatedByUserAccount)
                .HasForeignKey(item => item.CreatedByUserAccountId);
            entity.HasOne(item => item.LocalCredential)
                .WithOne(item => item.UserAccount)
                .HasForeignKey<LocalCredential>(item => item.UserAccountId);
        });

        modelBuilder.Entity<LocalCredential>(entity =>
        {
            entity.ToTable("app_LocalCredential");
            entity.HasKey(item => item.UserAccountId);
            entity.Property(item => item.PasswordHash).HasMaxLength(200);
        });

        modelBuilder.Entity<Kingdom>(entity =>
        {
            entity.ToTable("app_Kingdom");
            entity.HasKey(item => item.KingdomId);
            entity.Property(item => item.Name).HasMaxLength(200);
            entity.Property(item => item.Slug).HasMaxLength(100);
            entity.Property(item => item.Description).HasMaxLength(1000);
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
        });

        modelBuilder.Entity<KingdomParticipant>(entity =>
        {
            entity.ToTable("app_KingdomParticipant");
            entity.HasKey(item => item.KingdomParticipantId);
            entity.Property(item => item.DisplayNameOverride).HasMaxLength(200);
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
            entity.HasOne(item => item.Kingdom)
                .WithMany(item => item.Participants)
                .HasForeignKey(item => item.KingdomId);
            entity.HasOne(item => item.UserAccount)
                .WithMany(item => item.KingdomParticipants)
                .HasForeignKey(item => item.UserAccountId);
        });

        modelBuilder.Entity<KingdomParticipantRole>(entity =>
        {
            entity.ToTable("app_KingdomParticipantRole");
            entity.HasKey(item => new { item.KingdomParticipantId, item.KingdomRoleTypeId });
            entity.HasOne(item => item.KingdomParticipant)
                .WithMany(item => item.Roles)
                .HasForeignKey(item => item.KingdomParticipantId);
            entity.HasOne(item => item.RoleType)
                .WithMany(item => item.ParticipantRoles)
                .HasForeignKey(item => item.KingdomRoleTypeId);
        });

        modelBuilder.Entity<KingdomParticipantPermission>(entity =>
        {
            entity.ToTable("app_KingdomParticipantPermission");
            entity.HasKey(item => new { item.KingdomParticipantId, item.KingdomPermissionTypeId });
            entity.Property(item => item.PermissionState).HasMaxLength(1).IsFixedLength();
            entity.HasOne(item => item.KingdomParticipant)
                .WithMany(item => item.Permissions)
                .HasForeignKey(item => item.KingdomParticipantId);
            entity.HasOne(item => item.PermissionType)
                .WithMany(item => item.ParticipantPermissions)
                .HasForeignKey(item => item.KingdomPermissionTypeId);
        });

        modelBuilder.Entity<Hex>(entity =>
        {
            entity.ToTable("app_Hex");
            entity.HasKey(item => item.HexId);
            entity.HasAlternateKey(item => new { item.HexId, item.KingdomId });
            entity.Property(item => item.Notes).HasMaxLength(1000);
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
            entity.HasOne(item => item.Kingdom)
                .WithMany(item => item.Hexes)
                .HasForeignKey(item => item.KingdomId);
            entity.HasOne(item => item.TerrainType)
                .WithMany(item => item.Hexes)
                .HasForeignKey(item => item.HexTerrainTypeId);
            entity.HasOne(item => item.ClaimStatus)
                .WithMany(item => item.Hexes)
                .HasForeignKey(item => item.ClaimStatusId);
            entity.HasOne(item => item.FogState)
                .WithMany(item => item.Hexes)
                .HasForeignKey(item => item.FogStateId);
        });

        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.ToTable("app_Settlement");
            entity.HasKey(item => item.SettlementId);
            entity.Property(item => item.Name).HasMaxLength(200);
            entity.Property(item => item.Notes).HasMaxLength(1000);
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
            entity.HasOne(item => item.Kingdom)
                .WithMany(item => item.Settlements)
                .HasForeignKey(item => item.KingdomId);
            entity.HasOne(item => item.SettlementType)
                .WithMany(item => item.Settlements)
                .HasForeignKey(item => item.SettlementTypeId);
            entity.HasOne(item => item.Hex)
                .WithMany(item => item.Settlements)
                .HasForeignKey(item => new { item.HexId, item.KingdomId })
                .HasPrincipalKey(item => new { item.HexId, item.KingdomId })
                .IsRequired(false);
        });

        modelBuilder.Entity<HexUpgrade>(entity =>
        {
            entity.ToTable("app_HexUpgrade");
            entity.HasKey(item => item.HexUpgradeId);
            entity.Property(item => item.Notes).HasMaxLength(1000);
            entity.Property(item => item.RowVersion).IsConcurrencyToken();
            entity.HasOne(item => item.Hex)
                .WithMany(item => item.Upgrades)
                .HasForeignKey(item => item.HexId);
            entity.HasOne(item => item.HexUpgradeType)
                .WithMany(item => item.HexUpgrades)
                .HasForeignKey(item => item.HexUpgradeTypeId);
        });

        modelBuilder.Entity<ClaimStatus>(entity =>
        {
            entity.ToTable("ref_ClaimStatus");
            entity.HasKey(item => item.ClaimStatusId);
            entity.Property(item => item.Name).HasMaxLength(50);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
        });

        modelBuilder.Entity<FogState>(entity =>
        {
            entity.ToTable("ref_FogState");
            entity.HasKey(item => item.FogStateId);
            entity.Property(item => item.Name).HasMaxLength(50);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
        });

        modelBuilder.Entity<HexTerrainType>(entity =>
        {
            entity.ToTable("ref_HexTerrainType");
            entity.HasKey(item => item.HexTerrainTypeId);
            entity.Property(item => item.Name).HasMaxLength(50);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
            entity.Property(item => item.MovementCost).HasPrecision(4, 2);
        });

        modelBuilder.Entity<HexUpgradeType>(entity =>
        {
            entity.ToTable("ref_HexUpgradeType");
            entity.HasKey(item => item.HexUpgradeTypeId);
            entity.Property(item => item.Name).HasMaxLength(50);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
            entity.Property(item => item.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<SettlementType>(entity =>
        {
            entity.ToTable("ref_SettlementType");
            entity.HasKey(item => item.SettlementTypeId);
            entity.Property(item => item.Name).HasMaxLength(50);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
        });

        modelBuilder.Entity<KingdomPermissionType>(entity =>
        {
            entity.ToTable("ref_KingdomPermissionType");
            entity.HasKey(item => item.KingdomPermissionTypeId);
            entity.Property(item => item.Name).HasMaxLength(100);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
            entity.Property(item => item.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<KingdomRoleType>(entity =>
        {
            entity.ToTable("ref_KingdomRoleType");
            entity.HasKey(item => item.KingdomRoleTypeId);
            entity.Property(item => item.Name).HasMaxLength(100);
            entity.Property(item => item.DisplayName).HasMaxLength(100);
        });

        modelBuilder.Entity<KingdomRoleDefaultPermission>(entity =>
        {
            entity.ToTable("ref_KingdomRoleDefaultPermission");
            entity.HasKey(item => new { item.KingdomRoleTypeId, item.KingdomPermissionTypeId });
            entity.HasOne(item => item.RoleType)
                .WithMany(item => item.DefaultPermissions)
                .HasForeignKey(item => item.KingdomRoleTypeId);
            entity.HasOne(item => item.PermissionType)
                .WithMany(item => item.RoleDefaultPermissions)
                .HasForeignKey(item => item.KingdomPermissionTypeId);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        PrepareForSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        PrepareForSave();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void PrepareForSave()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not EntityState.Added and not EntityState.Modified)
            {
                continue;
            }

            switch (entry.Entity)
            {
                case UserAccount userAccount:
                    if (entry.State == EntityState.Added && userAccount.UserAccountId == Guid.Empty)
                    {
                        userAccount.UserAccountId = Guid.NewGuid();
                    }

                    userAccount.NormalizedEmail = string.IsNullOrWhiteSpace(userAccount.Email)
                        ? null
                        : userAccount.Email.Trim().ToUpperInvariant();

                    userAccount.ModifiedUtc = utcNow;
                    userAccount.CreatedUtc = entry.State == EntityState.Added && userAccount.CreatedUtc == default
                        ? utcNow
                        : userAccount.CreatedUtc;
                    userAccount.RowVersion = entry.State == EntityState.Added
                        ? Math.Max(userAccount.RowVersion, 1)
                        : userAccount.RowVersion + 1;
                    break;

                case Kingdom kingdom:
                    ApplyTrackedEntityDefaults(entry.State, kingdom, utcNow, () => kingdom.KingdomId = Guid.NewGuid());
                    break;

                case KingdomParticipant participant:
                    if (entry.State == EntityState.Added && participant.KingdomParticipantId == Guid.Empty)
                    {
                        participant.KingdomParticipantId = Guid.NewGuid();
                    }

                    participant.JoinedUtc = entry.State == EntityState.Added && participant.JoinedUtc == default
                        ? utcNow
                        : participant.JoinedUtc;
                    participant.ModifiedUtc = utcNow;
                    participant.RowVersion = entry.State == EntityState.Added
                        ? Math.Max(participant.RowVersion, 1)
                        : participant.RowVersion + 1;
                    break;

                case Hex hex:
                    ApplyTrackedEntityDefaults(entry.State, hex, utcNow, () => hex.HexId = Guid.NewGuid());
                    break;

                case Settlement settlement:
                    ApplyTrackedEntityDefaults(entry.State, settlement, utcNow, () => settlement.SettlementId = Guid.NewGuid());
                    break;

                case HexUpgrade hexUpgrade:
                    ApplyTrackedEntityDefaults(entry.State, hexUpgrade, utcNow, () => hexUpgrade.HexUpgradeId = Guid.NewGuid());
                    break;

                case KingdomParticipantRole participantRole when entry.State == EntityState.Added && participantRole.AssignedUtc == default:
                    participantRole.AssignedUtc = utcNow;
                    break;

                case KingdomParticipantPermission participantPermission when entry.State == EntityState.Added && participantPermission.AssignedUtc == default:
                    participantPermission.AssignedUtc = utcNow;
                    break;

                case LocalCredential localCredential:
                    localCredential.ModifiedUtc = utcNow;
                    localCredential.CreatedUtc = entry.State == EntityState.Added && localCredential.CreatedUtc == default
                        ? utcNow
                        : localCredential.CreatedUtc;
                    break;
            }
        }
    }

    private static void ApplyTrackedEntityDefaults(EntityState state, Kingdom kingdom, DateTime utcNow, Action initializeKey)
    {
        if (state == EntityState.Added && kingdom.KingdomId == Guid.Empty)
        {
            initializeKey();
        }

        kingdom.CreatedUtc = state == EntityState.Added && kingdom.CreatedUtc == default ? utcNow : kingdom.CreatedUtc;
        kingdom.ModifiedUtc = utcNow;
        kingdom.RowVersion = state == EntityState.Added ? Math.Max(kingdom.RowVersion, 1) : kingdom.RowVersion + 1;
    }

    private static void ApplyTrackedEntityDefaults(EntityState state, Hex hex, DateTime utcNow, Action initializeKey)
    {
        if (state == EntityState.Added && hex.HexId == Guid.Empty)
        {
            initializeKey();
        }

        hex.CreatedUtc = state == EntityState.Added && hex.CreatedUtc == default ? utcNow : hex.CreatedUtc;
        hex.ModifiedUtc = utcNow;
        hex.RowVersion = state == EntityState.Added ? Math.Max(hex.RowVersion, 1) : hex.RowVersion + 1;
    }

    private static void ApplyTrackedEntityDefaults(EntityState state, Settlement settlement, DateTime utcNow, Action initializeKey)
    {
        if (state == EntityState.Added && settlement.SettlementId == Guid.Empty)
        {
            initializeKey();
        }

        settlement.CreatedUtc = state == EntityState.Added && settlement.CreatedUtc == default ? utcNow : settlement.CreatedUtc;
        settlement.ModifiedUtc = utcNow;
        settlement.RowVersion = state == EntityState.Added ? Math.Max(settlement.RowVersion, 1) : settlement.RowVersion + 1;
    }

    private static void ApplyTrackedEntityDefaults(EntityState state, HexUpgrade hexUpgrade, DateTime utcNow, Action initializeKey)
    {
        if (state == EntityState.Added && hexUpgrade.HexUpgradeId == Guid.Empty)
        {
            initializeKey();
        }

        hexUpgrade.CreatedUtc = state == EntityState.Added && hexUpgrade.CreatedUtc == default ? utcNow : hexUpgrade.CreatedUtc;
        hexUpgrade.ModifiedUtc = utcNow;
        hexUpgrade.RowVersion = state == EntityState.Added ? Math.Max(hexUpgrade.RowVersion, 1) : hexUpgrade.RowVersion + 1;
    }
}
