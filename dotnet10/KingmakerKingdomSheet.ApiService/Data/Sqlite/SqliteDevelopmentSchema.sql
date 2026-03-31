PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS ref_ClaimStatus
(
    ClaimStatusId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    IsClaimed INTEGER NOT NULL DEFAULT 0,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_ClaimStatus_Name UNIQUE (Name)
);

CREATE TABLE IF NOT EXISTS ref_FogState
(
    FogStateId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_FogState_Name UNIQUE (Name)
);

CREATE TABLE IF NOT EXISTS ref_HexTerrainType
(
    HexTerrainTypeId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    MovementCost REAL NOT NULL DEFAULT 1.00,
    IsWater INTEGER NOT NULL DEFAULT 0,
    SupportsSettlement INTEGER NOT NULL DEFAULT 1,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_HexTerrainType_Name UNIQUE (Name),
    CONSTRAINT CK_ref_HexTerrainType_MovementCost CHECK (MovementCost > 0)
);

CREATE TABLE IF NOT EXISTS ref_HexUpgradeType
(
    HexUpgradeTypeId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    BuildPointCost INTEGER NOT NULL,
    CanStack INTEGER NOT NULL DEFAULT 0,
    RequiresClaimedHex INTEGER NOT NULL DEFAULT 1,
    Description TEXT NULL,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_HexUpgradeType_Name UNIQUE (Name),
    CONSTRAINT CK_ref_HexUpgradeType_BuildPointCost CHECK (BuildPointCost >= 0)
);

CREATE TABLE IF NOT EXISTS ref_SettlementType
(
    SettlementTypeId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    MinDistricts INTEGER NOT NULL DEFAULT 0,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_SettlementType_Name UNIQUE (Name),
    CONSTRAINT CK_ref_SettlementType_MinDistricts CHECK (MinDistricts >= 0)
);

CREATE TABLE IF NOT EXISTS ref_KingdomPermissionType
(
    KingdomPermissionTypeId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    Description TEXT NULL,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_KingdomPermissionType_Name UNIQUE (Name)
);

CREATE TABLE IF NOT EXISTS ref_KingdomRoleType
(
    KingdomRoleTypeId INTEGER NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    IsGameMasterRole INTEGER NOT NULL DEFAULT 0,
    SortOrder INTEGER NOT NULL,
    CONSTRAINT UQ_ref_KingdomRoleType_Name UNIQUE (Name)
);

CREATE TABLE IF NOT EXISTS app_UserAccount
(
    UserAccountId TEXT NOT NULL PRIMARY KEY,
    IdentityProvider TEXT NOT NULL DEFAULT 'local',
    ExternalSubject TEXT NOT NULL,
    DisplayName TEXT NOT NULL,
    Email TEXT NULL,
    NormalizedEmail TEXT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT UQ_app_UserAccount_IdentityProvider_ExternalSubject UNIQUE (IdentityProvider, ExternalSubject),
    CONSTRAINT CK_app_UserAccount_DisplayName_NotBlank CHECK (length(trim(DisplayName)) > 0)
);

CREATE UNIQUE INDEX IF NOT EXISTS UX_app_UserAccount_NormalizedEmail
    ON app_UserAccount (NormalizedEmail)
    WHERE NormalizedEmail IS NOT NULL;

CREATE TABLE IF NOT EXISTS app_Kingdom
(
    KingdomId TEXT NOT NULL PRIMARY KEY,
    Name TEXT NOT NULL,
    Slug TEXT NOT NULL,
    Description TEXT NULL,
    FoundedOn TEXT NULL,
    CreatedByUserAccountId TEXT NULL,
    IsArchived INTEGER NOT NULL DEFAULT 0,
    CreatedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT FK_app_Kingdom_CreatedByUserAccount FOREIGN KEY (CreatedByUserAccountId) REFERENCES app_UserAccount (UserAccountId),
    CONSTRAINT UQ_app_Kingdom_Slug UNIQUE (Slug),
    CONSTRAINT CK_app_Kingdom_Name_NotBlank CHECK (length(trim(Name)) > 0),
    CONSTRAINT CK_app_Kingdom_Slug_Format CHECK (length(Slug) > 0 AND Slug NOT GLOB '*[^a-z0-9-]*')
);

