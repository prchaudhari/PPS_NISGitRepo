CREATE FUNCTION [NIS].[FnUserTenant] (@UserId INTEGER)
RETURNS TABLE
AS
RETURN
(
    SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
     t.TenantCode, t.TenantName, mtu.OtherTenantAccessRoleId AS RoleId, t.TenantImage, t.TenantType
     FROM [NIS].[MultiTenantUserAccessMap] AS mtu INNER JOIN
     [TenantManager].[Tenant] AS t ON mtu.OtherTenantCode = t.TenantCode INNER JOIN
     [NIS].[User] AS u ON mtu.UserId = u.Id
     WHERE mtu.UserId = @UserId AND mtu.IsActive = 1 AND mtu.IsDeleted = 0
     UNION
    SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
     t.TenantCode, t.TenantName, ur.RoleId AS RoleId, t.TenantImage, t.TenantType
     FROM [NIS].[User] AS u INNER JOIN
     [TenantManager].[Tenant] AS t ON u.TenantCode = t.TenantCode INNER JOIN
     [NIS].[UserRoleMap] ur ON u.Id = ur.UserId
     WHERE u.Id = @UserId
)