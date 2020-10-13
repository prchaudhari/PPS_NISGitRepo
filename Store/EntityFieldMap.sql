CREATE TABLE [NIS].[EntityFieldMap]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(100) NOT NULL,
    [EntityId]BIGINT NOT NULL,
	[IsActive] BIT NOT NULL,
	[IsDeleted] BIT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL, 
   [DataType] [nvarchar](50) NOT NULL,
)
