CREATE VIEW [NIS].[View_ScheduleLog]
AS
SELECT sl.Id, sl.ScheduleId, sl.ScheduleName, sl.BatchId, sl.BatchName, bm.Status AS BatchStatus, sl.NumberOfRetry,
CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)/3600)+' Hr '+ 
CONVERT(VARCHAR(5),DATEDIFF(s, srh.StartDate, srh.EndDate)%3600/60)+' Min '+ 
CONVERT(VARCHAR(5),(DATEDIFF(s, srh.StartDate, srh.EndDate)%60)) + ' Sec' AS ProcessingTime,
CONVERT(VARCHAR,(SELECT COUNT(Id) FROM NIS.ScheduleLogDetail WHERE ScheduleLogId = sl.Id)) + ' / '+ 
CONVERT(VARCHAR, (SELECT COUNT(Id) FROM NIS.CustomerMaster WHERE BatchId = sl.BatchId)) AS RecordProccessed,
sl.Status, sl.CreationDate AS ExecutionDate, sl.TenantCode
FROM NIS.ScheduleLog sl
INNER JOIN NIS.BatchMaster bm ON sl.BatchId = bm.Id
LEFT JOIN NIS.ScheduleRunHistory srh ON sl.Id = srh.ScheduleLogId
GROUP BY sl.ScheduleId, sl.ScheduleName, sl.BatchId, sl.BatchName, bm.Status, sl.NumberOfRetry, sl.CreationDate, sl.Status, srh.StartDate, srh.EndDate, sl.Id, sl.TenantCode