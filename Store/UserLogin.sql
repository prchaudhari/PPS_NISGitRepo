CREATE TABLE [NIS].[UserLogin]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserIdentifier] nvarchar(100) NOT NULL,
	[Password] nvarchar(100) NOT NULL,
	[LastModifiedOn] datetime not null
)
