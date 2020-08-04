DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');

INSERT INTO [NIS].[TenantConfiguration]
           ([Description]
           ,[InputDataSourcePath]
           ,[OutputHTMLPath]
           ,[OutputPDFPath])
VALUES('','','','',@SuperTenantCode);
GO