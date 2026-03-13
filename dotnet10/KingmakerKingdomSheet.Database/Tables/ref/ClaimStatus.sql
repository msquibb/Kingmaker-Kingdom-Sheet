CREATE TABLE [ref].[ClaimStatus]
(
    [ClaimStatusId] TINYINT NOT NULL,
    [Name]          NVARCHAR(50) NOT NULL,
    [DisplayName]   NVARCHAR(100) NOT NULL,
    [IsClaimed]     BIT NOT NULL CONSTRAINT [DF_ClaimStatus_IsClaimed] DEFAULT ((0)),
    [SortOrder]     TINYINT NOT NULL,
    CONSTRAINT [PK_ClaimStatus] PRIMARY KEY CLUSTERED ([ClaimStatusId] ASC),
    CONSTRAINT [UQ_ClaimStatus_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO
