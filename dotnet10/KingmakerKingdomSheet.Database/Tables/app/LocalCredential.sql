CREATE TABLE [app].[LocalCredential] (
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [PasswordHash] NVARCHAR(200) NOT NULL,
    [FailedLoginAttempts] INT NOT NULL DEFAULT 0,
    [LockoutEndUtc] DATETIME2(0) NULL,
    [LastLoginUtc] DATETIME2(0) NULL,
    [CreatedUtc] DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    [ModifiedUtc] DATETIME2(0) NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [FK_LocalCredential_UserAccount] FOREIGN KEY ([UserAccountId]) REFERENCES [app].[UserAccount] ([UserAccountId])
);
