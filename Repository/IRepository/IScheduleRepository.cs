// <copyright file="IScheduleRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System;
using System.Collections.Generic;

namespace nIS
{
    public interface IScheduleRepository
    {
        /// <summary>
        /// This method adds the specified list of roles in role repository.
        /// </summary>
        /// <param name="roles">The list of roles</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are added successfully, else false.
        /// </returns>
        bool AddSchedules(IList<Schedule> roles, string tenantCode);

        /// <summary>
        /// This method updates the specified list of roles in role repository.
        /// </summary>
        /// <param name="roles">The list of roles</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are updated successfully, else false.
        /// </returns>
        bool UpdateSchedules(IList<Schedule> roles, string tenantCode);

        /// <summary>
        /// This method deletes the specified list of roles from role repository.
        /// </summary>
        /// <param name="roles">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are deleted successfully, else false.
        /// </returns>
        bool DeleteSchedules(IList<Schedule> roles, string tenantCode);

        /// <summary>
        /// This method gets the specified list of roles from role repository.
        /// </summary>
        /// <param name="roleSearchParameter">The role search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of roles
        /// </returns>
        IList<Schedule> GetSchedules(ScheduleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get role count
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Schedule count</returns>
        int GetScheduleCount(ScheduleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to activate roles
        /// </summary>
        /// <param name="roleIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool ActivateSchedule(long roleIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to deactivate roles
        /// </summary>
        /// <param name="roleIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool DeActivateSchedule(long roleIdentifier, string tenantCode);

        /// <summary>
        /// This method adds the specified list of roles in role repository.
        /// </summary>
        /// <param name="roles">The list of roles</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are added successfully, else false.
        /// </returns>
        bool AddScheduleRunHistorys(IList<ScheduleRunHistory> roles, string tenantCode);

        /// <summary>
        /// This method gets the specified list of roles from role repository.
        /// </summary>
        /// <param name="roleSearchParameter">The role search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of roles
        /// </returns>
        IList<ScheduleRunHistory> GetScheduleRunHistorys(ScheduleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get role count
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>ScheduleRunHistory count</returns>
        int GetScheduleRunHistoryCount(ScheduleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        bool RunSchedule(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client);

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        bool RunScheduleNew(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client);

        IList<BatchMaster> GetBatchMasters(long schdeuleIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to get batch list by search parameter.
        /// </summary>
        /// <param name="batchSearchParameter">The batch search parameter</param>
        /// <param name="tenantCode"></param>
        /// <returns>return list of batches</returns>
        IList<BatchMaster> GetBatches(BatchSearchParameter batchSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batchMaster">The batch object</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        bool RunScheduleNow(BatchMaster batchMaster, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client);

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool ApproveScheduleBatch(long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool ValidateApproveScheduleBatch(long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to clean batch and related data of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool CleanScheduleBatch(long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to update schedule status.
        /// </summary>
        /// <param name="SchedulIdentifier"></param>
        /// <param name="Status"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool UpdateScheduleStatus(long ScheduleIdentifier, string Status, string tenantCode);

        /// <summary>
        /// This method helps to update batch status.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="Status"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool UpdateBatchStatus(long BatchIdentifier, string Status, bool IsExecuted, string tenantCode);

        /// <summary>
        /// This method helps to update schedule run history end date.
        /// </summary>
        /// <param name="ScheduleLogIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        bool UpdateScheduleRunHistoryEndDate(long ScheduleLogIdentifier, string tenantCode);

    }
}
