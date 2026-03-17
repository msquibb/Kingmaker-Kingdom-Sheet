CREATE TABLE [app].[KingdomParticipant]
(
    [KingdomParticipantId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_KingdomParticipant_KingdomParticipantId] DEFAULT (NEWID()),
    [KingdomId]            UNIQUEIDENTIFIER NOT NULL,
    [UserAccountId]        UNIQUEIDENTIFIER NOT NULL,
    [DisplayNameOverride]  NVARCHAR(200) NULL,
    [IsActive]             BIT NOT NULL CONSTRAINT [DF_KingdomParticipant_IsActive] DEFAULT ((1)),
    [ReceivesRealtimeUpdates] BIT NOT NULL CONSTRAINT [DF_KingdomParticipant_ReceivesRealtimeUpdates] DEFAULT ((1)),
    [JoinedUtc]            DATETIME2(0) NOT NULL CONSTRAINT [DF_KingdomParticipant_JoinedUtc] DEFAULT (SYSUTCDATETIME()),
    [ModifiedUtc]          DATETIME2(0) NOT NULL CONSTRAINT [DF_KingdomParticipant_ModifiedUtc] DEFAULT (SYSUTCDATETIME()),
    [RowVersion]           ROWVERSION NOT NULL,
    CONSTRAINT [PK_KingdomParticipant] PRIMARY KEY CLUSTERED ([KingdomParticipantId] ASC),
    CONSTRAINT [FK_KingdomParticipant_Kingdom] FOREIGN KEY ([KingdomId]) REFERENCES [app].[Kingdom] ([KingdomId]),
    CONSTRAINT [FK_KingdomParticipant_UserAccount] FOREIGN KEY ([UserAccountId]) REFERENCES [app].[UserAccount] ([UserAccountId]),
    CONSTRAINT [UQ_KingdomParticipant_Kingdom_User] UNIQUE NONCLUSTERED ([KingdomId] ASC, [UserAccountId] ASC)
);
GO

CREATE INDEX [IX_KingdomParticipant_UserAccount]
    ON [app].[KingdomParticipant] ([UserAccountId] ASC, [IsActive] ASC);
GO
