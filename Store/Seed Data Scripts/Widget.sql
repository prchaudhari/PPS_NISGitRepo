DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @SuperTenantIdentifier AS NVARCHAR(MAX) = (select id from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @DateTime as DateTime =(SELECT GETDATE());

INSERT INTO [NIS].[Widget]([DisplayName],[WidgetName],[Description],[PageTypeId],[IsConfigurable],
[TenantCode],[IsDeleted],[IsActive],[LastUpdatedDate],[UpdateBy],[Instantiable])
VALUES
('Customer Information','CustomerInformation','','',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Account Information','AccountInformation','','',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Summary at Glance','Summary','','',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Image','Image','','',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Video','Video','','',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0);
