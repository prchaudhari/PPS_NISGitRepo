CREATE TABLE [NIS].[TTD_SubscriptionSpend] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]    BIGINT         NOT NULL,
    [CustomerId] BIGINT         NOT NULL,
    [Month]      NVARCHAR (100) NOT NULL,
    [Year]       BIGINT         NOT NULL,
    [Microsoft]  DECIMAL (18)   NOT NULL,
    [Zoom]       DECIMAL (18)   NOT NULL,
    [TenantCode] NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Subs__3214EC076D64A03C] PRIMARY KEY CLUSTERED ([Id] ASC)
);


