CREATE TABLE [NIS].[StatementAnalytics] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [StatementId] BIGINT         NOT NULL,
    [WidgetId]    BIGINT         NOT NULL,
    [WidgetName]  NVARCHAR (200) NOT NULL,
    [PageId]      BIGINT         NOT NULL,
    [PageName]    NVARCHAR (500) NOT NULL,
    [CustomerId]  BIGINT         NOT NULL,
    [Date]        DATETIME       NOT NULL,
    [Hour]        INT            NOT NULL,
    [Minute]      INT            NOT NULL,
    CONSTRAINT [PK_StatementAnalytics] PRIMARY KEY CLUSTERED ([Id] ASC)
);


