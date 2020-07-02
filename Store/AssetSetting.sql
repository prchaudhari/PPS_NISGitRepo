CREATE TABLE [NIS].[AssetSetting]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [ImageHeight] NUMERIC NOT NULL, 
    [ImageWidth] NUMERIC NOT NULL, 
    [ImageSize] DECIMAL NOT NULL, 
    [ImageFileExtension] NVARCHAR(50) NOT NULL,
    [VideoSize] DECIMAL NOT NULL, 
    [VideoFileExtension] NVARCHAR(50) NOT NULL,
    [TenantCode] NVARCHAR(50) NOT NULL,
)
