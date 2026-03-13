CREATE TABLE [app].[Hex]
(
    [HexId]          UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Hex_HexId] DEFAULT (NEWID()),
    [KingdomId]      UNIQUEIDENTIFIER NOT NULL,
    [CoordinateX]    INT NOT NULL,
    [CoordinateY]    INT NOT NULL,
    [HexTerrainTypeId] SMALLINT NOT NULL,
    [ClaimStatusId]  TINYINT NOT NULL CONSTRAINT [DF_Hex_ClaimStatusId] DEFAULT ((1)),
    [FogStateId]     TINYINT NOT NULL CONSTRAINT [DF_Hex_FogStateId] DEFAULT ((1)),
    [ExploredUtc]    DATETIME2(0) NULL,
    [ClaimedUtc]     DATETIME2(0) NULL,
    [Notes]          NVARCHAR(1000) NULL,
    [CreatedUtc]     DATETIME2(0) NOT NULL CONSTRAINT [DF_Hex_CreatedUtc] DEFAULT (SYSUTCDATETIME()),
    [ModifiedUtc]    DATETIME2(0) NOT NULL CONSTRAINT [DF_Hex_ModifiedUtc] DEFAULT (SYSUTCDATETIME()),
    [RowVersion]     ROWVERSION NOT NULL,
    CONSTRAINT [PK_Hex] PRIMARY KEY CLUSTERED ([HexId] ASC),
    CONSTRAINT [FK_Hex_Kingdom] FOREIGN KEY ([KingdomId]) REFERENCES [app].[Kingdom] ([KingdomId]),
    CONSTRAINT [FK_Hex_HexTerrainType] FOREIGN KEY ([HexTerrainTypeId]) REFERENCES [ref].[HexTerrainType] ([HexTerrainTypeId]),
    CONSTRAINT [FK_Hex_ClaimStatus] FOREIGN KEY ([ClaimStatusId]) REFERENCES [ref].[ClaimStatus] ([ClaimStatusId]),
    CONSTRAINT [FK_Hex_FogState] FOREIGN KEY ([FogStateId]) REFERENCES [ref].[FogState] ([FogStateId]),
    CONSTRAINT [UQ_Hex_Kingdom_Coordinates] UNIQUE NONCLUSTERED ([KingdomId] ASC, [CoordinateX] ASC, [CoordinateY] ASC),
    CONSTRAINT [UQ_Hex_HexId_Kingdom] UNIQUE NONCLUSTERED ([HexId] ASC, [KingdomId] ASC),
    CONSTRAINT [CK_Hex_CoordinateX_Range] CHECK ([CoordinateX] BETWEEN (-5000) AND (5000)),
    CONSTRAINT [CK_Hex_CoordinateY_Range] CHECK ([CoordinateY] BETWEEN (-5000) AND (5000))
);
GO

CREATE INDEX [IX_Hex_Kingdom_Claim_Fog]
    ON [app].[Hex] ([KingdomId] ASC, [ClaimStatusId] ASC, [FogStateId] ASC)
    INCLUDE ([HexTerrainTypeId], [CoordinateX], [CoordinateY]);
GO
