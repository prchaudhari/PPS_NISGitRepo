CREATE TABLE [NIS].[UserCredentialHistory]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserIdentifier] nvarchar(100) NOT NULL,
	[Password] nvarchar(100) NOT NULL,
	[IsSystemGenerated]  BIT NOT NULL,
	[CreatedAt] datetime not null,
	[TenantCode] NVARCHAR(50) NOT NULL
)
