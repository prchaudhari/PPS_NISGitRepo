CREATE VIEW [NIS].[View_MultiTenantUserAccessMap]
AS
SELECT tum.Id, tum.UserId, usr.FirstName+' '+usr.LastName AS UserName, usr.EmailAddress, 
tum.AssociatedTenantCode, t.TenantName AS AssociatedTenantName, t.TenantType AS AssociatedTenantType,
tum.OtherTenantCode, t1.TenantName AS OtherTenantName, t1.TenantType AS OtherTenantType,
r.Id AS RoleId, r.Name AS RoleName,tum.IsActive, tum.IsDeleted, tum.ParentTenantCode,
tum.LastUpdatedBy, usr1.FirstName+' '+usr1.LastName AS LastUpdatedByUserName, tum.LastUpdatedDate
FROM [NIS].[MultiTenantUserAccessMap] tum 
INNER JOIN [TenantManager].[Tenant] t ON tum.AssociatedTenantCode = t.TenantCode
INNER JOIN [TenantManager].[Tenant] t1 ON tum.OtherTenantCode = t1.TenantCode
INNER JOIN [NIS].[User] usr ON tum.UserId = usr.Id
INNER JOIN [NIS].[User] usr1 ON tum.LastUpdatedBy = usr1.Id
INNER JOIN [NIS].[Role] r ON tum.OtherTenantAccessRoleId = r.Id