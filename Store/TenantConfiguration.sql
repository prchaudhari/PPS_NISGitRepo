CREATE TABLE [NIS].[TenantConfiguration]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL, 
    [InputDataSourcePath] NVARCHAR(MAX) NULL, 
    [OutputHTMLPath] NVARCHAR(MAX) NULL, 
    [OutputPDFPath] NVARCHAR(MAX) NULL,
    [ArchivalPath] NVARCHAR(MAX) NULL, 
    [AssetPath] NVARCHAR(MAX) NULL,
    [TenantCode] [nvarchar](50) NOT NULL,
    [ArchivalPeriod] [int] NULL,
	[ArchivalPeriodUnit] [nvarchar](50) NULL,
	[DateFormat] [nvarchar](50) NULL, 
    [ApplicationTheme] NVARCHAR(50) NULL, 
    [WidgetThemeSetting] NVARCHAR(MAX) NULL,
)
