CREATE TABLE [app].[KingdomParticipantPermission]
(
    [KingdomParticipantId]     UNIQUEIDENTIFIER NOT NULL,
    [KingdomPermissionTypeId]  SMALLINT NOT NULL,
    [PermissionState]          CHAR(1) NOT NULL,
    [AssignedUtc]              DATETIME2(0) NOT NULL CONSTRAINT [DF_KingdomParticipantPermission_AssignedUtc] DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT [PK_KingdomParticipantPermission] PRIMARY KEY CLUSTERED ([KingdomParticipantId] ASC, [KingdomPermissionTypeId] ASC),
    CONSTRAINT [FK_KingdomParticipantPermission_Participant] FOREIGN KEY ([KingdomParticipantId]) REFERENCES [app].[KingdomParticipant] ([KingdomParticipantId]) ON DELETE CASCADE,
    CONSTRAINT [FK_KingdomParticipantPermission_PermissionType] FOREIGN KEY ([KingdomPermissionTypeId]) REFERENCES [ref].[KingdomPermissionType] ([KingdomPermissionTypeId]),
    CONSTRAINT [CK_KingdomParticipantPermission_PermissionState] CHECK ([PermissionState] IN ('G', 'D'))
);
GO

CREATE INDEX [IX_KingdomParticipantPermission_PermissionType]
    ON [app].[KingdomParticipantPermission] ([KingdomPermissionTypeId] ASC, [PermissionState] ASC);
GO
