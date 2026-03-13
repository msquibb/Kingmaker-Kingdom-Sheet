CREATE TABLE [app].[Settlement]
(
    [SettlementId]     UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Settlement_SettlementId] DEFAULT (NEWID()),
    [KingdomId]        UNIQUEIDENTIFIER NOT NULL,
    [SettlementTypeId] TINYINT NOT NULL,
    [HexId]            UNIQUEIDENTIFIER NULL,
    [Name]             NVARCHAR(200) NOT NULL,
    [Population]       INT NULL,
    [FoundedOn]        DATE NULL,
    [Notes]            NVARCHAR(1000) NULL,
    [CreatedUtc]       DATETIME2(0) NOT NULL CONSTRAINT [DF_Settlement_CreatedUtc] DEFAULT (SYSUTCDATETIME()),
    [ModifiedUtc]      DATETIME2(0) NOT NULL CONSTRAINT [DF_Settlement_ModifiedUtc] DEFAULT (SYSUTCDATETIME()),
    [RowVersion]       ROWVERSION NOT NULL,
    CONSTRAINT [PK_Settlement] PRIMARY KEY CLUSTERED ([SettlementId] ASC),
    CONSTRAINT [FK_Settlement_Kingdom] FOREIGN KEY ([KingdomId]) REFERENCES [app].[Kingdom] ([KingdomId]),
    CONSTRAINT [FK_Settlement_SettlementType] FOREIGN KEY ([SettlementTypeId]) REFERENCES [ref].[SettlementType] ([SettlementTypeId]),
    CONSTRAINT [FK_Settlement_Hex] FOREIGN KEY ([HexId], [KingdomId]) REFERENCES [app].[Hex] ([HexId], [KingdomId]),
    CONSTRAINT [UQ_Settlement_Kingdom_Name] UNIQUE NONCLUSTERED ([KingdomId] ASC, [Name] ASC),
    CONSTRAINT [CK_Settlement_Name_NotBlank] CHECK (LEN(LTRIM(RTRIM([Name]))) > (0)),
    CONSTRAINT [CK_Settlement_Population] CHECK ([Population] IS NULL OR [Population] >= (0))
);
GO

CREATE UNIQUE INDEX [UX_Settlement_HexId]
    ON [app].[Settlement] ([HexId] ASC)
    WHERE [HexId] IS NOT NULL;
GO

CREATE INDEX [IX_Settlement_Kingdom_Type]
    ON [app].[Settlement] ([KingdomId] ASC, [SettlementTypeId] ASC);
GO
