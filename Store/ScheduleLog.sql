CREATE TABLE [NIS].[ScheduleLog] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [ScheduleId]    BIGINT         NOT NULL,
    [ScheduleName]  NVARCHAR (50)  NOT NULL,
    [NumberOfRetry] INT            NOT NULL,
    [LogFilePath]   NVARCHAR (MAX) NULL,
    [CreationDate]  DATETIME       NOT NULL,
    [Status]        NVARCHAR (50)  NOT NULL,
    [TenantCode]    NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__Schedule__3214EC07B593A0D3] PRIMARY KEY CLUSTERED ([Id] ASC)
);


