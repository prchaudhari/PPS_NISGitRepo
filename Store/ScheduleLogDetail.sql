CREATE TABLE [NIS].[ScheduleLogDetail]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ScheduleLogId] [bigint] NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [nvarchar](250) NULL,
	[RenderEngineId] [bigint] NOT NULL,
	[RenderEngineName] [nvarchar](100) NULL,
	[RenderEngineURL] [nvarchar](max) NULL,
	[NumberOfRetry] [int] NOT NULL,
	[Status] [nvarchar](20) NULL,
	[LogMessage] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[StatementFilePath] [nvarchar](max) NULL,
)
