CREATE TABLE [NIS].[StatementPageMap]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [ReferencePageId] BIGINT NOT NULL, 
    [StatementId] BIGINT NOT NULL, 
    [SequenceNumber] BIGINT NOT NULL, 
    [TenantCode] NVARCHAR(50) NOT NULL
)
