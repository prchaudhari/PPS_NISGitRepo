// <copyright file="SQLScheduleLogRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Unity;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for schedule log and schedule log detail entity.
    /// </summary>
    ///
    public class SQLScheduleLogRepository : IScheduleLogRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        public SQLScheduleLogRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to get list of schedule log.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<ScheduleLog> GetScheduleLogs(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode)
        {
            IList<ScheduleLog> scheduleLogs = new List<ScheduleLog>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(scheduleLogSearchParameter, tenantCode);
                IList<ScheduleLogRecord> scheduleLogRecords = new List<ScheduleLogRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (scheduleLogSearchParameter.PagingParameter.PageIndex > 0 && scheduleLogSearchParameter.PagingParameter.PageSize > 0)
                    {
                        scheduleLogRecords = nISEntitiesDataContext.ScheduleLogRecords
                        .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleLogSearchParameter.PagingParameter.PageIndex - 1) * scheduleLogSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleLogSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        scheduleLogRecords = nISEntitiesDataContext.ScheduleLogRecords.Where(whereClause)
                        .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                }

                if (scheduleLogRecords != null && scheduleLogRecords.Count > 0)
                {
                    scheduleLogRecords.ToList().ForEach(logRecord => scheduleLogs.Add(new ScheduleLog()
                    {
                        Identifier = logRecord.Id,
                        ScheduleId = logRecord.ScheduleId,
                        ScheduleName = logRecord.ScheduleName,
                        CreateDate = logRecord.CreationDate,
                        LogFilePath = logRecord.LogFilePath,
                        NumberOfRetry = logRecord.NumberOfRetry,
                        RenderEngineId = logRecord.RenderEngineId,
                        RenderEngineName = logRecord.RenderEngineName,
                        RenderEngineURL = logRecord.RenderEngineURL,
                        ScheduleStatus = logRecord.Status,
                        ProcessingTime = "",
                        RecordProcessed = ""
                    }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleLogs;
        }

        /// <summary>
        /// This method helps to get count of schedule logs.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetScheduleLogsCount(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode)
        {
            int scheduleLogCount = 0;
            string whereClause = this.WhereClauseGenerator(scheduleLogSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleLogCount = nISEntitiesDataContext.ScheduleLogRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return scheduleLogCount;
        }

        /// <summary>
        /// This method helps to get list of schedule log detail.
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<ScheduleLogDetail> GetScheduleLogDetails(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter, string tenantCode)
        {
            IList<ScheduleLogDetail> scheduleLogDetails = new List<ScheduleLogDetail>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorLogDetail(scheduleLogDetailSearchParameter, tenantCode);
                IList<ScheduleLogDetailRecord> scheduleLogDetailRecords = new List<ScheduleLogDetailRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (scheduleLogDetailSearchParameter.PagingParameter.PageIndex > 0 && scheduleLogDetailSearchParameter.PagingParameter.PageSize > 0)
                    {
                        scheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords
                        .OrderBy(scheduleLogDetailSearchParameter.SortParameter.SortColumn + " " + scheduleLogDetailSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((scheduleLogDetailSearchParameter.PagingParameter.PageIndex - 1) * scheduleLogDetailSearchParameter.PagingParameter.PageSize)
                        .Take(scheduleLogDetailSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        scheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(whereClause)
                        .OrderBy(scheduleLogDetailSearchParameter.SortParameter.SortColumn + " " + scheduleLogDetailSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                }

                if (scheduleLogDetailRecords != null && scheduleLogDetailRecords.Count > 0)
                {
                    scheduleLogDetailRecords.ToList().ForEach(logDetail => scheduleLogDetails.Add(new ScheduleLogDetail()
                    {
                        Identifier = logDetail.Id,
                        CustomerId = logDetail.CustomerId,
                        CustomerName = logDetail.CustomerName,
                        LogMessage = logDetail.LogMessage,
                        NumberOfRetry = logDetail.NumberOfRetry ?? 0,
                        ScheduleId = logDetail.ScheduleId,
                        ScheduleLogId = logDetail.ScheduleLogId,
                        Status = logDetail.Status,
                        CreateDate = logDetail.CreationDate
                    }));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleLogDetails;
        }

        /// <summary>
        /// This method helps to get count of schedule log details.
        /// </summary>
        /// <param name="scheduleLogDetailSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetScheduleLogDetailsCount(ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter, string tenantCode)
        {
            int scheduleLogDetailCount = 0;
            string whereClause = this.WhereClauseGeneratorLogDetail(scheduleLogDetailSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleLogDetailCount = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return scheduleLogDetailCount;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="logSearchParameter">Schedule log search Parameters</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(ScheduleLogSearchParameter logSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(logSearchParameter.ScheduleLogId))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleLogId.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(logSearchParameter.RenderEngineId))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.RenderEngineId.ToString().Split(',').Select(item => string.Format("RenderEngineId.Equals({0}) ", item))) + ") and");
                }
                if (validationEngine.IsValidText(logSearchParameter.ScheduleId))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleId.ToString().Split(',').Select(item => string.Format("ScheduleId.Equals({0}) ", item))) + ") and");
                }
                if (validationEngine.IsValidText(logSearchParameter.ScheduleStatus))
                {
                    queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleStatus.ToString().Split(',').Select(item => string.Format("Status.Equals(\"{0}\") ", item))) + ") and ");
                }

                if (logSearchParameter.SearchMode == SearchMode.Equals)
                {
                    if (validationEngine.IsValidText(logSearchParameter.ScheduleName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleName.ToString().Split(',').Select(item => string.Format("ScheduleName.Equals(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logSearchParameter.RenderEngineName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.RenderEngineName.ToString().Split(',').Select(item => string.Format("RenderEngineName.Equals(\"{0}\") ", item))) + ") and ");
                    }
                }
                if (logSearchParameter.SearchMode == SearchMode.Contains)
                {
                    if (validationEngine.IsValidText(logSearchParameter.ScheduleName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.ScheduleName.ToString().Split(',').Select(item => string.Format("ScheduleName.Contains(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logSearchParameter.RenderEngineName))
                    {
                        queryString.Append("(" + string.Join("or ", logSearchParameter.RenderEngineName.ToString().Split(',').Select(item => string.Format("RenderEngineName.Contains(\"{0}\") ", item))) + ") and ");
                    }
                }

                if (this.validationEngine.IsValidDate(logSearchParameter.StartDate) && !this.validationEngine.IsValidDate(logSearchParameter.EndDate))
                {
                    DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.StartDate), DateTimeKind.Utc);
                    queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
                }
                if (this.validationEngine.IsValidDate(logSearchParameter.EndDate) && !this.validationEngine.IsValidDate(logSearchParameter.StartDate))
                {
                    DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.EndDate), DateTimeKind.Utc);
                    queryString.Append("EndDate <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
                }
                if (this.validationEngine.IsValidDate(logSearchParameter.StartDate) && this.validationEngine.IsValidDate(logSearchParameter.EndDate))
                {
                    DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.StartDate), DateTimeKind.Utc);
                    DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(logSearchParameter.EndDate), DateTimeKind.Utc);

                    queryString.Append("StartDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                                   "and EndDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
                }

                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
                return queryString.ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="logDetailSearchParameter">Schedule log search Parameters</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGeneratorLogDetail(ScheduleLogDetailSearchParameter logDetailSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(logDetailSearchParameter.ScheduleLogDetailId))
                {
                    queryString.Append("(" + string.Join("or ", logDetailSearchParameter.ScheduleLogDetailId.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(logDetailSearchParameter.ScheduleLogId))
                {
                    queryString.Append("(" + string.Join("or ", logDetailSearchParameter.ScheduleLogId.ToString().Split(',').Select(item => string.Format("ScheduleLogId.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(logDetailSearchParameter.CustomerId))
                {
                    queryString.Append("(" + string.Join("or ", logDetailSearchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
                }

                if (logDetailSearchParameter.SearchMode == SearchMode.Equals)
                {
                    if (validationEngine.IsValidText(logDetailSearchParameter.CustomerName))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.CustomerName.ToString().Split(',').Select(item => string.Format("CustomerName.Equals(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logDetailSearchParameter.Status))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.Status.ToString().Split(',').Select(item => string.Format("Status.Equals(\"{0}\") ", item))) + ") and ");
                    }
                }
                if (logDetailSearchParameter.SearchMode == SearchMode.Contains)
                {
                    if (validationEngine.IsValidText(logDetailSearchParameter.CustomerName))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.CustomerName.ToString().Split(',').Select(item => string.Format("CustomerName.Contains(\"{0}\") ", item))) + ") and ");
                    }
                    if (validationEngine.IsValidText(logDetailSearchParameter.Status))
                    {
                        queryString.Append("(" + string.Join("or ", logDetailSearchParameter.Status.ToString().Split(',').Select(item => string.Format("Status.Contains(\"{0}\") ", item))) + ") and ");
                    }
                }
                
                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
                return queryString.ToString();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #region Get Connection String

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
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
