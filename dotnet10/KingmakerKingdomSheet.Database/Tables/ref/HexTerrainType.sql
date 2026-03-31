CREATE TABLE [ref].[HexTerrainType]
(
    [HexTerrainTypeId] SMALLINT NOT NULL,
    [Name]             NVARCHAR(50) NOT NULL,
    [DisplayName]      NVARCHAR(100) NOT NULL,
    [MovementCost]     DECIMAL(4,2) NOT NULL CONSTRAINT [DF_HexTerrainType_MovementCost] DEFAULT ((1.00)),
    [IsWater]          BIT NOT NULL CONSTRAINT [DF_HexTerrainType_IsWater] DEFAULT ((0)),
    [SupportsSettlement] BIT NOT NULL CONSTRAINT [DF_HexTerrainType_SupportsSettlement] DEFAULT ((1)),
    [SortOrder]        SMALLINT NOT NULL,
    CONSTRAINT [PK_HexTerrainType] PRIMARY KEY CLUSTERED ([HexTerrainTypeId] ASC),
    CONSTRAINT [UQ_HexTerrainType_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [CK_HexTerrainType_MovementCost] CHECK ([MovementCost] > (0))
);
GO
