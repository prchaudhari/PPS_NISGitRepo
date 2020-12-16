CREATE TABLE [NIS].[ScheduleRunHistory] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [ScheduleId]    BIGINT         NOT NULL,
    [ScheduleLogId] BIGINT         NOT NULL,
    [StartDate]     DATETIME       NOT NULL,
    [EndDate]       DATETIME       NOT NULL,
    [TenantCode]    NVARCHAR (50)  NULL,
    [StatementId]   BIGINT         NOT NULL,
    [FilePath]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK__Schedule__3214EC07A1641CE6] PRIMARY KEY CLUSTERED ([Id] ASC)
);


