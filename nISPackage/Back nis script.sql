
/****** Object:  Schema [ConfigurationManager]    Script Date: 18-11-2020 12:21:36 ******/
CREATE SCHEMA [ConfigurationManager]
GO
/****** Object:  Schema [EntityManager]    Script Date: 18-11-2020 12:21:37 ******/
CREATE SCHEMA [EntityManager]
GO
/****** Object:  Schema [NIS]    Script Date: 18-11-2020 12:21:37 ******/
CREATE SCHEMA [NIS]
GO
/****** Object:  Schema [TenantManager]    Script Date: 18-11-2020 12:21:38 ******/
CREATE SCHEMA [TenantManager]
GO
/****** Object:  Table [ConfigurationManager].[ConfigurationManager]    Script Date: 18-11-2020 12:21:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
GO
/****** Object:  Table [NIS].[AccountMaster] Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[AccountTransaction]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[CustomerInfo]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[CustomerMaster]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[CustomerMedia]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
/****** Object:  Table [NIS].[Image]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
GO
/****** Object:  Table [NIS].[NewsAlert]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[ReminderAndRecommendation]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[SavingTrend]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Top4IncomeSources]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TransactionDetail]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_CustomerMaster]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_DataUsage]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_EmailsBySubscription]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_MeetingUsage]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_SubscriptionMaster]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_SubscriptionSpend]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_SubscriptionSummary]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_SubscriptionUsage]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_UserSubscriptions]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TTD_VendorSubscription]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Video]    Script Date: 18-11-2020 12:36:10 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO



CREATE TABLE [ConfigurationManager].[ConfigurationManager](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PartionKey] [nvarchar](100) NOT NULL,
	[RowKey] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [EntityManager].[DependentOperations]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [EntityManager].[Entities]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [EntityManager].[Operations]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[AnalyticsData]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Asset]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[AssetLibrary]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[AssetPathSetting]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[AssetSetting]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[BatchDetails]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[BatchMaster]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
SET ANSI_PADDING ON
GO
/****** Object:  Table [NIS].[City]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[ContactType]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Country]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[DynamicWidget]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[DynamicWidgetFilterDetail]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[EntityFieldMap]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO

/****** Object:  Table [NIS].[MultiTenantUserAccessMap]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Page]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[PageType]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[PageWidgetMap]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[RenderEngine]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Role]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[RolePrivilege]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Schedule]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[ScheduleLog]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[ScheduleLogDetail]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[ScheduleRunHistory]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[State]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Statement]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[StatementAnalytics]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[StatementMetadata]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[StatementPageMap]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TenantConfiguration]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TenantContact]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TenantEntity]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[TenantUser]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[User]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[UserCredentialHistory]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[UserLogin]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[UserLoginActivityHistory]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[UserRoleMap]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[Widget]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [NIS].[WidgetPageTypeMap]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [TenantManager].[Tenant]    Script Date: 18-11-2020 12:21:38 ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  UserDefinedFunction [NIS].[FnGetParentAndChildTenant]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  UserDefinedFunction [NIS].[FnGetStaticAndDynamicWidgets]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  UserDefinedFunction [NIS].[FnUserTenant]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_DynamicWidget]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_DynamicWidget1]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_MultiTenantUserAccessMap]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_Page]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_PageWidgetMap]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_Schedule]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_ScheduleLog]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_SourceData]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_StatementDefinition]    Script Date: 18-11-2020 12:21:38 ******/
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
/****** Object:  View [NIS].[View_User]    Script Date: 18-11-2020 12:21:38 ******/
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

