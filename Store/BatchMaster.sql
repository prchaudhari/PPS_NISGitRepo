CREATE TABLE [NIS].[BatchMaster] (
    [Id]                 BIGINT        IDENTITY (1, 1) NOT NULL,
    [TenantCode]         VARCHAR (100) NULL,
    [BatchName]          VARCHAR (100) NULL,
    [CreatedBy]          BIGINT        NOT NULL,
    [CreatedDate]        DATETIME      NOT NULL,
    [ScheduleId]         BIGINT        NOT NULL,
    [IsExecuted]         BIT           NOT NULL,
    [IsDataReady]        BIT           NOT NULL,
    [DataExtractionDate] DATETIME      NOT NULL,
    [BatchExecutionDate] DATETIME      NOT NULL,
    [Status]             VARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_BatchMaster] PRIMARY KEY CLUSTERED ([Id] ASC)
);

