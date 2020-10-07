DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');

INSERT INTO [NIS].[TenantConfiguration]
           ([Name]
           ,[Description]
           ,[InputDataSourcePath]
           ,[OutputHTMLPath]
           ,[OutputPDFPath]
           ,[ArchivalPath]
           ,[AssetPath]
           ,[ArchivalPeriod]
           ,[ArchivalPeriodUnit]
           ,[DateFormat]
           ,[TenantCode])
VALUES('Test','','','','','','','',0,'',@SuperTenantCode);
GO