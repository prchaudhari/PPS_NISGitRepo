CREATE TABLE [EntityManager].[Entities] (
    [Id]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [EntityName]      NVARCHAR (MAX) NOT NULL,
    [Keys]            NVARCHAR (MAX) NULL,
    [AssemblyName]    NVARCHAR (MAX) NULL,
    [NamespaceName]   NVARCHAR (MAX) NULL,
    [Operations]      NVARCHAR (MAX) NULL,
    [ComponentCode]   NVARCHAR (MAX) NOT NULL,
    [IsActive]        BIT            NULL,
    [IsImportEnabled] BIT            NULL,
    [ServiceURL]      NVARCHAR (MAX) NULL,
    [TenantCode]      NVARCHAR (MAX) NOT NULL,
    [IsDefaultEntity] BIT            NULL,
    [DisplayName]     NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


