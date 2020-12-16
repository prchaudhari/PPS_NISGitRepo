CREATE TABLE [ConfigurationManager].[ConfigurationManager] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [PartionKey] NVARCHAR (100) NOT NULL,
    [RowKey]     NVARCHAR (100) NOT NULL,
    [Value]      NVARCHAR (500) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


