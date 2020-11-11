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
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        /// <summary>
        /// The statement repository.
        /// </summary>
        private IStatementRepository statementRepository = null;

        /// <summary>
        /// The Page repository.
        /// </summary>
        private IPageRepository pageRepository = null;

        /// <summary>
        /// The Tenant configuration repository.
        /// </summary>
        private ITenantConfigurationRepository tenantConfigurationRepository = null;

        #endregion

        #region Constructor

        public SQLScheduleLogRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
            this.statementRepository = this.unityContainer.Resolve<IStatementRepository>();
            this.tenantConfigurationRepository = this.unityContainer.Resolve<ITenantConfigurationRepository>();
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
                IList<View_ScheduleLog> scheduleLogRecords = new List<View_ScheduleLog>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        if (scheduleLogSearchParameter.PagingParameter.PageIndex > 0 && scheduleLogSearchParameter.PagingParameter.PageSize > 0)
                        {
                            scheduleLogRecords = nISEntitiesDataContext.View_ScheduleLog
                            .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString())
                            .Where(whereClause)
                            .Skip((scheduleLogSearchParameter.PagingParameter.PageIndex - 1) * scheduleLogSearchParameter.PagingParameter.PageSize)
                            .Take(scheduleLogSearchParameter.PagingParameter.PageSize)
                            .ToList();
                        }
                        else
                        {
                            scheduleLogRecords = nISEntitiesDataContext.View_ScheduleLog.Where(whereClause)
                            .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                            .ToList();
                        } 
                    }
                    else
                    {
                        if (scheduleLogSearchParameter.PagingParameter.PageIndex > 0 && scheduleLogSearchParameter.PagingParameter.PageSize > 0)
                        {
                            scheduleLogRecords = nISEntitiesDataContext.View_ScheduleLog
                            .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString())
                            .Skip((scheduleLogSearchParameter.PagingParameter.PageIndex - 1) * scheduleLogSearchParameter.PagingParameter.PageSize)
                            .Take(scheduleLogSearchParameter.PagingParameter.PageSize)
                            .ToList();
                        }
                        else
                        {
                            scheduleLogRecords = nISEntitiesDataContext.View_ScheduleLog
                            .OrderBy(scheduleLogSearchParameter.SortParameter.SortColumn + " " + scheduleLogSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                            .ToList();
                        }
                    }
                }

                if (scheduleLogRecords != null && scheduleLogRecords.Count > 0)
                {
                    IList<ScheduleLogDetailRecord> logDetails = new List<ScheduleLogDetailRecord>();
                    IList<ScheduleRunHistoryRecord> runHistoryRecords = new List<ScheduleRunHistoryRecord>();
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        logDetails = nISEntitiesDataContext.ScheduleLogDetailRecords.ToList();
                        runHistoryRecords = nISEntitiesDataContext.ScheduleRunHistoryRecords.ToList();
                    }

                    for (int i = 0; i < scheduleLogRecords.Count; i++)
                    {
                        ScheduleLog log = new ScheduleLog();
                        log.Identifier = scheduleLogRecords[i].Id;
                        log.ProcessingTime = scheduleLogRecords[i].ProcessingTime;
                        log.RecordProcessed = scheduleLogRecords[i].RecordProccessed;
                        log.ScheduleStatus = scheduleLogRecords[i].Status;
                        log.ScheduleName = scheduleLogRecords[i].ScheduleName;
                        log.CreateDate = DateTime.SpecifyKind((DateTime)scheduleLogRecords[i].ExecutionDate, DateTimeKind.Utc);
                        scheduleLogs.Add(log);
                    }

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
                    if(!string.IsNullOrEmpty(whereClause))
                    {
                        scheduleLogCount = nISEntitiesDataContext.ScheduleLogRecords.Where(whereClause.ToString()).Count();

                    }
                    else
                    {
                        scheduleLogCount = nISEntitiesDataContext.ScheduleLogRecords.Count();

                    }
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
                        RenderEngineId = logDetail.RenderEngineId,
                        RenderEngineName = logDetail.RenderEngineName,
                        RenderEngineURL = logDetail.RenderEngineURL,
                        NumberOfRetry = logDetail.NumberOfRetry,
                        ScheduleId = logDetail.ScheduleId,
                        ScheduleLogId = logDetail.ScheduleLogId,
                        Status = logDetail.Status,
                        //CreateDate = logDetail.CreationDate
                        CreateDate = DateTime.SpecifyKind((DateTime)logDetail.CreationDate, DateTimeKind.Utc)

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

        /// <summary>
        /// This method helps to retry to generate html statements for failed customer records
        /// </summary>
        /// <param name="scheduleLogDetails">The schedule log detail object list</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        public bool RetryStatementForFailedCustomerReocrds(IList<ScheduleLogDetail> scheduleLogDetails, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount)
        {
            bool retryStatementStatus = false;
            IList<ScheduleRecord> schedules = new List<ScheduleRecord>();

            try
            {
                IList<ScheduleLogDetailRecord> scheduleLogDetailRecords = null;
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", scheduleLogDetails.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));

                    scheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (scheduleLogDetailRecords == null || scheduleLogDetailRecords.Count <= 0 || scheduleLogDetailRecords.Count() != string.Join(",", scheduleLogDetailRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new ScheduleLogDetailNotFoundException(tenantCode);
                    }
                }

                if (scheduleLogDetailRecords.Count != 0)
                {
                    var firstScheduleLogDetailRecord = scheduleLogDetailRecords.ToList().FirstOrDefault();
                    var batchMaster = new BatchMasterRecord();
                    var scheduleRecord = new ScheduleRecord();
                    var batchDetails = new List<BatchDetailRecord>();
                    var renderEngine = new RenderEngineRecord();

                    var tenantConfiguration = this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode)?.FirstOrDefault();

                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        scheduleRecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == firstScheduleLogDetailRecord.ScheduleId && item.TenantCode == tenantCode)?.FirstOrDefault();
                        if (scheduleRecord != null)
                        {
                            batchMaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == scheduleRecord.Id && item.TenantCode == tenantCode)?.FirstOrDefault();
                            if (batchMaster != null)
                            {
                                batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchMaster.Id && item.TenantCode == tenantCode)?.ToList();
                            }
                        }
                    }

                    if (batchMaster != null)
                    {
                        StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                        {
                            Identifier = scheduleRecord.StatementId,
                            IsActive = true,
                            IsStatementPagesRequired = true,
                            PagingParameter = new PagingParameter
                            {
                                PageIndex = 0,
                                PageSize = 0,
                            },
                            SortParameter = new SortParameter()
                            {
                                SortOrder = SortOrder.Ascending,
                                SortColumn = "Name",
                            },
                            SearchMode = SearchMode.Equals
                        };
                        var statements = this.statementRepository.GetStatements(statementSearchParameter, tenantCode);
                        if (statements.Count > 0)
                        {
                            var statement = statements.ToList().FirstOrDefault();
                            var statementPageContents = this.statementRepository.GenerateHtmlFormatOfStatement(statement, tenantCode);
                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                            {
                                renderEngine = nISEntitiesDataContext.RenderEngineRecords.Where(item => item.Id == 1).FirstOrDefault();
                            }

                            ParallelOptions parallelOptions = new ParallelOptions();
                            parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                            Parallel.ForEach(scheduleLogDetailRecords, parallelOptions, scheduleLogDetail =>
                            {
                                this.ReGenerateFailedCustomerStatements(scheduleLogDetail, statement, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, outputLocation, renderEngine, tenantConfiguration);
                            });
                        }
                    }
                }

                retryStatementStatus = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retryStatementStatus;
        }


        /// <summary>
        /// This method helps to re run the schedule for failed customer records
        /// </summary>
        /// <param name="scheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        public bool ReRunScheduleForFailedCases(long scheduleLogIdentifier, string baseURL, string outputLocation, string tenantCode, int parallelThreadCount)
        {
            bool scheduleRunStatus = false;
            try
            {
                IList<ScheduleLogDetailRecord> failedScheduleLogDetailRecords = null;
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    failedScheduleLogDetailRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == scheduleLogIdentifier && item.Status == ScheduleLogStatus.Failed.ToString() && item.TenantCode == tenantCode).ToList();
                    if (failedScheduleLogDetailRecords == null || failedScheduleLogDetailRecords.Count == 0)
                    {
                        throw new ScheduleLogDetailNotFoundException(tenantCode);
                    }
                }

                var failedRecords = new List<ScheduleLogDetail>();
                failedScheduleLogDetailRecords.ToList().ForEach(item => failedRecords.Add(new ScheduleLogDetail()
                {
                    Identifier = item.Id
                }));

                if (failedRecords.Count != 0)
                {
                    scheduleRunStatus = RetryStatementForFailedCustomerReocrds(failedRecords, baseURL, outputLocation, tenantCode, parallelThreadCount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to get error log message of schedule for failed customer records
        /// </summary>
        /// <param name="ScheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>list of schedule log error detail object</returns>
        public List<ScheduleLogErrorDetail> GetScheduleLogErrorDetails(long ScheduleLogIdentifier, string tenantCode)
        {
            List<ScheduleLogErrorDetail> scheduleLogErrors = new List<ScheduleLogErrorDetail>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    scheduleLogErrors = (from sl in nISEntitiesDataContext.ScheduleLogRecords
                                         join sld in nISEntitiesDataContext.ScheduleLogDetailRecords on sl.Id equals sld.ScheduleLogId
                                         join srh in nISEntitiesDataContext.ScheduleRunHistoryRecords on sl.Id equals srh.ScheduleLogId
                                         join s in nISEntitiesDataContext.StatementRecords on srh.StatementId equals s.Id
                                         where sl.Id == ScheduleLogIdentifier && sld.Status.ToLower() == ScheduleLogStatus.Failed.ToString().ToLower()
                                         && sl.TenantCode == tenantCode
                                         orderby sld.Id ascending
                                         select new ScheduleLogErrorDetail()
                                         {
                                             ScheduleId = sl.ScheduleId,
                                             ScheduleName = sl.ScheduleName,
                                             ScheduleLogId = sl.Id,
                                             StatementId = srh.StatementId,
                                             StatementName = s.Name,
                                             ScheduleLogDetailId = sld.Id,
                                             CustomerId = sld.CustomerId,
                                             CustomerName = sld.CustomerName,
                                             ErrorLogMessage = sld.LogMessage,
                                             ExecutionDate = sld.CreationDate
                                         }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return scheduleLogErrors;
        }

        /// <summary>
        /// This method helps to get dashboard data
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>dashboard data object</returns>
        public DashboardData GetDashboardData(string tenantCode)
        {
            DashboardData dashboardData = new DashboardData();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var schedulelog = nISEntitiesDataContext.ScheduleLogRecords.OrderByDescending(item => item.CreationDate).ToList().FirstOrDefault();
                    dashboardData.LastSCheduleRunDate = schedulelog.CreationDate;
                    var schedulelogdetails = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == schedulelog.Id).ToList();
                    dashboardData.GeneratedStatementsOfLastscheduleRun = schedulelogdetails.Where(item => item.Status == ScheduleLogStatus.Completed.ToString()).ToList().Count;
                    dashboardData.ActiveExceptionsOfLastscheduleRun = schedulelogdetails.Where(item => item.Status == ScheduleLogStatus.Failed.ToString()).ToList().Count;
                    dashboardData.TotalGeneratedStatements = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.Status == ScheduleLogStatus.Completed.ToString()).ToList().Count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dashboardData;
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
                    queryString.Append("CreationDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
                }
                //if (queryString.ToString() != string.Empty)
                //{
                //    queryString.Remove(queryString.Length - 4, 4);
                //}
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

        private bool ReGenerateFailedCustomerStatements(ScheduleLogDetailRecord scheduleLogDetail, Statement statement, IList<StatementPageContent> statementPageContents, BatchMasterRecord batchMaster, IList<BatchDetailRecord> batchDetails, string baseURL, string tenantCode, string outputLocation, RenderEngineRecord renderEngine, TenantConfiguration tenantConfiguration)
        {
            try
            {
                var customerMaster = new CustomerMasterRecord();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                { 
                    customerMaster = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.Id == scheduleLogDetail.CustomerId && item.BatchId == batchMaster.Id && item.TenantCode == tenantCode).FirstOrDefault();
                }

                if (customerMaster != null)
                {
                    var newStatementPageContents = new List<StatementPageContent>();
                    statementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
                    {
                        Id = it.Id,
                        PageId = it.PageId,
                        PageTypeId = it.PageTypeId,
                        HtmlContent = it.HtmlContent,
                        PageHeaderContent = it.PageHeaderContent,
                        PageFooterContent = it.PageFooterContent,
                        DisplayName = it.DisplayName,
                        TabClassName = it.TabClassName
                    }));

                    var logDetailRecord = this.statementRepository.GenerateStatements(customerMaster, statement, newStatementPageContents, batchMaster, batchDetails, baseURL, tenantCode, outputLocation, tenantConfiguration);
                    if (logDetailRecord != null)
                    {
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                        {
                            this.utility.DeleteUnwantedDirectory(batchMaster.Id, customerMaster.Id, outputLocation);
                        }
                        using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                        {
                            var scheduleLogDetailRecord = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.Id == scheduleLogDetail.Id && item.TenantCode == tenantCode).FirstOrDefault();
                            if (scheduleLogDetailRecord != null)
                            {
                                scheduleLogDetailRecord.CustomerId = customerMaster.Id;
                                scheduleLogDetailRecord.CustomerName = customerMaster.FirstName.Trim() + (customerMaster.MiddleName == string.Empty ? string.Empty : " " + customerMaster.MiddleName.Trim()) + " " + customerMaster.LastName.Trim();
                                scheduleLogDetailRecord.RenderEngineId = renderEngine.Id; //To be change once render engine implmentation start
                                scheduleLogDetailRecord.RenderEngineName = renderEngine.Name;
                                scheduleLogDetailRecord.RenderEngineURL = renderEngine.URL;
                                scheduleLogDetailRecord.LogMessage = logDetailRecord.LogMessage;
                                scheduleLogDetailRecord.Status = logDetailRecord.Status;
                                scheduleLogDetailRecord.NumberOfRetry++;
                                scheduleLogDetailRecord.StatementFilePath = logDetailRecord.StatementFilePath;
                                scheduleLogDetailRecord.CreationDate = DateTime.UtcNow;

                                if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()) && logDetailRecord.StatementMetadataRecords.Count > 0)
                                {
                                    IList<StatementMetadataRecord> statementMetadataRecords = new List<StatementMetadataRecord>();
                                    logDetailRecord.StatementMetadataRecords.ToList().ForEach(metarec =>
                                    {
                                        metarec.ScheduleLogId = logDetailRecord.ScheduleLogId;
                                        metarec.ScheduleId = logDetailRecord.ScheduleLogId;
                                        metarec.StatementDate = DateTime.UtcNow;
                                        metarec.StatementURL = logDetailRecord.StatementFilePath;
                                        statementMetadataRecords.Add(metarec);
                                    });
                                }

                                nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLogDetail.ScheduleLogId && item.ScheduleId == scheduleLogDetail.ScheduleId && item.TenantCode == tenantCode)?.ToList().ForEach(scheduleLog =>
                                {
                                    var _lstScheduleLogDetail = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == scheduleLogDetail.ScheduleLogId && item.ScheduleId == scheduleLogDetail.ScheduleId && item.TenantCode == tenantCode).ToList();
                                    var successRecords = _lstScheduleLogDetail.Where(item => item.Status == ScheduleLogStatus.Completed.ToString() && item.TenantCode == tenantCode)?.ToList();
                                    if (successRecords != null && successRecords.Count == _lstScheduleLogDetail.Count)
                                    {
                                        scheduleLog.Status = ScheduleLogStatus.Completed.ToString();
                                    }
                                    else
                                    {
                                        scheduleLog.Status = ScheduleLogStatus.Failed.ToString();
                                    }
                                });

                                nISEntitiesDataContext.SaveChanges();
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        #endregion

        #endregion
    }
}
