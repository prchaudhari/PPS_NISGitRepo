DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');

INSERT INTO [NIS].[TenantConfiguration]
           ([Name]
           ,[Description]
           ,[InputDataSourcePath]
           ,[OutputHTMLPath]
           ,[OutputPDFPath]
           ,[TenantCode])
VALUES('','','','','',@SuperTenantCode);
GO