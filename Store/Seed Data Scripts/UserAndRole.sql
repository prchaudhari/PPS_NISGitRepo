
DECLARE @SuperAdminUserIdentifier AS BIGINT
DECLARE @SuperAdminEmailAddress AS NVARCHAR(100) = N'superadmin@nis.com';
INSERT INTO [NIS].[User] VALUES ('NIS', 'SuperAdmin', '+91-1234567890', @SuperAdminEmailAddress, '', 0, 0, 1,0, @SuperTenantIdentifier)
SET @SuperAdminUserIdentifier = @@IDENTITY

INSERT INTO [NIS].[UserLogin] VALUES (@SuperAdminEmailAddress, 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())

DECLARE @RoleIdentifier AS BIGINT
INSERT [NIS].[Role] ([Name], [Description], [IsDeleted], [TenantCode]) VALUES (N'Super Admin', N'Super Admin Role', 0, @SuperTenantIdentifier)
SET @RoleIdentifier = @@IDENTITY

INSERT INTO [NIS].[UserRoleMap] ([UserId], [RoleId]) VALUES (@SuperAdminUserIdentifier, @RoleIdentifier)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Dashboard', N'View', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Role', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'User', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Asset Library', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Widget', N'View', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Page', N'Publish', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Definition', N'Publish', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Schedule Management', N'Delete', 1)

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (@RoleIdentifier, N'Statement Search', N'View', 1)

