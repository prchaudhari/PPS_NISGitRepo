CREATE TABLE [NIS].[TransactionDetail] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [CustomerId]      BIGINT          NOT NULL,
    [AccountId]       BIGINT          NOT NULL,
    [BatchId]         BIGINT          NOT NULL,
    [Date]            DATETIME        NOT NULL,
    [TransactionType] NVARCHAR (50)   NOT NULL,
    [AccountType]     NVARCHAR (50)   NOT NULL,
    [Details]         NVARCHAR (500)  NULL,
    [Amount]          DECIMAL (11, 2) NOT NULL,
    CONSTRAINT [PK__Transact__3214EC074B6784C5] PRIMARY KEY CLUSTERED ([Id] ASC)
);


