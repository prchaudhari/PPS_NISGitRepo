CREATE VIEW [NIS].[View_Schedule]
AS

SELECT sc.*, s.Name As StatementName, (SELECT COUNT(Id) from NIS.BatchMaster where ScheduleId = sc.Id AND IsDataReady = 1 AND IsExecuted=1) AS ExecutedBatchCount
FROM NIS.Schedule sc 
INNER JOIN NIS.Statement s ON sc.StatementId = s.Id