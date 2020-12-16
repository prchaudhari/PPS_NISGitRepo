CREATE TABLE [NIS].[Video] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [BatchId]     BIGINT         NOT NULL,
    [StatementId] BIGINT         NOT NULL,
    [PageId]      BIGINT         NOT NULL,
    [WidgetId]    BIGINT         NOT NULL,
    [Date]        DATETIME       NOT NULL,
    [Title]       NVARCHAR (100) NOT NULL,
    [Video]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK__Video__3214EC07B09B4F6B] PRIMARY KEY CLUSTERED ([Id] ASC)
);


