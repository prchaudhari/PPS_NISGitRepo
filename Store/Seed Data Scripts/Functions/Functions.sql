-- to get user tenant role mapping
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE Name = 'FnUserTenant'
             AND Type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
    --PRINT 'User defined function Exists'
	DROP FUNCTION NIS.FnUserTenant;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION NIS.FnUserTenant (@UserId INTEGER)
RETURNS TABLE
AS
RETURN
(
    SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
	t.TenantCode, t.TenantName, mtu.OtherTenantAccessRoleId AS RoleId
	FROM [NIS].[MultiTenantUserAccessMap] AS mtu INNER JOIN
	[TenantManager].[Tenant] AS t ON mtu.OtherTenantCode = t.TenantCode INNER JOIN
	[NIS].[User] AS u ON mtu.UserId = u.Id
	WHERE mtu.UserId = @UserId AND mtu.IsActive = 1 AND mtu.IsDeleted = 0
	UNION
	SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
	t.TenantCode, t.TenantName, ur.RoleId AS RoleId
	FROM [NIS].[User] AS u INNER JOIN
	[TenantManager].[Tenant] AS t ON u.TenantCode = t.TenantCode INNER JOIN
	[NIS].[UserRoleMap] ur ON u.Id = ur.UserId
	WHERE u.Id = @UserId
)

Go

--To get parent as well as it's child tenants
IF EXISTS (SELECT 1 FROM sys.objects 
           WHERE Name = 'FnGetParentAndChildTenant'
             AND Type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
    --PRINT 'User defined function Exists'
	DROP FUNCTION NIS.FnGetParentAndChildTenant;
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION NIS.FnGetParentAndChildTenant (@ParentTenantCode NVARCHAR(50))
RETURNS TABLE
AS
RETURN
(
    SELECT * FROM [TenantManager].[Tenant] WHERE (TenantCode = @ParentTenantCode OR ParentTenantCode = @ParentTenantCode) AND IsActive = 1 AND IsDeleted = 0
)

Go
