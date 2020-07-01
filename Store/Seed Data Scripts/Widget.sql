DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @SuperTenantIdentifier AS NVARCHAR(MAX) = (select id from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @DateTime as DateTime =(SELECT GETDATE());

INSERT INTO [NIS].[Widget]([DisplayName],[WidgetName],[Description],[PageTypeId],[IsConfigurable],
[TenantCode],[IsDeleted],[IsActive],[LastUpdatedDate],[UpdateBy],[Instantiable])
VALUES
('Customer Information','CustomerInformation','','2,3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Account Information','AccountInformation','Account details','1',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Summary at Glance','Summary','','1',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Image','Image','Marketing widget - Configuration for image and click through URL ','1,2,3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Video','Video','Customer Information - Allowing to upload video ','1,2,3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0);
