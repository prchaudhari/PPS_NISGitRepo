CREATE TABLE [NIS].[StatementMetadata]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleLogId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[StatementDate] [datetime] NULL,
	[StatementPeriod] [nvarchar](50) NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [nvarchar](500) NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[StatementURL] [nvarchar](max) NOT NULL,
)
