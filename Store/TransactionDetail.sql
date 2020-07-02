CREATE TABLE [NIS].[TransactionDetail]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Date] DATETIME NOT NULL, 
    [TransactionType] NVARCHAR(50) NOT NULL, 
    [AccountType] NVARCHAR(50) NOT NULL, 
    [Details] NVARCHAR(500) NULL, 
    [Amount] DECIMAL NOT NULL
)
