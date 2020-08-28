CREATE TABLE [NIS].[BatchMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[TenantCode] [varchar](100) NULL,
	[BatchName] [varchar](100) NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ScheduleId] [bigint] NULL,
	[IsExecuted] [bit] NULL,
	[IsDataReady] [bit] NOT NULL,
	[DataExtractionDate] [datetime] NOT NULL,
	[BatchExecutionDate] [datetime] NOT NULL,
	[Status] [varchar](50) NOT NULL
)