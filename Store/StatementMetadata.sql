CREATE TABLE [NIS].[StatementMetadata] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [ScheduleId]      BIGINT         NOT NULL,
    [ScheduleLogId]   BIGINT         NOT NULL,
    [StatementId]     BIGINT         NOT NULL,
    [StatementDate]   DATETIME       NULL,
    [StatementPeriod] NVARCHAR (50)  NULL,
    [CustomerId]      BIGINT         NOT NULL,
    [CustomerName]    NVARCHAR (500) NULL,
    [AccountNumber]   NVARCHAR (50)  NOT NULL,
    [AccountType]     NVARCHAR (50)  NOT NULL,
    [StatementURL]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_StatementMetadata] PRIMARY KEY CLUSTERED ([Id] ASC)
);


