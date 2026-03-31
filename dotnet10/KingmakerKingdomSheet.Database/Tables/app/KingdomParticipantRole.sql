CREATE TABLE [app].[KingdomParticipantRole]
(
    [KingdomParticipantId] UNIQUEIDENTIFIER NOT NULL,
    [KingdomRoleTypeId]    SMALLINT NOT NULL,
    [IsPrimaryRole]        BIT NOT NULL CONSTRAINT [DF_KingdomParticipantRole_IsPrimaryRole] DEFAULT ((0)),
    [AssignedUtc]          DATETIME2(0) NOT NULL CONSTRAINT [DF_KingdomParticipantRole_AssignedUtc] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_KingdomParticipantRole] PRIMARY KEY CLUSTERED ([KingdomParticipantId] ASC, [KingdomRoleTypeId] ASC),
    CONSTRAINT [FK_KingdomParticipantRole_Participant] FOREIGN KEY ([KingdomParticipantId]) REFERENCES [app].[KingdomParticipant] ([KingdomParticipantId]) ON DELETE CASCADE,
    CONSTRAINT [FK_KingdomParticipantRole_RoleType] FOREIGN KEY ([KingdomRoleTypeId]) REFERENCES [ref].[KingdomRoleType] ([KingdomRoleTypeId])
);
GO

CREATE UNIQUE INDEX [UX_KingdomParticipantRole_PrimaryRole]
    ON [app].[KingdomParticipantRole] ([KingdomParticipantId] ASC)
    WHERE [IsPrimaryRole] = (1);
GO

CREATE INDEX [IX_KingdomParticipantRole_RoleType]
    ON [app].[KingdomParticipantRole] ([KingdomRoleTypeId] ASC, [KingdomParticipantId] ASC);
GO
