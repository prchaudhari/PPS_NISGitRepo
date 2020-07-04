CREATE TABLE [NIS].[AssetSetting]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [ImageHeight] DECIMAL NOT NULL, 
    [ImageWidth] DECIMAL NOT NULL, 
    [ImageSize] DECIMAL NOT NULL, 
    [ImageFileExtension] NVARCHAR(50) NOT NULL,
    [VideoSize] DECIMAL NOT NULL, 
    [VideoFileExtension] NVARCHAR(50) NOT NULL,
    [TenantCode] NVARCHAR(50) NOT NULL,
)
