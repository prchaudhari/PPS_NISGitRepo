CREATE TABLE [NIS].[TTD_DataUsage] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [BatchId]    BIGINT         NOT NULL,
    [CustomerId] BIGINT         NOT NULL,
    [Month]      NVARCHAR (100) NOT NULL,
    [Year]       BIGINT         NOT NULL,
    [Microsoft]  BIGINT         NOT NULL,
    [Zoom]       BIGINT         NOT NULL,
    [TenantCode] NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__TTD_Data__3214EC072CBB9CDD] PRIMARY KEY CLUSTERED ([Id] ASC)
);


