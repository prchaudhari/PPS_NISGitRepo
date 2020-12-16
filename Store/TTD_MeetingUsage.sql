CREATE TABLE [NIS].[TTD_MeetingUsage] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]    BIGINT         NOT NULL,
    [CustomerId] BIGINT         NOT NULL,
    [Month]      NVARCHAR (100) NOT NULL,
    [Year]       BIGINT         NOT NULL,
    [Microsoft]  BIGINT         NOT NULL,
    [Zoom]       BIGINT         NOT NULL,
    [TenantCode] NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Meet__3214EC0726A0BBA3] PRIMARY KEY CLUSTERED ([Id] ASC)
);


