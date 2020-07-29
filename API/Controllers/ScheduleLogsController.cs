// <copyright file="ScheduleLogsController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
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
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for schedule log
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("ScheduleLog")]
    public class ScheduleLogsController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The schedule log manager object.
        /// </summary>
        private ScheduleLogManager scheduleLogManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public ScheduleLogsController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.scheduleLogManager = new ScheduleLogManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to get schedule log list based on the search parameters.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public IList<ScheduleLog> List(ScheduleLogSearchParameter scheduleLogSearchParameter)
        {
            IList<ScheduleLog> scheduleLogs = new List<ScheduleLog>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                scheduleLogs = this.scheduleLogManager.GetScheduleLogs(scheduleLogSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleLogManager.GetScheduleLogsCount(scheduleLogSearchParameter, tenantCode).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleLogs;
        }

        /// <summary>
        /// This method helps to re run the schedule for failed customer records
        /// </summary>
        /// <param name="scheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        [HttpPost]
        public bool ReRunSchedule(long scheduleLogIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                return this.scheduleLogManager.ReRunScheduleForFailedCases(scheduleLogIdentifier, baseURL, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to get schedule log details list based on the search parameters.
        /// </summary>
        /// <param name="scheduleLogDetailSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        [Route("ScheduleLogDetail/List")]
        public IList<ScheduleLogDetail> GetScheduleLogDetails(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter)
        {
            IList<ScheduleLogDetail> scheduleLogDetails = new List<ScheduleLogDetail>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                scheduleLogDetails = this.scheduleLogManager.GetScheduleLogDetails(scheduleLogDetailSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleLogManager.GetScheduleLogDetailsCount(scheduleLogDetailSearchParameter, tenantCode).ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scheduleLogDetails;
        }

        /// <summary>
        /// This method helps to retry to generate html statements for failed customer records
        /// </summary>
        /// <param name="scheduleLogDetails">The schedule log detail object list</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        [HttpPost]
        [Route("ScheduleLogDetail/Retry")]
        public bool RetryStatementForFailedCustomerReocrds(IList<ScheduleLogDetail> scheduleLogDetails)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                return this.scheduleLogManager.RetryStatementForFailedCustomerReocrds(scheduleLogDetails, baseURL, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}