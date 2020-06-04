CREATE TABLE [NIS].[PageWidgetMap]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [ReferenceWidgetId] BIGINT NOT NULL, 
    [Height] INT NOT NULL, 
    [Width] INT NOT NULL, 
    [Xposition] INT NOT NULL, 
    [Yposition] INT NOT NULL, 
    [PageId] BIGINT NOT NULL, 
    [WidgetSetting] NVARCHAR (MAX) NOT NULL,
    [TenantCode] NVARCHAR(50) NOT NULL
)
