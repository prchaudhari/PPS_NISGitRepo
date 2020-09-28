CREATE TABLE [NIS].[TenantUser]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FirstName] NVARCHAR(100) NOT NULL,
	[LastName] NVARCHAR(100) NOT NULL,
	[ContactNumber] NVARCHAR(20) NOT NULL,	
	[EmailAddress] NVARCHAR(50),	
	[Image] NVARCHAR(MAX),
	[IsLocked] BIT NOT NULL,
	[NoofAttempts] INT NOT NULL,
	[IsActive] BIT NOT NULL,
	[IsDeleted] BIT NOT NULL,
	[TenantCode] NVARCHAR(50) NOT NULL,
	[CountryId] [bigint] NULL,
)