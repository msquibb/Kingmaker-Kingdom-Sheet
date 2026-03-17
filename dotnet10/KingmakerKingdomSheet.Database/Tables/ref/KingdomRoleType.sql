CREATE TABLE [ref].[KingdomRoleType]
(
    [KingdomRoleTypeId] SMALLINT NOT NULL,
    [Name]              NVARCHAR(100) NOT NULL,
    [DisplayName]       NVARCHAR(100) NOT NULL,
    [IsGameMasterRole]  BIT NOT NULL CONSTRAINT [DF_KingdomRoleType_IsGameMasterRole] DEFAULT ((0)),
    [SortOrder]         SMALLINT NOT NULL,
    CONSTRAINT [PK_KingdomRoleType] PRIMARY KEY CLUSTERED ([KingdomRoleTypeId] ASC),
    CONSTRAINT [UQ_KingdomRoleType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO
