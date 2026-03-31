CREATE TABLE [ref].[KingdomPermissionType]
(
    [KingdomPermissionTypeId] SMALLINT NOT NULL,
    [Name]                   NVARCHAR(100) NOT NULL,
    [DisplayName]            NVARCHAR(100) NOT NULL,
    [Description]            NVARCHAR(500) NULL,
    [SortOrder]              SMALLINT NOT NULL,
    CONSTRAINT [PK_KingdomPermissionType] PRIMARY KEY CLUSTERED ([KingdomPermissionTypeId] ASC),
    CONSTRAINT [UQ_KingdomPermissionType_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO
