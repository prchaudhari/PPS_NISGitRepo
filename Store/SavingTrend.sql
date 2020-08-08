CREATE TABLE [NIS].[SavingTrend](
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[AccountId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Month] [nvarchar](50) NOT NULL,
	[SpendAmount] [decimal](11, 2) NOT NULL,
	[SpendPercentage] [decimal](4, 2) NULL,
	[Income] [decimal](11, 2) NULL,
	[IncomePercentage] [decimal](4, 2) NULL
)
