CREATE TABLE [NIS].[Top4IncomeSources] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [CustomerId]   BIGINT          NOT NULL,
    [BatchId]      BIGINT          NOT NULL,
    [Source]       NVARCHAR (100)  NULL,
    [CurrentSpend] DECIMAL (11, 2) NULL,
    [AverageSpend] DECIMAL (11, 2) NULL,
    [TenantCode]   NVARCHAR (50)   NULL,
    CONSTRAINT [PK_Top4IncomeSources] PRIMARY KEY CLUSTERED ([Id] ASC)
);


