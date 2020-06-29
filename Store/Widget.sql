
CREATE TABLE [NIS].[Widget] (
   [Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductTypeId] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[WidgetName] [nvarchar](100) NOT NULL,
	[IsConfigurable] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[UpdateBy] [bigint] NOT NULL,
	[Instantiable] [bit] NOT NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)
);

