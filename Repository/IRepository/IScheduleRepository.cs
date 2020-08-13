// <copyright file="IScheduleRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

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

        bool RunSchedule(string baseURL, string tenantCode);

        IList<BatchMaster> GetBatchMasters(long schdeuleIdentifier, string tenantCode);

    }
}
