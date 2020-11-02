CREATE TABLE [NIS].[TTD_SubscriptionSummary]
(
	Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Vendor	NVARCHAR(250) NOT NULL,	
	Subscription NVARCHAR(250) NOT NULL,	
	Total Decimal NOT NULL,
	AverageSpend Decimal NOT NULL
)
