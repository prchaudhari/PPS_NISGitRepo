CREATE TABLE [NIS].[Statement]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	[Name] NVARCHAR(100) NOT NULL,	
	[Description] NVARCHAR(250) NOT NULL,	
	[PublishedBy] BIGINT NOT NULL,
	[Owner] BIGINT NOT NULL,
	[Version] NVARCHAR(100) NOT NULL,
	[Status] NVARCHAR(50) NOT NULL,	
	[CreatedDate] DateTime  NULL,
	[PublishedOn] DateTime  NULL,
    [IsActive] BIT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL
	[LastUpdatedDate] DateTime  NULL,
	[UpdateBy] BIGINT NOT NULL,
)
