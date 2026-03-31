CREATE TABLE [ref].[SettlementType]
(
    [SettlementTypeId] TINYINT NOT NULL,
    [Name]             NVARCHAR(50) NOT NULL,
    [DisplayName]      NVARCHAR(100) NOT NULL,
    [MinDistricts]     SMALLINT NOT NULL CONSTRAINT [DF_SettlementType_MinDistricts] DEFAULT ((0)),
    [SortOrder]        TINYINT NOT NULL,
    CONSTRAINT [PK_SettlementType] PRIMARY KEY CLUSTERED ([SettlementTypeId] ASC),
    CONSTRAINT [UQ_SettlementType_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [CK_SettlementType_MinDistricts] CHECK ([MinDistricts] >= (0))
);
GO
