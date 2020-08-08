CREATE TABLE [NIS].[ScheduleLog]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleName] [nvarchar](50) NOT NULL,
	[NumberOfRetry] [int] NOT NULL,
	[LogFilePath] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
)
