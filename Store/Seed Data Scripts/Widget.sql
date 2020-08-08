DECLARE @SuperTenantCode AS NVARCHAR(MAX) = (select tenantCode from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @SuperTenantIdentifier AS NVARCHAR(MAX) = (select id from TenantManager.Tenant where EmailAddress='nvsuperadmin@nIS.com');
DECLARE @DateTime as DateTime =(SELECT GETDATE());


--1-Home
--2-Saving Account
--3-Current Account

INSERT INTO [NIS].[Widget] (PageTypeId, [Description], WidgetName, DisplayName, IsConfigurable, TenantCode, IsDeleted, IsActive, LastUpdatedDate, UpdateBy, Instantiable)
VALUES
('2,3', 'Customer Information Details', 'CustomerInformation', 'Customer Information', 0, @SuperTenantCode, 0,1, @DateTime, @SuperTenantIdentifier, 0),
('1', 'Account Details', 'AccountInformation','Account Information',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('1', 'Summary at Glance Details', 'Summary','Summary at Glance',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('1,2,3', 'Marketing widget - Configuration for image and click through URL', 'Image','Image',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,1),
('1,2,3','Customer Information - Allowing to upload video','Video','Video',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,1),
('1','Customer Account Analytics Details', 'Analytics','Analytics',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('2','Saving Transaction Details','Saving Transaction','SavingTransaction',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('3', 'Current Transaction Details','Current Transaction','CurrentTransaction',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('2','Customer Saving Trend chart','Saving Trend','SavingTrend',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('2', 'Customer Top 4 Income Sources details', 'Top4IncomeSources', 'Top 4 Income Sources',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('3', 'Current : Available Balance Details', 'Current : Available Balance','CurrentAvailableBalance',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('2', 'Saving : Available Balance Details', 'Saving : Available Balance','SavingAvailableBalance',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('1', 'Reminder and Recommendation details', 'Reminder and Recommendation','ReminderaAndRecommendation',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0),
('3', 'Customer Sprending trend chart', 'Spending Trend','SpendingTrend',0,@SuperTenantCode,0,1,@DateTime,@SuperTenantIdentifier,0);

