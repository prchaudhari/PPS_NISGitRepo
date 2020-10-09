// <copyright file="ScheduleController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
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

        /// <summary>
        /// The tenant config manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;


        #endregion

        #region Constructor

        public ScheduleController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
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

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <returns>True if schedule runs successfully false otherwise</returns>
        [HttpPost]
        public bool RunSchedule()
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                var outputLocation = AppDomain.CurrentDomain.BaseDirectory;
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                {
                    baseURL = tenantConfiguration.OutputHTMLPath;
                    outputLocation = tenantConfiguration.OutputHTMLPath;
                }
                return this.scheduleManager.RunSchedule(baseURL, outputLocation, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batchMaster">The batch object</param>
        /// <returns>True if schedule runs successfully false otherwise</returns>
        [HttpPost]
        public bool RunScheduleNow(BatchMaster batchMaster)
        {
            try
            {
                if (batchMaster == null)
                {
                    return false;
                }
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                var outputLocation = AppDomain.CurrentDomain.BaseDirectory;
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                {
                    baseURL = tenantConfiguration.OutputHTMLPath;
                    outputLocation = tenantConfiguration.OutputHTMLPath;
                }
                return this.scheduleManager.RunScheduleNow(batchMaster, baseURL, outputLocation, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Batch master

        [HttpPost]
        public IList<BatchMaster> GetBatchMaster(long scheduleIdentifier)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                batchMasters = this.scheduleManager.GetBatchMasters(scheduleIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return batchMasters;
        }

        #endregion

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

        #region Download

        [HttpGet]
        [Route("ScheduleHistory/Download")]
        public HttpResponseMessage Download(string scheduelHistoryIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                string path = string.Empty;
                ScheduleRunHistory history = this.scheduleManager.GetScheduleRunHistorys(new ScheduleSearchParameter()
                {
                    ScheduleHistoryIdentifier = scheduelHistoryIdentifier,
                    SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
                }, tenantCode).FirstOrDefault();

                string FileName = history.StatementFilePath.Split('\'').ToList().LastOrDefault();
                path = history.StatementFilePath;
                if (!File.Exists(path))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", FileName);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                        }
                    }
                }
                catch (IOException)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

    }
}
