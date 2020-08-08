CREATE TABLE [NIS].[AccountTransaction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[TransactionType] [nvarchar](50) NULL,
	[Narration] [nvarchar](50) NULL,
	[FCY] [nvarchar](50) NULL,
	[CurrentRate] [nvarchar](50) NULL,
	[LCY] [nvarchar](50) NULL,
 CONSTRAINT [PK_AccountTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
