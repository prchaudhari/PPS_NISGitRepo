CREATE TABLE [NIS].[SavingTrend] (
    [Id]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [AccountId]        BIGINT          NOT NULL,
    [CustomerId]       BIGINT          NOT NULL,
    [BatchId]          BIGINT          NOT NULL,
    [Month]            NVARCHAR (50)   NOT NULL,
    [SpendAmount]      DECIMAL (11, 2) NOT NULL,
    [SpendPercentage]  DECIMAL (4, 2)  NULL,
    [Income]           DECIMAL (11, 2) NULL,
    [IncomePercentage] DECIMAL (4, 2)  NULL,
    [TenantCode]       NVARCHAR (50)   NOT NULL,
    CONSTRAINT [PK_SavingTrend] PRIMARY KEY CLUSTERED ([Id] ASC)
);


