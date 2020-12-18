CREATE TABLE [NIS].[TenantSubscription]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [TenantCode] UNIQUEIDENTIFIER NOT NULL, 
    [SubscriptionStartDate] DATETIME NOT NULL,
    [SubscriptionEndDate] DATETIME NOT NULL,
    [LastModifiedBy] BIGINT NOT NULL, 
    [LastModifiedOn] DATETIME NOT NULL,
    [SubscriptionKey]  NVARCHAR(100) NOT NULL
)