CREATE TABLE IF NOT EXISTS app_KingdomParticipant
(
    KingdomParticipantId TEXT NOT NULL PRIMARY KEY,
    KingdomId TEXT NOT NULL,
    UserAccountId TEXT NOT NULL,
    DisplayNameOverride TEXT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    ReceivesRealtimeUpdates INTEGER NOT NULL DEFAULT 1,
    JoinedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT FK_app_KingdomParticipant_Kingdom FOREIGN KEY (KingdomId) REFERENCES app_Kingdom (KingdomId),
    CONSTRAINT FK_app_KingdomParticipant_UserAccount FOREIGN KEY (UserAccountId) REFERENCES app_UserAccount (UserAccountId),
    CONSTRAINT UQ_app_KingdomParticipant_Kingdom_User UNIQUE (KingdomId, UserAccountId)
);

CREATE INDEX IF NOT EXISTS IX_app_KingdomParticipant_UserAccount
    ON app_KingdomParticipant (UserAccountId, IsActive);

CREATE TABLE IF NOT EXISTS app_Hex
(
    HexId TEXT NOT NULL PRIMARY KEY,
    KingdomId TEXT NOT NULL,
    CoordinateX INTEGER NOT NULL,
    CoordinateY INTEGER NOT NULL,
    HexTerrainTypeId INTEGER NOT NULL,
    ClaimStatusId INTEGER NOT NULL DEFAULT 1,
    FogStateId INTEGER NOT NULL DEFAULT 1,
    ExploredUtc TEXT NULL,
    ClaimedUtc TEXT NULL,
    Notes TEXT NULL,
    CreatedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT FK_app_Hex_Kingdom FOREIGN KEY (KingdomId) REFERENCES app_Kingdom (KingdomId),
    CONSTRAINT FK_app_Hex_HexTerrainType FOREIGN KEY (HexTerrainTypeId) REFERENCES ref_HexTerrainType (HexTerrainTypeId),
    CONSTRAINT FK_app_Hex_ClaimStatus FOREIGN KEY (ClaimStatusId) REFERENCES ref_ClaimStatus (ClaimStatusId),
    CONSTRAINT FK_app_Hex_FogState FOREIGN KEY (FogStateId) REFERENCES ref_FogState (FogStateId),
    CONSTRAINT UQ_app_Hex_Kingdom_Coordinates UNIQUE (KingdomId, CoordinateX, CoordinateY),
    CONSTRAINT CK_app_Hex_CoordinateX_Range CHECK (CoordinateX BETWEEN -5000 AND 5000),
    CONSTRAINT CK_app_Hex_CoordinateY_Range CHECK (CoordinateY BETWEEN -5000 AND 5000)
);

CREATE UNIQUE INDEX IF NOT EXISTS UQ_app_Hex_HexId_Kingdom
    ON app_Hex (HexId, KingdomId);

CREATE INDEX IF NOT EXISTS IX_app_Hex_Kingdom_Claim_Fog
    ON app_Hex (KingdomId, ClaimStatusId, FogStateId);

CREATE TABLE IF NOT EXISTS app_Settlement
(
    SettlementId TEXT NOT NULL PRIMARY KEY,
    KingdomId TEXT NOT NULL,
    SettlementTypeId INTEGER NOT NULL,
    HexId TEXT NULL,
    Name TEXT NOT NULL,
    Population INTEGER NULL,
    FoundedOn TEXT NULL,
    Notes TEXT NULL,
    CreatedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT FK_app_Settlement_Kingdom FOREIGN KEY (KingdomId) REFERENCES app_Kingdom (KingdomId),
    CONSTRAINT FK_app_Settlement_SettlementType FOREIGN KEY (SettlementTypeId) REFERENCES ref_SettlementType (SettlementTypeId),
    CONSTRAINT FK_app_Settlement_Hex FOREIGN KEY (HexId, KingdomId) REFERENCES app_Hex (HexId, KingdomId),
    CONSTRAINT UQ_app_Settlement_Kingdom_Name UNIQUE (KingdomId, Name),
    CONSTRAINT CK_app_Settlement_Name_NotBlank CHECK (length(trim(Name)) > 0),
    CONSTRAINT CK_app_Settlement_Population CHECK (Population IS NULL OR Population >= 0)
);

