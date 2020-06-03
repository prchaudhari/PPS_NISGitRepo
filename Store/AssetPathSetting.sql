CREATE TABLE [NIS].[AssetPathSetting]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [AssetPath] NVARCHAR(500) NOT NULL, 
    [IsActive] BIT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL,
	
)
