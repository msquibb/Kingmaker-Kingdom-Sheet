CREATE TABLE [ref].[FogState]
(
    [FogStateId]  TINYINT NOT NULL,
    [Name]        NVARCHAR(50) NOT NULL,
    [DisplayName] NVARCHAR(100) NOT NULL,
    [SortOrder]   TINYINT NOT NULL,
    CONSTRAINT [PK_FogState] PRIMARY KEY CLUSTERED ([FogStateId] ASC),
    CONSTRAINT [UQ_FogState_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO
