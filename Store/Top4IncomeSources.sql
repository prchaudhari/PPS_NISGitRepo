CREATE TABLE [NIS].[Top4IncomeSources]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Source] [nvarchar](100) NULL,
	[CurrentSpend] [decimal](11, 2) NULL,
	[AverageSpend] [decimal](11, 2) NULL,
)
