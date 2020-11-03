CREATE TABLE [NIS].[ReminderAndRecommendation]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[LabelText] [nvarchar](500) NOT NULL,
	[TargetURL] [nvarchar](500) NULL,
	[TenantCode] NVARCHAR(50) NOT NULL
)
