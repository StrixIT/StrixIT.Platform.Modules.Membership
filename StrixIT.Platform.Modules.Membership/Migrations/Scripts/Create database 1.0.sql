/**
 * Copyright 2015 StrixIT
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
CREATE TABLE [dbo].[Applications] (
    [Id] [uniqueidentifier] NOT NULL,
    [Name] [nvarchar](250) NOT NULL,
    CONSTRAINT [PK_dbo.Applications] PRIMARY KEY ([Id])
)
CREATE TABLE [dbo].[Groups] (
    [Id] [uniqueidentifier] NOT NULL,
    [Name] [nvarchar](250) NOT NULL,
    [UsePermissions] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.Groups] PRIMARY KEY ([Id])
)
CREATE TABLE [dbo].[Roles] (
    [Id] [uniqueidentifier] NOT NULL,
    [GroupId] [uniqueidentifier] NOT NULL,
    [Name] [nvarchar](250) NOT NULL,
    [Description] [nvarchar](500),
    CONSTRAINT [PK_dbo.Roles] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_GroupId] ON [dbo].[Roles]([GroupId])
CREATE TABLE [dbo].[GroupsInRoles] (
    [GroupId] [uniqueidentifier] NOT NULL,
    [RoleId] [uniqueidentifier] NOT NULL,
    [StartDate] [datetime] NOT NULL,
    [EndDate] [datetime],
    [MaxNumberOfUsers] [int],
    [CurrentNumberOfUsers] [int] NOT NULL,
    CONSTRAINT [PK_dbo.GroupsInRoles] PRIMARY KEY ([GroupId], [RoleId])
)
CREATE INDEX [IX_GroupId] ON [dbo].[GroupsInRoles]([GroupId])
CREATE INDEX [IX_RoleId] ON [dbo].[GroupsInRoles]([RoleId])
CREATE TABLE [dbo].[Permissions] (
    [Id] [uniqueidentifier] NOT NULL,
    [ApplicationId] [uniqueidentifier] NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    CONSTRAINT [PK_dbo.Permissions] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_ApplicationId] ON [dbo].[Permissions]([ApplicationId])
CREATE TABLE [dbo].[UserProfileFields] (
    [Id] [uniqueidentifier] NOT NULL,
    [FieldType] [int] NOT NULL,
    [Name] [nvarchar](100) NOT NULL,
    [Section] [nvarchar](100),
    CONSTRAINT [PK_dbo.UserProfileFields] PRIMARY KEY ([Id])
)
CREATE TABLE [dbo].[UserProfileValues] (
    [Id] [bigint] NOT NULL IDENTITY,
    [UserId] [uniqueidentifier] NOT NULL,
    [Culture] [nvarchar](5) NOT NULL,
    [CustomFieldId] [uniqueidentifier] NOT NULL,
    [NumberValue] [float],
    [StringValue] [nvarchar](max),
    CONSTRAINT [PK_dbo.UserProfileValues] PRIMARY KEY ([Id])
)
CREATE INDEX [IX_UserId] ON [dbo].[UserProfileValues]([UserId])
CREATE INDEX [IX_CustomFieldId] ON [dbo].[UserProfileValues]([CustomFieldId])
CREATE TABLE [dbo].[Users] (
    [Id] [uniqueidentifier] NOT NULL,
    [Name] [nvarchar](250),
    [Email] [nvarchar](250) NOT NULL,
    [PreferredCulture] [nvarchar](5) NOT NULL,
    [DateAcceptedTerms] [datetime],
    [LockedOut] [bit] NOT NULL,
    [Approved] [bit] NOT NULL,
    [Password] [nvarchar](256) NOT NULL,
    [VerificationId] [uniqueidentifier],
    [VerificationWindowStart] [datetime],
    [LastLoginDate] [datetime],
    [FailedPasswordAttemptCount] [int] NOT NULL,
    [FailedPasswordAttemptWindowStart] [datetime],
    [RegistrationComment] [nvarchar](500),
    [Session] [nvarchar](max),
    CONSTRAINT [PK_dbo.Users] PRIMARY KEY ([Id])
)
CREATE TABLE [dbo].[UsersInRoles] (
    [GroupRoleGroupId] [uniqueidentifier] NOT NULL,
    [GroupRoleRoleId] [uniqueidentifier] NOT NULL,
    [UserId] [uniqueidentifier] NOT NULL,
    [StartDate] [datetime] NOT NULL,
    [EndDate] [datetime],
    CONSTRAINT [PK_dbo.UsersInRoles] PRIMARY KEY ([GroupRoleGroupId], [GroupRoleRoleId], [UserId])
)
CREATE INDEX [IX_GroupRoleGroupId_GroupRoleRoleId] ON [dbo].[UsersInRoles]([GroupRoleGroupId], [GroupRoleRoleId])
CREATE INDEX [IX_UserId] ON [dbo].[UsersInRoles]([UserId])
CREATE TABLE [dbo].[RolePermissions] (
    [RoleId] [uniqueidentifier] NOT NULL,
    [PermissionId] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_dbo.RolePermissions] PRIMARY KEY ([RoleId], [PermissionId])
)
CREATE INDEX [IX_RoleId] ON [dbo].[RolePermissions]([RoleId])
CREATE INDEX [IX_PermissionId] ON [dbo].[RolePermissions]([PermissionId])
ALTER TABLE [dbo].[Roles] ADD CONSTRAINT [FK_dbo.Roles_dbo.Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[GroupsInRoles] ADD CONSTRAINT [FK_dbo.GroupsInRoles_dbo.Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[GroupsInRoles] ADD CONSTRAINT [FK_dbo.GroupsInRoles_dbo.Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([Id])
ALTER TABLE [dbo].[Permissions] ADD CONSTRAINT [FK_dbo.Permissions_dbo.Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications] ([Id])
ALTER TABLE [dbo].[UserProfileValues] ADD CONSTRAINT [FK_dbo.UserProfileValues_dbo.UserProfileFields_CustomFieldId] FOREIGN KEY ([CustomFieldId]) REFERENCES [dbo].[UserProfileFields] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[UserProfileValues] ADD CONSTRAINT [FK_dbo.UserProfileValues_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
ALTER TABLE [dbo].[UsersInRoles] ADD CONSTRAINT [FK_dbo.UsersInRoles_dbo.GroupsInRoles_GroupRoleGroupId_GroupRoleRoleId] FOREIGN KEY ([GroupRoleGroupId], [GroupRoleRoleId]) REFERENCES [dbo].[GroupsInRoles] ([GroupId], [RoleId])
ALTER TABLE [dbo].[UsersInRoles] ADD CONSTRAINT [FK_dbo.UsersInRoles_dbo.Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[RolePermissions] ADD CONSTRAINT [FK_dbo.RolePermissions_dbo.Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
ALTER TABLE [dbo].[RolePermissions] ADD CONSTRAINT [FK_dbo.RolePermissions_dbo.Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE