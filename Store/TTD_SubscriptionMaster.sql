CREATE TABLE [NIS].[TTD_SubscriptionMaster] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]      BIGINT         NOT NULL,
    [CustomerId]   BIGINT         NOT NULL,
    [CustomerCode] NVARCHAR (100) NULL,
    [VendorName]   NVARCHAR (100) NOT NULL,
    [Subscription] NVARCHAR (100) NOT NULL,
    [Email]        NVARCHAR (100) NOT NULL,
    [StartDate]    DATETIME       NULL,
    [EndDate]      DATETIME       NULL,
    [TenantCode]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Subs__3214EC07341A8EE5] PRIMARY KEY CLUSTERED ([Id] ASC)
);


