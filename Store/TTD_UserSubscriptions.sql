CREATE TABLE [NIS].[TTD_UserSubscriptions] (
    [Id]                  BIGINT        IDENTITY (1, 1) NOT NULL,
    [BatchId]             BIGINT        NOT NULL,
    [CustomerId]          BIGINT        NOT NULL,
    [CountOfSubscription] BIGINT        NOT NULL,
    [TenantCode]          NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK__TTD_User__3214EC07BA09300A] PRIMARY KEY CLUSTERED ([Id] ASC)
);


