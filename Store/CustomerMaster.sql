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
 CONSTRAINT [PK_CustomerMaster] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)