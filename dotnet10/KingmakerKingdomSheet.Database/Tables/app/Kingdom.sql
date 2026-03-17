CREATE TABLE [app].[Kingdom]
(
    [KingdomId]           UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Kingdom_KingdomId] DEFAULT (NEWID()),
    [Name]                NVARCHAR(200) NOT NULL,
    [Slug]                NVARCHAR(100) NOT NULL,
    [Description]         NVARCHAR(1000) NULL,
    [FoundedOn]           DATE NULL,
    [CreatedByUserAccountId] UNIQUEIDENTIFIER NULL,
    [IsArchived]          BIT NOT NULL CONSTRAINT [DF_Kingdom_IsArchived] DEFAULT ((0)),
    [CreatedUtc]          DATETIME2(0) NOT NULL CONSTRAINT [DF_Kingdom_CreatedUtc] DEFAULT (SYSUTCDATETIME()),
    [ModifiedUtc]         DATETIME2(0) NOT NULL CONSTRAINT [DF_Kingdom_ModifiedUtc] DEFAULT (SYSUTCDATETIME()),
    [RowVersion]          ROWVERSION NOT NULL,
    CONSTRAINT [PK_Kingdom] PRIMARY KEY CLUSTERED ([KingdomId] ASC),
    CONSTRAINT [FK_Kingdom_CreatedByUserAccount] FOREIGN KEY ([CreatedByUserAccountId]) REFERENCES [app].[UserAccount] ([UserAccountId]),
    CONSTRAINT [UQ_Kingdom_Slug] UNIQUE NONCLUSTERED ([Slug] ASC),
    CONSTRAINT [CK_Kingdom_Name_NotBlank] CHECK (LEN(LTRIM(RTRIM([Name]))) > (0)),
    CONSTRAINT [CK_Kingdom_Slug_Format] CHECK (LEN([Slug]) > (0) AND PATINDEX(N'%[^a-z0-9-]%', [Slug]) = (0))
);
GO
