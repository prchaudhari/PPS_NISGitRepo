CREATE TABLE [NIS].[ScheduleRunHistory]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleLogId] [bigint] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NULL,
	[StatementId] [bigint] NOT NULL,
	[FilePath] [nvarchar](max) NULL,
)
