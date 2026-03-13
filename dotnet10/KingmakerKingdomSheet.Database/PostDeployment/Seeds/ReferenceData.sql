MERGE [ref].[ClaimStatus] AS [target]
USING (VALUES
    (CONVERT(TINYINT, 1), N'unclaimed', N'Unclaimed', CONVERT(BIT, 0), CONVERT(TINYINT, 10)),
    (CONVERT(TINYINT, 2), N'claimed', N'Claimed', CONVERT(BIT, 1), CONVERT(TINYINT, 20)),
    (CONVERT(TINYINT, 3), N'disputed', N'Disputed', CONVERT(BIT, 0), CONVERT(TINYINT, 30)),
    (CONVERT(TINYINT, 4), N'abandoned', N'Abandoned', CONVERT(BIT, 0), CONVERT(TINYINT, 40))
) AS [source] ([ClaimStatusId], [Name], [DisplayName], [IsClaimed], [SortOrder])
ON [target].[ClaimStatusId] = [source].[ClaimStatusId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [IsClaimed] = [source].[IsClaimed],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([ClaimStatusId], [Name], [DisplayName], [IsClaimed], [SortOrder])
    VALUES ([source].[ClaimStatusId], [source].[Name], [source].[DisplayName], [source].[IsClaimed], [source].[SortOrder]);
GO

MERGE [ref].[FogState] AS [target]
USING (VALUES
    (CONVERT(TINYINT, 1), N'hidden', N'Hidden', CONVERT(TINYINT, 10)),
    (CONVERT(TINYINT, 2), N'explored', N'Explored', CONVERT(TINYINT, 20)),
    (CONVERT(TINYINT, 3), N'revealed', N'Revealed', CONVERT(TINYINT, 30))
) AS [source] ([FogStateId], [Name], [DisplayName], [SortOrder])
ON [target].[FogStateId] = [source].[FogStateId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([FogStateId], [Name], [DisplayName], [SortOrder])
    VALUES ([source].[FogStateId], [source].[Name], [source].[DisplayName], [source].[SortOrder]);
GO

MERGE [ref].[HexTerrainType] AS [target]
USING (VALUES
    (CONVERT(SMALLINT, 1), N'plains', N'Plains', CONVERT(DECIMAL(4,2), 1.00), CONVERT(BIT, 0), CONVERT(BIT, 1), CONVERT(SMALLINT, 10)),
    (CONVERT(SMALLINT, 2), N'forest', N'Forest', CONVERT(DECIMAL(4,2), 1.50), CONVERT(BIT, 0), CONVERT(BIT, 1), CONVERT(SMALLINT, 20)),
    (CONVERT(SMALLINT, 3), N'hills', N'Hills', CONVERT(DECIMAL(4,2), 1.50), CONVERT(BIT, 0), CONVERT(BIT, 1), CONVERT(SMALLINT, 30)),
    (CONVERT(SMALLINT, 4), N'mountains', N'Mountains', CONVERT(DECIMAL(4,2), 2.00), CONVERT(BIT, 0), CONVERT(BIT, 0), CONVERT(SMALLINT, 40)),
    (CONVERT(SMALLINT, 5), N'swamp', N'Swamp', CONVERT(DECIMAL(4,2), 2.00), CONVERT(BIT, 0), CONVERT(BIT, 0), CONVERT(SMALLINT, 50)),
    (CONVERT(SMALLINT, 6), N'river', N'River', CONVERT(DECIMAL(4,2), 1.00), CONVERT(BIT, 1), CONVERT(BIT, 0), CONVERT(SMALLINT, 60)),
    (CONVERT(SMALLINT, 7), N'lake', N'Lake', CONVERT(DECIMAL(4,2), 1.00), CONVERT(BIT, 1), CONVERT(BIT, 0), CONVERT(SMALLINT, 70)),
    (CONVERT(SMALLINT, 8), N'ruins', N'Ruins', CONVERT(DECIMAL(4,2), 1.50), CONVERT(BIT, 0), CONVERT(BIT, 1), CONVERT(SMALLINT, 80))
) AS [source] ([HexTerrainTypeId], [Name], [DisplayName], [MovementCost], [IsWater], [SupportsSettlement], [SortOrder])
ON [target].[HexTerrainTypeId] = [source].[HexTerrainTypeId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [MovementCost] = [source].[MovementCost],
        [IsWater] = [source].[IsWater],
        [SupportsSettlement] = [source].[SupportsSettlement],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([HexTerrainTypeId], [Name], [DisplayName], [MovementCost], [IsWater], [SupportsSettlement], [SortOrder])
    VALUES ([source].[HexTerrainTypeId], [source].[Name], [source].[DisplayName], [source].[MovementCost], [source].[IsWater], [source].[SupportsSettlement], [source].[SortOrder]);
GO

MERGE [ref].[HexUpgradeType] AS [target]
USING (VALUES
    (CONVERT(SMALLINT, 1), N'farm', N'Farm', CONVERT(SMALLINT, 1), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Food-producing improvement for fertile land.', CONVERT(SMALLINT, 10)),
    (CONVERT(SMALLINT, 2), N'fishery', N'Fishery', CONVERT(SMALLINT, 2), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Harvests river or lake hex resources.', CONVERT(SMALLINT, 20)),
    (CONVERT(SMALLINT, 3), N'lumber-camp', N'Lumber Camp', CONVERT(SMALLINT, 2), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Converts wooded hexes into usable lumber output.', CONVERT(SMALLINT, 30)),
    (CONVERT(SMALLINT, 4), N'mine', N'Mine', CONVERT(SMALLINT, 3), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Extracts ore or stone from mineral-rich hexes.', CONVERT(SMALLINT, 40)),
    (CONVERT(SMALLINT, 5), N'quarry', N'Quarry', CONVERT(SMALLINT, 2), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Provides stone and construction materials.', CONVERT(SMALLINT, 50)),
    (CONVERT(SMALLINT, 6), N'watchtower', N'Watchtower', CONVERT(SMALLINT, 3), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Extends visibility and supports fog-of-war reveal flows.', CONVERT(SMALLINT, 60)),
    (CONVERT(SMALLINT, 7), N'roadway', N'Roadway', CONVERT(SMALLINT, 1), CONVERT(BIT, 0), CONVERT(BIT, 1), N'Infrastructure improvement for connecting claimed hexes.', CONVERT(SMALLINT, 70))
) AS [source] ([HexUpgradeTypeId], [Name], [DisplayName], [BuildPointCost], [CanStack], [RequiresClaimedHex], [Description], [SortOrder])
ON [target].[HexUpgradeTypeId] = [source].[HexUpgradeTypeId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [BuildPointCost] = [source].[BuildPointCost],
        [CanStack] = [source].[CanStack],
        [RequiresClaimedHex] = [source].[RequiresClaimedHex],
        [Description] = [source].[Description],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([HexUpgradeTypeId], [Name], [DisplayName], [BuildPointCost], [CanStack], [RequiresClaimedHex], [Description], [SortOrder])
    VALUES ([source].[HexUpgradeTypeId], [source].[Name], [source].[DisplayName], [source].[BuildPointCost], [source].[CanStack], [source].[RequiresClaimedHex], [source].[Description], [source].[SortOrder]);
GO

MERGE [ref].[SettlementType] AS [target]
USING (VALUES
    (CONVERT(TINYINT, 1), N'camp', N'Camp', CONVERT(SMALLINT, 0), CONVERT(TINYINT, 10)),
    (CONVERT(TINYINT, 2), N'village', N'Village', CONVERT(SMALLINT, 1), CONVERT(TINYINT, 20)),
    (CONVERT(TINYINT, 3), N'town', N'Town', CONVERT(SMALLINT, 4), CONVERT(TINYINT, 30)),
    (CONVERT(TINYINT, 4), N'city', N'City', CONVERT(SMALLINT, 9), CONVERT(TINYINT, 40)),
    (CONVERT(TINYINT, 5), N'metropolis', N'Metropolis', CONVERT(SMALLINT, 16), CONVERT(TINYINT, 50))
) AS [source] ([SettlementTypeId], [Name], [DisplayName], [MinDistricts], [SortOrder])
ON [target].[SettlementTypeId] = [source].[SettlementTypeId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [MinDistricts] = [source].[MinDistricts],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([SettlementTypeId], [Name], [DisplayName], [MinDistricts], [SortOrder])
    VALUES ([source].[SettlementTypeId], [source].[Name], [source].[DisplayName], [source].[MinDistricts], [source].[SortOrder]);
GO

MERGE [ref].[KingdomPermissionType] AS [target]
USING (VALUES
    (CONVERT(SMALLINT, 1), N'view-kingdom', N'View kingdom', N'Read shared kingdom data and public map state.', CONVERT(SMALLINT, 10)),
    (CONVERT(SMALLINT, 2), N'manage-kingdom', N'Manage kingdom', N'Edit kingdom profile and high-level kingdom state.', CONVERT(SMALLINT, 20)),
    (CONVERT(SMALLINT, 3), N'manage-membership', N'Manage membership', N'Invite participants and change role assignments.', CONVERT(SMALLINT, 30)),
    (CONVERT(SMALLINT, 4), N'edit-map', N'Edit map', N'Create or modify hexes and related map content.', CONVERT(SMALLINT, 40)),
    (CONVERT(SMALLINT, 5), N'manage-settlements', N'Manage settlements', N'Create, update, and retire settlements.', CONVERT(SMALLINT, 50)),
    (CONVERT(SMALLINT, 6), N'manage-hex-upgrades', N'Manage hex upgrades', N'Add or change improvements attached to claimed hexes.', CONVERT(SMALLINT, 60)),
    (CONVERT(SMALLINT, 7), N'reveal-fog', N'Reveal fog', N'Advance fog-of-war visibility states.', CONVERT(SMALLINT, 70)),
    (CONVERT(SMALLINT, 8), N'view-private-notes', N'View private notes', N'Read GM-only map and kingdom notes.', CONVERT(SMALLINT, 80))
) AS [source] ([KingdomPermissionTypeId], [Name], [DisplayName], [Description], [SortOrder])
ON [target].[KingdomPermissionTypeId] = [source].[KingdomPermissionTypeId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [Description] = [source].[Description],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([KingdomPermissionTypeId], [Name], [DisplayName], [Description], [SortOrder])
    VALUES ([source].[KingdomPermissionTypeId], [source].[Name], [source].[DisplayName], [source].[Description], [source].[SortOrder]);
GO

MERGE [ref].[KingdomRoleType] AS [target]
USING (VALUES
    (CONVERT(SMALLINT, 1), N'game-master', N'Game Master', CONVERT(BIT, 1), CONVERT(SMALLINT, 10)),
    (CONVERT(SMALLINT, 2), N'assistant-game-master', N'Assistant Game Master', CONVERT(BIT, 1), CONVERT(SMALLINT, 20)),
    (CONVERT(SMALLINT, 3), N'ruler', N'Ruler', CONVERT(BIT, 0), CONVERT(SMALLINT, 30)),
    (CONVERT(SMALLINT, 4), N'treasurer', N'Treasurer', CONVERT(BIT, 0), CONVERT(SMALLINT, 40)),
    (CONVERT(SMALLINT, 5), N'general', N'General', CONVERT(BIT, 0), CONVERT(SMALLINT, 50)),
    (CONVERT(SMALLINT, 6), N'warden', N'Warden', CONVERT(BIT, 0), CONVERT(SMALLINT, 60)),
    (CONVERT(SMALLINT, 7), N'emissary', N'Emissary', CONVERT(BIT, 0), CONVERT(SMALLINT, 70)),
    (CONVERT(SMALLINT, 8), N'magister', N'Magister', CONVERT(BIT, 0), CONVERT(SMALLINT, 80)),
    (CONVERT(SMALLINT, 9), N'high-priest', N'High Priest', CONVERT(BIT, 0), CONVERT(SMALLINT, 90)),
    (CONVERT(SMALLINT, 10), N'viceroy', N'Viceroy', CONVERT(BIT, 0), CONVERT(SMALLINT, 100)),
    (CONVERT(SMALLINT, 11), N'councilor', N'Councilor', CONVERT(BIT, 0), CONVERT(SMALLINT, 110)),
    (CONVERT(SMALLINT, 12), N'player-observer', N'Player Observer', CONVERT(BIT, 0), CONVERT(SMALLINT, 120))
) AS [source] ([KingdomRoleTypeId], [Name], [DisplayName], [IsGameMasterRole], [SortOrder])
ON [target].[KingdomRoleTypeId] = [source].[KingdomRoleTypeId]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = [source].[Name],
        [DisplayName] = [source].[DisplayName],
        [IsGameMasterRole] = [source].[IsGameMasterRole],
        [SortOrder] = [source].[SortOrder]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([KingdomRoleTypeId], [Name], [DisplayName], [IsGameMasterRole], [SortOrder])
    VALUES ([source].[KingdomRoleTypeId], [source].[Name], [source].[DisplayName], [source].[IsGameMasterRole], [source].[SortOrder]);
GO

MERGE [ref].[KingdomRoleDefaultPermission] AS [target]
USING (VALUES
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 2)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 3)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 4)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 5)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 6)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 7)),
    (CONVERT(SMALLINT, 1), CONVERT(SMALLINT, 8)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 2)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 3)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 4)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 5)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 6)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 7)),
    (CONVERT(SMALLINT, 2), CONVERT(SMALLINT, 8)),
    (CONVERT(SMALLINT, 3), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 3), CONVERT(SMALLINT, 2)),
    (CONVERT(SMALLINT, 3), CONVERT(SMALLINT, 4)),
    (CONVERT(SMALLINT, 3), CONVERT(SMALLINT, 5)),
    (CONVERT(SMALLINT, 3), CONVERT(SMALLINT, 6)),
    (CONVERT(SMALLINT, 4), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 4), CONVERT(SMALLINT, 2)),
    (CONVERT(SMALLINT, 5), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 5), CONVERT(SMALLINT, 4)),
    (CONVERT(SMALLINT, 5), CONVERT(SMALLINT, 6)),
    (CONVERT(SMALLINT, 6), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 6), CONVERT(SMALLINT, 4)),
    (CONVERT(SMALLINT, 6), CONVERT(SMALLINT, 6)),
    (CONVERT(SMALLINT, 6), CONVERT(SMALLINT, 7)),
    (CONVERT(SMALLINT, 7), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 8), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 8), CONVERT(SMALLINT, 5)),
    (CONVERT(SMALLINT, 9), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 9), CONVERT(SMALLINT, 5)),
    (CONVERT(SMALLINT, 10), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 10), CONVERT(SMALLINT, 2)),
    (CONVERT(SMALLINT, 10), CONVERT(SMALLINT, 5)),
    (CONVERT(SMALLINT, 11), CONVERT(SMALLINT, 1)),
    (CONVERT(SMALLINT, 11), CONVERT(SMALLINT, 2)),
    (CONVERT(SMALLINT, 12), CONVERT(SMALLINT, 1))
) AS [source] ([KingdomRoleTypeId], [KingdomPermissionTypeId])
ON [target].[KingdomRoleTypeId] = [source].[KingdomRoleTypeId]
    AND [target].[KingdomPermissionTypeId] = [source].[KingdomPermissionTypeId]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([KingdomRoleTypeId], [KingdomPermissionTypeId])
    VALUES ([source].[KingdomRoleTypeId], [source].[KingdomPermissionTypeId])
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;
GO
