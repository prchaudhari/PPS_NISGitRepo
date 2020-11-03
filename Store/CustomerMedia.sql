CREATE TABLE [NIS].[CustomerMedia](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[BatchId] [bigint] NOT NULL,
	[StatementId] [bigint] NOT NULL,
	[PageId] [bigint] NOT NULL,
	[WidgetId] [bigint] NOT NULL,
	[ImageURL] [nvarchar](max) NULL,
	[VideoURL] [nvarchar](max) NULL,
	[TenantCode] NVARCHAR(50) NOT NULL, 
 CONSTRAINT [PK_CustomerMedia] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
