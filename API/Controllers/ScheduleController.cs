// <copyright file="ScheduleController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for schedule
    /// </summary>
    public class ScheduleController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The schedule manager object.
        /// </summary>
        private ScheduleManager scheduleManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public ScheduleController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.scheduleManager = new ScheduleManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        #region Schedule 

        /// <summary>
        /// This method helps to add schedules
        /// </summary>
        /// <param name="schedules"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.AddSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update schedules.
        /// </summary>
        /// <param name="schedules"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.UpdateSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete schedules.
        /// </summary>
        /// <param name="schedules"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<Schedule> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.DeleteSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get schedules list based on the search parameters.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <returns>List of schedules</returns>
        [HttpPost]
        public IList<Schedule> List(ScheduleSearchParameter scheduleSearchParameter)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                schedules = this.scheduleManager.GetSchedules(scheduleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleManager.GetScheduleCount(scheduleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules;
        }

        /// <summary>
        /// This method helps to get schedule based on given identifier.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <returns>schedule record</returns>
        [HttpGet]
        public Schedule Detail(long scheduleIdentifier)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                ScheduleSearchParameter scheduleSearchParameter = new ScheduleSearchParameter();
                scheduleSearchParameter.Identifier = scheduleIdentifier.ToString();
                scheduleSearchParameter.SortParameter.SortColumn = "Id";
                schedules = this.scheduleManager.GetSchedules(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules.First();
        }

        /// <summary>
        /// This method helps to activate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>       
        /// <returns>True if schedule activated successfully false otherwise</returns>
        [HttpGet]
        public bool Activate(long scheduleIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.scheduleManager.ActivateSchedule(scheduleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method helps to deactivate the schedule
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>       
        /// <returns>True if schedule deactivated successfully false otherwise</returns>
        [HttpGet]
        public bool Deactivate(long scheduleIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.scheduleManager.DeactivateSchedule(scheduleIdentifier, tenantCode);
                result = true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }
        #endregion

        #region ScheduleRunHistory 

        /// <summary>
        /// This method helps to add schedules
        /// </summary>
        /// <param name="schedules"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool AddScheduleHistory(IList<ScheduleRunHistory> schedules)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.scheduleManager.AddScheduleRunHistorys(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

      

        /// <summary>
        /// This method helps to get schedules list based on the search parameters.
        /// </summary>
        /// <param name="scheduleSearchParameter"></param>
        /// <returns>List of schedules</returns>
        [HttpPost]
        public IList<ScheduleRunHistory> GetScheduleRunHistories(ScheduleSearchParameter scheduleSearchParameter)
        {
            IList<ScheduleRunHistory> schedules = new List<ScheduleRunHistory>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                schedules = this.scheduleManager.GetScheduleRunHistorys(scheduleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleManager.GetScheduleRunHistoryCount(scheduleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return schedules;
        }

    
        #endregion

        #endregion

    }
}
