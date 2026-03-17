CREATE TABLE [ref].[KingdomRoleDefaultPermission]
(
    [KingdomRoleTypeId]       SMALLINT NOT NULL,
    [KingdomPermissionTypeId] SMALLINT NOT NULL,
    CONSTRAINT [PK_KingdomRoleDefaultPermission] PRIMARY KEY CLUSTERED ([KingdomRoleTypeId] ASC, [KingdomPermissionTypeId] ASC),
    CONSTRAINT [FK_KingdomRoleDefaultPermission_RoleType] FOREIGN KEY ([KingdomRoleTypeId]) REFERENCES [ref].[KingdomRoleType] ([KingdomRoleTypeId]),
    CONSTRAINT [FK_KingdomRoleDefaultPermission_PermissionType] FOREIGN KEY ([KingdomPermissionTypeId]) REFERENCES [ref].[KingdomPermissionType] ([KingdomPermissionTypeId])
);
GO

CREATE INDEX [IX_KingdomRoleDefaultPermission_PermissionType]
    ON [ref].[KingdomRoleDefaultPermission] ([KingdomPermissionTypeId] ASC);
GO
