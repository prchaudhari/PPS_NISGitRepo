CREATE TABLE [NIS].[Image] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [BatchId]     BIGINT         NOT NULL,
    [StatementId] BIGINT         NOT NULL,
    [PageId]      BIGINT         NOT NULL,
    [WidgetId]    BIGINT         NOT NULL,
    [Date]        DATETIME       NOT NULL,
    [Title]       NVARCHAR (100) NOT NULL,
    [Image]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK__Image__3214EC070AC15F5F] PRIMARY KEY CLUSTERED ([Id] ASC)
);


