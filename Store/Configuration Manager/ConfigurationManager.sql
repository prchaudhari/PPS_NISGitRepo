CREATE TABLE [ConfigurationManager].[ConfigurationManager]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,  
	[PartionKey] NVARCHAR(100) NOT NULL,
	[RowKey] NVARCHAR(100) NOT NULL,
	[Value] NVARCHAR(100) NOT NULL
)
