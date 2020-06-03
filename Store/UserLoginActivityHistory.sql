CREATE TABLE [NIS].[UserLoginActivityHistory]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserIdentifier] nvarchar(100) NOT NULL,
	[Activity]  NVARCHAR(50) NOT NULL,
	[CreatedAt] datetime not null,
    [IsActive] BIT NOT NULL,
    [IsDeleted] BIT NOT NULL,
	[TenantCode] NVARCHAR(50) NOT NULL
)
