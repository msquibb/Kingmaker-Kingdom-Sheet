CREATE TABLE [app].[HexUpgrade]
(
    [HexUpgradeId]     UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_HexUpgrade_HexUpgradeId] DEFAULT (NEWID()),
    [HexId]            UNIQUEIDENTIFIER NOT NULL,
    [HexUpgradeTypeId] SMALLINT NOT NULL,
    [Quantity]         SMALLINT NOT NULL CONSTRAINT [DF_HexUpgrade_Quantity] DEFAULT ((1)),
    [BuiltOn]          DATE NULL,
    [Notes]            NVARCHAR(1000) NULL,
    [CreatedUtc]       DATETIME2(0) NOT NULL CONSTRAINT [DF_HexUpgrade_CreatedUtc] DEFAULT (SYSUTCDATETIME()),
    [ModifiedUtc]      DATETIME2(0) NOT NULL CONSTRAINT [DF_HexUpgrade_ModifiedUtc] DEFAULT (SYSUTCDATETIME()),
    [RowVersion]       ROWVERSION NOT NULL,
    CONSTRAINT [PK_HexUpgrade] PRIMARY KEY CLUSTERED ([HexUpgradeId] ASC),
    CONSTRAINT [FK_HexUpgrade_Hex] FOREIGN KEY ([HexId]) REFERENCES [app].[Hex] ([HexId]) ON DELETE CASCADE,
    CONSTRAINT [FK_HexUpgrade_HexUpgradeType] FOREIGN KEY ([HexUpgradeTypeId]) REFERENCES [ref].[HexUpgradeType] ([HexUpgradeTypeId]),
    CONSTRAINT [UQ_HexUpgrade_Hex_UpgradeType] UNIQUE NONCLUSTERED ([HexId] ASC, [HexUpgradeTypeId] ASC),
    CONSTRAINT [CK_HexUpgrade_Quantity] CHECK ([Quantity] > (0))
);
GO

CREATE INDEX [IX_HexUpgrade_UpgradeType]
    ON [app].[HexUpgrade] ([HexUpgradeTypeId] ASC, [HexId] ASC);
GO
