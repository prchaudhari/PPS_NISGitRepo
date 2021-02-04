
Go
/****** Object:  Schema [ConfigurationManager]    Script Date: 19-11-2020 15:12:18 ******/
--CREATE SCHEMA [ConfigurationManager]
GO
/****** Object:  Schema [EntityManager]    Script Date: 19-11-2020 15:12:18 ******/
CREATE SCHEMA [EntityManager]
GO
/****** Object:  Schema [NIS]    Script Date: 19-11-2020 15:12:18 ******/
CREATE SCHEMA [NIS]
GO
/****** Object:  Schema [TenantManager]    Script Date: 19-11-2020 15:12:18 ******/
--CREATE SCHEMA [TenantManager]
GO
/****** Object:  Table [ConfigurationManager].[ConfigurationManager]    Script Date: 19-11-2020 15:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
--GO
--CREATE TABLE [ConfigurationManager].[ConfigurationManager](
--	[Id] [bigint] IDENTITY(1,1) NOT NULL,
--	[PartionKey] [nvarchar](100) NOT NULL,
--	[RowKey] [nvarchar](100) NOT NULL,
--	[Value] [nvarchar](100) NOT NULL,
--PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

GO
/****** Object:  Table [EntityManager].[DependentOperations]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [EntityManager].[DependentOperations](
	[Id] [bigint] NOT NULL,
	[OperationId] [bigint] NOT NULL,
	[DependentOperationId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [EntityManager].[Entities]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [EntityManager].[Entities](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[Keys] [nvarchar](max) NULL,
	[AssemblyName] [nvarchar](max) NULL,
	[NamespaceName] [nvarchar](max) NULL,
	[Operations] [nvarchar](max) NULL,
	[ComponentCode] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NULL,
	[IsImportEnabled] [bit] NULL,
	[ServiceURL] [nvarchar](max) NULL,
	[TenantCode] [nvarchar](max) NOT NULL,
	[IsDefaultEntity] [bit] NULL,
	[DisplayName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [EntityManager].[Operations]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [EntityManager].[Operations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenantCode] [nvarchar](max) NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[ComponentCode] [nvarchar](max) NOT NULL,
	[Operation] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[AccountMaster]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[AccountMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[Currency] [nvarchar](50) NOT NULL,
	[Balance] [decimal](11, 2) NOT NULL,
	[TotalDeposit] [decimal](11, 2) NOT NULL,
	[TotalSpend] [decimal](11, 2) NULL,
	[ProfitEarned] [decimal](11, 2) NULL,
	[Indicator] [nvarchar](50) NULL,
	[FeesPaid] [decimal](11, 2) NULL,
	[GrandTotal] [decimal](11, 2) NULL,
	[Percentage] [decimal](4, 2) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AccountMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[AccountTransaction]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[AccountTransaction](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[TransactionType] [nvarchar](50) NULL,
	[Narration] [nvarchar](50) NULL,
	[FCY] [nvarchar](50) NULL,
	[CurrentRate] [nvarchar](50) NULL,
	[LCY] [nvarchar](50) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AccountTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[AnalyticsData]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[AnalyticsData](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[AccountId] [nvarchar](100) NULL,
	[PageWidgetId] [bigint] NULL,
	[PageId] [bigint] NULL,
	[WidgetId] [bigint] NULL,
	[EventDate] [datetime] NOT NULL,
	[EventType] [nvarchar](50) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Analytic__3214EC0714B37CDF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Asset]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Asset](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[FilePath] [nvarchar](500) NOT NULL,
	[AssetLibraryId] [bigint] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[LastUpdatedBy] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[AssetLibrary]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[AssetLibrary](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[AssetPathSetting]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[AssetPathSetting](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AssetPath] [nvarchar](500) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[AssetSetting]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[AssetSetting](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ImageHeight] [decimal](18, 2) NOT NULL,
	[ImageWidth] [decimal](18, 2) NOT NULL,
	[ImageSize] [decimal](18, 2) NOT NULL,
	[ImageFileExtension] [nvarchar](50) NOT NULL,
	[VideoSize] [decimal](18, 2) NOT NULL,
	[VideoFileExtension] [nvarchar](50) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__AssetSet__3214EC07141A2A67] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[BatchDetails]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[BatchDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[ImageURL] [nvarchar](max) NULL,
	[VideoURL] [nvarchar](max) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_BatchDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[BatchMaster]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [NIS].[BatchMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenantCode] [varchar](100) NULL,
	[BatchName] [varchar](100) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[IsExecuted] [bit] NOT NULL,
	[IsDataReady] [bit] NOT NULL,
	[DataExtractionDate] [datetime] NOT NULL,
	[BatchExecutionDate] [datetime] NOT NULL,
	[Status] [varchar](50) NOT NULL,
 CONSTRAINT [PK_BatchMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [NIS].[City]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[City](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[StateId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ContactType]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[ContactType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Country]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Country](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](50) NULL,
	[DialingCode] [nvarchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Country__3214EC07D53F21D4] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[CustomerMaster]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[CustomerMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerCode] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[LastName] [nvarchar](500) NOT NULL,
	[AddressLine1] [nvarchar](500) NOT NULL,
	[AddressLine2] [nvarchar](500) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[Zip] [nvarchar](10) NULL,
	[StatementDate] [datetime] NULL,
	[StatementPeriod] [nvarchar](50) NULL,
	[RmName] [nvarchar](50) NULL,
	[RmContactNumber] [nvarchar](50) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[Field1] [nvarchar](100) NOT NULL,
	[Field2] [nvarchar](100) NOT NULL,
	[Field3] [nvarchar](100) NOT NULL,
	[Field4] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_CustomerMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[CustomerMedia]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[CustomerMedia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[ImageURL] [nvarchar](max) NULL,
	[VideoURL] [nvarchar](max) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CustomerMedia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[DynamicWidget]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[DynamicWidget](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WidgetName] [nvarchar](100) NOT NULL,
	[WidgetType] [nvarchar](50) NOT NULL,
	[PageTypeId] [bigint] NOT NULL,
	[EntityId] [bigint] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[ThemeType] [nvarchar](50) NOT NULL,
	[ThemeCSS] [nvarchar](max) NULL,
	[WidgetSettings] [nvarchar](max) NULL,
	[WidgetFilterSettings] [nvarchar](max) NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastUpdatedBy] [bigint] NOT NULL,
	[PublishedBy] [bigint] NULL,
	[PublishedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[CloneOfWidgetId] [bigint] NULL,
	[Version] [nvarchar](100) NULL,
	[PreviewData] [nvarchar](max) NOT NULL,
	[FilterCondition] NVARCHAR(MAX) NULL, 
 CONSTRAINT [PK__DynamicW__3214EC071888ACAB] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[DynamicWidgetFilterDetail]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[DynamicWidgetFilterDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DynamicWidgetId] [bigint] NOT NULL,
	[FieldId] [bigint] NOT NULL,
	[Operator] [nvarchar](50) NULL,
	[ConditionalOperator] [nvarchar](50) NULL,
	[Sequence] [bigint] NOT NULL,
	[Value] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[EntityFieldMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[EntityFieldMap](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[EntityId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[DataType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__EntityFi__3214EC07B7BDD3B3] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[MultiTenantUserAccessMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[MultiTenantUserAccessMap](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[AssociatedTenantCode] [nvarchar](50) NOT NULL,
	[OtherTenantCode] [nvarchar](50) NOT NULL,
	[OtherTenantAccessRoleId] [bigint] NOT NULL,
	[ParentTenantCode] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LastUpdatedBy] [bigint] NOT NULL,
	[LastUpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK__MultiTen__3214EC076B0070F9] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Page]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Page](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[PageTypeId] [bigint] NOT NULL,
	[PublishedBy] [bigint] NULL,
	[Owner] [bigint] NOT NULL,
	[Version] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[PublishedOn] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[UpdateBy] [bigint] NULL,
	[BackgroundImageAssetId] [bigint] NULL,
	[BackgroundImageURL] [nvarchar](max) NULL,
 CONSTRAINT [PK__Page__3214EC0778D5B88B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[PageType]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[PageType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[PageWidgetMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[PageWidgetMap](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReferenceWidgetId] [bigint] NOT NULL,
	[Height] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Xposition] [int] NOT NULL,
	[Yposition] [int] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetSetting] [nvarchar](max) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsDynamicWidget] [bit] NOT NULL,
 CONSTRAINT [PK__PageWidg__3214EC0793EB5877] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ReminderAndRecommendation]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[ReminderAndRecommendation](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[LabelText] [nvarchar](500) NOT NULL,
	[TargetURL] [nvarchar](500) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ReminderAndRecommendation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[RenderEngine]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[RenderEngine](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[URL] [nvarchar](250) NULL,
	[PriorityLevel] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[NumberOfThread] [int] NOT NULL,
	[InUse] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Role]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Role](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[RolePrivilege]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[RolePrivilege](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleIdentifier] [bigint] NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[Operation] [nvarchar](max) NOT NULL,
	[IsEnable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[SavingTrend]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[SavingTrend](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AccountId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Month] [nvarchar](50) NOT NULL,
	[SpendAmount] [decimal](11, 2) NOT NULL,
	[SpendPercentage] [decimal](4, 2) NULL,
	[Income] [decimal](11, 2) NULL,
	[IncomePercentage] [decimal](4, 2) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SavingTrend] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Schedule]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Schedule](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[DayOfMonth] [bigint] NOT NULL,
	[HourOfDay] [bigint] NOT NULL,
	[MinuteOfDay] [bigint] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Status] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[UpdateBy] [bigint] NOT NULL,
	[IsExportToPDF] [bit] NOT NULL,
	[RecurrancePattern] [nvarchar](50) NULL,
	[RepeatEveryDayMonWeekYear] [bigint] NULL,
	[WeekDays] [nvarchar](200) NULL,
	[IsEveryWeekDay] [bit] NULL,
	[MonthOfYear] [nvarchar](50) NULL,
	[IsEndsAfterNoOfOccurrences] [bit] NULL,
	[NoOfOccurrences] [bigint] NULL,
 CONSTRAINT [PK__Schedule__3214EC07DE2298BC] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ScheduleLog]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[ScheduleLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleName] [nvarchar](50) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[BatchName] [nvarchar](150) NOT NULL,
	[NumberOfRetry] [int] NOT NULL,
	[LogFilePath] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Schedule__3214EC07B593A0D3] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ScheduleLogArchive]    Script Date: 2021-01-28 10:34:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [NIS].[ScheduleLogArchive](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleName] [nvarchar](50) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[BatchName] [nvarchar](150) NOT NULL,
	[NumberOfRetry] [int] NOT NULL,
	[LogFilePath] [nvarchar](max) NULL,
	[LogCreationDate] [datetime] NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[ArchivalDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Schedule__3214EC07BFCAE630] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ScheduleLogDetail]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[ScheduleLogDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleLogId] [bigint] NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [nvarchar](250) NULL,
	[RenderEngineId] [bigint] NOT NULL,
	[RenderEngineName] [nvarchar](100) NULL,
	[RenderEngineURL] [nvarchar](max) NULL,
	[NumberOfRetry] [int] NOT NULL,
	[Status] [nvarchar](20) NULL,
	[LogMessage] [nvarchar](max) NULL,
	[CreationDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[StatementFilePath] [nvarchar](max) NULL,
 CONSTRAINT [PK__Schedule__3214EC073B2F0802] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ScheduleLogDetailArchive]    Script Date: 2021-01-28 10:35:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [NIS].[ScheduleLogDetailArchive](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleLogArchiveId] [bigint] NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [nvarchar](250) NULL,
	[RenderEngineId] [bigint] NOT NULL,
	[RenderEngineName] [nvarchar](100) NULL,
	[RenderEngineURL] [nvarchar](max) NULL,
	[NumberOfRetry] [int] NOT NULL,
	[Status] [nvarchar](20) NULL,
	[LogMessage] [nvarchar](max) NULL,
	[LogDetailCreationDate] [datetime] NOT NULL,
	[PdfStatementPath] [nvarchar](max) NULL,
	[ArchivalDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[ScheduleRunHistory]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[ScheduleRunHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleLogId] [bigint] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NULL,
	[StatementId] [bigint] NOT NULL,
	[FilePath] [nvarchar](max) NULL,
 CONSTRAINT [PK__Schedule__3214EC07A1641CE6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[State]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[State](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[CountryId] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Statement]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Statement](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](250) NOT NULL,
	[PublishedBy] [bigint] NOT NULL,
	[Owner] [bigint] NOT NULL,
	[Version] [nvarchar](100) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[PublishedOn] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[UpdateBy] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[StatementMetadata]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[StatementMetadata](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleLogId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[StatementDate] [datetime] NULL,
	[StatementPeriod] [nvarchar](50) NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [nvarchar](500) NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[StatementURL] [nvarchar](max) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsPasswordGenerated] [bit] NOT NULL,
	[Password] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_StatementMetadata] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[StatementMetadataArchive]    Script Date: 2021-01-28 10:37:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [NIS].[StatementMetadataArchive](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [bigint] NOT NULL,
	[ScheduleLogArchiveId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[StatementDate] [datetime] NULL,
	[StatementPeriod] [nvarchar](50) NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerName] [nvarchar](500) NULL,
	[AccountNumber] [nvarchar](50) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[StatementURL] [nvarchar](max) NOT NULL,
	[ArchivalDate] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsPasswordGenerated] [bit] NULL,
	[Password] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[StatementPageMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[StatementPageMap](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ReferencePageId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[SequenceNumber] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TenantConfiguration]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TenantConfiguration](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[InputDataSourcePath] [nvarchar](max) NULL,
	[OutputHTMLPath] [nvarchar](max) NULL,
	[OutputPDFPath] [nvarchar](max) NULL,
	[ArchivalPath] [nvarchar](max) NULL,
	[AssetPath] [nvarchar](max) NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[ArchivalPeriod] [int] NULL,
	[ArchivalPeriodUnit] [nvarchar](50) NULL,
	[DateFormat] [nvarchar](50) NULL,
	[ApplicationTheme] [nvarchar](50) NULL,
	[WidgetThemeSetting] [nvarchar](max) NULL,
	[BaseUrlForTransactionData] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TenantContact]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TenantContact](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[ContactNumber] [nvarchar](20) NOT NULL,
	[ContactType] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](50) NOT NULL,
	[Image] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActivationLinkSent] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[CountryId] [bigint] NOT NULL,
 CONSTRAINT [PK__TenantCo__3214EC077E43916F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TenantEntity]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TenantEntity](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[LastUpdatedOn] [datetime] NOT NULL,
	[LastUpdatedBy] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[APIPath] [nvarchar](100) NOT NULL,
	[RequestType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__Entity__3214EC0726944DFF] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TenantSecurityCodeFormat]    Script Date: 2021-01-28 10:38:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [NIS].[TenantSecurityCodeFormat](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[Format] [nvarchar](500) NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedOn] [datetime] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TenantSubscription]    Script Date: 2021-01-28 10:38:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [NIS].[TenantSubscription](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenantCode] [uniqueidentifier] NOT NULL,
	[SubscriptionStartDate] [datetime] NOT NULL,
	[SubscriptionEndDate] [datetime] NOT NULL,
	[LastModifiedBy] [bigint] NOT NULL,
	[LastModifiedOn] [datetime] NOT NULL,
	[SubscriptionKey] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TenantUser]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TenantUser](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[ContactNumber] [nvarchar](20) NOT NULL,
	[EmailAddress] [nvarchar](50) NULL,
	[Image] [nvarchar](max) NULL,
	[IsLocked] [bit] NOT NULL,
	[NoofAttempts] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[CountryId] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Top4IncomeSources]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Top4IncomeSources](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Source] [nvarchar](100) NULL,
	[CurrentSpend] [decimal](11, 2) NULL,
	[AverageSpend] [decimal](11, 2) NULL,
	[TenantCode] [nvarchar](50) NULL,
 CONSTRAINT [PK_Top4IncomeSources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[User]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[User](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[ContactNumber] [nvarchar](20) NOT NULL,
	[EmailAddress] [nvarchar](50) NOT NULL,
	[Image] [nvarchar](max) NULL,
	[IsLocked] [bit] NOT NULL,
	[NoofAttempts] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[CountryId] [bigint] NULL,
	[IsInstanceManager] [bit] NOT NULL,
	[IsGroupManager] [bit] NOT NULL,
	[IsPasswordResetByAdmin] [bit] NULL,
 CONSTRAINT [PK__User__3214EC07CA7430B6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[UserCredentialHistory]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[UserCredentialHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdentifier] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[IsSystemGenerated] [bit] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[UserLogin]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[UserLogin](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdentifier] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[LastModifiedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[UserLoginActivityHistory]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[UserLoginActivityHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserIdentifier] [nvarchar](100) NOT NULL,
	[Activity] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[UserRoleMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[UserRoleMap](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[Widget]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Widget](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PageTypeId] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[WidgetName] [nvarchar](100) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[IsConfigurable] [bit] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[LastUpdatedDate] [datetime] NULL,
	[UpdateBy] [bigint] NOT NULL,
	[Instantiable] [bit] NOT NULL,
 CONSTRAINT [PK__Widget__3214EC0775085A22] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [NIS].[WidgetPageTypeMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[WidgetPageTypeMap](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[PageTypeId] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
	[IsDynamicWidget] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [TenantManager].[Tenant]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
--GO
--CREATE TABLE [TenantManager].[Tenant](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[TenantCode] [nvarchar](max) NOT NULL,
--	[TenantName] [nvarchar](max) NOT NULL,
--	[TenantDescription] [nvarchar](max) NULL,
--	[TenantType] [nvarchar](max) NULL,
--	[TenantImage] [nvarchar](max) NULL,
--	[TenantDomainName] [nvarchar](max) NOT NULL,
--	[FirstName] [nvarchar](max) NULL,
--	[LastName] [nvarchar](max) NULL,
--	[ContactNumber] [nvarchar](max) NOT NULL,
--	[EmailAddress] [nvarchar](max) NOT NULL,
--	[SecondaryContactName] [nvarchar](max) NULL,
--	[SecondaryContactNumber] [nvarchar](max) NULL,
--	[SecondaryEmailAddress] [nvarchar](max) NULL,
--	[AddressLine1] [nvarchar](max) NULL,
--	[AddressLine2] [nvarchar](max) NULL,
--	[TenantCity] [nvarchar](max) NULL,
--	[TenantState] [nvarchar](max) NULL,
--	[TenantCountry] [nvarchar](max) NULL,
--	[PinCode] [nvarchar](max) NULL,
--	[StartDate] [date] NULL,
--	[EndDate] [date] NULL,
--	[StorageAccount] [nvarchar](max) NOT NULL,
--	[AccessToken] [nvarchar](max) NOT NULL,
--	[ApplicationURL] [nvarchar](max) NULL,
--	[ApplicationModules] [nvarchar](max) NULL,
--	[BillingEmailAddress] [nvarchar](max) NULL,
--	[SecondaryLastName] [nvarchar](max) NULL,
--	[BillingFirstName] [nvarchar](max) NULL,
--	[BillingLastName] [nvarchar](max) NULL,
--	[BillingContactNumber] [nvarchar](max) NULL,
--	[PanNumber] [nvarchar](max) NULL,
--	[ServiceTax] [nvarchar](max) NULL,
--	[IsPrimaryTenant] [bit] NULL,
--	[ManageType] [nvarchar](max) NULL,
--	[ExternalCode] [nvarchar](max) NULL,
--	[AutheticationMode] [nvarchar](max) NULL,
--	[IsActive] [bit] NOT NULL CONSTRAINT [DF__Tenant__IsActive__6C190EBB]  DEFAULT ((1)),
--	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF__Tenant__IsDelete__6D0D32F4]  DEFAULT ((0)),
--	[ParentTenantCode] [nvarchar](max) NULL,
--	[IsTenantConfigured] [bit] NOT NULL,
-- CONSTRAINT [PK__Tenant__3214EC073F2DE561] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
--)
--GO
/****** Object:  UserDefinedFunction [NIS].[FnGetParentAndChildTenant]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [NIS].[FnGetParentAndChildTenant] (@ParentTenantCode NVARCHAR(50))
RETURNS TABLE
AS
RETURN
(
    SELECT * FROM [TenantManager].[Tenant] WHERE (TenantCode = @ParentTenantCode OR ParentTenantCode = @ParentTenantCode) AND IsActive = 1 AND IsDeleted = 0
)
GO
/****** Object:  UserDefinedFunction [NIS].[FnGetStaticAndDynamicWidgets]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [NIS].[FnGetStaticAndDynamicWidgets] (@PageTypeId BIGINT, @TenantCode VARCHAR(50))
RETURNS TABLE
AS
RETURN
(
 SELECT dw.ID, dw.WidgetName, wpt.PageTypeId, dw.Title AS DisplayName, 0 AS Instantiable, dw.WidgetType 
 FROM  [NIS].[DynamicWidget] dw 
 INNER JOIN [NIS].[WidgetPageTypeMap] wpt ON wpt.WidgetId = dw.Id AND wpt.IsDynamicWidget = 1
 WHERE wpt.PageTypeId = @PageTypeId AND dw.IsActive = 1 AND dw.IsDeleted = 0 AND Status = 'Published' AND dw.TenantCode = @TenantCode
 UNION
 SELECT w.Id, w.WidgetName, @PageTypeId AS PageTypeId, w.DisplayName, w.Instantiable, 'Static' AS WidgetType
 FROM [NIS].[Widget] w
 WHERE w.PageTypeId LIKE '%'+CAST(@PageTypeId AS VARCHAR)+'%' AND w.IsActive = 1 AND w.IsDeleted = 0 AND w.TenantCode = @TenantCode
)
GO
/****** Object:  UserDefinedFunction [NIS].[FnUserTenant]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [NIS].[FnUserTenant] (@UserId INTEGER)
RETURNS TABLE
AS
RETURN
(
    SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
 t.TenantCode, t.TenantName, mtu.OtherTenantAccessRoleId AS RoleId, t.TenantImage, t.TenantType
 FROM [NIS].[MultiTenantUserAccessMap] AS mtu INNER JOIN
 [TenantManager].[Tenant] AS t ON mtu.OtherTenantCode = t.TenantCode INNER JOIN
 [NIS].[User] AS u ON mtu.UserId = u.Id
 WHERE mtu.UserId = @UserId AND mtu.IsActive = 1 AND mtu.IsDeleted = 0
 UNION
 SELECT u.Id AS UserId, u.FirstName+' '+u.LastName as UserName,
 t.TenantCode, t.TenantName, ur.RoleId AS RoleId, t.TenantImage, t.TenantType
 FROM [NIS].[User] AS u INNER JOIN
 [TenantManager].[Tenant] AS t ON u.TenantCode = t.TenantCode INNER JOIN
 [NIS].[UserRoleMap] ur ON u.Id = ur.UserId
 WHERE u.Id = @UserId
)
GO
/****** Object:  View [NIS].[View_DynamicWidget]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_DynamicWidget]
AS

SELECT DISTINCT(s.Id) AS Id, MAX(s.WidgetName) AS WidgetName, MAX(s.WidgetType) AS WidgetType, s.EntityId,
MAX(s.Title) AS Title, MAX(s.ThemeType) AS ThemeType, MAX(s.ThemeCSS) AS ThemeCSS, MAX(s.WidgetSettings) AS WidgetSettings,
MAX(s.WidgetFilterSettings) AS WidgetFilterSettings,MAX(s.FilterCondition) AS FilterCondition, MAX(s.Status) AS Status, MAX(s.CreatedBy) AS CreatedBy, MAX(s.CreatedOn) AS CreatedOn,
s.LastUpdatedBy, s.PublishedBy, MAX(s.PublishedDate) AS PublishedDate, s.IsActive, s.IsDeleted,
MAX(s.TenantCode) AS TenantCode, s.CloneOfWidgetId, MAX(s.Version) As Version, 
MAX(s.PreviewData) AS PreviewData, usr1.FirstName+' '+usr1.LastName AS PublishedByName, usr2.FirstName+' '+usr2.LastName AS CreatedByName,
ent.Name AS EntityName, ent.APIPath, ent.RequestType, wpt.PageTypeId
FROM [NIS].[DynamicWidget] s 
LEFT JOIN [NIS].[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN [NIS].[User] usr2 ON s.CreatedBy = usr2.Id
INNER JOIN [NIS].[TenantEntity] ent ON s.EntityId = ent.Id
INNER JOIN [NIS].[WidgetPageTypeMap] wpt ON wpt.WidgetId = s.Id AND wpt.IsDynamicWidget = 1
LEFT JOIN [NIS].[PageType] pt ON wpt.PageTypeId = pt.id
GROUP BY s.Id, s.EntityId, s.PublishedBy, s.CloneOfWidgetId, s.LastUpdatedBy, usr1.FirstName, usr1.LastName, 
usr2.FirstName, usr2.LastName, ent.Name, ent.APIPath, ent.RequestType, s.IsActive, s.IsDeleted, wpt.PageTypeId
GO

/****** Object:  View [NIS].[View_MultiTenantUserAccessMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_MultiTenantUserAccessMap]
AS
SELECT tum.Id, tum.UserId, usr.FirstName+' '+usr.LastName AS UserName, usr.EmailAddress, 
tum.AssociatedTenantCode, t.TenantName AS AssociatedTenantName, t.TenantType AS AssociatedTenantType,
tum.OtherTenantCode, t1.TenantName AS OtherTenantName, t1.TenantType AS OtherTenantType,
r.Id AS RoleId, r.Name AS RoleName,tum.IsActive, tum.IsDeleted, tum.ParentTenantCode,
tum.LastUpdatedBy, usr1.FirstName+' '+usr1.LastName AS LastUpdatedByUserName, tum.LastUpdatedDate
FROM [NIS].[MultiTenantUserAccessMap] tum 
INNER JOIN [TenantManager].[Tenant] t ON tum.AssociatedTenantCode = t.TenantCode
INNER JOIN [TenantManager].[Tenant] t1 ON tum.OtherTenantCode = t1.TenantCode
INNER JOIN [NIS].[User] usr ON tum.UserId = usr.Id
INNER JOIN [NIS].[User] usr1 ON tum.LastUpdatedBy = usr1.Id
INNER JOIN [NIS].[Role] r ON tum.OtherTenantAccessRoleId = r.Id
GO
/****** Object:  View [NIS].[View_Page]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_Page]
AS
SELECT p.*, pt.Name, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS PageOwnerName, ast.AssetLibraryId AS BackgroundImageAssetLibraryId
FROM NIS.Page p 
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.[User] usr1 ON p.PublishedBy = usr1.Id
LEFT JOIN NIS.[User] usr2 ON p.Owner = usr2.Id
LEFT JOIN NIS.[Asset] ast ON p.BackgroundImageAssetId = ast.Id
GO
/****** Object:  View [NIS].[View_PageWidgetMap]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_PageWidgetMap]
AS

SELECT pw.*, CASE WHEN pw.IsDynamicWidget = 0 THEN w.WidgetName ELSE dw.WidgetName END AS WidgetName
FROM [NIS].[PageWidgetMap] pw 
LEFT JOIN [NIS].[Widget] w ON pw.ReferenceWidgetid = w.id AND pw.IsDynamicWidget = 0
LEFT JOIN [NIS].[DynamicWidget] dw ON pw.ReferenceWidgetid = dw.id AND pw.IsDynamicWidget = 1
GO
/****** Object:  View [NIS].[View_Schedule]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_Schedule]
AS

SELECT sc.*, s.Name As StatementName, (SELECT COUNT(Id) from NIS.BatchMaster where ScheduleId = sc.Id AND IsDataReady = 1 AND IsExecuted=1) AS ExecutedBatchCount
FROM NIS.Schedule sc 
INNER JOIN NIS.Statement s ON sc.StatementId = s.Id
GO
/****** Object:  View [NIS].[View_ScheduleLog]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_ScheduleLog]
AS
SELECT sl.Id, sl.ScheduleId, sl.ScheduleName, sl.BatchId, sl.BatchName, bm.Status AS BatchStatus, sl.NumberOfRetry,
CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)/3600)+' Hr '+ 
CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)%3600/60)+' Min '+ 
CONVERT(VARCHAR(5),(DATEDIFF(s, srh.StartDate, srh.EndDate)%60)) + ' Sec' AS ProcessingTime,
CONVERT(VARCHAR,(SELECT COUNT(Id) FROM NIS.ScheduleLogDetail WHERE ScheduleLogId = sl.Id)) + ' / '+ 
CONVERT(VARCHAR, (SELECT COUNT(Id) FROM NIS.CustomerMaster WHERE BatchId = sl.BatchId)) AS RecordProccessed,
sl.Status, sl.CreationDate AS ExecutionDate, sl.TenantCode
FROM NIS.ScheduleLog sl
INNER JOIN NIS.BatchMaster bm ON sl.BatchId = bm.Id
LEFT JOIN NIS.ScheduleRunHistory srh ON sl.Id = srh.ScheduleLogId
GROUP BY sl.ScheduleId, sl.ScheduleName, sl.BatchId, sl.BatchName, bm.Status, sl.NumberOfRetry, sl.CreationDate, 
sl.Status, srh.StartDate, srh.EndDate, sl.Id, sl.TenantCode
GO
/****** Object:  View [NIS].[View_SourceData]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_SourceData]
AS
SELECT ad.*, p.DisplayName AS PageName, CASE WHEN w.DisplayName IS NULL THEN dw.WidgetType ELSE w.DisplayName END AS WidgetName,
cm.FirstName+' '+cm.MiddleName+' '+cm.LastName AS CustomerName, pt.Name AS PageTypeName
FROM NIS.AnalyticsData ad
INNER JOIN NIS.CustomerMaster cm ON ad.CustomerId = cm.Id
LEFT JOIN NIS.Page p ON ad.PageId = p.Id
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.Widget w ON w.Id = ad.WidgetId
LEFT JOIN NIS.DynamicWidget dw ON dw.Id = ad.WidgetId
GO
/****** Object:  View [NIS].[View_StatementDefinition]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_StatementDefinition]
AS
SELECT s.*, usr1.FirstName+' '+usr1.LastName AS PublishedByName,  usr2.FirstName+' '+usr2.LastName AS OwnerName 
FROM NIS.Statement s 
LEFT JOIN NIS.[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN NIS.[User] usr2 ON s.Owner = usr2.Id
GO
/****** Object:  View [NIS].[View_StatementMetadata]    Script Date: 2021-01-28 10:42:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_StatementMetadata]
AS
SELECT st.Id, st.ScheduleId, st.ScheduleLogId, st.StatementId, st.StatementDate, st.StatementPeriod, st.CustomerId, st.CustomerName, 
st.AccountNumber, st.AccountType, st.StatementURL, st.TenantCode, st.IsPasswordGenerated, st.Password, bm.Id AS BatchId, bm.BatchName
FROM NIS.StatementMetadata st
INNER JOIN NIS.ScheduleLog sl ON st.ScheduleLogId = sl.Id
INNER JOIN NIS.BatchMaster bm ON sl.BatchId = bm.Id
GO
/****** Object:  View [NIS].[View_TenantSubscription]    Script Date: 2021-01-28 10:43:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_TenantSubscription]
AS
SELECT s.TenantCode,s.SubscriptionStartDate,s.SubscriptionEndDate,s.SubscriptionKey,
s.LastModifiedBy,s.LastModifiedOn, usr2.FirstName+' '+usr2.LastName AS LastModifiedName 
FROM NIS.TenantSubscription s 
INNER JOIN NIS.[User] usr2 ON s.LastModifiedBy = usr2.Id
GO
/****** Object:  View [NIS].[View_User]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_User]
AS
SELECT usr.*, r.Name 
FROM NIS.[User] usr 
INNER JOIN NIS.UserRoleMap urm ON usr.Id = urm.UserId 
INNER JOIN NIS.Role r ON urm.RoleId = r.Id
GO
SET IDENTITY_INSERT [ConfigurationManager].[ConfigurationManager] ON 

--INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (1, N'nIS', N'nISConnectionString', N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]')
--INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (2, N'EntityManager', N'EntityManagerConnectionString', N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]')
--INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (3, N'EventManager', N'EventManagerConnectionString', N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]')
--INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (4, N'SubscriptionManager', N'SubscriptionManagerConnectionString', N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]')
--INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (5, N'TemplateManager', N'NotificationEngineConnectionString', N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (6, N'EmailConfiguration', N'EnableSSL', N'false')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (7, N'EmailConfiguration', N'PortNumber', N'587')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (8, N'EmailConfiguration', N'PrimaryFromEmail', N'nis@n4mative.net')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (9, N'EmailConfiguration', N'PrimaryPassword', N'Gauch022')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (10, N'EmailConfiguration', N'SMTPAddress', N'smtp.gmail.com')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (11, N'EmailConfiguration', N'SecondaryFromEmail', N'nis@n4mative.net')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (12, N'EmailConfiguration', N'SecondaryPassword', N'Gauch022')
SET IDENTITY_INSERT [ConfigurationManager].[ConfigurationManager] OFF

SET IDENTITY_INSERT [EntityManager].[Entities] ON 
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (1, N'Dashboard', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Dashboard')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (2, N'User', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'User')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (3, N'Role', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Role')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (4, N'Asset Library', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Asset Library')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (5, N'Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Widget')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (6, N'Page', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Page')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (7, N'Statement Definition', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Statement Definition')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (8, N'Schedule Management', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Schedule Management')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (9, N'Log', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Log')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (10, N'Analytics', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Analytics')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (11, N'Statement Search', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Statement Search')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (12, N'Tenant', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Tenant')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (13, N'Dynamic Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, N'00000000-0000-0000-0000-000000000000', 1, N'Dynamic Widget')
SET IDENTITY_INSERT [EntityManager].[Entities] OFF

SET IDENTITY_INSERT [EntityManager].[Operations] ON 
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (1, N'00000000-0000-0000-0000-000000000000', N'Dashboard', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (2, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (3, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (4, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (5, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (6, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (7, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (8, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (9, N'00000000-0000-0000-0000-000000000000', N'Role', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (10, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (11, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (12, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (13, N'00000000-0000-0000-0000-000000000000', N'Asset Library', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (14, N'00000000-0000-0000-0000-000000000000', N'Widget', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (15, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (16, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (17, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (18, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (19, N'00000000-0000-0000-0000-000000000000', N'Page', N'nIS', N'Publish')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (20, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (21, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (22, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (23, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (24, N'00000000-0000-0000-0000-000000000000', N'Statement Definition', N'nIS', N'Publish')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (25, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (26, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (27, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (28, N'00000000-0000-0000-0000-000000000000', N'Schedule Management', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (29, N'00000000-0000-0000-0000-000000000000', N'Log', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (30, N'00000000-0000-0000-0000-000000000000', N'Analytics', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (31, N'00000000-0000-0000-0000-000000000000', N'Statement Search', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (32, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (33, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (34, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (35, N'00000000-0000-0000-0000-000000000000', N'Tenant', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (36, N'00000000-0000-0000-0000-000000000000', N'User', N'nIS', N'Reset Password')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (37, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (38, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (39, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (40, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (41, N'00000000-0000-0000-0000-000000000000', N'Dynamic Widget', N'nIS', N'Publish')
SET IDENTITY_INSERT [EntityManager].[Operations] OFF

SET IDENTITY_INSERT [NIS].[AssetSetting] ON 
INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (1, CAST(1800.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), N'jpeg,png', CAST(5.00 AS Decimal(18, 2)), N'mp4', N'00000000-0000-0000-0000-000000000000')
SET IDENTITY_INSERT [NIS].[AssetSetting] OFF

SET IDENTITY_INSERT [NIS].[ContactType] ON 
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'Primary', N'Primary contact type', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'Secondary', N'Secondary contact type', 1, 0, N'00000000-0000-0000-0000-000000000000')
SET IDENTITY_INSERT [NIS].[ContactType] OFF

SET IDENTITY_INSERT [NIS].[Country] ON 
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'USA', N'USA', N'+1', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'South Africa', N'South Africa', N'+27', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'Germany', N'Germany', N'+49', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'India', N'IN', N'+91', 1, 0, N'00000000-0000-0000-0000-000000000000')
SET IDENTITY_INSERT [NIS].[Country] OFF

SET IDENTITY_INSERT [NIS].[PageType] ON 
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (1, N'Common', N'Common pages', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (2, N'Home', N'Home pages', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (3, N'Saving Account', N'Saving Account Page Type', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (4, N'Current Account', N'Current Account Page Type', N'00000000-0000-0000-0000-000000000000', 0, 1)
SET IDENTITY_INSERT [NIS].[PageType] OFF

SET IDENTITY_INSERT [NIS].[Role] ON 
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (1, N'Instance Manager', N'Instance Manager', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (2, N'Group Manager', N'Group Manager Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (3, N'Tenant Admin', N'Tenant Admin Role', 0, N'00000000-0000-0000-0000-000000000000')
SET IDENTITY_INSERT [NIS].[Role] OFF

SET IDENTITY_INSERT [NIS].[RolePrivilege] ON 
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, 3, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (2, 3, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (3, 3, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (4, 3, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (5, 3, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, 3, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (7, 3, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, 3, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (9, 3, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (10, 3, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (11, 3, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (12, 3, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (13, 3, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (14, 3, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (15, 3, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (16, 3, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (17, 3, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (18, 3, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (19, 3, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (20, 3, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (21, 3, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (22, 3, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (23, 3, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (24, 3, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (25, 3, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (26, 3, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (27, 3, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (28, 3, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (29, 3, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (30, 3, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (31, 3, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (32, 3, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (33, 3, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (34, 3, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (35, 3, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (36, 3, N'Dynamic Widget', N'Publish', 1)
SET IDENTITY_INSERT [NIS].[RolePrivilege] OFF

SET IDENTITY_INSERT [NIS].[TenantConfiguration] ON 
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (1, N'TEST', N'', N'', N'', N'', N'', N'', N'00000000-0000-0000-0000-000000000000', 0, NULL, N'MM/dd/yyyy', N'Theme1', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman", "HeaderColor":"#5ccc60", "HeaderSize":14, "HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":13,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
SET IDENTITY_INSERT [NIS].[TenantConfiguration] OFF

SET IDENTITY_INSERT [NIS].[User] ON 
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (1, N'NIS', N'SuperAdmin', N'9999999999', N'instancemanager@nis.com', N'', 0, 0, 1, 0, N'00000000-0000-0000-0000-000000000000', 1, 1, 0, 0)
SET IDENTITY_INSERT [NIS].[User] OFF

SET IDENTITY_INSERT [NIS].[UserCredentialHistory] ON 
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (1, N'instancemanager@nis.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, GETDATE(), N'00000000-0000-0000-0000-000000000000')
SET IDENTITY_INSERT [NIS].[UserCredentialHistory] OFF

SET IDENTITY_INSERT [NIS].[UserLogin] ON 
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (1, N'instancemanager@nis.com', N'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())
SET IDENTITY_INSERT [NIS].[UserLogin] OFF

SET IDENTITY_INSERT [NIS].[UserLoginActivityHistory] ON 
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'1', N'LogIn', GETDATE(), 1, 0, N'00000000-0000-0000-0000-000000000000')
SET IDENTITY_INSERT [NIS].[UserLoginActivityHistory] OFF

SET IDENTITY_INSERT [NIS].[UserRoleMap] ON 
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (1, 1, 1)
SET IDENTITY_INSERT [NIS].[UserRoleMap] OFF

--SET IDENTITY_INSERT [TenantManager].[Tenant] ON 

--INSERT [TenantManager].[Tenant] VALUES ( N'00000000-0000-0000-0000-000000000000', N'nIS SuperAdmin', N'', N'Instance', N'', N'default.com', N'Super', N'Admin', N'+91-1234567890', N'instancemanager@nis.com', N'', N'', N'', N'Mumbai', N'', N'1', N'1', N'1', N'123456', CAST(N'2015-12-31' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]', N'', N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'Self', NULL, NULL, 1, 0, NULL, 1)
--INSERT [TenantManager].[Tenant] VALUES ( N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', N'Tenant Group 05102020', N'', N'Group', N'', N'', N'pramod', N'shinde', N'+91-9876567834', N'pramod.shinde45123@gmail.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-10-05' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, NULL, 1)
--INSERT [TenantManager].[Tenant] VALUES ( N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Tenant UK', N'', N'Tenant', N'', N'domain.com', N'tenant', N'UK', N'+44-7867868767', N'tenantuk@demo.com', N'', N'', N'', N'test tenant', N'', N'London', N'London', N'18', N'545342', CAST(N'2020-10-06' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 1)
--INSERT [TenantManager].[Tenant] VALUES ( N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', N'SS_Group', N'Group Created for Testing', N'Group', N'', N'', N'SSGroup', N'manager', N'+91-1254632589', N'ss_group@mailinator.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'', 1)
--INSERT [TenantManager].[Tenant] VALUES ( N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'SS_Websym1', N'', N'Tenant', N'', N'websym1.com', N'ss', N'websym', N'+91-2342342321', N'sswebsym@mailinator.com', N'', N'', N'', N'123', N'', N'PN', N'MH', N'36', N'57', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=[INSTANCENAME];Initial Catalog=[DBNAME];User ID=[USERNAME];Password=[PASSWORD]', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 1)
--SET IDENTITY_INSERT [TenantManager].[Tenant] OFF
USE [master]
GO
ALTER DATABASE [NIS] SET  READ_WRITE 
GO
