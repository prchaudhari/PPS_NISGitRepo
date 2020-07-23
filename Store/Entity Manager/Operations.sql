CREATE TABLE [EntityManager].[Operations]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenantCode] [nvarchar](max) NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[ComponentCode] [nvarchar](max) NOT NULL,
	[Operation] [nvarchar](max) NOT NULL,
)
