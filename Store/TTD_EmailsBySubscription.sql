CREATE TABLE [NIS].[TTD_EmailsBySubscription] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]      BIGINT         NOT NULL,
    [CustomerId]   BIGINT         NOT NULL,
    [Subscription] NVARCHAR (100) NOT NULL,
    [Emails]       BIGINT         NOT NULL,
    [TenantCode]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Emai__3214EC07043E00AC] PRIMARY KEY CLUSTERED ([Id] ASC)
);


