DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @SuperTenantIdentifier AS NVARCHAR(MAX) = (select id from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @DateTime as DateTime =(SELECT GETDATE());


--1-Home
--2-Saving Account
--3-Current Account

INSERT INTO [NIS].[Widget]([DisplayName],[WidgetName],[Description],[PageTypeId],[IsConfigurable],
[TenantCode],[IsDeleted],[IsActive],[LastUpdatedDate],[UpdateBy],[Instantiable])
VALUES
('Customer Information','CustomerInformation','','2,3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Account Information','AccountInformation','Account details','1',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Summary at Glance','Summary','','1',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Image','Image','Marketing widget - Configuration for image and click through URL ','1,2,3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Video','Video','Customer Information - Allowing to upload video ','1,2,3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),

('Analytics','Analytics','','1',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Saving Transaction','SavingTransaction','','2',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Current Transaction','CurrentTransaction','','3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Saving Trend','SavingTrend','','2',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Top 4 Income Sources','Top4IncomeSources','','2',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Current : Available Balance','CurrentAvailableBalance','','3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Saving : Available Balance','SavingAvailableBalance','','2',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Reminder and Recommendation','ReminderaAndRecommendation','','1',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('Spending Trend','SpendingTrend','','3',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0);