Declare @SuperTenantIdentifier AS NVARCHAR(MAX) = N'00000000-0000-0000-0000-000000000000';
Declare @FinancialTenantGroupIdentifier AS NVARCHAR(MAX) = N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289';
Declare @FinancialTenantIdentifier AS NVARCHAR(MAX) = N'fd51e101-35e5-49b4-ac29-1224d278e430';
Declare @SubscriptionTenantGroupIdentifier AS NVARCHAR(MAX) = N'b22f7d5e-3b49-46fd-9c8c-18e87c901320';
Declare @SubscriptionTenantIdentifier AS NVARCHAR(MAX) = N'fc9366f8-cd57-4dbc-886d-94f4e8cb1550';

SET IDENTITY_INSERT [ConfigurationManager].[ConfigurationManager] ON 

INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (1, N'nIS', N'nISConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (2, N'EntityManager', N'EntityManagerConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (3, N'EventManager', N'EventManagerConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (4, N'SubscriptionManager', N'SubscriptionManagerConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (5, N'TemplateManager', N'NotificationEngineConnectionString', N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (6, N'EmailConfiguration', N'EnableSSL', N'false')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (7, N'EmailConfiguration', N'PortNumber', N'587')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (8, N'EmailConfiguration', N'PrimaryFromEmail', N'nis@n4mative.net')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (9, N'EmailConfiguration', N'PrimaryPassword', N'Gauch022')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (10, N'EmailConfiguration', N'SMTPAddress', N'smtp.gmail.com')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (11, N'EmailConfiguration', N'SecondaryFromEmail', N'nis@n4mative.net')
INSERT [ConfigurationManager].[ConfigurationManager] ([Id], [PartionKey], [RowKey], [Value]) VALUES (12, N'EmailConfiguration', N'SecondaryPassword', N'Gauch022')
SET IDENTITY_INSERT [ConfigurationManager].[ConfigurationManager] OFF
SET IDENTITY_INSERT [EntityManager].[Entities] ON 

INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (1, N'Dashboard', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Dashboard')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (2, N'User', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'User')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (3, N'Role', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Role')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (4, N'Asset Library', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Asset Library')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (5, N'Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Widget')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (6, N'Page', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Page')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (7, N'Statement Definition', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Statement Definition')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (8, N'Schedule Management', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Schedule Management')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (9, N'Log', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Log')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (10, N'Analytics', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Analytics')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (11, N'Statement Search', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Statement Search')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (12, N'Tenant', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Tenant')
INSERT [EntityManager].[Entities] ([Id], [EntityName], [Keys], [AssemblyName], [NamespaceName], [Operations], [ComponentCode], [IsActive], [IsImportEnabled], [ServiceURL], [TenantCode], [IsDefaultEntity], [DisplayName]) VALUES (13, N'Dynamic Widget', NULL, NULL, NULL, NULL, N'nIS', 1, 0, NULL, @SuperTenantIdentifier, 1, N'Dynamic Widget')
SET IDENTITY_INSERT [EntityManager].[Entities] OFF
SET IDENTITY_INSERT [EntityManager].[Operations] ON 

INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (1, @SuperTenantIdentifier, N'Dashboard', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (2, @SuperTenantIdentifier, N'User', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (3, @SuperTenantIdentifier, N'User', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (4, @SuperTenantIdentifier, N'User', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (5, @SuperTenantIdentifier, N'User', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (6, @SuperTenantIdentifier, N'Role', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (7, @SuperTenantIdentifier, N'Role', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (8, @SuperTenantIdentifier, N'Role', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (9, @SuperTenantIdentifier, N'Role', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (10, @SuperTenantIdentifier, N'Asset Library', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (11, @SuperTenantIdentifier, N'Asset Library', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (12, @SuperTenantIdentifier, N'Asset Library', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (13, @SuperTenantIdentifier, N'Asset Library', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (14, @SuperTenantIdentifier, N'Widget', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (15, @SuperTenantIdentifier, N'Page', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (16, @SuperTenantIdentifier, N'Page', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (17, @SuperTenantIdentifier, N'Page', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (18, @SuperTenantIdentifier, N'Page', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (19, @SuperTenantIdentifier, N'Page', N'nIS', N'Publish')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (20, @SuperTenantIdentifier, N'Statement Definition', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (21, @SuperTenantIdentifier, N'Statement Definition', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (22, @SuperTenantIdentifier, N'Statement Definition', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (23, @SuperTenantIdentifier, N'Statement Definition', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (24, @SuperTenantIdentifier, N'Statement Definition', N'nIS', N'Publish')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (25, @SuperTenantIdentifier, N'Schedule Management', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (26, @SuperTenantIdentifier, N'Schedule Management', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (27, @SuperTenantIdentifier, N'Schedule Management', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (28, @SuperTenantIdentifier, N'Schedule Management', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (29, @SuperTenantIdentifier, N'Log', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (30, @SuperTenantIdentifier, N'Analytics', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (31, @SuperTenantIdentifier, N'Statement Search', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (32, @SuperTenantIdentifier, N'Tenant', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (33, @SuperTenantIdentifier, N'Tenant', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (34, @SuperTenantIdentifier, N'Tenant', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (35, @SuperTenantIdentifier, N'Tenant', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (36, @SuperTenantIdentifier, N'User', N'nIS', N'Reset Password')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (37, @SuperTenantIdentifier, N'Dynamic Widget', N'nIS', N'Create')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (38, @SuperTenantIdentifier, N'Dynamic Widget', N'nIS', N'Edit')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (39, @SuperTenantIdentifier, N'Dynamic Widget', N'nIS', N'Delete')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (40, @SuperTenantIdentifier, N'Dynamic Widget', N'nIS', N'View')
INSERT [EntityManager].[Operations] ([Id], [TenantCode], [EntityName], [ComponentCode], [Operation]) VALUES (41, @SuperTenantIdentifier, N'Dynamic Widget', N'nIS', N'Publish')
SET IDENTITY_INSERT [EntityManager].[Operations] OFF

SET IDENTITY_INSERT [TenantManager].[Tenant] ON 
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (1, @SuperTenantIdentifier, N'nIS SuperAdmin', N'', N'Instance', N'', N'default.com', N'Super', N'Admin', N'+91-1234567890', N'instancemanager@nis.com', N'', N'', N'', N'Mumbai', N'', N'1', N'1', N'1', N'123456', CAST(N'2015-12-31' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123', N'', N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'Self', NULL, NULL, 1, 0, NULL, 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (2, @FinancialTenantGroupIdentifier, N'Tenant Group 05102020', N'', N'Group', N'', N'', N'pramod', N'shinde', N'+91-9876567834', N'pramod.shinde45123@gmail.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-10-05' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, NULL, 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (3, @FinancialTenantIdentifier, N'Tenant UK', N'', N'Tenant', N'', N'domain.com', N'tenant', N'UK', N'+44-7867868767', N'tenantuk@demo.com', N'', N'', N'', N'test tenant', N'', N'London', N'London', N'18', N'545342', CAST(N'2020-10-06' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'6553f11e-4dc5-4450-afa7-3c5b1fa2d289', 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (4, @SubscriptionTenantGroupIdentifier, N'SS_Group', N'Group Created for Testing', N'Group', N'', N'', N'SSGroup', N'manager', N'+91-1254632589', N'ss_group@mailinator.com', N'', N'', N'', N'', N'', N'', N'', N'', N'', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'', 1)
INSERT [TenantManager].[Tenant] ([Id], [TenantCode], [TenantName], [TenantDescription], [TenantType], [TenantImage], [TenantDomainName], [FirstName], [LastName], [ContactNumber], [EmailAddress], [SecondaryContactName], [SecondaryContactNumber], [SecondaryEmailAddress], [AddressLine1], [AddressLine2], [TenantCity], [TenantState], [TenantCountry], [PinCode], [StartDate], [EndDate], [StorageAccount], [AccessToken], [ApplicationURL], [ApplicationModules], [BillingEmailAddress], [SecondaryLastName], [BillingFirstName], [BillingLastName], [BillingContactNumber], [PanNumber], [ServiceTax], [IsPrimaryTenant], [ManageType], [ExternalCode], [AutheticationMode], [IsActive], [IsDeleted], [ParentTenantCode], [IsTenantConfigured]) VALUES (5, @SubscriptionTenantIdentifier, N'SS_Websym1', N'', N'Tenant', N'', N'websym1.com', N'ss', N'websym', N'+91-2342342321', N'sswebsym@mailinator.com', N'', N'', N'', N'123', N'', N'PN', N'MH', N'36', N'57', CAST(N'2020-11-02' AS Date), CAST(N'9999-12-31' AS Date), N'Data Source=WSPL_LAP_012;Initial Catalog=NIS;User ID=sa;Password=Admin@123', N'', N'', N'', N'', N'', N'', N'', N'', N'', N'', 0, N'Self', N'', N'', 1, 0, N'b22f7d5e-3b49-46fd-9c8c-18e87c901320', 1)
SET IDENTITY_INSERT [TenantManager].[Tenant] OFF

SET IDENTITY_INSERT [NIS].[AssetSetting] ON 
INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (1, CAST(1800.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), N'jpeg,png', CAST(5.00 AS Decimal(18, 2)),   N'mp4',@SuperTenantIdentifier)
INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (2, CAST(1800.00 AS Decimal(18, 2)), CAST(1200.00 AS Decimal(18, 2)), CAST(1.00 AS Decimal(18, 2)), N'png,jpeg', CAST(5.00 AS Decimal(18, 2)),   N'mp4',@FinancialTenantIdentifier)
INSERT [NIS].[AssetSetting] ([Id], [ImageHeight], [ImageWidth], [ImageSize], [ImageFileExtension], [VideoSize], [VideoFileExtension], [TenantCode]) VALUES (3, CAST(2000.00 AS Decimal(18, 2)), CAST(2000.00 AS Decimal(18, 2)), CAST(10.00 AS Decimal(18, 2)), N'png,jpeg', CAST(15.00 AS Decimal(18, 2)), N'mp4',@SubscriptionTenantIdentifier)
SET IDENTITY_INSERT [NIS].[AssetSetting] OFF

SET IDENTITY_INSERT [NIS].[ContactType] ON 
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'Primary', N'Teat', 1, 0, @SuperTenantIdentifier)
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'Secondary', N'Test', 1, 0, @SuperTenantIdentifier)
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'Primary', N'Teat', 1, 0, @FinancialTenantGroupIdentifier)
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'Secondary', N'Test', 1, 0, @FinancialTenantGroupIdentifier)
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'Primary', N'Teat', 1, 0, @SubscriptionTenantGroupIdentifier)
INSERT [NIS].[ContactType] ([Id], [Name], [Description], [IsActive], [IsDeleted], [TenantCode]) VALUES (6, N'Secondary', N'Test', 1, 0, @SubscriptionTenantGroupIdentifier)
SET IDENTITY_INSERT [NIS].[ContactType] OFF

SET IDENTITY_INSERT [NIS].[Country] ON 
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (1, N'India', N'IN', N'+91', 1, 0, @SuperTenantIdentifier)
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (2, N'India', N'IN', N'+91', 1, 0, @FinancialTenantGroupIdentifier)
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (3, N'India', N'IN', N'+91', 1, 0, @SubscriptionTenantGroupIdentifier)
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (4, N'India', N'IN', N'+91', 1, 0, @FinancialTenantIdentifier)
INSERT [NIS].[Country] ([Id], [Name], [Code], [DialingCode], [IsActive], [IsDeleted], [TenantCode]) VALUES (5, N'India', N'IN', N'+91', 1, 0, @SubscriptionTenantIdentifier)
SET IDENTITY_INSERT [NIS].[Country] OFF

SET IDENTITY_INSERT [NIS].[PageType] ON 
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (1, N'Home', N'Home pages', @SuperTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (2, N'Saving Account', N'Saving Account Page Type', @SuperTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (3, N'Current Account', N'Current Account Page Type', @SuperTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (4, N'Home', N'Home pages', @FinancialTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (5, N'Saving Account', N'Saving Account Page Type', @FinancialTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (6, N'Current Account', N'Current Account Page Type', @FinancialTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (7, N'Usage', N'Usage page type', @SubscriptionTenantIdentifier, 0, 1)
INSERT [NIS].[PageType] ([Id], [Name], [Description], [TenantCode], [IsDeleted], [IsActive]) VALUES (8, N'Billing', N'Billing Page Type', @SubscriptionTenantIdentifier, 0, 1)

SET IDENTITY_INSERT [NIS].[PageType] OFF

SET IDENTITY_INSERT [NIS].[Role] ON 
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (1, N'Super Admin', N'Super Admin Role', 0, @SuperTenantIdentifier)
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (2, N'Tenant Admin', N'Tenant Admin Role', 0, @SuperTenantIdentifier)
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (3, N'Group Manager', N'Group Manager Role', 0, @SuperTenantIdentifier)
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (4, N'Instance Manager', N'Instance Manager', 0, @SuperTenantIdentifier)
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (5, N'Group Manager', N'Group Manager Role', 0, @FinancialTenantGroupIdentifier) 
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (6, N'Tenant Admin', N'Tenant Admin Role', 0, @FinancialTenantIdentifier)
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (7, N'Group Manager', N'Group Manager Role', 0, @SubscriptionTenantGroupIdentifier) 
INSERT [NIS].[Role] ([Id], [Name], [Description], [IsDeleted], [TenantCode]) VALUES (8, N'Tenant Admin', N'Tenant Admin Role', 0, @SubscriptionTenantIdentifier)
SET IDENTITY_INSERT [NIS].[Role] OFF 

INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (1, N'Dynamic Widget', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (6, N'Dynamic Widget', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Dashboard', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'User', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'User', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'User', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'User', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Role', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Role', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Role', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Role', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Asset Library', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Asset Library', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Asset Library', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Asset Library', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Page', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Page', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Page', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Page', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Page', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Statement Definition', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Statement Definition', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Statement Definition', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Statement Definition', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Statement Definition', N'Publish', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Schedule Management', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Schedule Management', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Schedule Management', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Schedule Management', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Log', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Analytics', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Statement Search', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Dynamic Widget', N'Create', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Dynamic Widget', N'Edit', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Dynamic Widget', N'Delete', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Dynamic Widget', N'View', 1)
INSERT [NIS].[RolePrivilege] ([RoleIdentifier], [EntityName], [Operation], [IsEnable]) VALUES (8, N'Dynamic Widget', N'Publish', 1)


SET IDENTITY_INSERT [NIS].[TenantConfiguration] ON 
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (1, N'TEST', N'', N'', N'', N'', N'', N'', @SuperTenantIdentifier, 0, NULL, N'MM/dd/yyyy', NULL, NULL, N'http://localhost/API/')
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (2, N'TEST', N'', N'', N'', N'', N'', N'', @FinancialTenantIdentifier, 0, NULL, N'MM/dd/yyyy', NULL, NULL, N'http://localhost/API/')
INSERT [NIS].[TenantConfiguration] ([Id], [Name], [Description], [InputDataSourcePath], [OutputHTMLPath], [OutputPDFPath], [ArchivalPath], [AssetPath], [TenantCode], [ArchivalPeriod], [ArchivalPeriodUnit], [DateFormat], [ApplicationTheme], [WidgetThemeSetting], [BaseUrlForTransactionData]) VALUES (3, N'TEST', N'', N'', N'', N'', N'', N'', @SubscriptionTenantIdentifier, 0, NULL, N'MM/dd/yyyy', NULL, NULL, N'http://localhost/API/')
SET IDENTITY_INSERT [NIS].[TenantConfiguration] OFF

INSERT [NIS].[User] ([FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (N'NIS', N'SuperAdmin', N'7878322333', N'instancemanager@nis.com', N'', 0, 0, 1, 0, @SuperTenantIdentifier, 1, 1, 0, 0)
INSERT [NIS].[User] ([FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (N'Tenant', N'UK Group', N'7878322334', N'pramod.shinde45123@gmail.com', N'', 0, 0, 1, 0, @FinancialTenantGroupIdentifier, 2, 0, 1, 0)
INSERT [NIS].[User] ([FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (N'UK', N'Tenant Admin', N'7878322335', N'tenantuk@demo.com', N'', 0, 0, 1, 0, @FinancialTenantIdentifier, 3, 0, 0, 0)
INSERT [NIS].[User] ([FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (N'SS', N'Tenant Group', N'7878322336', N'ss_group@mailinator.com', N'', 0, 0, 1, 0, @SubscriptionTenantGroupIdentifier, 4, 0, 1, 0)
INSERT [NIS].[User] ([FirstName], [LastName], [ContactNumber], [EmailAddress], [Image], [IsLocked], [NoofAttempts], [IsActive], [IsDeleted], [TenantCode], [CountryId], [IsInstanceManager], [IsGroupManager], [IsPasswordResetByAdmin]) VALUES (N'SS', N'Websym', N'7878322337', N'sswebsym@mailinator.com', N'', 0, 0, 1, 0, @SubscriptionTenantIdentifier, 5, 0, 0, 0)

INSERT [NIS].[UserCredentialHistory] ([UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (N'instancemanager@nis.com', N'BhRR3ldjH5b5Olk3QA/NJg==', 1, CAST(N'2020-07-26 06:33:32.657' AS DateTime), @SuperTenantIdentifier)
INSERT [NIS].[UserCredentialHistory] ([UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (N'pramod.shinde45123@gmail.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:45:54.327' AS DateTime), @FinancialTenantGroupIdentifier)
INSERT [NIS].[UserCredentialHistory] ([UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (N'tenantuk@demo.com', N'WeF+NbyoPcWG8hyio5qShg==', 1, CAST(N'2020-11-02 07:51:03.803' AS DateTime), @FinancialTenantIdentifier)
INSERT [NIS].[UserCredentialHistory] ([UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (N'ss_group@mailinator.com', N'DnUXFB+1PyHuQ5s1W1odCg==', 0, CAST(N'2020-11-02 07:45:54.327' AS DateTime),@SubscriptionTenantGroupIdentifier)
INSERT [NIS].[UserCredentialHistory] ([UserIdentifier], [Password], [IsSystemGenerated], [CreatedAt], [TenantCode]) VALUES (N'sswebsym@mailinator.com', N'WeF+NbyoPcWG8hyio5qShg==', 1, CAST(N'2020-11-02 07:51:03.803' AS DateTime), @SubscriptionTenantIdentifier)

INSERT [NIS].[UserRoleMap] ([UserId], [RoleId]) VALUES (1, 1)
INSERT [NIS].[UserRoleMap] ([UserId], [RoleId]) VALUES (3, 6)
INSERT [NIS].[UserRoleMap] ([UserId], [RoleId]) VALUES (5, 8)

INSERT INTO [NIS].[UserLogin] VALUES (N'instancemanager@nis.com', 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())
INSERT INTO [NIS].[UserLogin] VALUES (N'pramod.shinde45123@gmail.com', 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())
INSERT INTO [NIS].[UserLogin] VALUES (N'tenantuk@demo.com', 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())
INSERT INTO [NIS].[UserLogin] VALUES (N'ss_group@mailinator.com' 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())
INSERT INTO [NIS].[UserLogin] VALUES (N'sswebsym@mailinator.com', 'DnUXFB+1PyHuQ5s1W1odCg==', GETDATE())