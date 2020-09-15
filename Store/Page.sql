CREATE TABLE [NIS].[Page]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	[DisplayName] NVARCHAR(100) NOT NULL,	
	[PageTypeId] BIGINT NOT NULL,
	[PublishedBy] BIGINT NULL,
	[Owner] BIGINT NOT NULL,
	[Version] NVARCHAR(100) NOT NULL,
	[Status] NVARCHAR(50) NOT NULL,	
	[CreatedDate] DateTime  NULL,
	[PublishedOn] DateTime  NULL,
    [IsActive] BIT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL,
	[LastUpdatedDate] DateTime  NULL,
	[UpdateBy] BIGINT NULL, 
    [BackgroundImageAssetId] BIGINT NULL, 
    [BackgroundImageURL] NVARCHAR(MAX) NULL,
)
