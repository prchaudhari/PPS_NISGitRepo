
/****** Object:  Schema [ConfigurationManager]    Script Date: 19-11-2020 15:12:18 ******/
CREATE SCHEMA [ConfigurationManager]
GO
/****** Object:  Schema [EntityManager]    Script Date: 19-11-2020 15:12:18 ******/
CREATE SCHEMA [EntityManager]
GO
/****** Object:  Schema [NIS]    Script Date: 19-11-2020 15:12:18 ******/
CREATE SCHEMA [NIS]
GO
/****** Object:  Schema [TenantManager]    Script Date: 19-11-2020 15:12:18 ******/
CREATE SCHEMA [TenantManager]
GO
/****** Object:  Table [ConfigurationManager].[ConfigurationManager]    Script Date: 19-11-2020 15:12:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [ConfigurationManager].[ConfigurationManager](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PartionKey] [nvarchar](100) NOT NULL,
	[RowKey] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

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
/****** Object:  Table [NIS].[CustomerInfo]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[CustomerInfo](
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[Lastname] [nvarchar](100) NOT NULL,
	[AddressLine1] [nvarchar](500) NOT NULL,
	[AddressLine2] [nvarchar](500) NULL,
	[City] [bigint] NOT NULL,
	[State] [bigint] NOT NULL,
	[Country] [bigint] NOT NULL,
	[Zip] [nvarchar](500) NULL,
 CONSTRAINT [PK__Customer__3214EC0774BA5E09] PRIMARY KEY CLUSTERED 
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
/****** Object:  Table [NIS].[Image]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Image](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Image] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK__Image__3214EC070AC15F5F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

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
/****** Object:  Table [NIS].[NewsAlert]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[NewsAlert](
	[Id] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Details] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
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
/****** Object:  Table [NIS].[StatementAnalytics]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[StatementAnalytics](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[WidgetName] [nvarchar](200) NOT NULL,
	[PageId] [bigint] NOT NULL,
	[PageName] [nvarchar](500) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Hour] [int] NOT NULL,
	[Minute] [int] NOT NULL,
 CONSTRAINT [PK_StatementAnalytics] PRIMARY KEY CLUSTERED 
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
 CONSTRAINT [PK_StatementMetadata] PRIMARY KEY CLUSTERED 
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
/****** Object:  Table [NIS].[TransactionDetail]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TransactionDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[AccountId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[TransactionType] [nvarchar](50) NOT NULL,
	[AccountType] [nvarchar](50) NOT NULL,
	[Details] [nvarchar](500) NULL,
	[Amount] [decimal](11, 2) NOT NULL,
 CONSTRAINT [PK__Transact__3214EC074B6784C5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_CustomerMaster]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_CustomerMaster](
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
	[TenantCode] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_DataUsage]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_DataUsage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Month] [nvarchar](100) NOT NULL,
	[Year] [bigint] NOT NULL,
	[Microsoft] [bigint] NOT NULL,
	[Zoom] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Data__3214EC072CBB9CDD] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_EmailsBySubscription]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_EmailsBySubscription](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Subscription] [nvarchar](100) NOT NULL,
	[Emails] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Emai__3214EC07043E00AC] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_MeetingUsage]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_MeetingUsage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Month] [nvarchar](100) NOT NULL,
	[Year] [bigint] NOT NULL,
	[Microsoft] [bigint] NOT NULL,
	[Zoom] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Meet__3214EC0726A0BBA3] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_SubscriptionMaster]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_SubscriptionMaster](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[CustomerCode] [nvarchar](100) NULL,
	[VendorName] [nvarchar](100) NOT NULL,
	[Subscription] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Subs__3214EC07341A8EE5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_SubscriptionSpend]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_SubscriptionSpend](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Month] [nvarchar](100) NOT NULL,
	[Year] [bigint] NOT NULL,
	[Microsoft] [decimal](18, 0) NOT NULL,
	[Zoom] [decimal](18, 0) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Subs__3214EC076D64A03C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_SubscriptionSummary]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_SubscriptionSummary](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[VendorName] [nvarchar](250) NOT NULL,
	[Subscription] [nvarchar](250) NOT NULL,
	[Total] [decimal](18, 0) NOT NULL,
	[AverageSpend] [decimal](18, 0) NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Subs__3214EC079DE2FFB0] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_SubscriptionUsage]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_SubscriptionUsage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Subscription] [nvarchar](100) NOT NULL,
	[VendorName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Usage] [decimal](18, 0) NOT NULL,
	[Emails] [bigint] NOT NULL,
	[Meetings] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Subs__3214EC075391A67E] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_UserSubscriptions]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_UserSubscriptions](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[CountOfSubscription] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_User__3214EC07BA09300A] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [NIS].[TTD_VendorSubscription]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[TTD_VendorSubscription](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[VenderName] [nvarchar](100) NOT NULL,
	[CountOfSubscription] [bigint] NOT NULL,
	[TenantCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK__TTD_Vend__3214EC073B18693D] PRIMARY KEY CLUSTERED 
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
/****** Object:  Table [NIS].[Video]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [NIS].[Video](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Video] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK__Video__3214EC07B09B4F6B] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

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
GO
CREATE TABLE [TenantManager].[Tenant](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantCode] [nvarchar](max) NOT NULL,
	[TenantName] [nvarchar](max) NOT NULL,
	[TenantDescription] [nvarchar](max) NULL,
	[TenantType] [nvarchar](max) NULL,
	[TenantImage] [nvarchar](max) NULL,
	[TenantDomainName] [nvarchar](max) NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[ContactNumber] [nvarchar](max) NOT NULL,
	[EmailAddress] [nvarchar](max) NOT NULL,
	[SecondaryContactName] [nvarchar](max) NULL,
	[SecondaryContactNumber] [nvarchar](max) NULL,
	[SecondaryEmailAddress] [nvarchar](max) NULL,
	[AddressLine1] [nvarchar](max) NULL,
	[AddressLine2] [nvarchar](max) NULL,
	[TenantCity] [nvarchar](max) NULL,
	[TenantState] [nvarchar](max) NULL,
	[TenantCountry] [nvarchar](max) NULL,
	[PinCode] [nvarchar](max) NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[StorageAccount] [nvarchar](max) NOT NULL,
	[AccessToken] [nvarchar](max) NOT NULL,
	[ApplicationURL] [nvarchar](max) NULL,
	[ApplicationModules] [nvarchar](max) NULL,
	[BillingEmailAddress] [nvarchar](max) NULL,
	[SecondaryLastName] [nvarchar](max) NULL,
	[BillingFirstName] [nvarchar](max) NULL,
	[BillingLastName] [nvarchar](max) NULL,
	[BillingContactNumber] [nvarchar](max) NULL,
	[PanNumber] [nvarchar](max) NULL,
	[ServiceTax] [nvarchar](max) NULL,
	[IsPrimaryTenant] [bit] NULL,
	[ManageType] [nvarchar](max) NULL,
	[ExternalCode] [nvarchar](max) NULL,
	[AutheticationMode] [nvarchar](max) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF__Tenant__IsActive__6C190EBB]  DEFAULT ((1)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF__Tenant__IsDelete__6D0D32F4]  DEFAULT ((0)),
	[ParentTenantCode] [nvarchar](max) NULL,
	[IsTenantConfigured] [bit] NOT NULL,
 CONSTRAINT [PK__Tenant__3214EC073F2DE561] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
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
MAX(s.WidgetFilterSettings) AS WidgetFilterSettings, MAX(s.Status) AS Status, MAX(s.CreatedBy) AS CreatedBy, MAX(s.CreatedOn) AS CreatedOn,
s.LastUpdatedBy, s.PublishedBy,MAX(s.PublishedDate) AS PublishedDate, s.IsActive, s.IsDeleted,
MAX(s.TenantCode) AS TenantCode, s.CloneOfWidgetId, MAX(s.Version) As Version, 
MAX(s.PreviewData) AS PreviewData,usr1.FirstName+' '+usr1.LastName AS PublishedByName, usr2.FirstName+' '+usr2.LastName AS CreatedByName,
ent.Name as EntityName, ent.APIPath, ent.RequestType, (SELECT TOP 1 PageTypeId From [NIS].[WidgetPageTypeMap] WHERE WidgetId = s.Id) AS PageTypeId
FROM NIS.DynamicWidget s 
LEFT JOIN NIS.[User] usr1 ON s.PublishedBy = usr1.Id
INNER JOIN NIS.[User] usr2 ON s.CreatedBy = usr2.Id
INNER JOIN NIS.TenantEntity ent ON s.EntityId = ent.Id
INNER JOIN [NIS].[WidgetPageTypeMap] wpt ON wpt.WidgetId = s.Id
WHERE wpt.IsDynamicWidget = 1
GROUP BY s.Id, s.EntityId, s.PublishedBy, s.CloneOfWidgetId, s.LastUpdatedBy,
usr1.FirstName, usr1.LastName, usr2.FirstName, usr2.LastName, ent.Name, ent.APIPath, ent.RequestType, s.IsActive, s.IsDeleted
GO
/****** Object:  View [NIS].[View_DynamicWidget1]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_DynamicWidget1]
AS

SELECT DISTINCT(s.Id) AS Id, MAX(s.WidgetName) AS WidgetName, MAX(s.WidgetType) AS WidgetType, s.EntityId,
MAX(s.Title) AS Title, MAX(s.ThemeType) AS ThemeType, MAX(s.ThemeCSS) AS ThemeCSS, MAX(s.WidgetSettings) AS WidgetSettings,
MAX(s.WidgetFilterSettings) AS WidgetFilterSettings, MAX(s.Status) AS Status, MAX(s.CreatedBy) AS CreatedBy, MAX(s.CreatedOn) AS CreatedOn,
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
SELECT sl.Id,sl.ScheduleName,CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)/3600)+' Hr '+
CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)%3600/60)+' Min '+
CONVERT(VARCHAR(5),(DATEDIFF(s, srh.StartDate, srh.EndDate)%60)) + ' Sec' AS ProcessingTime,
CONVERT(VARCHAR,(SELECT COUNT(Id) FROM nis.ScheduleLogDetail WHERE Status = 'Completed' AND ScheduleLogId = sl.Id))+ ' / '+CONVERT(VARCHAR, COUNT(sld.Id)) AS RecordProccessed,
sl.Status, sl.CreationDate AS ExecutionDate, sl.TenantCode
FROM nis.ScheduleLog sl
LEFT JOIN nis.ScheduleLogDetail sld ON sl.Id = sld.ScheduleLogId
LEFT JOIN nis.ScheduleRunHistory srh ON sl.Id = srh.ScheduleLogId
GROUP BY sl.ScheduleName, sl.CreationDate, sl.Status, srh.StartDate, srh.EndDate, sl.Id, sl.TenantCode
GO
/****** Object:  View [NIS].[View_SourceData]    Script Date: 19-11-2020 15:12:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [NIS].[View_SourceData]
AS
SELECT ad.*, p.DisplayName AS PageName, w.DisplayName AS WidgetName, cm.FirstName+' '+cm.MiddleName+' '+cm.LastName AS CustomerName, pt.Name AS PageTypeName 
FROM NIS.AnalyticsData ad 
INNER JOIN NIS.CustomerMaster cm ON ad.CustomerId = cm.Id
LEFT JOIN NIS.Page p ON ad.PageId = p.Id
LEFT JOIN NIS.PageType pt ON p.PageTypeId = pt.Id
LEFT JOIN NIS.Widget w ON w.Id = ad.WidgetId
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

INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (1, N'nIS', N'nISConnectionString', N'{{DataBaseConnectionString}}')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (2, N'EntityManager', N'EntityManagerConnectionString', N'{{DataBaseConnectionString}}')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (3, N'EventManager', N'EventManagerConnectionString', N'{{DataBaseConnectionString}}')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (4, N'SubscriptionManager', N'SubscriptionManagerConnectionString', N'{{DataBaseConnectionString}}')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (5, N'TemplateManager', N'NotificationEngineConnectionString', N'{{DataBaseConnectionString}}')
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
SET IDENTITY_INSERT [NIS].[Asset] ON 

INSERT [NIS].[Asset] ([Id], [Name], [FilePath], [AssetLibraryId], [IsDeleted], [LastUpdatedDate], [LastUpdatedBy]) VALUES (1, N'image-link.png', N'\\WSPL_LAP_012\NISAssets/assets/2/image-link.png', 2, 0, CAST(N'2020-11-19 08:15:20.763' AS DateTime), 5)
INSERT [NIS].[Asset] ([Id], [Name], [FilePath], [AssetLibraryId], [IsDeleted], [LastUpdatedDate], [LastUpdatedBy]) VALUES (2, N'COPY (1).png', N'\\WSPL_LAP_012\NISAssets/assets/2/COPY (1).png', 2, 0, CAST(N'2020-11-19 08:17:13.390' AS DateTime), 5)
INSERT [NIS].[Asset] ([Id], [Name], [FilePath], [AssetLibraryId], [IsDeleted], [LastUpdatedDate], [LastUpdatedBy]) VALUES (3, N'default-user.png', N'\\WSPL_LAP_012\NISAssets/assets/1/default-user.png', 1, 0, CAST(N'2020-11-19 08:26:16.717' AS DateTime), 3)
INSERT [NIS].[Asset] ([Id], [Name], [FilePath], [AssetLibraryId], [IsDeleted], [LastUpdatedDate], [LastUpdatedBy]) VALUES (4, N'testvideo.mp4', N'\\WSPL_LAP_012\NISAssets/assets/1/testvideo.mp4', 1, 0, CAST(N'2020-11-19 08:34:47.913' AS DateTime), 3)
SET IDENTITY_INSERT [NIS].[Asset] OFF
SET IDENTITY_INSERT [NIS].[AssetLibrary] ON 

INSERT [NIS].[AssetLibrary] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'Test Asse tLibrary', NULL, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[AssetLibrary] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'Asset Library Test', NULL, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[AssetLibrary] OFF
SET IDENTITY_INSERT [NIS].[AssetSetting] ON 

INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (1, CAST(1800.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), N'jpeg,png', CAST(5.00 AS Decimal(18, 2)), N'mp4', N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (2, CAST(1800.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), N'png,jpeg', CAST(5.00 AS Decimal(18, 2)), N'mp4', N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (3, CAST(2000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), N'png,jpeg', CAST(15.00 AS Decimal(18, 2)), N'mp4', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[AssetSetting] OFF
SET IDENTITY_INSERT [NIS].[BatchMaster] ON 

INSERT [NIS].[BatchMaster] ([Id], [TenantCode], [BatchName], [CreatedBy], [CreatedDate], [ScheduleId], [IsExecuted], [IsDataReady], [DataExtractionDate], [BatchExecutionDate], [Status]) VALUES (1, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Batch 1 of Tes Schedule', 3, CAST(N'2020-11-19 07:22:24.507' AS DateTime), 1, 0, 0, CAST(N'2020-11-20 14:30:00.000' AS DateTime), CAST(N'2020-11-21 14:30:00.000' AS DateTime), N'New')
SET IDENTITY_INSERT [NIS].[BatchMaster] OFF
SET IDENTITY_INSERT [NIS].[ContactType] ON 

INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'Primary', N'Teat', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'Secondary', N'Test', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'Primary', N'Teat', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'Secondary', N'Test', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'Primary', N'Teat', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (6, N'Secondary', N'Test', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
SET IDENTITY_INSERT [NIS].[ContactType] OFF
SET IDENTITY_INSERT [NIS].[Country] ON 

INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'India', N'IN', N'+91', 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'India', N'IN', N'+91', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'India', N'IN', N'+91', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'India', N'IN', N'+91', 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'India', N'IN', N'+91', 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[Country] OFF
SET IDENTITY_INSERT [NIS].[DynamicWidget] ON 

INSERT [NIS].[DynamicWidget] ([Id], [WidgetName], [WidgetType], [PageTypeId], [EntityId], [Title], [ThemeType], [ThemeCSS], [WidgetSettings], [WidgetFilterSettings], [Status], [CreatedBy], [CreatedOn], [LastUpdatedBy], [PublishedBy], [PublishedDate], [IsActive], [IsDeleted], [TenantCode], [CloneOfWidgetId], [Version], [PreviewData]) VALUES (1, N'Saving Account Details', N'Form', 0, 2, N'Saving Account Details', N'Default', N'', N'[{"DisplayName":"Account Number","FieldId":"6","FieldName":"AccountNumber"},{"DisplayName":"Balance","FieldId":"8","FieldName":"Balance"},{"DisplayName":"Deposite","FieldId":"9","FieldName":"TotalDeposit"}]', N'', N'Published', 3, CAST(N'2020-11-19 06:59:11.260' AS DateTime), 3, 3, CAST(N'2020-11-19 07:13:06.367' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', NULL, N'1', N'<div class=''row''><div class=''col-sm-6''><label>Account Number</label></div><div class=''col-sm-6''>AccountNumber1</div></div><div class=''row''><div class=''col-sm-6''><label>Balance</label></div><div class=''col-sm-6''>Balance1</div></div><div class=''row''><div class=''col-sm-6''><label>Deposite</label></div><div class=''col-sm-6''>TotalDeposit1</div></div>')
INSERT [NIS].[DynamicWidget] ([Id], [WidgetName], [WidgetType], [PageTypeId], [EntityId], [Title], [ThemeType], [ThemeCSS], [WidgetSettings], [WidgetFilterSettings], [Status], [CreatedBy], [CreatedOn], [LastUpdatedBy], [PublishedBy], [PublishedDate], [IsActive], [IsDeleted], [TenantCode], [CloneOfWidgetId], [Version], [PreviewData]) VALUES (2, N'Current Acc Transaction Details', N'Table', 0, 3, N'Current Acc Transaction Details', N'Custome', N'{"TitleColor":"#d7adad","TitleSize":22,"TitleWeight":"Normal","TitleType":"Sans-serif","HeaderColor":"#5372d0","HeaderSize":18,"HeaderWeight":"Normal","HeaderType":"Serif","DataColor":"#4fc994","DataSize":20,"DataWeight":"Italic","DataType":"Sans-serif","ChartColorTheme":""}', N'[{"HeaderName":"Date","FieldId":"11","FieldName":"TransactionDate","IsSorting":false},{"HeaderName":"Narration","FieldId":"13","FieldName":"Narration","IsSorting":false},{"HeaderName":"Fcy","FieldId":"15","FieldName":"FCY","IsSorting":false},{"HeaderName":"Lcy","FieldId":"16","FieldName":"LCY","IsSorting":false}]', N'', N'Published', 3, CAST(N'2020-11-19 07:01:37.807' AS DateTime), 3, 3, CAST(N'2020-11-19 07:13:00.557' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', NULL, N'1', N'<tr><td>TransactionDate1</td> <td>Narration1</td> <td>FCY1</td> <td>LCY1</td> </tr><tr><td>TransactionDate2</td> <td>Narration2</td> <td>FCY2</td> <td>LCY2</td> </tr><tr><td>TransactionDate3</td> <td>Narration3</td> <td>FCY3</td> <td>LCY3</td> </tr><tr><td>TransactionDate4</td> <td>Narration4</td> <td>FCY4</td> <td>LCY4</td> </tr>')
INSERT [NIS].[DynamicWidget] ([Id], [WidgetName], [WidgetType], [PageTypeId], [EntityId], [Title], [ThemeType], [ThemeCSS], [WidgetSettings], [WidgetFilterSettings], [Status], [CreatedBy], [CreatedOn], [LastUpdatedBy], [PublishedBy], [PublishedDate], [IsActive], [IsDeleted], [TenantCode], [CloneOfWidgetId], [Version], [PreviewData]) VALUES (3, N'TestHTMLWidget', N'Html', 0, 1, N'Test', N'Default', N'', NULL, N'', N'New', 3, CAST(N'2020-11-19 08:36:02.983' AS DateTime), 3, NULL, NULL, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', NULL, N'1', N'<p><img src="http://localhost:8082/assets/1/default-user.png" class="e-rte-image e-imginline e-resize" style=""> </p><p><br></p><p><video src="http://localhost:8082/assets/1/testvideo.mp4" controls=""></video></p>')
SET IDENTITY_INSERT [NIS].[DynamicWidget] OFF
SET IDENTITY_INSERT [NIS].[EntityFieldMap] ON 

INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (1, N'CutomerCode', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (2, N'FirstName', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (3, N'LastName', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (4, N'RMName', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (5, N'RMContactNo', 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (6, N'AccountNumber', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (7, N'AccountType', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (8, N'Balance', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (9, N'TotalDeposit', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (10, N'TotalSpend', 2, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (11, N'TransactionDate', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'DateTime')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (12, N'TransactionType', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (13, N'Narration', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (14, N'AccountType', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (15, N'FCY', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (16, N'LCY', 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (17, N'Month', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (18, N'SpendAmount', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (19, N'SpendPercentage', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (20, N'Income', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (21, N'IncomePercentage', 4, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (22, N'VendorName', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (23, N'Subscription', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (24, N'EmployeeID', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (25, N'EmployeeName', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (26, N'Email', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (27, N'StartDate', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'DateTime')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (28, N'EndDate', 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'DateTime')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (29, N'Vendor', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (30, N'Subscription', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (31, N'Total', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (32, N'AverageSpend', 6, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (33, N'Month', 7, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (34, N'Microsoft', 7, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (35, N'Zoom', 7, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (36, N'UserName', 8, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (37, N'CountOfSubscription', 8, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (38, N'VenderName', 9, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (39, N'CountOfSubscription', 9, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (40, N'EmployeeID', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (41, N'Subscription', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (42, N'VendorName', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (43, N'EmployeeName', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (44, N'Email', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (45, N'Usage', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (46, N'Emails', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (47, N'Meetings', 10, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (48, N'Month', 11, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (49, N'Microsoft', 11, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (50, N'Zoom', 11, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (51, N'Month', 12, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (52, N'Microsoft', 12, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (53, N'Zoom', 12, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'Decimal')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (54, N'Subscription', 13, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'String')
INSERT [NIS].[EntityFieldMap] ([Id], [Name], [EntityId], [IsActive], [IsDeleted], [TenantCode], [DataType]) VALUES (55, N'Emails', 13, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'BigInt')
SET IDENTITY_INSERT [NIS].[EntityFieldMap] OFF
SET IDENTITY_INSERT [NIS].[Page] ON 

INSERT [NIS].[Page] ([Id], [DisplayName], [PageTypeId], [PublishedBy], [Owner], [Version], [Status], [CreatedDate], [PublishedOn], [IsActive], [IsDeleted], [TenantCode], [LastUpdatedDate], [UpdateBy], [BackgroundImageAssetId], [BackgroundImageURL]) VALUES (1, N'Saving Account', 5, 3, 3, N'1', N'Published', CAST(N'2020-11-19 12:50:25.403' AS DateTime), CAST(N'2020-11-19 07:21:11.593' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', CAST(N'2020-11-19 12:50:25.403' AS DateTime), NULL, 0, NULL)
SET IDENTITY_INSERT [NIS].[Page] OFF
SET IDENTITY_INSERT [NIS].[PageType] ON 

INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (1, N'Home', N'Home pages', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (2, N'Saving Account', N'Saving Account Page Type', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (3, N'Current Account', N'Current Account Page Type', N'00000000-0000-0000-0000-000000000000', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (4, N'Home', N'Home pages', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (5, N'Saving Account', N'Saving Account Page Type', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (6, N'Current Account', N'Current Account Page Type', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (7, N'Usage', N'Home pages', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (8, N'Billing', N'Saving Account Page Type', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 0, 1)
SET IDENTITY_INSERT [NIS].[PageType] OFF
SET IDENTITY_INSERT [NIS].[PageWidgetMap] ON 

INSERT [NIS].[PageWidgetMap] ([Id], [ReferenceWidgetId], [Height], [Width], [Xposition], [Yposition], [PageId], [WidgetSetting], [TenantCode], [IsDynamicWidget]) VALUES (1, 1, 4, 3, 0, 0, 1, N'', N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[PageWidgetMap] ([Id], [ReferenceWidgetId], [Height], [Width], [Xposition], [Yposition], [PageId], [WidgetSetting], [TenantCode], [IsDynamicWidget]) VALUES (2, 2, 4, 6, 3, 0, 1, N'', N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[PageWidgetMap] ([Id], [ReferenceWidgetId], [Height], [Width], [Xposition], [Yposition], [PageId], [WidgetSetting], [TenantCode], [IsDynamicWidget]) VALUES (3, 4, 4, 3, 9, 0, 1, N'{"AssetLibraryId":0,"AssetLibrayName":"","AssetId":0,"AssetName":"","SourceUrl":"","isPersonalize":true,"WidgetId":4}', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
SET IDENTITY_INSERT [NIS].[PageWidgetMap] OFF
SET IDENTITY_INSERT [NIS].[Role] ON 

INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (1, N'Super Admin', N'Super Admin Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (2, N'Tenant Admin', N'Tenant Admin Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (3, N'Group Manager', N'Group Manager Role', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (4, N'Instance Manager', N'Instance Manager', 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (5, N'Group Manager', N'Group Manager Role', 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (6, N'Tenant Admin', N'Tenant Admin Role', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (7, N'Group Manager', N'Group Manager Role', 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (8, N'Tenant Admin', N'Tenant Admin Role', 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[Role] OFF
SET IDENTITY_INSERT [NIS].[RolePrivilege] ON 

INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, 1, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (2, 1, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (3, 1, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (4, 1, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (5, 1, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, 1, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (7, 1, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, 1, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (9, 1, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (10, 1, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (11, 1, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (12, 1, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (13, 1, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (14, 1, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (15, 1, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (16, 1, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (17, 1, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (18, 1, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (19, 1, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (20, 1, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (21, 1, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (22, 1, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (23, 1, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (24, 1, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (25, 1, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (26, 1, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (27, 1, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (28, 1, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (29, 1, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (30, 1, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (31, 1, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (32, 1, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (33, 1, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (34, 1, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (35, 1, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (36, 1, N'Dynamic Widget', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (37, 6, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (38, 6, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (39, 6, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (40, 6, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (41, 6, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (42, 6, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (43, 6, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (44, 6, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (45, 6, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (46, 6, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (47, 6, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (48, 6, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (49, 6, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (50, 6, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (51, 6, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (52, 6, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (53, 6, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (54, 6, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (55, 6, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (56, 6, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (57, 6, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (58, 6, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (59, 6, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (60, 6, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (61, 6, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (62, 6, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (63, 6, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (64, 6, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (65, 6, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (66, 6, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (67, 6, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (68, 6, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (69, 6, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (70, 6, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (71, 6, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (72, 6, N'Dynamic Widget', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (73, 8, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (74, 8, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (75, 8, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (76, 8, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (77, 8, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (78, 8, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (79, 8, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (80, 8, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (81, 8, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (82, 8, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (83, 8, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (84, 8, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (85, 8, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (86, 8, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (87, 8, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (88, 8, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (89, 8, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (90, 8, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (91, 8, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (92, 8, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (93, 8, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (94, 8, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (95, 8, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (96, 8, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (97, 8, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (98, 8, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (99, 8, N'Schedule Management', N'Delete', 1)
GO
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (100, 8, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (101, 8, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (102, 8, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (103, 8, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (104, 8, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (105, 8, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (106, 8, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (107, 8, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([Id], [RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (108, 8, N'Dynamic Widget', N'Publish', 1)
SET IDENTITY_INSERT [NIS].[RolePrivilege] OFF
SET IDENTITY_INSERT [NIS].[Schedule] ON 

INSERT [NIS].[Schedule] ([Id], [Name], [StatementId], [Description], [DayOfMonth], [HourOfDay], [MinuteOfDay], [StartDate], [EndDate], [Status], [IsActive], [IsDeleted], [TenantCode], [LastUpdatedDate], [UpdateBy], [IsExportToPDF], [RecurrancePattern], [RepeatEveryDayMonWeekYear], [WeekDays], [IsEveryWeekDay], [MonthOfYear], [IsEndsAfterNoOfOccurrences], [NoOfOccurrences]) VALUES (1, N'Tes Schedule', 1, N'', 0, 14, 30, CAST(N'2020-11-20 18:30:00.000' AS DateTime), NULL, N'New', 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', CAST(N'2020-11-19 07:22:23.837' AS DateTime), 3, 0, N'DoesNotRepeat', NULL, NULL, 0, NULL, NULL, NULL)
SET IDENTITY_INSERT [NIS].[Schedule] OFF
SET IDENTITY_INSERT [NIS].[Statement] ON 

INSERT [NIS].[Statement] ([Id], [Name], [Description], [PublishedBy], [Owner], [Version], [Status], [CreatedDate], [PublishedOn], [IsActive], [IsDeleted], [TenantCode], [LastUpdatedDate], [UpdateBy]) VALUES (1, N'Test SD', N'', 3, 3, N'1', N'Published', CAST(N'2020-11-19 12:51:35.690' AS DateTime), CAST(N'2020-11-19 07:21:40.427' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', CAST(N'2020-11-19 12:51:35.690' AS DateTime), 0)
SET IDENTITY_INSERT [NIS].[Statement] OFF
SET IDENTITY_INSERT [NIS].[StatementPageMap] ON 

INSERT [NIS].[StatementPageMap] ([Id], [ReferencePageId], [StatementId], [SequenceNumber], [TenantCode]) VALUES (1, 1, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
SET IDENTITY_INSERT [NIS].[StatementPageMap] OFF
SET IDENTITY_INSERT [NIS].[TenantConfiguration] ON 

INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (1, N'TEST', N'', N'', N'', N'', N'', N'', N'00000000-0000-0000-0000-000000000000', 0, NULL, N'MM/dd/yyyy', N'Theme1', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman","HeaderColor":"#5ccc60","HeaderSize":14,"HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":12,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (2, N'TEST', N'', N'', N'', N'', N'', N'\\WSPL_LAP_012\NISAssets', N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, NULL, N'MM/dd/yyyy', N'Theme0', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman","HeaderColor":"#5ccc60","HeaderSize":14,"HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":12,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (3, N'TEST', N'', N'', N'', N'', N'', N'\\WSPL_LAP_012\NISAssets', N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 0, NULL, N'MM/dd/yyyy', N'Theme0', N'{"ColorTheme":"charttheme3","TitleColor":"#6c90e5","TitleSize":16,"TitleWeight":"Bold","TitleType":"Times Roman","HeaderColor":"#5ccc60","HeaderSize":14,"HeaderWeight":"Bold","HeaderType":"Tahoma","DataColor":"#a04040","DataSize":12,"DataWeight":"Normal","DataType":"Serif"}', N'http://localhost/API/')
SET IDENTITY_INSERT [NIS].[TenantConfiguration] OFF
SET IDENTITY_INSERT [NIS].[TenantEntity] ON 

INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (1, N'Customer Information', N'', CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_CustomerMasters', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (2, N'Account Balalnce', N'', CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_AccountMaster', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (3, N'Account Transaction', N'', CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, CAST(N'2020-10-14 06:37:42.297' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_AccountTransaction', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (4, N'Saving Trend', N'', CAST(N'2020-11-09 11:30:14.500' AS DateTime), 5, CAST(N'2020-11-09 11:30:14.500' AS DateTime), 3, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'TenantTransactionData/Get_SavingTrend', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (5, N'Subscription Master', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionMasters', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (6, N'Subscription Summary', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionSummaries', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (7, N'Subscription Spend', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionSpends', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (8, N'User Subscriptions', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_UserSubscriptions', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (9, N'Vendor Subscription', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_VendorSubscriptions', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (10, N'Subscription Usage', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_SubscriptionUsages', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (11, N'Data Usage', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_DataUsages', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (12, N'Meeting Usage', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_MeetingUsages', N'POST')
INSERT [NIS].[TenantEntity] ([Id], [Name], [Description], [CreatedOn], [CreatedBy], [LastUpdatedOn], [LastUpdatedBy], [IsActive], [IsDeleted], [TenantCode], [APIPath], [RequestType]) VALUES (13, N'Emails By Subscription', N'', CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, CAST(N'2020-02-11 06:37:00.000' AS DateTime), 5, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'TenantTransactionData/Get_TTD_EmailsBySubscription', N'POST')
SET IDENTITY_INSERT [NIS].[TenantEntity] OFF
SET IDENTITY_INSERT [NIS].[User] ON 

INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (1, N'NIS', N'SuperAdmin', N'7878322333', N'instancemanager@nis.com', N'', 0, 0, 1, 0, N'00000000-0000-0000-0000-000000000000', 1, 1, 0, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (2, N'Tenant', N'UK Group', N'7878322334', N'pramod.shinde45123@gmail.com', N'', 0, 0, 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 2, 0, 1, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (3, N'UK', N'Tenant Admin', N'7878322335', N'tenantuk@demo.com', N'', 0, 0, 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 3, 0, 0, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (4, N'SS', N'Tenant Group', N'7878322336', N'ss_group@mailinator.com', N'', 0, 0, 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 4, 0, 1, 0)
INSERT [NIS].[User] ([Id], [FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (5, N'SS', N'Websym', N'7878322337', N'sswebsym@mailinator.com', N'', 0, 0, 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', 5, 0, 0, 0)
SET IDENTITY_INSERT [NIS].[User] OFF
SET IDENTITY_INSERT [NIS].[UserCredentialHistory] ON 

INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (1, N'instancemanager@nis.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-07-26 06:33:32.657' AS DateTime), N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (2, N'pramod.shinde45123@gmail.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:45:54.327' AS DateTime), N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (3, N'tenantuk@demo.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:51:03.803' AS DateTime), N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (4, N'ss_group@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:45:54.327' AS DateTime), N'b22f7d5e-3b49-46fd-9c8c-18e87c901320')
INSERT [NIS].[UserCredentialHistory] ([Id], [UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (5, N'sswebsym@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:51:03.803' AS DateTime), N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
SET IDENTITY_INSERT [NIS].[UserCredentialHistory] OFF
SET IDENTITY_INSERT [NIS].[UserLogin] ON 

INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (1, N'instancemanager@nis.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.420' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (2, N'pramod.shinde45123@gmail.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.423' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (3, N'tenantuk@demo.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.427' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (4, N'ss_group@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.427' AS DateTime))
INSERT [NIS].[UserLogin] ([Id], [UserIdentifier], [Password], [LastModifiedOn]) VALUES (5, N'sswebsym@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', CAST(N'2020-11-18 23:35:01.427' AS DateTime))
SET IDENTITY_INSERT [NIS].[UserLogin] OFF
SET IDENTITY_INSERT [NIS].[UserLoginActivityHistory] ON 

INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'1', N'LogIn', CAST(N'2020-11-18 18:05:30.793' AS DateTime), 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'3', N'LogIn', CAST(N'2020-11-18 18:05:56.023' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'3', N'LogIn', CAST(N'2020-11-18 18:06:08.997' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'2', N'LogIn', CAST(N'2020-11-18 18:06:21.860' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'2', N'LogIn', CAST(N'2020-11-19 03:20:34.687' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (6, N'1', N'LogIn', CAST(N'2020-11-19 03:21:00.660' AS DateTime), 1, 0, N'00000000-0000-0000-0000-000000000000')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (7, N'3', N'LogIn', CAST(N'2020-11-19 03:21:17.187' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (8, N'3', N'LogIn', CAST(N'2020-11-19 03:29:16.973' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (9, N'3', N'LogIn', CAST(N'2020-11-19 03:29:31.673' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (10, N'3', N'LogIn', CAST(N'2020-11-19 03:38:37.990' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (11, N'3', N'LogIn', CAST(N'2020-11-19 03:38:59.030' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (12, N'3', N'LogIn', CAST(N'2020-11-19 03:43:06.073' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (13, N'3', N'LogIn', CAST(N'2020-11-19 03:51:12.813' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (14, N'3', N'LogIn', CAST(N'2020-11-19 03:53:46.347' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (15, N'3', N'LogIn', CAST(N'2020-11-19 03:54:37.443' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (16, N'2', N'LogIn', CAST(N'2020-11-19 03:57:18.447' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (17, N'3', N'LogIn', CAST(N'2020-11-19 03:57:52.113' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (18, N'2', N'LogIn', CAST(N'2020-11-19 04:03:08.810' AS DateTime), 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (19, N'5', N'LogIn', CAST(N'2020-11-19 04:06:21.347' AS DateTime), 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (20, N'3', N'LogIn', CAST(N'2020-11-19 04:13:01.550' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (21, N'3', N'LogIn', CAST(N'2020-11-19 04:56:07.857' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (22, N'3', N'LogIn', CAST(N'2020-11-19 06:43:29.293' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (23, N'3', N'LogIn', CAST(N'2020-11-19 06:44:18.343' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (24, N'3', N'LogIn', CAST(N'2020-11-19 07:13:24.863' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (25, N'3', N'LogIn', CAST(N'2020-11-19 07:13:39.497' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (26, N'5', N'LogIn', CAST(N'2020-11-19 07:28:40.550' AS DateTime), 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (27, N'3', N'LogIn', CAST(N'2020-11-19 08:03:46.217' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (28, N'5', N'LogIn', CAST(N'2020-11-19 08:14:15.343' AS DateTime), 1, 0, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550')
INSERT [NIS].[UserLoginActivityHistory] ([Id], [UserIdentifier], [Activity], [CreatedAt], [IsActive], [IsDeleted], [TenantCode]) VALUES (29, N'3', N'LogIn', CAST(N'2020-11-19 08:20:18.667' AS DateTime), 1, 0, N'fd51e101-35e5-49b4-ac29-1224d278e430')
SET IDENTITY_INSERT [NIS].[UserLoginActivityHistory] OFF
SET IDENTITY_INSERT [NIS].[UserRoleMap] ON 

INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (1, 1, 1)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (2, 3, 6)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (3, 5, 8)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (4, 2, 3)
INSERT [NIS].[UserRoleMap] ([Id], [UserId], [RoleId]) VALUES (5, 4, 3)
SET IDENTITY_INSERT [NIS].[UserRoleMap] OFF
SET IDENTITY_INSERT [NIS].[Widget] ON 

INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (1, N'4', N'Customer Information Details', N'CustomerInformation', N'Customer Information', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (2, N'4', N'Account Details', N'AccountInformation', N'Account Information', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (3, N'4', N'Summary at Glance Details', N'Summary', N'Summary at Glance', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (4, N'4,5,6', N'Marketing widget - Configuration for image and click through URL', N'Image', N'Image', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 1)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (5, N'4,4,6', N'Customer Information - Allowing to upload video', N'Video', N'Video', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 1)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (6, N'4', N'Customer Account Analytics Details', N'Analytics', N'Analytics', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (7, N'4', N'Saving Transaction Details', N'SavingTransaction', N'Saving Transaction', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (8, N'6', N'Current Transaction Details', N'CurrentTransaction', N'Current Transaction', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (9, N'4', N'Customer Saving Trend chart', N'SavingTrend', N'Saving Trend', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (10, N'4', N'Customer Top 4 Income Sources details', N'Top4IncomeSources', N'Top 4 Income Sources', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (11, N'6', N'Current : Available Balance Details', N'CurrentAvailableBalance', N'Current : Available Balance', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (12, N'4', N'Saving : Available Balance Details', N'SavingAvailableBalance', N'Saving : Available Balance', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (13, N'4', N'Reminder and Recommendation details', N'ReminderaAndRecommendation', N'Reminder & Recommendation', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
INSERT [NIS].[Widget] ([Id], [PageTypeId], [Description], [WidgetName], [DisplayName], [IsConfigurable], [TenantCode], [IsDeleted], [IsActive], [LastUpdatedDate], [UpdateBy], [Instantiable]) VALUES (14, N'6', N'Customer Sprending trend chart', N'SpendingTrend', N'Spending Trend', 0, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0, 1, CAST(N'2020-10-27 10:42:23.213' AS DateTime), 1, 0)
SET IDENTITY_INSERT [NIS].[Widget] OFF
SET IDENTITY_INSERT [NIS].[WidgetPageTypeMap] ON 

INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (1, 1, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (2, 2, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (3, 3, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (4, 4, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (5, 4, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (6, 4, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (7, 5, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (8, 5, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (9, 5, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (10, 6, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (11, 7, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (12, 8, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (13, 9, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (14, 10, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (15, 11, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (16, 12, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (17, 13, 3, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (18, 14, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 0)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (19, 1, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (20, 2, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (21, 2, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (22, 3, 4, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (23, 3, 5, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
INSERT [NIS].[WidgetPageTypeMap] ([Id], [WidgetId], [PageTypeId], [TenantCode], [IsDynamicWidget]) VALUES (24, 3, 6, N'fd51e101-35e5-49b4-ac29-1224d278e430', 1)
SET IDENTITY_INSERT [NIS].[WidgetPageTypeMap] OFF
SET IDENTITY_INSERT [TenantManager].[Tenant] ON 

INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (1, N'00000000-0000-0000-0000-000000000000', N'nIS SuperAdmin', N'', N'Instance', N'', N'default.com', N'Super', N'Admin', N'+91-1234567890', N'instancemanager@nis.com', N'', N'', N'', N'Mumbai', N'', N'1', N'1', N'1', N'123456', CAST(N'2015-12-31' AS Date), CAST(N'9999-12-31' AS Date), N'{{DataBaseConnectionString}}', N'', N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'Self', NULL, NULL, 1, 0, NULL, 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (2, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', N'Tenant Group 05102020', N'', N'Group', N'', N'', N'pramod', N'shinde', N'+91-9876567834', N'pramod.shinde45123@gmail.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-10-05' AS Date), CAST(N'9999-12-31' AS Date), N'{{DataBaseConnectionString}}', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, NULL, 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (3, N'fd51e101-35e5-49b4-ac29-1224d278e430', N'Tenant UK', N'', N'Tenant', N'', N'domain.com', N'tenant', N'UK', N'+44-7867868767', N'tenantuk@demo.com', N'', N'', N'', N'test tenant', N'', N'London', N'London', N'18', N'545342', CAST(N'2020-10-06' AS Date), CAST(N'9999-12-31' AS Date), N'{{DataBaseConnectionString}}', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (4, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', N'SS_Group', N'Group Created for Testing', N'Group', N'', N'', N'SSGroup', N'manager', N'+91-1254632589', N'ss_group@mailinator.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'{{DataBaseConnectionString}}', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'', 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (5, N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550', N'SS_Websym1', N'', N'Tenant', N'', N'websym1.com', N'ss', N'websym', N'+91-2342342321', N'sswebsym@mailinator.com', N'', N'', N'', N'123', N'', N'PN', N'MH', N'36', N'57', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'{{DataBaseConnectionString}}', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 1)
SET IDENTITY_INSERT [TenantManager].[Tenant] OFF
USE [master]
GO
ALTER DATABASE [NIS] SET  READ_WRITE 
GO
