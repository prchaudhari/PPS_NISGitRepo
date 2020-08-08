CREATE TABLE [NIS].[StatementAnalytics]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[StatementId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[WidgetName] [nvarchar](200) NOT NULL,
	[PageId] [bigint] NOT NULL,
	[PageName] [nvarchar](500) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Hour] [int] NOT NULL,
	[Minute] [int] NOT NULL,
)
