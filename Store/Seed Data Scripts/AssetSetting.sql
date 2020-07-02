DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');

INSERT INTO [NIS].[AssetSetting]
([ImageHeight],[ImageWidth],[ImageSize],[ImageFileExtension],[VideoSize],[VideoFileExtension],[TenantCode])
VALUES(300,400,1,'png,jpeg',2,'mp4',@SuperTenantCode);
GO