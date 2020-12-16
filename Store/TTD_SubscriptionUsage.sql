CREATE TABLE [NIS].[TTD_SubscriptionUsage] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]      BIGINT         NOT NULL,
    [CustomerId]   BIGINT         NOT NULL,
    [Subscription] NVARCHAR (100) NOT NULL,
    [VendorName]   NVARCHAR (100) NOT NULL,
    [Email]        NVARCHAR (100) NOT NULL,
    [Usage]        DECIMAL (18)   NOT NULL,
    [Emails]       BIGINT         NOT NULL,
    [Meetings]     BIGINT         NOT NULL,
    [TenantCode]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Subs__3214EC075391A67E] PRIMARY KEY CLUSTERED ([Id] ASC)
);


