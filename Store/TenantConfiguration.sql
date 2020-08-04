CREATE TABLE [NIS].[TenantConfiguration]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Description] NVARCHAR(500) NOT NULL, 
    [InputDataSourcePath] NVARCHAR(MAX) NOT NULL, 
    [OutputHTMLPath] NVARCHAR(MAX) NOT NULL, 
    [OutputPDFPath] NVARCHAR(MAX) NOT NULL,
)
