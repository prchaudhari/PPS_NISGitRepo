CREATE TABLE [NIS].[TransactionDetail]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [CustomerId] [bigint] NOT NULL,
	[AccountId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
    [Date] DATETIME NOT NULL, 
    [TransactionType] NVARCHAR(50) NOT NULL, 
    [AccountType] NVARCHAR(50) NOT NULL, 
    [Details] NVARCHAR(500) NULL, 
    [Amount] DECIMAL(8, 2) NOT NULL
)
