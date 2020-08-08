CREATE TABLE [NIS].[Asset]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Name] [nvarchar](100) NULL,
	[FilePath] [nvarchar](500) NOT NULL,
	[AssetLibraryId] [bigint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[LastUpdatedBy] [bigint] NULL,
)