CREATE UNIQUE INDEX IF NOT EXISTS UX_app_Settlement_HexId
    ON app_Settlement (HexId)
    WHERE HexId IS NOT NULL;

CREATE INDEX IF NOT EXISTS IX_app_Settlement_Kingdom_Type
    ON app_Settlement (KingdomId, SettlementTypeId);

CREATE TABLE IF NOT EXISTS app_HexUpgrade
(
    HexUpgradeId TEXT NOT NULL PRIMARY KEY,
    HexId TEXT NOT NULL,
    HexUpgradeTypeId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL DEFAULT 1,
    BuiltOn TEXT NULL,
    Notes TEXT NULL,
    CreatedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    CONSTRAINT FK_app_HexUpgrade_Hex FOREIGN KEY (HexId) REFERENCES app_Hex (HexId) ON DELETE CASCADE,
    CONSTRAINT FK_app_HexUpgrade_HexUpgradeType FOREIGN KEY (HexUpgradeTypeId) REFERENCES ref_HexUpgradeType (HexUpgradeTypeId),
    CONSTRAINT UQ_app_HexUpgrade_Hex_UpgradeType UNIQUE (HexId, HexUpgradeTypeId),
    CONSTRAINT CK_app_HexUpgrade_Quantity CHECK (Quantity > 0)
);

CREATE INDEX IF NOT EXISTS IX_app_HexUpgrade_UpgradeType
    ON app_HexUpgrade (HexUpgradeTypeId, HexId);

CREATE TABLE IF NOT EXISTS app_KingdomParticipantRole
(
    KingdomParticipantId TEXT NOT NULL,
    KingdomRoleTypeId INTEGER NOT NULL,
    IsPrimaryRole INTEGER NOT NULL DEFAULT 0,
    AssignedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (KingdomParticipantId, KingdomRoleTypeId),
    CONSTRAINT FK_app_KingdomParticipantRole_Participant FOREIGN KEY (KingdomParticipantId) REFERENCES app_KingdomParticipant (KingdomParticipantId) ON DELETE CASCADE,
    CONSTRAINT FK_app_KingdomParticipantRole_RoleType FOREIGN KEY (KingdomRoleTypeId) REFERENCES ref_KingdomRoleType (KingdomRoleTypeId)
);

CREATE UNIQUE INDEX IF NOT EXISTS UX_app_KingdomParticipantRole_PrimaryRole
    ON app_KingdomParticipantRole (KingdomParticipantId)
    WHERE IsPrimaryRole = 1;

CREATE INDEX IF NOT EXISTS IX_app_KingdomParticipantRole_RoleType
    ON app_KingdomParticipantRole (KingdomRoleTypeId, KingdomParticipantId);

CREATE TABLE IF NOT EXISTS app_KingdomParticipantPermission
(
    KingdomParticipantId TEXT NOT NULL,
    KingdomPermissionTypeId INTEGER NOT NULL,
    PermissionState TEXT NOT NULL,
    AssignedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (KingdomParticipantId, KingdomPermissionTypeId),
    CONSTRAINT FK_app_KingdomParticipantPermission_Participant FOREIGN KEY (KingdomParticipantId) REFERENCES app_KingdomParticipant (KingdomParticipantId) ON DELETE CASCADE,
    CONSTRAINT FK_app_KingdomParticipantPermission_PermissionType FOREIGN KEY (KingdomPermissionTypeId) REFERENCES ref_KingdomPermissionType (KingdomPermissionTypeId),
    CONSTRAINT CK_app_KingdomParticipantPermission_PermissionState CHECK (PermissionState IN ('G', 'D'))
);

CREATE INDEX IF NOT EXISTS IX_app_KingdomParticipantPermission_PermissionType
    ON app_KingdomParticipantPermission (KingdomPermissionTypeId, PermissionState);

