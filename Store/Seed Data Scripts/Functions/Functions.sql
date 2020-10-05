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
	WHERE mtu.UserId = @UserId OR u.Id = @UserId
	UNION
	SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
	t.TenantCode, t.TenantName, ur.RoleId AS RoleId
	FROM [NIS].[User] AS u INNER JOIN
	[TenantManager].[Tenant] AS t ON u.TenantCode = t.TenantCode INNER JOIN
	[NIS].[UserRoleMap] ur ON u.Id = ur.UserId
	WHERE u.Id = @UserId
)

Go