CREATE TABLE [NIS].[EntityFieldMap] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (100) NOT NULL,
    [EntityId]   BIGINT         NOT NULL,
    [IsActive]   BIT            NOT NULL,
    [IsDeleted]  BIT            NOT NULL,
    [TenantCode] NVARCHAR (50)  NOT NULL,
    [DataType]   NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK__EntityFi__3214EC07B7BDD3B3] PRIMARY KEY CLUSTERED ([Id] ASC)
);


