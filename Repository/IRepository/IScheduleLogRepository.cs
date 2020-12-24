// <copyright file="IScheduleLogRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface IScheduleLogRepository
    {
        /// <summary>
        /// This method helps to get list of schedule log.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        IList<ScheduleLog> GetScheduleLogs(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get schedule log count
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Schedule count</returns>
        int GetScheduleLogsCount(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to get list of schedule log detail.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        IList<ScheduleLogDetail> GetScheduleLogDetails(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to get count of schedule log details.
        /// </summary>
        /// <param name="scheduleLogDetailSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        int GetScheduleLogDetailsCount(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to retry to generate html statements for failed customer records
        /// </summary>
        /// <param name="scheduleLogDetails">The schedule log detail object list</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        bool RetryStatementForFailedCustomerReocrds(IList<ScheduleLogDetail> scheduleLogDetails, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, Client client);

        /// <summary>
        /// This method helps to re run the schedule for failed customer records
        /// </summary>
        /// <param name="scheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        bool ReRunScheduleForFailedCases(long scheduleLogIdentifier, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, Client client);

        /// <summary>
        /// This method helps to get error log message of schedule for failed customer records
        /// </summary>
        /// <param name="ScheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>list of schedule log error detail object</returns>
        List<ScheduleLogErrorDetail> GetScheduleLogErrorDetails(long ScheduleLogIdentifier, string tenantCode);
        
        /// <summary>
        /// This method helps to get dashboard data
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>dashboard data object</returns>
        DashboardData GetDashboardData(string tenantCode);

        /// <summary>
        /// This method adds the specified list of schedule log in the repository.
        /// </summary>
        /// <param name="scheduleLogs"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log values are added successfully, false otherwise
        /// </returns>
        bool SaveScheduleLog(IList<ScheduleLog> scheduleLogs, string tenantCode);

        /// <summary>
        /// This method adds the specified list of schedule log detail in the repository.
        /// </summary>
        /// <param name="scheduleLogDetails"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log details values are added successfully, false otherwise
        /// </returns>
        bool SaveScheduleLogDetails(IList<ScheduleLogDetail> scheduleLogDetails, string tenantCode);

        /// <summary>
        /// This method adds the specified list of statement metadata in the repository.
        /// </summary>
        /// <param name="statementMetadata"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the statement metadata values are added successfully, false otherwise
        /// </returns>
        bool SaveStatementMetadata(IList<StatementMetadata> statementMetadata, string tenantCode);

        /// <summary>
        /// This method helps to update schedule log status.
        /// </summary>
        /// <param name="ScheduleLogIdentifier"></param>
        /// <param name="Status"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool UpdateScheduleLogStatus(long ScheduleLogIdentifier, string Status, string tenantCode);
    }
}
