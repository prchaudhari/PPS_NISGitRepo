CREATE TABLE [NIS].[ReminderAndRecommendation] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [CustomerId]  BIGINT         NOT NULL,
    [BatchId]     BIGINT         NOT NULL,
    [Description] NVARCHAR (500) NOT NULL,
    [LabelText]   NVARCHAR (500) NOT NULL,
    [TargetURL]   NVARCHAR (500) NULL,
    [TenantCode]  NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_ReminderAndRecommendation] PRIMARY KEY CLUSTERED ([Id] ASC)
);


