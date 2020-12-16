CREATE TABLE [NIS].[AnalyticsData] (
    [Id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [StatementId]  BIGINT         NOT NULL,
    [CustomerId]   BIGINT         NOT NULL,
    [AccountId]    NVARCHAR (100) NULL,
    [PageWidgetId] BIGINT         NULL,
    [PageId]       BIGINT         NULL,
    [WidgetId]     BIGINT         NULL,
    [EventDate]    DATETIME       NOT NULL,
    [EventType]    NVARCHAR (50)  NOT NULL,
    [TenantCode]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__Analytic__3214EC0714B37CDF] PRIMARY KEY CLUSTERED ([Id] ASC)
);


