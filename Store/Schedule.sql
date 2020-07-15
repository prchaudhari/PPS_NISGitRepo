CREATE TABLE [NIS].[Schedule]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	[Name] NVARCHAR(100) NOT NULL,	
	[Description] NVARCHAR(250) NOT NULL,	
	[DayOfMonth] BIGINT NOT NULL,
	[HourOfDay] BIGINT NOT NULL,
	[MinuteOfDay] BIGINT NOT NULL,
	[StartDate] DateTime  NULL,
	[EndDate] DateTime  NULL,
	[Status] NVARCHAR(50) NOT NULL,	
	[IsActive] BIT NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL,
	[LastUpdatedDate] DateTime  NULL,
	[UpdateBy] BIGINT NOT NULL, 
    [IsExportToPDF] BIT NOT NULL, 
    [StatementId] BIGINT NOT NULL,
)
