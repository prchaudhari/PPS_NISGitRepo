CREATE TABLE [NIS].[TTD_SubscriptionSummary] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]      BIGINT         NOT NULL,
    [CustomerId]   BIGINT         NOT NULL,
    [VendorName]   NVARCHAR (250) NOT NULL,
    [Subscription] NVARCHAR (250) NOT NULL,
    [Total]        DECIMAL (18)   NOT NULL,
    [AverageSpend] DECIMAL (18)   NOT NULL,
    [TenantCode]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Subs__3214EC079DE2FFB0] PRIMARY KEY CLUSTERED ([Id] ASC)
);


