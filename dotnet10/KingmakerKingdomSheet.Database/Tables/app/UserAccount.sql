CREATE TABLE [app].[UserAccount]
(
    [UserAccountId]    UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_UserAccount_UserAccountId] DEFAULT (NEWID()),
    [IdentityProvider] NVARCHAR(100) NOT NULL CONSTRAINT [DF_UserAccount_IdentityProvider] DEFAULT (N'local'),
    [ExternalSubject]  NVARCHAR(200) NOT NULL,
    [DisplayName]      NVARCHAR(200) NOT NULL,
    [Email]            NVARCHAR(320) NULL,
    [NormalizedEmail]  AS (CASE WHEN [Email] IS NULL THEN NULL ELSE UPPER([Email]) END) PERSISTED,
    [IsActive]         BIT NOT NULL CONSTRAINT [DF_UserAccount_IsActive] DEFAULT ((1)),
    [CreatedUtc]       DATETIME2(0) NOT NULL CONSTRAINT [DF_UserAccount_CreatedUtc] DEFAULT (SYSUTCDATETIME()),
    [ModifiedUtc]      DATETIME2(0) NOT NULL CONSTRAINT [DF_UserAccount_ModifiedUtc] DEFAULT (SYSUTCDATETIME()),
    [RowVersion]       ROWVERSION NOT NULL,
    CONSTRAINT [PK_UserAccount] PRIMARY KEY CLUSTERED ([UserAccountId] ASC),
    CONSTRAINT [UQ_UserAccount_IdentityProvider_ExternalSubject] UNIQUE NONCLUSTERED ([IdentityProvider] ASC, [ExternalSubject] ASC),
    CONSTRAINT [CK_UserAccount_DisplayName_NotBlank] CHECK (LEN(LTRIM(RTRIM([DisplayName]))) > (0))
);
GO

CREATE UNIQUE INDEX [UX_UserAccount_NormalizedEmail]
    ON [app].[UserAccount] ([NormalizedEmail] ASC)
    WHERE [NormalizedEmail] IS NOT NULL;
GO