CREATE TABLE IF NOT EXISTS ref_KingdomRoleDefaultPermission
(
    KingdomRoleTypeId INTEGER NOT NULL,
    KingdomPermissionTypeId INTEGER NOT NULL,
    PRIMARY KEY (KingdomRoleTypeId, KingdomPermissionTypeId),
    CONSTRAINT FK_ref_KingdomRoleDefaultPermission_RoleType FOREIGN KEY (KingdomRoleTypeId) REFERENCES ref_KingdomRoleType (KingdomRoleTypeId),
    CONSTRAINT FK_ref_KingdomRoleDefaultPermission_PermissionType FOREIGN KEY (KingdomPermissionTypeId) REFERENCES ref_KingdomPermissionType (KingdomPermissionTypeId)
);

CREATE INDEX IF NOT EXISTS IX_ref_KingdomRoleDefaultPermission_PermissionType
    ON ref_KingdomRoleDefaultPermission (KingdomPermissionTypeId);

INSERT INTO ref_ClaimStatus (ClaimStatusId, Name, DisplayName, IsClaimed, SortOrder)
VALUES
    (1, 'unclaimed', 'Unclaimed', 0, 10),
    (2, 'claimed', 'Claimed', 1, 20),
    (3, 'disputed', 'Disputed', 0, 30),
    (4, 'abandoned', 'Abandoned', 0, 40)
ON CONFLICT (ClaimStatusId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    IsClaimed = excluded.IsClaimed,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_FogState (FogStateId, Name, DisplayName, SortOrder)
VALUES
    (1, 'hidden', 'Hidden', 10),
    (2, 'explored', 'Explored', 20),
    (3, 'revealed', 'Revealed', 30)
ON CONFLICT (FogStateId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_HexTerrainType (HexTerrainTypeId, Name, DisplayName, MovementCost, IsWater, SupportsSettlement, SortOrder)
VALUES
    (1, 'plains', 'Plains', 1.00, 0, 1, 10),
    (2, 'forest', 'Forest', 1.50, 0, 1, 20),
    (3, 'hills', 'Hills', 1.50, 0, 1, 30),
    (4, 'mountains', 'Mountains', 2.00, 0, 0, 40),
    (5, 'swamp', 'Swamp', 2.00, 0, 0, 50),
    (6, 'river', 'River', 1.00, 1, 0, 60),
    (7, 'lake', 'Lake', 1.00, 1, 0, 70),
    (8, 'ruins', 'Ruins', 1.50, 0, 1, 80)
ON CONFLICT (HexTerrainTypeId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    MovementCost = excluded.MovementCost,
    IsWater = excluded.IsWater,
    SupportsSettlement = excluded.SupportsSettlement,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_HexUpgradeType (HexUpgradeTypeId, Name, DisplayName, BuildPointCost, CanStack, RequiresClaimedHex, Description, SortOrder)
VALUES
    (1, 'farm', 'Farm', 1, 0, 1, 'Food-producing improvement for fertile land.', 10),
    (2, 'fishery', 'Fishery', 2, 0, 1, 'Harvests river or lake hex resources.', 20),
    (3, 'lumber-camp', 'Lumber Camp', 2, 0, 1, 'Converts wooded hexes into usable lumber output.', 30),
    (4, 'mine', 'Mine', 3, 0, 1, 'Extracts ore or stone from mineral-rich hexes.', 40),
    (5, 'quarry', 'Quarry', 2, 0, 1, 'Provides stone and construction materials.', 50),
    (6, 'watchtower', 'Watchtower', 3, 0, 1, 'Extends visibility and supports fog-of-war reveal flows.', 60),
    (7, 'roadway', 'Roadway', 1, 0, 1, 'Infrastructure improvement for connecting claimed hexes.', 70)
ON CONFLICT (HexUpgradeTypeId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    BuildPointCost = excluded.BuildPointCost,
    CanStack = excluded.CanStack,
    RequiresClaimedHex = excluded.RequiresClaimedHex,
    Description = excluded.Description,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_SettlementType (SettlementTypeId, Name, DisplayName, MinDistricts, SortOrder)
VALUES
    (1, 'camp', 'Camp', 0, 10),
    (2, 'village', 'Village', 1, 20),
    (3, 'town', 'Town', 4, 30),
    (4, 'city', 'City', 9, 40),
    (5, 'metropolis', 'Metropolis', 16, 50)
ON CONFLICT (SettlementTypeId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    MinDistricts = excluded.MinDistricts,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_KingdomPermissionType (KingdomPermissionTypeId, Name, DisplayName, Description, SortOrder)
VALUES
    (1, 'view-kingdom', 'View kingdom', 'Read shared kingdom data and public map state.', 10),
    (2, 'manage-kingdom', 'Manage kingdom', 'Edit kingdom profile and high-level kingdom state.', 20),
    (3, 'manage-membership', 'Manage membership', 'Invite participants and change role assignments.', 30),
    (4, 'edit-map', 'Edit map', 'Create or modify hexes and related map content.', 40),
    (5, 'manage-settlements', 'Manage settlements', 'Create, update, and retire settlements.', 50),
    (6, 'manage-hex-upgrades', 'Manage hex upgrades', 'Add or change improvements attached to claimed hexes.', 60),
    (7, 'reveal-fog', 'Reveal fog', 'Advance fog-of-war visibility states.', 70),
    (8, 'view-private-notes', 'View private notes', 'Read GM-only map and kingdom notes.', 80)
ON CONFLICT (KingdomPermissionTypeId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    Description = excluded.Description,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_KingdomRoleType (KingdomRoleTypeId, Name, DisplayName, IsGameMasterRole, SortOrder)
VALUES
    (1, 'game-master', 'Game Master', 1, 10),
    (2, 'assistant-game-master', 'Assistant Game Master', 1, 20),
    (3, 'ruler', 'Ruler', 0, 30),
    (4, 'treasurer', 'Treasurer', 0, 40),
    (5, 'general', 'General', 0, 50),
    (6, 'warden', 'Warden', 0, 60),
    (7, 'emissary', 'Emissary', 0, 70),
    (8, 'magister', 'Magister', 0, 80),
    (9, 'high-priest', 'High Priest', 0, 90),
    (10, 'viceroy', 'Viceroy', 0, 100),
    (11, 'councilor', 'Councilor', 0, 110),
    (12, 'player-observer', 'Player Observer', 0, 120)
ON CONFLICT (KingdomRoleTypeId) DO UPDATE SET
    Name = excluded.Name,
    DisplayName = excluded.DisplayName,
    IsGameMasterRole = excluded.IsGameMasterRole,
    SortOrder = excluded.SortOrder;

INSERT INTO ref_KingdomRoleDefaultPermission (KingdomRoleTypeId, KingdomPermissionTypeId)
VALUES
    (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8),
    (2, 1), (2, 2), (2, 3), (2, 4), (2, 5), (2, 6), (2, 7), (2, 8),
    (3, 1), (3, 2), (3, 4), (3, 5), (3, 6),
    (4, 1), (4, 2),
    (5, 1), (5, 4), (5, 6),
    (6, 1), (6, 4), (6, 6), (6, 7),
    (7, 1),
    (8, 1), (8, 5),
    (9, 1), (9, 5),
    (10, 1), (10, 2), (10, 5),
    (11, 1), (11, 2),
    (12, 1)
ON CONFLICT (KingdomRoleTypeId, KingdomPermissionTypeId) DO NOTHING;

CREATE TABLE IF NOT EXISTS app_LocalCredential
(
    UserAccountId TEXT NOT NULL PRIMARY KEY,
    PasswordHash TEXT NOT NULL,
    FailedLoginAttempts INTEGER NOT NULL DEFAULT 0,
    LockoutEndUtc TEXT NULL,
    LastLoginUtc TEXT NULL,
    CreatedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModifiedUtc TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_app_LocalCredential_UserAccount FOREIGN KEY (UserAccountId) REFERENCES app_UserAccount (UserAccountId)
);
