CREATE VIEW [NIS].[View_StatementMetadata]
AS 
	SELECT st.Id, st.ScheduleId, st.ScheduleLogId, st.StatementId, st.StatementDate, st.StatementPeriod, st.CustomerId, st.CustomerName, 
	st.AccountNumber, st.AccountType, st.StatementURL, st.TenantCode, st.IsPasswordGenerated, st.Password, bm.Id AS BatchId, bm.BatchName
	FROM NIS.StatementMetadata st
	INNER JOIN NIS.ScheduleLog sl ON st.ScheduleLogId = sl.Id
	INNER JOIN NIS.BatchMaster bm ON sl.BatchId = bm.Id

