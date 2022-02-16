// <copyright file="IScheduleRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System;
using System.Collections.Generic;

namespace nIS
{
    /// <summary>
    /// 
    /// </summary>
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
        /// Adds the schedules with language.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool AddSchedulesWithLanguage(IList<Schedule> roles, string tenantCode);

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
        /// Updates the schedules with language.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool UpdateSchedulesWithLanguage(IList<Schedule> roles, string tenantCode);

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
        /// <param name="roleSearchParameter">The role search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Schedule count
        /// </returns>
        int GetScheduleCount(ScheduleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to activate roles
        /// </summary>
        /// <param name="roleIdentifier">The role identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool ActivateSchedule(long roleIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to deactivate roles
        /// </summary>
        /// <param name="roleIdentifier">The role identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
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
        /// <param name="roleSearchParameter">The role search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// ScheduleRunHistory count
        /// </returns>
        int GetScheduleRunHistoryCount(ScheduleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        bool RunSchedule(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client);

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        bool RunScheduleNew(string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client);

        /// <summary>
        /// Gets the batch masters.
        /// </summary>
        /// <param name="schdeuleIdentifier">The schdeule identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        IList<BatchMaster> GetBatchMasters(long schdeuleIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to get batch list by search parameter.
        /// </summary>
        /// <param name="batchSearchParameter">The batch search parameter</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// return list of batches
        /// </returns>
        IList<BatchMaster> GetBatches(BatchSearchParameter batchSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batchMaster">The batch object</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        bool RunScheduleNow(BatchMaster batchMaster, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount, TenantConfiguration tenantConfiguration, Client client);

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        bool ApproveScheduleBatch(long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        bool ValidateApproveScheduleBatch(long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to clean batch and related data of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        bool CleanScheduleBatch(long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to update schedule status.
        /// </summary>
        /// <param name="ScheduleIdentifier">The schedule identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        bool UpdateScheduleStatus(long ScheduleIdentifier, string Status, string tenantCode);

        /// <summary>
        /// This method helps to update batch status.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="IsExecuted">if set to <c>true</c> [is executed].</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        bool UpdateBatchStatus(long BatchIdentifier, string Status, bool IsExecuted, string tenantCode);

        /// <summary>
        /// This method helps to update schedule run history end date.
        /// </summary>
        /// <param name="ScheduleLogIdentifier">The schedule log identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        bool UpdateScheduleRunHistoryEndDate(long ScheduleLogIdentifier, string tenantCode);

    }
}
