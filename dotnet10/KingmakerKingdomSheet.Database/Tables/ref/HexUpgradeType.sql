CREATE TABLE [ref].[HexUpgradeType]
(
    [HexUpgradeTypeId] SMALLINT NOT NULL,
    [Name]             NVARCHAR(50) NOT NULL,
    [DisplayName]      NVARCHAR(100) NOT NULL,
    [BuildPointCost]   SMALLINT NOT NULL,
    [CanStack]         BIT NOT NULL CONSTRAINT [DF_HexUpgradeType_CanStack] DEFAULT ((0)),
    [RequiresClaimedHex] BIT NOT NULL CONSTRAINT [DF_HexUpgradeType_RequiresClaimedHex] DEFAULT ((1)),
    [Description]      NVARCHAR(500) NULL,
    [SortOrder]        SMALLINT NOT NULL,
    CONSTRAINT [PK_HexUpgradeType] PRIMARY KEY CLUSTERED ([HexUpgradeTypeId] ASC),
    CONSTRAINT [UQ_HexUpgradeType_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [CK_HexUpgradeType_BuildPointCost] CHECK ([BuildPointCost] >= (0))
);
GO
