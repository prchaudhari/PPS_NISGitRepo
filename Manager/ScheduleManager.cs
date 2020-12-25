// <copyright file="ScheduleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of schedule manager.
    /// </summary>
    public class ScheduleManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The schedule repository.
        /// </summary>
        IScheduleRepository scheduleRepository = null;

        /// <summary>
        /// The schedule log repository.
        /// </summary>
        IScheduleLogRepository scheduleLogRepository = null;

        /// <summary>
        /// The statement manager.
        /// </summary>
        StatementManager statementManager = null;

        /// <summary>
        /// The tenant transaction data repository.
        /// </summary>
        ITenantTransactionDataRepository tenantTransactionDataRepository = null;

        /// <summary>
        /// The render engine repository.
        /// </summary>
        IRenderEngineRepository renderEngineRepository = null;

        /// <summary>
        /// The Dynamic widget repository.
        /// </summary>
        private IDynamicWidgetRepository dynamicWidgetRepository = null;

        /// <summary>
        /// The Client manager.
        /// </summary>
        private ClientManager clientManager = null;

        /// <summary>
        /// The tenant config manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for schedule manager, which initialise
        /// schedule repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public ScheduleManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.scheduleRepository = this.unityContainer.Resolve<IScheduleRepository>();
                this.scheduleLogRepository = this.unityContainer.Resolve<IScheduleLogRepository>();
                this.statementManager = this.unityContainer.Resolve<StatementManager>();
                this.tenantTransactionDataRepository = this.unityContainer.Resolve<ITenantTransactionDataRepository>();
                this.renderEngineRepository = this.unityContainer.Resolve<IRenderEngineRepository>();
                this.utility = new Utility();
                this.dynamicWidgetRepository = this.unityContainer.Resolve<IDynamicWidgetRepository>();
                this.clientManager = this.unityContainer.Resolve<ClientManager>();
                this.tenantConfigurationManager = this.unityContainer.Resolve<TenantConfigurationManager>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Schedule 
        /// <summary>
        /// This method will call add schedules method of repository.
        /// </summary>
        /// <param name="schedules">Schedules are to be add.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidSchedules(schedules, tenantCode);
                this.IsDuplicateSchedule(schedules, tenantCode);
                result = this.scheduleRepository.AddSchedules(schedules, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update schedules method of repository
        /// </summary>
        /// <param name="schedules">Schedules are to be update.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if schedules updated successfully, false otherwise.
        /// </returns>
        public bool UpdateSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidSchedules(schedules, tenantCode);
                this.IsDuplicateSchedule(schedules, tenantCode);
                result = this.scheduleRepository.UpdateSchedules(schedules, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete schedules method of repository
        /// </summary>
        /// <param name="schedules">Schedules are to be delete.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if schedules deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteSchedules(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.scheduleRepository.DeleteSchedules(schedules, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get schedules method of repository.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns schedules if found for given parameters, else return null
        /// </returns>
        public IList<Schedule> GetSchedules(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<Schedule> schedules = new List<Schedule>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    scheduleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                schedules = this.scheduleRepository.GetSchedules(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return schedules;
        }

        /// <summary>
        /// This method helps to get count of schedules.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of schedules
        /// </returns>
        public int GetScheduleCount(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            try
            {
                scheduleCount = this.scheduleRepository.GetScheduleCount(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return scheduleCount;
        }

        /// <summary>
        /// This method helps to activate the customers
        /// </summary>
        /// <param name="scheduleIdentifier">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedule activated successfully false otherwise</returns>
        public bool ActivateSchedule(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.scheduleRepository.ActivateSchedule(scheduleIdentifier, tenantCode);
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedule deactivated successfully false otherwise</returns>
        public bool DeactivateSchedule(long scheduleIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.scheduleRepository.DeActivateSchedule(scheduleIdentifier, tenantCode);
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
        /// This method will call add schedules method of repository.
        /// </summary>
        /// <param name="schedules">ScheduleRunHistorys are to be add.</param>
        /// <param name="tenantCode">Tenant code of schedule.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddScheduleRunHistorys(IList<ScheduleRunHistory> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.scheduleRepository.AddScheduleRunHistorys(schedules, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call get schedules method of repository.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns schedules if found for given parameters, else return null
        /// </returns>
        public IList<ScheduleRunHistory> GetScheduleRunHistorys(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            IList<ScheduleRunHistory> schedulesHistory = new List<ScheduleRunHistory>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    scheduleSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                schedulesHistory = this.scheduleRepository.GetScheduleRunHistorys(scheduleSearchParameter, tenantCode);
                if (schedulesHistory?.Count > 0)
                {
                    ScheduleSearchParameter parameter = new ScheduleSearchParameter();
                    parameter.Identifier = string.Join(",", schedulesHistory.Select(item => item.Schedule.Identifier).ToList());
                    parameter.IsStatementDefinitionRequired = true;
                    parameter.SortParameter.SortColumn = "Id";
                    IList<Schedule> schedules = new List<Schedule>();
                    schedules = this.GetSchedules(parameter, tenantCode);
                    schedulesHistory.ToList().ForEach(item =>
                    {
                        item.Schedule = schedules.Where(i => i.Identifier == item.Schedule.Identifier)?.FirstOrDefault();
                    });
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return schedulesHistory;
        }

        /// <summary>
        /// This method helps to get count of schedules.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of schedules
        /// </returns>
        public int GetScheduleRunHistoryCount(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
        {
            int scheduleCount = 0;
            try
            {
                scheduleCount = this.scheduleRepository.GetScheduleRunHistoryCount(scheduleSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return scheduleCount;
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location for HTML statements</param>
        /// <param name="tenantConfiguration">The tenant configuration object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunSchedule(string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);
                return this.scheduleRepository.RunScheduleNew(baseURL, outputLocation, tenantCode, parallelThreadCount,tenantConfiguration, client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location for HTML statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunScheduleNew(string baseURL, string outputLocation, string tenantCode)
        {
            bool scheduleRunStatus = false;

            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);

                var fromdate = DateTime.Now;
                var todate = fromdate.AddMinutes(60);

                BatchSearchParameter batchSearchParameter = new BatchSearchParameter()
                {
                    FromDate = fromdate,
                    ToDate = todate,
                    Status = BatchStatus.New.ToString(),
                    IsExecuted = false
                };
                var batches = this.scheduleRepository.GetBatches(batchSearchParameter, tenantCode);

                if (batches.Count > 0)
                {
                    var scheduleSearchParameter = new ScheduleSearchParameter()
                    {
                        Identifier = string.Join(",", batches.Select(item => item.ScheduleId).Distinct()).ToString(),
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
                    var schedules = this.scheduleRepository.GetSchedules(scheduleSearchParameter, tenantCode);
                    if (schedules != null && schedules.Count > 0)
                    {
                        schedules.ToList().ForEach(schedule =>
                        {
                            tenantCode = schedule.TenantCode;
                            var batch = batches.Where(item => item.ScheduleId == schedule.Identifier && !item.IsExecuted && item.Status == BatchStatus.New.ToString()).ToList().FirstOrDefault();

                            ScheduleLog scheduleLog = new ScheduleLog();
                            scheduleLog.ScheduleId = schedule.Identifier;
                            scheduleLog.ScheduleName = schedule.Name;
                            scheduleLog.BatchId = batch.Identifier;
                            scheduleLog.BatchName = batch.BatchName;
                            scheduleLog.NumberOfRetry = 1;
                            scheduleLog.CreateDate = DateTime.UtcNow;

                            if (batch != null)
                            {
                                if (batch.IsDataReady)
                                {
                                    StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                                    {
                                        Identifier = schedule.Statement.Identifier,
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
                                    var statements = this.statementManager.GetStatements(statementSearchParameter, tenantCode);
                                    if (statements.Count > 0)
                                    {
                                        scheduleLog.ScheduleStatus = ScheduleLogStatus.InProgress.ToString();
                                        IList<ScheduleLog> scheduleLogs = new List<ScheduleLog>();
                                        scheduleLogs.Add(scheduleLog);
                                        this.scheduleLogRepository.SaveScheduleLog(scheduleLogs, tenantCode);

                                        this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.InProgress.ToString(), tenantCode);
                                        this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.Running.ToString(), false, tenantCode);

                                        var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                                        if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                                        {
                                            baseURL = tenantConfiguration.OutputHTMLPath;
                                            outputLocation = tenantConfiguration.OutputHTMLPath;
                                        }

                                        var statement = statements.FirstOrDefault();
                                        var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                                        if (statementPageContents.Count > 0)
                                        {
                                            var statementPreviewData = this.statementManager.BindDataToCommonStatement(statement, statementPageContents, tenantConfiguration, tenantCode, client);
                                            string fileName = "Statement_" + statement.Identifier + "_" + batch.Identifier + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                                            var filesDict = new Dictionary<string, string>();
                                            if (statementPreviewData.SampleFiles != null && statementPreviewData.SampleFiles.Count > 0)
                                            {
                                                statementPreviewData.SampleFiles.ToList().ForEach(file =>
                                                {
                                                    if (!filesDict.ContainsKey(file.FileName))
                                                    {
                                                        filesDict.Add(file.FileName, file.FileUrl);
                                                    }
                                                });
                                            }

                                            string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batch.Identifier, baseURL, outputLocation, filesDict);

                                            ScheduleLogSearchParameter logSearchParameter = new ScheduleLogSearchParameter()
                                            {
                                                ScheduleName = schedule.Name,
                                                BatchId = batch.Identifier.ToString(),
                                                PagingParameter = new PagingParameter
                                                {
                                                    PageIndex = 0,
                                                    PageSize = 0,
                                                },
                                                SortParameter = new SortParameter()
                                                {
                                                    SortOrder = SortOrder.Ascending,
                                                    SortColumn = "Id",
                                                },
                                                SearchMode = SearchMode.Equals
                                            };
                                            scheduleLog = this.scheduleLogRepository.GetScheduleLogs(logSearchParameter, tenantCode).ToList().FirstOrDefault();
                                            scheduleLog.ScheduleId = schedule.Identifier;

                                            ScheduleRunHistory runHistory = new ScheduleRunHistory();
                                            runHistory.StartDate = DateTime.UtcNow;
                                            runHistory.ScheduleId = schedule.Identifier;
                                            runHistory.StatementId = statement.Identifier;
                                            runHistory.ScheduleLogId = scheduleLog.Identifier;
                                            runHistory.EndDate = DateTime.UtcNow;
                                            runHistory.StatementFilePath = CommonStatementZipFilePath;
                                            IList<ScheduleRunHistory> scheduleRunHistory = new List<ScheduleRunHistory>();
                                            scheduleRunHistory.Add(runHistory);
                                            this.scheduleRepository.AddScheduleRunHistorys(scheduleRunHistory, tenantCode);

                                            var BatchDetails = this.tenantTransactionDataRepository.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);

                                            CustomerSearchParameter customerSearchParameter = new CustomerSearchParameter()
                                            {
                                                BatchId = batch.Identifier,
                                            };
                                            var customers = this.tenantTransactionDataRepository.Get_CustomerMasters(customerSearchParameter, tenantCode);
                                            var renderEngine = this.renderEngineRepository.GetRenderEngine(tenantCode).FirstOrDefault();

                                            if (customers.Count > 0)
                                            {
                                                var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);

                                                ParallelOptions parallelOptions = new ParallelOptions();
                                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                                Parallel.ForEach(customers, parallelOptions, customer =>
                                                {
                                                    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batch, BatchDetails, baseURL, tenantCode, customers.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                                                });
                                                //customerMasters.ToList().ForEach(customer =>
                                                //{
                                                //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batch, batchDetails, baseURL, tenantCode, customers.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                                                //});
                                            }
                                            else
                                            {
                                                this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                                                this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                                                this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                                            }
                                        }
                                    }
                                    else 
                                    {
                                        throw new StatementNotFoundException(tenantCode);
                                    }
                                }
                                else
                                {
                                    this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                                    this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                                    this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                                }
                            }
                        });
                    }
                    
                    scheduleRunStatus = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to run the schedule now
        /// </summary>
        /// <param name="batchMaster">The batch object</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location for HTML statements</param>
        /// <param name="tenantConfiguration">The tenant configuration object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunScheduleNow(BatchMaster batchMaster, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);
                return this.scheduleRepository.RunScheduleNow(batchMaster, baseURL, outputLocation, tenantCode, parallelThreadCount, tenantConfiguration, client);
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
        /// <param name="baseURL">The base URL</param>
        /// <param name="outputLocation">The output location for HTML statements</param>
        /// <param name="tenantConfiguration">The tenant configuration object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if schedules runs successfully, false otherwise</returns>
        public bool RunScheduleNowNew(BatchMaster batchMaster, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            bool scheduleRunStatus = false;
            try
            {
                ClientSearchParameter clientSearchParameter = new ClientSearchParameter
                {
                    TenantCode = tenantCode,
                    IsCountryRequired = false,
                    IsContactRequired = false,
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var client = this.clientManager.GetClients(clientSearchParameter, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);

                var scheduleSearchParameter = new ScheduleSearchParameter()
                {
                    Identifier = batchMaster.ScheduleId.ToString(),
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
                var scheduleRecord = this.scheduleRepository.GetSchedules(scheduleSearchParameter, tenantCode)?.FirstOrDefault();
                if (scheduleRecord == null)
                {
                    throw new ScheduleNotFoundException(tenantCode);
                }

                BatchSearchParameter batchSearchParameter = new BatchSearchParameter()
                {
                    Identifier = batchMaster.Identifier.ToString(),
                    ScheduleId = batchMaster.ScheduleId.ToString(),
                    Status = BatchStatus.New.ToString(),
                    IsExecuted = false
                };
                var batch = this.scheduleRepository.GetBatches(batchSearchParameter, tenantCode)?.FirstOrDefault();
                if (batch != null)
                {
                    ScheduleLog scheduleLog = new ScheduleLog();
                    scheduleLog.ScheduleId = scheduleRecord.Identifier;
                    scheduleLog.ScheduleName = scheduleRecord.Name;
                    scheduleLog.BatchId = batch.Identifier;
                    scheduleLog.BatchName = batch.BatchName;
                    scheduleLog.NumberOfRetry = 1;
                    scheduleLog.CreateDate = DateTime.UtcNow;

                    var IsDataAvail = false;
                    var batchStatus = string.Empty;
                    var scheduleStatus = string.Empty;

                    if (batch.IsDataReady)
                    {
                        batchStatus = BatchStatus.Running.ToString();
                        scheduleStatus = ScheduleStatus.InProgress.ToString();
                        scheduleLog.ScheduleStatus = ScheduleLogStatus.InProgress.ToString();
                        IsDataAvail = batch.IsDataReady;
                    }
                    else
                    {
                        batchStatus = BatchStatus.BatchDataNotAvailable.ToString();
                        scheduleStatus = ScheduleStatus.BatchDataNotAvailable.ToString();
                        scheduleLog.ScheduleStatus = ScheduleLogStatus.BatchDataNotAvailable.ToString();
                    }

                    this.scheduleRepository.UpdateBatchStatus(batch.Identifier, batchStatus, false, tenantCode);
                    this.scheduleRepository.UpdateScheduleStatus(scheduleRecord.Identifier, scheduleStatus, tenantCode);

                    IList<ScheduleLog> scheduleLogs = new List<ScheduleLog>();
                    scheduleLogs.Add(scheduleLog);
                    this.scheduleLogRepository.SaveScheduleLog(scheduleLogs, tenantCode);

                    if (!IsDataAvail)
                    {
                        return scheduleRunStatus;
                    }

                    StatementSearchParameter statementSearchParameter = new StatementSearchParameter
                    {
                        Identifier = scheduleRecord.Statement.Identifier,
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
                    var statements = this.statementManager.GetStatements(statementSearchParameter, tenantCode);
                    if (statements.Count == 0)
                    {
                        throw new StatementNotFoundException(tenantCode);
                    }

                    var statement = statements.FirstOrDefault();
                    var pages = statement.Pages;
                    var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                    if (statementPageContents.Count > 0)
                    {
                        var statementPreviewData = this.statementManager.BindDataToCommonStatement(statement, statementPageContents, tenantConfiguration, tenantCode, client);
                        string fileName = "Statement_" + statement.Identifier + "_" + batchMaster.Identifier + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                        var filesDict = new Dictionary<string, string>();
                        for (int i = 0; i < statementPreviewData.SampleFiles.Count; i++)
                        {
                            filesDict.Add(statementPreviewData.SampleFiles[i].FileName, statementPreviewData.SampleFiles[i].FileUrl);
                        }
                        string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batchMaster.Identifier, baseURL, outputLocation, filesDict);

                        ScheduleLogSearchParameter logSearchParameter = new ScheduleLogSearchParameter()
                        {
                            ScheduleName = scheduleRecord.Name,
                            BatchId = batch.Identifier.ToString(),
                            PagingParameter = new PagingParameter
                            {
                                PageIndex = 0,
                                PageSize = 0,
                            },
                            SortParameter = new SortParameter()
                            {
                                SortOrder = SortOrder.Ascending,
                                SortColumn = "Id",
                            },
                            SearchMode = SearchMode.Equals
                        };
                        scheduleLog = this.scheduleLogRepository.GetScheduleLogs(logSearchParameter, tenantCode).ToList().FirstOrDefault();
                        scheduleLog.ScheduleId = scheduleRecord.Identifier;

                        ScheduleRunHistory runHistory = new ScheduleRunHistory();
                        runHistory.StartDate = DateTime.UtcNow;
                        runHistory.ScheduleId = scheduleRecord.Identifier;
                        runHistory.StatementId = statement.Identifier;
                        runHistory.ScheduleLogId = scheduleLog.Identifier;
                        runHistory.EndDate = DateTime.UtcNow;
                        runHistory.StatementFilePath = CommonStatementZipFilePath;
                        IList<ScheduleRunHistory> scheduleRunHistory = new List<ScheduleRunHistory>();
                        scheduleRunHistory.Add(runHistory);
                        this.scheduleRepository.AddScheduleRunHistorys(scheduleRunHistory, tenantCode);

                        var BatchDetails = this.tenantTransactionDataRepository.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);

                        CustomerSearchParameter customerSearchParameter = new CustomerSearchParameter()
                        {
                            BatchId = batch.Identifier,
                        };
                        var customers = this.tenantTransactionDataRepository.Get_CustomerMasters(customerSearchParameter, tenantCode);
                        var renderEngine = this.renderEngineRepository.GetRenderEngine(tenantCode).FirstOrDefault();

                        if (customers.Count > 0)
                        {
                            var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);

                            ParallelOptions parallelOptions = new ParallelOptions();
                            parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                            Parallel.ForEach(customers, parallelOptions, customer =>
                            {
                                this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batch, BatchDetails, baseURL, tenantCode, customers.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                            });
                            //customers.ToList().ForEach(customer =>
                            //{
                            //    this.CreateCustomerStatement(customer, statement, scheduleLog, statementPageContents, batch, BatchDetails, baseURL, tenantCode, customers.Count, outputLocation, tenantConfiguration, client, tenantEntities, renderEngine);
                            //});
                        }
                        else
                        {
                            this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                            this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                            this.scheduleRepository.UpdateScheduleStatus(scheduleRecord.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return scheduleRunStatus;
        }

        #endregion

        #region Batch master

        /// <summary>
        /// This method will call get schedules method of repository.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns schedules if found for given parameters, else return null
        /// </returns>
        public IList<BatchMaster> GetBatchMasters(long scheduleIdentifer, string tenantCode)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                batchMasters = this.scheduleRepository.GetBatchMasters(scheduleIdentifer, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return batchMasters;
        }

        /// <summary>
        /// This method helps to get batch list by search parameter.
        /// </summary>
        /// <param name="batchSearchParameter">The batch search parameter</param>
        /// <param name="tenantCode"></param>
        /// <returns>return list of batches</returns>
        public IList<BatchMaster> GetBatches(BatchSearchParameter batchSearchParameter, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.GetBatches(batchSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        public bool ApproveScheduleBatch(long BatchIdentifier, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.ApproveScheduleBatch(BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to clean batch and related data of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        public bool CleanScheduleBatch(long BatchIdentifier, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.CleanScheduleBatch(BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to update batch status.
        /// </summary>
        /// <param name="BatchIdentifier"></param>
        /// <param name="Status"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        public bool UpdateBatchStatus(long BatchIdentifier, string Status, bool IsExecuted, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.UpdateBatchStatus(BatchIdentifier, Status, IsExecuted, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate schedules.
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        private void IsValidSchedules(IList<Schedule> schedules, string tenantCode)
        {
            try
            {
                if (schedules?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidScheduleException invalidScheduleException = new InvalidScheduleException(tenantCode);
                schedules.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidScheduleException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidScheduleException.Data.Count > 0)
                {
                    throw invalidScheduleException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate schedule in the list
        /// </summary>
        /// <param name="schedules"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateSchedule(IList<Schedule> schedules, string tenantCode)
        {
            try
            {
                int isDuplicateSchedule = schedules.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateSchedule > 0)
                {
                    throw new DuplicateScheduleFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void CreateCustomerStatement(CustomerMaster customer, Statement statement, ScheduleLog scheduleLog, IList<StatementPageContent> statementPageContents, BatchMaster batchMaster, IList<BatchDetail> batchDetails, string baseURL, string tenantCode, int customerCount, string outputLocation, TenantConfiguration tenantConfiguration, Client client, IList<TenantEntity> tenantEntities, RenderEngine renderEngine)
        {
            IList<StatementMetadata> statementMetadataRecords = new List<StatementMetadata>();
            try
            {
                //call to generate actual HTML statement file for current customer record
                var logDetailRecord = this.statementManager.GenerateStatements(customer, statement, statementPageContents, batchMaster, batchDetails, baseURL, tenantCode, outputLocation, tenantConfiguration, client, tenantEntities);

                if (logDetailRecord != null)
                {
                    //save schedule log details for current customer
                    logDetailRecord.ScheduleLogId = scheduleLog.Identifier;
                    logDetailRecord.CustomerId = customer.Identifier;
                    logDetailRecord.CustomerName = customer.FirstName.Trim() + (customer.MiddleName == string.Empty ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim();
                    logDetailRecord.ScheduleId = scheduleLog.ScheduleId;
                    logDetailRecord.RenderEngineId = renderEngine != null ? renderEngine.Identifier : 0; //To be change once render engine implmentation start
                    logDetailRecord.RenderEngineName = renderEngine != null ? renderEngine.RenderEngineName : "";
                    logDetailRecord.RenderEngineURL = renderEngine != null ? renderEngine.URL : "";
                    logDetailRecord.NumberOfRetry = 1;
                    logDetailRecord.CreateDate = DateTime.UtcNow;

                    IList<ScheduleLogDetail> logDetails = new List<ScheduleLogDetail>();
                    logDetails.Add(logDetailRecord);
                    this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                    //if statement generated successfully, then save statement metadata with actual html statement file path
                    if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()))
                    {
                        if (logDetailRecord.statementMetadata.Count > 0)
                        {
                            logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
                            {
                                metarec.ScheduleLogId = scheduleLog.Identifier;
                                metarec.ScheduleId = scheduleLog.ScheduleId;
                                metarec.StatementDate = DateTime.UtcNow;
                                metarec.StatementURL = logDetailRecord.StatementFilePath;
                                metarec.TenantCode = tenantCode;
                                statementMetadataRecords.Add(metarec);
                            });
                            this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
                        }
                    }

                    //If any error occurs during statement generation then delete all files from output directory of current customer html statement
                    if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                    {
                        this.utility.DeleteUnwantedDirectory(batchMaster.Identifier, customer.Identifier, outputLocation);
                    }
                }

                //update status for respective schedule log, schedule log details entities
                //as well as update batch status if statement generation done for all customers of current batch
                ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter = new ScheduleLogDetailSearchParameter()
                {
                    ScheduleLogId = scheduleLog.Identifier.ToString(),
                    PagingParameter = new PagingParameter
                    {
                        PageIndex = 0,
                        PageSize = 0,
                    },
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = "Id",
                    },
                    SearchMode = SearchMode.Equals
                };
                var logDetailsRecords = this.scheduleLogRepository.GetScheduleLogDetails(scheduleLogDetailSearchParameter, tenantCode);
                if (customerCount == logDetailsRecords.Count)
                {
                    var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                    var batchStatus = BatchStatus.Completed.ToString();

                    var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                    if (failedRecords != null && failedRecords.Count > 0)
                    {
                        scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                        batchStatus = BatchStatus.Failed.ToString();
                    }

                    this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, scheduleLogStatus, tenantCode);
                    this.scheduleRepository.UpdateScheduleRunHistoryEndDate(scheduleLog.Identifier, tenantCode);
                    this.scheduleRepository.UpdateBatchStatus(batchMaster.Identifier, batchStatus, true, tenantCode);
                    this.scheduleRepository.UpdateScheduleStatus(scheduleLog.ScheduleId, ScheduleStatus.Completed.ToString(), tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
