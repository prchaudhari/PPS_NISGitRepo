CREATE TABLE [NIS].[TenantSecurityCodeFormat]
(
	 [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [TenantCode] NVARCHAR(50) NOT NULL, 
    [Format] NVARCHAR(500) NOT NULL, 
    [LastModifiedBy] BIGINT NOT NULL, 
    [LastModifiedOn]  DATETIME NOT NULL,
)
