// <copyright file="RoleController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using nIS.Models.NedBank;
    //using nIS.Models;
    //using nIS.Models.NedBank;
    using nIS.NedBank;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for DataHub
    /// </summary>
    public class DataHubController : ApiController
    {
        #region Private Members
        /// <summary>
        /// The schedule manager object.
        /// </summary>
        private ScheduleManager scheduleManager = null;

        /// <summary>
        /// the Data Hub Manager object
        /// </summary>
        private ETLScheduleManager eTLScheduleManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        /// <summary>
        /// The tenant config manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleController"/> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public DataHubController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
            this.scheduleManager = new ScheduleManager(this.unityContainer);
            this.eTLScheduleManager = new ETLScheduleManager(this.unityContainer);
        }

        #endregion

        //#region Public Methods

        /// <summary>
        /// This method helps to get ETL schedules list based on the search parameters.
        /// </summary>
        /// <param name="eTLScheduleSearchParameter">The etl schedule search parameter.</param>
        /// <returns>List of ETL schedules.</returns>
        [HttpPost]
        public IList<ETLScheduleListModel> ETLScheduleList(ETLScheduleSearchParameter eTLScheduleSearchParameter)
        {
            IList<ETLScheduleListModel> schedules = new List<ETLScheduleListModel>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                schedules = this.eTLScheduleManager.GetETLSchedule(eTLScheduleSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.eTLScheduleManager.GetETLScheduleCount(eTLScheduleSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw;
            }
            return schedules;
        }

        [HttpPost]
        public IList<ETLBatchMasterViewModel> GetETLBatches(long productBatchId)
        {
            IList<ETLBatchMasterViewModel> batchMasters = new List<ETLBatchMasterViewModel>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                batchMasters = this.eTLScheduleManager.GetETLBatches(productBatchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }

            return batchMasters;
        }

        //#region ETLScheduleBatchLogList

        ///// <summary>
        ///// This method helps to get ETL schedules batch log list based on the search parameters.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogSearchParameter">The ETL schedules batch log search parameter.</param>
        ///// <returns>List of ETL schedules batch log.</returns>
        //[HttpPost]
        //public IList<ETLScheduleBatchLogModel> ETLScheduleBatchLogList(ETLScheduleBatchLogSearchParameter eTLScheduleBatchLogSearchParameter)
        //{
        //    IList<ETLScheduleBatchLogModel> eTLscheduleLogs = new List<ETLScheduleBatchLogModel>();
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        eTLscheduleLogs = this.eTLScheduleManager.GetETLScheduleBatchLogs(eTLScheduleBatchLogSearchParameter, tenantCode, out int recordCount);
        //        HttpContext.Current.Response.AppendHeader("recordCount", recordCount.ToString());
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return eTLscheduleLogs;
        //}

        //#endregion

        //#region ETLScheduleBatchLogDetail

        ///// <summary>
        ///// This method helps to get ETL Schedule Detail For Batch Log Detail based on the search parameters.
        ///// </summary>
        ///// <param name="eTLScheduleDetailForBatchLogDetailSearchParameter">The ETL schedules batch log detail search parameter.</param>
        ///// <returns> List of ETL schedules batch log detail.</returns>
        //[HttpPost]
        //public ETLScheduleDetailForBatchLogDetail GetETLScheduleDetailForBatchLogDetail(ETLScheduleDetailForBatchLogDetailSearchParameter eTLScheduleDetailForBatchLogDetailSearchParameter)
        //{
        //    ETLScheduleDetailForBatchLogDetail eTLScheduleDetailForBatchLogDetail = new ETLScheduleDetailForBatchLogDetail();
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        eTLScheduleDetailForBatchLogDetail = this.eTLScheduleManager.GetETLScheduleDetailForBatchLogDetail(eTLScheduleDetailForBatchLogDetailSearchParameter, tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return eTLScheduleDetailForBatchLogDetail;
        //}

        ///// <summary>
        ///// This method helps to get ETL schedules batch log detail list based on the search parameters.
        ///// </summary>
        ///// <param name="eTLScheduleBatchLogDetailSearchParameter">The ETL schedules batch log detail search parameter.</param>
        ///// <returns> List of ETL schedules batch log detail.</returns>
        //[HttpPost]
        //public IList<ETLScheduleBatchLogDetailModel> ETLScheduleBatchLogDetailList(ETLScheduleBatchLogDetailSearchParameter eTLScheduleBatchLogDetailSearchParameter)
        //{
        //    IList<ETLScheduleBatchLogDetailModel> schedules = new List<ETLScheduleBatchLogDetailModel>();
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        schedules = this.eTLScheduleManager.GetETLScheduleBatchLogDetail(eTLScheduleBatchLogDetailSearchParameter, tenantCode, out int recordCount);
        //        HttpContext.Current.Response.AppendHeader("recordCount", recordCount.ToString());
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return schedules;
        //}

        //#endregion

        ///// <summary>
        ///// This method helps to get schedules list based on the search parameters.
        ///// </summary>
        ///// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        ///// <returns>
        ///// List of schedules
        ///// </returns>
        //[HttpPost]
        //public IList<ScheduleListModel> ETLScheduleDetail(ScheduleSearchParameter scheduleSearchParameter)
        //{
        //    IList<ScheduleListModel> schedules = new List<ScheduleListModel>();
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        schedules = this.scheduleManager.GetSchedulesWithProduct(scheduleSearchParameter, tenantCode);
        //        HttpContext.Current.Response.AppendHeader("recordCount", this.scheduleManager.GetScheduleCount(scheduleSearchParameter, tenantCode).ToString());
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }

        //    return schedules;
        //}
        //#endregion

        //#region ETLRunManually

        //[HttpPost]
        //public async Task<bool> RunETLManually(long etlBatchId)
        //{
        //    bool result = false;
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        result = await this.eTLScheduleManager.RunETLManually(etlBatchId, tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return result;
        //}

        //[HttpPost]
        //public async Task<bool> RetryETLManually(long etlBatchId)
        //{
        //    bool result = false;
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        result = await this.eTLScheduleManager.RetryETLManually(etlBatchId, tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return result;
        //}
        //#endregion

        #region ETLBatchApproved
        [HttpPost]
        public bool ApproveETLBatch(long etlBatchId)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.eTLScheduleManager.ApproveETLBatch(etlBatchId, tenantCode);
            }
            catch (Exception exception)
            {
                throw;
            }
            return result;
        }
        #endregion

        //#region DeleteETLBatch
        //[HttpPost]
        //public bool DeleteETLBatch(long etlBatchId)
        //{
        //    bool result = false;
        //    try
        //    {
        //        string tenantCode = Helper.CheckTenantCode(Request.Headers);
        //        result = this.eTLScheduleManager.DeleteETLBatch(etlBatchId, tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw;
        //    }
        //    return result;
        //}
        //#endregion
    }
}