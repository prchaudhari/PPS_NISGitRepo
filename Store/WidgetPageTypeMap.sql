CREATE TABLE [NIS].[WidgetPageTypeMap]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [WidgetId] BIGINT NOT NULL, 
    [PageTypeId] BIGINT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL, 
    [IsDynamicWidget] BIT NOT NULL
)
