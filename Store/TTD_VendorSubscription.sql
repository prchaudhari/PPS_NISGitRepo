CREATE TABLE [NIS].[TTD_VendorSubscription] (
    [Id]                  BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]             BIGINT         NOT NULL,
    [CustomerId]          BIGINT         NOT NULL,
    [VenderName]          NVARCHAR (100) NOT NULL,
    [CountOfSubscription] BIGINT         NOT NULL,
    [TenantCode]          NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Vend__3214EC073B18693D] PRIMARY KEY CLUSTERED ([Id] ASC)
);


