// <copyright file="ScheduleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Newtonsoft.Json;
    #region References

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
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

                                            if (customers.Count > 0)
                                            {
                                                var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
                                                GenerateStatementRawData statementRawData = new GenerateStatementRawData()
                                                {
                                                    Statement = statement,
                                                    ScheduleLog = scheduleLog,
                                                    StatementPageContents = statementPageContents,
                                                    Batch = batch,
                                                    BatchDetails = BatchDetails,
                                                    BaseURL = baseURL,
                                                    CustomerCount = customers.Count,
                                                    OutputLocation = outputLocation,
                                                    TenantConfiguration = tenantConfiguration,
                                                    Client = client,
                                                    TenantEntities = tenantEntities,
                                                };

                                                //Render engine implementation logic
                                                for (int i = 0; customers.Count > 0; i++)
                                                {
                                                    var availableRenderEngines = this.renderEngineRepository.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                                                    ParallelOptions parallelOptions = new ParallelOptions();

                                                    if (customers.Count > availableRenderEngines.Count * parallelThreadCount)
                                                    {
                                                        parallelOptions.MaxDegreeOfParallelism = availableRenderEngines.Count;
                                                        var parallelRequest = new List<CustomerParallelRequest>();
                                                        int count = 0;
                                                        for (int j = 1; availableRenderEngines.Count > 0; j++)
                                                        {
                                                            parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableRenderEngines.FirstOrDefault() });
                                                            customers = customers.Skip(parallelThreadCount).ToList();
                                                            count += 1;
                                                            availableRenderEngines = availableRenderEngines.Skip(count).ToList();
                                                        }
                                                        
                                                        ParallleProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                                    }
                                                    else
                                                    {
                                                        parallelOptions.MaxDegreeOfParallelism = customers.ToList().Count % parallelThreadCount == 0 ? customers.ToList().Count / parallelThreadCount : customers.ToList().Count / parallelThreadCount + 1;
                                                        var parallelRequest = new List<CustomerParallelRequest>();
                                                        int count = 0;

                                                        for (int k = 0; customers.Count > 0; k++)
                                                        {
                                                            if (customers.Count > parallelThreadCount)
                                                            {
                                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableRenderEngines[count] });
                                                                customers = customers.Skip(parallelThreadCount).ToList();
                                                                count += 1;
                                                            }
                                                            else
                                                            {
                                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.ToList(), RenderEngine = availableRenderEngines[count] });
                                                                customers = new List<CustomerMaster>();
                                                            }
                                                        }

                                                        ParallleProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                                    }
                                                }
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
                WriteToFile(ex.StackTrace.ToString());
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

                        if (customers.Count > 0)
                        {
                            var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);

                            GenerateStatementRawData statementRawData = new GenerateStatementRawData()
                            {
                                Statement = statement,
                                ScheduleLog = scheduleLog,
                                StatementPageContents = statementPageContents,
                                Batch = batch,
                                BatchDetails = BatchDetails,
                                BaseURL = baseURL,
                                CustomerCount = customers.Count,
                                OutputLocation = outputLocation,
                                TenantConfiguration = tenantConfiguration,
                                Client = client,
                                TenantEntities = tenantEntities,
                            };

                            //Render engine implementation logic
                            for (int i = 0; customers.Count > 0; i++)
                            {
                                var availableRenderEngines = this.renderEngineRepository.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                                ParallelOptions parallelOptions = new ParallelOptions();

                                if (customers.Count > availableRenderEngines.Count * parallelThreadCount)
                                {
                                    parallelOptions.MaxDegreeOfParallelism = availableRenderEngines.Count;
                                    var parallelRequest = new List<CustomerParallelRequest>();
                                    int count = 0;
                                    for (int j = 1; availableRenderEngines.Count > 0; j++)
                                    {
                                        parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableRenderEngines.FirstOrDefault() });
                                        customers = customers.Skip(parallelThreadCount).ToList();
                                        count += 1;
                                        availableRenderEngines = availableRenderEngines.Skip(count).ToList();
                                    }

                                    ParallleProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                                else
                                {
                                    parallelOptions.MaxDegreeOfParallelism = customers.ToList().Count % parallelThreadCount == 0 ? customers.ToList().Count / parallelThreadCount : customers.ToList().Count / parallelThreadCount + 1;
                                    var parallelRequest = new List<CustomerParallelRequest>();
                                    int count = 0;

                                    for (int k = 0; customers.Count > 0; k++)
                                    {
                                        if (customers.Count > parallelThreadCount)
                                        {
                                            parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableRenderEngines[count] });
                                            customers = customers.Skip(parallelThreadCount).ToList();
                                            count += 1;
                                        }
                                        else
                                        {
                                            parallelRequest.Add(new CustomerParallelRequest { Customers = customers.ToList(), RenderEngine = availableRenderEngines[count] });
                                            customers = new List<CustomerMaster>();
                                        }
                                    }

                                    ParallleProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                            }
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
                WriteToFile(ex.StackTrace.ToString());
                throw ex;
            }

            return scheduleRunStatus;
        }

        /// <summary>
        /// This method helps to update schedule status.
        /// </summary>
        /// <param name="SchedulIdentifier"></param>
        /// <param name="Status"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        public bool UpdateScheduleStatus(long ScheduleIdentifier, string Status, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.UpdateScheduleStatus(ScheduleIdentifier, Status, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to update schedule run history end date.
        /// </summary>
        /// <param name="ScheduleLogIdentifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        public bool UpdateScheduleRunHistoryEndDate(long ScheduleLogIdentifier, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.UpdateScheduleRunHistoryEndDate(ScheduleLogIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        /// <summary>
        /// This method helps to write content into the file
        /// </summary>
        /// <param name="Message">content to write into the file</param>
        private void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\StatementGenerationLogs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\StatementGenerationLogs\\Log_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        /// <summary>
        /// This method helps to process request in parallel
        /// </summary>
        /// <param name="statementRawData">raw data object requires in statement generate process</param>
        /// <param name="parallelOptions">parallel option object of threading</param>
        /// <param name="parallelRequests">the list of customer parallel request object</param>
        public void ParallleProcessing(GenerateStatementRawData statementRawData, string tenantCode, ParallelOptions parallelOptions, List<CustomerParallelRequest> parallelRequests, int parallelThreadCount)
        {
            Parallel.ForEach(parallelRequests, parallelOptions, item =>
            {
                CallGenearateStatementWebAPI(statementRawData, tenantCode, item, parallelThreadCount);
            });
        }

        /// <summary>
        /// This method helps to call web api of create customer statement file
        /// </summary>
        /// <param name="statementRawData">raw data object requires in statement generate process</param>
        /// <param name="TenantCode">The tenant code</param>
        /// <param name="parallelRequest">the customer parallel request object</param>
        /// <param name="parallelThreadCount">the thread count to run request in parallel</param>
        public void CallGenearateStatementWebAPI(GenerateStatementRawData statementRawData, string TenantCode, CustomerParallelRequest parallelRequest, int parallelThreadCount)
        {
            try
            {
                var renderEngine = parallelRequest.RenderEngine;
                string RenderEngineBaseUrl = string.IsNullOrEmpty(renderEngine?.URL) ? ConfigurationManager.AppSettings["DefaultGenerateStatementApiUrl"].ToString() : renderEngine?.URL;

                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;

                Parallel.ForEach(parallelRequest.Customers, parallelOptions, customer =>
                {
                    var newStatementRawData = new GenerateStatementRawData()
                    {
                        Statement = statementRawData.Statement,
                        ScheduleLog = statementRawData.ScheduleLog,
                        StatementPageContents = statementRawData.StatementPageContents,
                        Batch = statementRawData.Batch,
                        BatchDetails = statementRawData.BatchDetails,
                        BaseURL = statementRawData.BaseURL,
                        CustomerCount = statementRawData.CustomerCount,
                        OutputLocation = statementRawData.OutputLocation,
                        TenantConfiguration = statementRawData.TenantConfiguration,
                        Client = statementRawData.Client,
                        TenantEntities = statementRawData.TenantEntities,
                        Customer = customer,
                        RenderEngine = parallelRequest.RenderEngine
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(RenderEngineBaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("TenantCode", TenantCode);
                    var response = client.PostAsync("GenerateStatement/CreateCustomerStatement", new StringContent(JsonConvert.SerializeObject(newStatementRawData), Encoding.UTF8, "application/json")).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(result);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public class CustomerParallelRequest
        {
            public List<CustomerMaster> Customers { get; set; }
            public RenderEngine RenderEngine { get; set; }
        }
    }
}
