CREATE TABLE [NIS].[CustomerInfo]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(100) NOT NULL, 
    [MiddleName] NVARCHAR(100) NOT NULL, 
    [Lastname] NVARCHAR(100) NOT NULL, 
    [AddressLine1] NVARCHAR(500) NULL, 
    [AddressLine2] NVARCHAR(500) NOT NULL, 
    [City] BIGINT NOT NULL, 
    [State] BIGINT NOT NULL, 
    [Country] BIGINT NOT NULL, 
    [Zip] NCHAR(10) NULL
)
