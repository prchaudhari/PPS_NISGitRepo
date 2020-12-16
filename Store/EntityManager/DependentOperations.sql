CREATE TABLE [EntityManager].[DependentOperations] (
    [Id]                   BIGINT NOT NULL,
    [OperationId]          BIGINT NOT NULL,
    [DependentOperationId] BIGINT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

