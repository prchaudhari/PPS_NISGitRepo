CREATE TABLE [NIS].[AnalyticsData]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [StatementId] BIGINT NOT NULL, 
    [CustomerId] BIGINT NOT NULL, 
    [AccountId] BIGINT NULL, 
    [PageWidgetId] BIGINT NULL, 
    [PageId] BIGINT NULL, 
    [WidgetId] BIGINT NULL, 
    [EventDate] DATETIME NOT NULL, 
    [EventType] NVARCHAR(50) NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL, 
)
