CREATE TABLE [NIS].[BatchMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenantCode] [varchar](100) NULL,
	[BatchName] [varchar](100) NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[ScheduleId] [bigint] NULL,
	[IsExecuted] [bit] NULL,
 CONSTRAINT [PK_BatchMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)