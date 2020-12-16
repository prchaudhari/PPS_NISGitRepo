CREATE TABLE [NIS].[ScheduleLogDetail] (
    [Id]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ScheduleLogId]     BIGINT         NOT NULL,
    [ScheduleId]        BIGINT         NOT NULL,
    [CustomerId]        BIGINT         NOT NULL,
    [CustomerName]      NVARCHAR (250) NULL,
    [RenderEngineId]    BIGINT         NOT NULL,
    [RenderEngineName]  NVARCHAR (100) NULL,
    [RenderEngineURL]   NVARCHAR (MAX) NULL,
    [NumberOfRetry]     INT            NOT NULL,
    [Status]            NVARCHAR (20)  NULL,
    [LogMessage]        NVARCHAR (MAX) NULL,
    [CreationDate]      DATETIME       NOT NULL,
    [TenantCode]        NVARCHAR (50)  NOT NULL,
    [StatementFilePath] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__Schedule__3214EC073B2F0802] PRIMARY KEY CLUSTERED ([Id] ASC)
);


