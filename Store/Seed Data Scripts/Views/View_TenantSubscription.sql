CREATE VIEW [NIS].[View_TenantSubscription]
AS
SELECT s.TenantCode,s.SubscriptionStartDate,s.SubscriptionEndDate,s.SubscriptionKey,
s.LastModifiedBy,s.LastModifiedOn, usr2.FirstName+' '+usr2.LastName AS LastModifiedName 
FROM NIS.TenantSubscription s 
INNER JOIN NIS.[User] usr2 ON s.LastModifiedBy = usr2.Id
