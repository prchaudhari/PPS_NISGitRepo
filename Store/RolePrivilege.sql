CREATE TABLE [NIS].[RolePrivilege]
(
	[Id] bigint NOT NULL identity(1,1) PRIMARY KEY,
	[RoleIdentifier] bigint not null,
	[EntityName] nvarchar(max) not null,
	[Operation] nvarchar(max) not null,
	[IsEnable] bit not null
)
