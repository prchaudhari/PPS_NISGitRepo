CREATE TABLE [EntityManager].[Entities]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[Keys] [nvarchar](max) NULL,
	[AssemblyName] [nvarchar](max) NULL,
	[NamespaceName] [nvarchar](max) NULL,
	[Operations] [nvarchar](max) NULL,
	[ComponentCode] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NULL,
	[IsImportEnabled] [bit] NULL,
	[ServiceURL] [nvarchar](max) NULL,
	[TenantCode] [nvarchar](max) NOT NULL,
	[IsDefaultEntity] [bit] NULL,
	[DisplayName] [nvarchar](max) NULL,
)
