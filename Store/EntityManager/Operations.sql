CREATE TABLE [EntityManager].[Operations] (
    [Id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantCode]    NVARCHAR (MAX) NOT NULL,
    [EntityName]    NVARCHAR (MAX) NOT NULL,
    [ComponentCode] NVARCHAR (MAX) NOT NULL,
    [Operation]     NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


