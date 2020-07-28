CREATE TABLE [NIS].[Image]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [BatchId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
    [Date] DATETIME NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Image] NVARCHAR(MAX) NOT NULL
)
