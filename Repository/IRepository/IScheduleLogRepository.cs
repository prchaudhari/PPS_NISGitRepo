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
        bool RetryStatementForFailedCustomerReocrds(IList<ScheduleLogDetail> scheduleLogDetails, string baseURL, string outputLocation, string tenantCode);

        /// <summary>
        /// This method helps to re run the schedule for failed customer records
        /// </summary>
        /// <param name="scheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        bool ReRunScheduleForFailedCases(long scheduleLogIdentifier, string baseURL, string outputLocation, string tenantCode);

        /// <summary>
        /// This method helps to get error log message of schedule for failed customer records
        /// </summary>
        /// <param name="ScheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>list of schedule log error detail object</returns>
        List<ScheduleLogErrorDetail> GetScheduleLogErrorDetails(long ScheduleLogIdentifier, string tenantCode);
    }
}
