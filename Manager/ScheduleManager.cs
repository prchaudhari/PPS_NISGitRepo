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

        /// <summary>
        /// The system activity history manager object.
        /// </summary>
        private SystemActivityHistoryManager systemActivityHistoryManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for schedule manager, which initialise
        /// schedule repository.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
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
                this.systemActivityHistoryManager = new SystemActivityHistoryManager(unityContainer);
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
        /// Adds the schedules with language.
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public bool AddSchedulesWithLanguage(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidSchedules(schedules, tenantCode);
                this.IsDuplicateSchedule(schedules, tenantCode);
                result = this.scheduleRepository.AddSchedulesWithLanguage(schedules, tenantCode);
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
        /// Updates the schedules with language.
        /// </summary>
        /// <param name="schedules">The schedules.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public bool UpdateSchedulesWithLanguage(IList<Schedule> schedules, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidSchedules(schedules, tenantCode);
                this.IsDuplicateSchedule(schedules, tenantCode);
                result = this.scheduleRepository.UpdateSchedulesWithLanguage(schedules, tenantCode);
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
        /// Gets the schedules with language.
        /// </summary>
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public IList<Schedule> GetSchedulesWithLanguage(ScheduleSearchParameter scheduleSearchParameter, string tenantCode)
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

                schedules = this.scheduleRepository.GetSchedulesWithLanguage(scheduleSearchParameter, tenantCode);
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
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
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
        /// <returns>
        /// True if schedule activated successfully false otherwise
        /// </returns>
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
        /// <returns>
        /// True if schedule deactivated successfully false otherwise
        /// </returns>
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
        /// <param name="scheduleSearchParameter">The schedule search parameter.</param>
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
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        public bool RunSchedule(string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                var client = this.clientManager.GetClients(new ClientSearchParameter
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
                }, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings[ModelConstant.PARALLEL_THREAD_COUNT]);
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
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        public bool RunScheduleNew(string baseURL, string outputLocation, string tenantCode)
        {
            bool scheduleRunStatus = false;
            IList<SystemActivityHistory> activityHistories = null;

            try
            {
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings[ModelConstant.PARALLEL_THREAD_COUNT]);
                
                //get batches which has execution time in between given time interval
                var fromdate = DateTime.Now;
                var todate = fromdate.AddMinutes(60);
                var batches = this.scheduleRepository.GetBatches(new BatchSearchParameter()
                {
                    FromDate = fromdate,
                    ToDate = todate,
                    Status = BatchStatus.New.ToString(),
                    IsExecuted = false
                }, tenantCode);

                if (batches.Count > 0)
                {
                    var schedules = this.scheduleRepository.GetSchedules(new ScheduleSearchParameter()
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
                    }, tenantCode);
                    if (schedules != null && schedules.Count > 0)
                    {
                        schedules.ToList().ForEach(schedule =>
                        {
                            activityHistories = new List<SystemActivityHistory>();
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
                                    var statements = this.statementManager.GetStatements(new StatementSearchParameter
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
                                    }, tenantCode);
                                    if (statements.Count > 0)
                                    {
                                        var statement = statements.FirstOrDefault();

                                        scheduleLog.ScheduleStatus = ScheduleLogStatus.InProgress.ToString();
                                        IList<ScheduleLog> scheduleLogs = new List<ScheduleLog>();
                                        scheduleLogs.Add(scheduleLog);
                                        this.scheduleLogRepository.SaveScheduleLog(scheduleLogs, tenantCode);

                                        this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.InProgress.ToString(), tenantCode);
                                        this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.Running.ToString(), false, tenantCode);

                                        var functionName = string.Empty;
                                        var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                                        if (tenantConfiguration != null)
                                        {
                                            if (!string.IsNullOrEmpty(tenantConfiguration.OutputHTMLPath))
                                            {
                                                baseURL = tenantConfiguration.OutputHTMLPath;
                                                outputLocation = tenantConfiguration.OutputHTMLPath;
                                            }
                                            functionName = tenantConfiguration.GenerateStatementRunNowScheduleFunctionName;
                                        }

                                        switch (functionName)
                                        {
                                            case ModelConstant.GENERATE_FINANCIAL_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW:
                                                this.GenerateFinancialTenantCustomerStatementsByScheduleRunTime(statement, tenantConfiguration, batch, schedule, tenantCode, baseURL, outputLocation, parallelThreadCount);
                                                break;

                                            case ModelConstant.GENERATE_NEDBANK_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW:
                                                this.GenerateNedbankCustomerStatementsByScheduleRunTime(statement, tenantConfiguration, batch, schedule, tenantCode, baseURL, outputLocation, parallelThreadCount);
                                                break;

                                            default:
                                                this.GenerateFinancialTenantCustomerStatementsByScheduleRunTime(statement, tenantConfiguration, batch, schedule, tenantCode, baseURL, outputLocation, parallelThreadCount);
                                                break;
                                        }
                                        
                                        activityHistories.Add(new SystemActivityHistory()
                                        {
                                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                                            EntityId = schedule.Identifier,
                                            EntityName = schedule.Name,
                                            SubEntityId = batch.Identifier,
                                            SubEntityName = batch.BatchName,
                                            ActionTaken = "AutoScheduleRunSuccess",
                                        });
                                        this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
                                    }
                                    else
                                    {
                                        activityHistories.Add(new SystemActivityHistory()
                                        {
                                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                                            EntityId = schedule.Identifier,
                                            EntityName = schedule.Name,
                                            SubEntityId = batch.Identifier,
                                            SubEntityName = batch.BatchName,
                                            ActionTaken = "AutoScheduleRun - Statement not found error",
                                        });
                                        this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
                                        throw new StatementNotFoundException(tenantCode);
                                    }
                                }
                                else
                                {
                                    this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                                    this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                                    this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);

                                    activityHistories.Add(new SystemActivityHistory()
                                    {
                                        Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                                        EntityId = schedule.Identifier,
                                        EntityName = schedule.Name,
                                        SubEntityId = batch.Identifier,
                                        SubEntityName = batch.BatchName,
                                        ActionTaken = "AutoScheduleRun - Batch data not availalble error",
                                    });
                                    this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
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
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        public bool RunScheduleNow(BatchMaster batchMaster, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                var client = this.clientManager.GetClients(new ClientSearchParameter
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
                }, tenantCode).FirstOrDefault();
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings[ModelConstant.PARALLEL_THREAD_COUNT]);
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
        /// <returns>
        /// True if schedules runs successfully, false otherwise
        /// </returns>
        /// <exception cref="nIS.ScheduleNotFoundException"></exception>
        /// <exception cref="nIS.StatementNotFoundException"></exception>
        public bool RunScheduleNowNew(BatchMaster batchMaster, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            bool scheduleRunStatus = false;
            IList<SystemActivityHistory> activityHistories = new List<SystemActivityHistory>();

            try
            {
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings[ModelConstant.PARALLEL_THREAD_COUNT]);
                var scheduleRecord = this.scheduleRepository.GetSchedules(new ScheduleSearchParameter()
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
                }, tenantCode)?.FirstOrDefault();
                if (scheduleRecord == null)
                {
                    throw new ScheduleNotFoundException(tenantCode);
                }

                var batch = this.scheduleRepository.GetBatches(new BatchSearchParameter()
                {
                    Identifier = batchMaster.Identifier.ToString(),
                    ScheduleId = batchMaster.ScheduleId.ToString(),
                    Status = BatchStatus.New.ToString(),
                    IsExecuted = false
                }, tenantCode)?.FirstOrDefault();
                if (batch != null)
                {
                    var scheduleLog = new ScheduleLog();
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

                    var scheduleLogs = new List<ScheduleLog>();
                    scheduleLogs.Add(scheduleLog);
                    this.scheduleLogRepository.SaveScheduleLog(scheduleLogs, tenantCode);

                    if (!IsDataAvail)
                    {
                        activityHistories.Add(new SystemActivityHistory()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = scheduleRecord.Identifier,
                            EntityName = scheduleRecord.Name,
                            SubEntityId = batch.Identifier,
                            SubEntityName = batch.BatchName,
                            ActionTaken = "ManualScheduleRun - Batch data not available error",
                        });
                        this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);

                        return scheduleRunStatus;
                    }

                    var statements = this.statementManager.GetStatements(new StatementSearchParameter
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
                    }, tenantCode);
                    if (statements.Count == 0)
                    {
                        activityHistories.Add(new SystemActivityHistory()
                        {
                            Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                            EntityId = scheduleRecord.Identifier,
                            EntityName = scheduleRecord.Name,
                            SubEntityId = batch.Identifier,
                            SubEntityName = batch.BatchName,
                            ActionTaken = "ManualScheduleRun - Statement not found error",
                        });
                        this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
                        throw new StatementNotFoundException(tenantCode);
                    }

                    var statement = statements.FirstOrDefault();
                    var functionName = string.Empty;
                    if (tenantConfiguration != null && !string.IsNullOrEmpty(tenantConfiguration.GenerateStatementRunNowScheduleFunctionName))
                    {
                        functionName = tenantConfiguration.GenerateStatementRunNowScheduleFunctionName;
                    }

                    switch (functionName)
                    {
                        case ModelConstant.GENERATE_FINANCIAL_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW:
                            scheduleRunStatus = this.GenerateFinancialTenantCustomerStatementsByScheduleRunNow(statement, tenantConfiguration, batch, scheduleRecord, tenantCode, baseURL, outputLocation, parallelThreadCount);
                            break;

                        case ModelConstant.GENERATE_NEDBANK_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW:
                            scheduleRunStatus = this.GenerateNedbankCustomerStatementsByScheduleRunNow(statement, tenantConfiguration, batch, scheduleRecord, tenantCode, baseURL, outputLocation, parallelThreadCount);
                            break;

                        default:
                            scheduleRunStatus = this.GenerateFinancialTenantCustomerStatementsByScheduleRunNow(statement, tenantConfiguration, batch, scheduleRecord, tenantCode, baseURL, outputLocation, parallelThreadCount);
                            break;
                    }

                    activityHistories.Add(new SystemActivityHistory()
                    {
                        Module = ModelConstant.SCHEDULE_MODEL_SECTION,
                        EntityId = scheduleRecord.Identifier,
                        EntityName = scheduleRecord.Name,
                        SubEntityId = batch.Identifier,
                        SubEntityName = batch.BatchName,
                        ActionTaken = "ManualScheduleRunSuccess",
                    });
                    this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
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
        /// <param name="ScheduleIdentifier">The schedule identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
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
        /// <param name="ScheduleLogIdentifier">The schedule log identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
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
        /// <param name="scheduleIdentifer">The schedule identifer.</param>
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
        /// Gets the batch masters by language.
        /// </summary>
        /// <param name="scheduleIdentifer">The schedule identifer.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public IList<BatchMaster> GetBatchMastersByLanguage(long scheduleIdentifer, string tenantCode)
        {
            IList<BatchMaster> batchMasters = new List<BatchMaster>();
            try
            {
                batchMasters = this.scheduleRepository.GetBatchMastersByLanguage(scheduleIdentifer, tenantCode);
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
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// return list of batches
        /// </returns>
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
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
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
        /// This method helps to approve batch of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
        public bool ValidateApproveScheduleBatch(long BatchIdentifier, string tenantCode)
        {
            try
            {
                return this.scheduleRepository.ValidateApproveScheduleBatch(BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to clean batch and related data of the respective schedule.
        /// </summary>
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
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
        /// <param name="BatchIdentifier">The batch identifier.</param>
        /// <param name="Status">The status.</param>
        /// <param name="IsExecuted">if set to <c>true</c> [is executed].</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// True if success, otherwise false
        /// </returns>
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
        /// <param name="schedules">The schedules.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <exception cref="nIS.NullArgumentException"></exception>
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
        /// <param name="schedules">The schedules.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <exception cref="nIS.DuplicateScheduleFoundException"></exception>
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
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="parallelOptions">parallel option object of threading</param>
        /// <param name="parallelRequests">the list of customer parallel request object</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        private void ParalllelProcessing(GenerateStatementRawData statementRawData, string tenantCode, ParallelOptions parallelOptions, List<CustomerParallelRequest> parallelRequests, int parallelThreadCount)
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
        private void CallGenearateStatementWebAPI(GenerateStatementRawData statementRawData, string TenantCode, CustomerParallelRequest parallelRequest, int parallelThreadCount)
        {
            try
            {
                var renderEngine = parallelRequest.RenderEngine;
                string RenderEngineBaseUrl = string.IsNullOrEmpty(renderEngine?.URL) ? ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString() : renderEngine?.URL;

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
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
                    client.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, TenantCode);
                    var response = client.PostAsync(ModelConstant.CREATE_CUSTOMER_STATEMENT_API_URL, new StringContent(JsonConvert.SerializeObject(newStatementRawData), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to process request in parallel
        /// </summary>
        /// <param name="statementRawData">raw data object requires in statement generate process</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="parallelOptions">parallel option object of threading</param>
        /// <param name="parallelRequests">the list of customer parallel request object</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        private void DM_Customer_ParalllelProcessing(GenerateStatementRawData statementRawData, string tenantCode, ParallelOptions parallelOptions, List<DM_CustomerParallelRequest> parallelRequests, int parallelThreadCount)
        {
            Parallel.ForEach(parallelRequests, parallelOptions, item =>
            {
                CallGenearateNedbankStatementWebAPI(statementRawData, tenantCode, item, parallelThreadCount);
            });
        }

        /// <summary>
        /// This method helps to call web api of create customer statement file
        /// </summary>
        /// <param name="statementRawData">raw data object requires in statement generate process</param>
        /// <param name="TenantCode">The tenant code</param>
        /// <param name="parallelRequest">the customer parallel request object</param>
        /// <param name="parallelThreadCount">the thread count to run request in parallel</param>
        private void CallGenearateNedbankStatementWebAPI(GenerateStatementRawData statementRawData, string TenantCode, DM_CustomerParallelRequest parallelRequest, int parallelThreadCount)
        {
            try
            {
                var renderEngine = parallelRequest.RenderEngine;
                string RenderEngineBaseUrl = string.IsNullOrEmpty(renderEngine?.URL) ? ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString() : renderEngine?.URL;

                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;

                Parallel.ForEach(parallelRequest.DM_Customers, parallelOptions, customer =>
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
                        DM_Customer = customer,
                        RenderEngine = parallelRequest.RenderEngine
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(RenderEngineBaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
                    client.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, TenantCode);
                    var response = client.PostAsync(ModelConstant.CREATE_NEDBANK_CUSTOMER_STATEMENT_API_URL, new StringContent(JsonConvert.SerializeObject(newStatementRawData), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generates the financial tenant customer statements by schedule run now.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="scheduleRecord">The schedule record.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        /// <returns></returns>
        private bool GenerateFinancialTenantCustomerStatementsByScheduleRunNow(Statement statement, TenantConfiguration tenantConfiguration, BatchMaster batch, Schedule scheduleRecord, string tenantCode, string baseURL, string outputLocation, int parallelThreadCount)
        {
            try
            {
                var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                if (statementPageContents.Count > 0)
                {
                    var client = this.clientManager.GetClients(new ClientSearchParameter
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
                    }, tenantCode).FirstOrDefault();
                    var statementPreviewData = this.statementManager.BindDataToCommonStatement(statement, statementPageContents, tenantConfiguration, tenantCode, client);
                    string fileName = "Statement_" + statement.Identifier + "_" + batch.Identifier + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                    var filesDict = new Dictionary<string, string>();
                    for (int i = 0; i < statementPreviewData.SampleFiles.Count; i++)
                    {
                        filesDict.Add(statementPreviewData.SampleFiles[i].FileName, statementPreviewData.SampleFiles[i].FileUrl);
                    }
                    string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batch.Identifier, baseURL, outputLocation, filesDict);

                    var scheduleLog = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
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
                    }, tenantCode).ToList().FirstOrDefault();
                    scheduleLog.ScheduleId = scheduleRecord.Identifier;

                    var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
                    var BatchDetails = this.tenantTransactionDataRepository.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);

                    var customers = this.tenantTransactionDataRepository.Get_CustomerMasters(new CustomerSearchParameter()
                    {
                        BatchId = batch.Identifier,
                    }, tenantCode);
                    var scheduleRunStartTime = DateTime.UtcNow;
                    if (customers.Count > 0)
                    {
                        long CustomerCount = customers.Count;
                        var statementRawData = new GenerateStatementRawData()
                        {
                            Statement = statement,
                            ScheduleLog = scheduleLog,
                            StatementPageContents = statementPageContents,
                            Batch = batch,
                            BatchDetails = BatchDetails,
                            BaseURL = baseURL,
                            CustomerCount = CustomerCount,
                            OutputLocation = outputLocation,
                            TenantConfiguration = tenantConfiguration,
                            Client = client,
                            TenantEntities = tenantEntities,
                        };

                        //NIS engine implementation logic
                        bool IsWantToUseNisEngines = true;
                        if (ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES] != null)
                        {
                            bool.TryParse(ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES], out IsWantToUseNisEngines);
                        }

                        if (IsWantToUseNisEngines)
                        {
                            var NisEngines = this.renderEngineRepository.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                            if (NisEngines.Count > 0)
                            {
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    var availableNisEngines = new List<RenderEngine>(NisEngines);
                                    ParallelOptions parallelOptions = new ParallelOptions();
                                    if (customers.Count > availableNisEngines.Count * parallelThreadCount)
                                    {
                                        parallelOptions.MaxDegreeOfParallelism = availableNisEngines.Count;
                                        var parallelRequest = new List<CustomerParallelRequest>();
                                        int count = 0;
                                        for (int j = 1; availableNisEngines.Count > 0; j++)
                                        {
                                            parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines.FirstOrDefault() });
                                            customers = customers.Skip(parallelThreadCount).ToList();
                                            count += 1;
                                            availableNisEngines = availableNisEngines.Skip(count).ToList();
                                        }

                                        ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
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
                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = customers.Skip(parallelThreadCount).ToList();
                                                count += 1;
                                            }
                                            else
                                            {
                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = new List<CustomerMaster>();
                                            }
                                        }

                                        ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                    }
                                }
                            }
                            else
                            {
                                RenderEngine renderEngine = new RenderEngine()
                                {
                                    Identifier = 0,
                                    RenderEngineName = "DeFault NIS Engine",
                                    URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                    IsActive = true,
                                    IsDeleted = false,
                                    InUse = false,
                                    NumberOfThread = 1,
                                    PriorityLevel = 1
                                };
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    ParallelOptions parallelOptions = new ParallelOptions();
                                    parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                    var parallelRequest = new List<CustomerParallelRequest>();
                                    parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                    customers = customers.Skip(parallelThreadCount).ToList();
                                    ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                            }
                        }
                        else
                        {
                            RenderEngine renderEngine = new RenderEngine()
                            {
                                Identifier = 0,
                                RenderEngineName = "DeFault NIS Engine",
                                URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                IsActive = true,
                                IsDeleted = false,
                                InUse = false,
                                NumberOfThread = 1,
                                PriorityLevel = 1
                            };
                            for (int i = 0; customers.Count > 0; i++)
                            {
                                ParallelOptions parallelOptions = new ParallelOptions();
                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                var parallelRequest = new List<CustomerParallelRequest>();
                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                customers = customers.Skip(parallelThreadCount).ToList();
                                ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                            }
                        }


                        var scheduleRunHistory = new List<ScheduleRunHistory>();
                        scheduleRunHistory.Add(new ScheduleRunHistory()
                        {
                            StartDate = scheduleRunStartTime,
                            ScheduleId = scheduleRecord.Identifier,
                            StatementId = statement.Identifier,
                            ScheduleLogId = scheduleLog.Identifier,
                            EndDate = DateTime.UtcNow,
                            StatementFilePath = CommonStatementZipFilePath
                        });
                        this.scheduleRepository.AddScheduleRunHistorys(scheduleRunHistory, tenantCode);

                        //update status for respective schedule log, schedule log details entities as well as update batch status if statement generation done for all customers of current batch
                        var logDetailsRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
                        {
                            ScheduleLogId = statementRawData.ScheduleLog.Identifier.ToString(),
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
                        }, tenantCode);
                        var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                        var _batchStatus = BatchStatus.Completed.ToString();

                        var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                        if (failedRecords != null && failedRecords.Count > 0)
                        {
                            scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                            _batchStatus = BatchStatus.Failed.ToString();
                        }

                        this.scheduleLogRepository.UpdateScheduleLogStatus(statementRawData.ScheduleLog.Identifier, scheduleLogStatus, tenantCode);
                        this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, _batchStatus, true, tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(statementRawData.ScheduleLog.ScheduleId, ScheduleStatus.Completed.ToString(), tenantCode);
                    }
                    else
                    {
                        this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                        this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(scheduleRecord.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generates the financial tenant customer statements by schedule run time.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        private void GenerateFinancialTenantCustomerStatementsByScheduleRunTime(Statement statement, TenantConfiguration tenantConfiguration, BatchMaster batch, Schedule schedule, string tenantCode, string baseURL, string outputLocation, int parallelThreadCount)
        {
            try
            {
                var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                if (statementPageContents.Count > 0)
                {
                    var client = this.clientManager.GetClients(new ClientSearchParameter
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
                    }, tenantCode).FirstOrDefault();
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

                    var scheduleLog = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
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
                    }, tenantCode).ToList().FirstOrDefault();
                    scheduleLog.ScheduleId = schedule.Identifier;

                    var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
                    var BatchDetails = this.tenantTransactionDataRepository.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);

                    var customers = this.tenantTransactionDataRepository.Get_CustomerMasters(new CustomerSearchParameter()
                    {
                        BatchId = batch.Identifier,
                    }, tenantCode);
                    var scheduleRunStartTime = DateTime.UtcNow;
                    if (customers.Count > 0)
                    {
                        long CustomerCount = customers.Count;
                        var statementRawData = new GenerateStatementRawData()
                        {
                            Statement = statement,
                            ScheduleLog = scheduleLog,
                            StatementPageContents = statementPageContents,
                            Batch = batch,
                            BatchDetails = BatchDetails,
                            BaseURL = baseURL,
                            CustomerCount = CustomerCount,
                            OutputLocation = outputLocation,
                            TenantConfiguration = tenantConfiguration,
                            Client = client,
                            TenantEntities = tenantEntities,
                        };

                        //NIS engine implementation logic
                        bool IsWantToUseNisEngines = true;
                        if (ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES] != null)
                        {
                            bool.TryParse(ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES], out IsWantToUseNisEngines);
                        }

                        if (IsWantToUseNisEngines)
                        {
                            var NisEngines = this.renderEngineRepository.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                            if (NisEngines.Count > 0)
                            {
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    var availableNisEngines = new List<RenderEngine>(NisEngines);
                                    ParallelOptions parallelOptions = new ParallelOptions();

                                    if (customers.Count > availableNisEngines.Count * parallelThreadCount)
                                    {
                                        parallelOptions.MaxDegreeOfParallelism = availableNisEngines.Count;
                                        var parallelRequest = new List<CustomerParallelRequest>();
                                        int count = 0;
                                        for (int j = 1; availableNisEngines.Count > 0; j++)
                                        {
                                            parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines.FirstOrDefault() });
                                            customers = customers.Skip(parallelThreadCount).ToList();
                                            count += 1;
                                            availableNisEngines = availableNisEngines.Skip(count).ToList();
                                        }

                                        ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
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
                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = customers.Skip(parallelThreadCount).ToList();
                                                count += 1;
                                            }
                                            else
                                            {
                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = new List<CustomerMaster>();
                                            }
                                        }

                                        ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                    }
                                }
                            }
                            else
                            {
                                RenderEngine renderEngine = new RenderEngine()
                                {
                                    Identifier = 0,
                                    RenderEngineName = "DeFault NIS Engine",
                                    URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                    IsActive = true,
                                    IsDeleted = false,
                                    InUse = false,
                                    NumberOfThread = 1,
                                    PriorityLevel = 1
                                };
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    ParallelOptions parallelOptions = new ParallelOptions();
                                    parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                    var parallelRequest = new List<CustomerParallelRequest>();
                                    parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                    customers = customers.Skip(parallelThreadCount).ToList();
                                    ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                            }
                        }
                        else
                        {
                            RenderEngine renderEngine = new RenderEngine()
                            {
                                Identifier = 0,
                                RenderEngineName = "DeFault NIS Engine",
                                URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                IsActive = true,
                                IsDeleted = false,
                                InUse = false,
                                NumberOfThread = 1,
                                PriorityLevel = 1
                            };
                            for (int i = 0; customers.Count > 0; i++)
                            {
                                ParallelOptions parallelOptions = new ParallelOptions();
                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                var parallelRequest = new List<CustomerParallelRequest>();
                                parallelRequest.Add(new CustomerParallelRequest { Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                customers = customers.Skip(parallelThreadCount).ToList();
                                ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                            }
                        }

                        var scheduleRunHistory = new List<ScheduleRunHistory>();
                        scheduleRunHistory.Add(new ScheduleRunHistory()
                        {
                            StartDate = scheduleRunStartTime,
                            ScheduleId = schedule.Identifier,
                            StatementId = statement.Identifier,
                            ScheduleLogId = scheduleLog.Identifier,
                            EndDate = DateTime.UtcNow,
                            StatementFilePath = CommonStatementZipFilePath
                        });
                        this.scheduleRepository.AddScheduleRunHistorys(scheduleRunHistory, tenantCode);

                        //update status for respective schedule log, schedule log details entities as well as update batch status if statement generation done for all customers of current batch
                        var logDetailsRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
                        {
                            ScheduleLogId = statementRawData.ScheduleLog.Identifier.ToString(),
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
                        }, tenantCode);
                        var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                        var _batchStatus = BatchStatus.Completed.ToString();

                        var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                        if (failedRecords != null && failedRecords.Count > 0)
                        {
                            scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                            _batchStatus = BatchStatus.Failed.ToString();
                        }

                        this.scheduleLogRepository.UpdateScheduleLogStatus(statementRawData.ScheduleLog.Identifier, scheduleLogStatus, tenantCode);
                        this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, _batchStatus, true, tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(statementRawData.ScheduleLog.ScheduleId, ScheduleStatus.Completed.ToString(), tenantCode);
                    }
                    else
                    {
                        this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                        this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generates the nedbank customer statements by schedule run now.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="scheduleRecord">The schedule record.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        /// <returns></returns>
        private bool GenerateNedbankCustomerStatementsByScheduleRunNow(Statement statement, TenantConfiguration tenantConfiguration, BatchMaster batch, Schedule scheduleRecord, string tenantCode, string baseURL, string outputLocation, int parallelThreadCount)
        {
            try
            {
                var statementPageContents = this.statementManager.GenerateHtmlFormatOfNedbankStatement(statement, tenantCode, tenantConfiguration);
                if (statementPageContents.Count > 0)
                {
                    var statementPreviewData = this.statementManager.BindDataToCommonNedbankStatement(statement, statementPageContents, tenantCode);
                    string fileName = "Statement_" + statement.Identifier + "_" + batch.Identifier + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";

                    var filesDict = new Dictionary<string, string>();
                    for (int i = 0; i < statementPreviewData.SampleFiles.Count; i++)
                    {
                        filesDict.Add(statementPreviewData.SampleFiles[i].FileName, statementPreviewData.SampleFiles[i].FileUrl);
                    }
                    string CommonStatementZipFilePath = this.utility.CreateAndWriteToZipFile(statementPreviewData.FileContent, fileName, batch.Identifier, baseURL, outputLocation, filesDict);
                    var scheduleLog = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
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
                    }, tenantCode).ToList().FirstOrDefault();
                    scheduleLog.ScheduleId = scheduleRecord.Identifier;

                    var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
                    var customers = this.tenantTransactionDataRepository.Get_DM_CustomerMasters(new CustomerSearchParameter()
                    {
                        BatchId = batch.Identifier,
                    }, tenantCode);
                    var scheduleRunStartTime = DateTime.UtcNow;

                    if (customers != null && customers.Count > 0)
                    {
                        long CustomerCount = customers.Count;
                        var statementRawData = new GenerateStatementRawData()
                        {
                            Statement = statement,
                            ScheduleLog = scheduleLog,
                            StatementPageContents = statementPageContents,
                            Batch = batch,
                            BaseURL = baseURL,
                            CustomerCount = CustomerCount,
                            OutputLocation = outputLocation,
                            TenantConfiguration = tenantConfiguration,
                            TenantEntities = tenantEntities,
                        };

                        //NIS engine implementation logic
                        bool IsWantToUseNisEngines = true;
                        if (ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES] != null)
                        {
                            bool.TryParse(ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES], out IsWantToUseNisEngines);
                        }

                        if (IsWantToUseNisEngines)
                        {
                            var NisEngines = this.renderEngineRepository.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                            if (NisEngines.Count > 0)
                            {
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    var availableNisEngines = new List<RenderEngine>(NisEngines);
                                    ParallelOptions parallelOptions = new ParallelOptions();
                                    if (customers.Count > availableNisEngines.Count * parallelThreadCount)
                                    {
                                        parallelOptions.MaxDegreeOfParallelism = availableNisEngines.Count;
                                        var parallelRequest = new List<DM_CustomerParallelRequest>();
                                        int count = 0;
                                        for (int j = 1; availableNisEngines.Count > 0; j++)
                                        {
                                            parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines.FirstOrDefault() });
                                            customers = customers.Skip(parallelThreadCount).ToList();
                                            count += 1;
                                            availableNisEngines = availableNisEngines.Skip(count).ToList();
                                        }

                                        DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                    }
                                    else
                                    {
                                        parallelOptions.MaxDegreeOfParallelism = customers.ToList().Count % parallelThreadCount == 0 ? customers.ToList().Count / parallelThreadCount : customers.ToList().Count / parallelThreadCount + 1;
                                        var parallelRequest = new List<DM_CustomerParallelRequest>();
                                        int count = 0;

                                        for (int k = 0; customers.Count > 0; k++)
                                        {
                                            if (customers.Count > parallelThreadCount)
                                            {
                                                parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = customers.Skip(parallelThreadCount).ToList();
                                                count += 1;
                                            }
                                            else
                                            {
                                                parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = new List<DM_CustomerMaster>();
                                            }
                                        }

                                        DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                    }
                                }
                            }
                            else
                            {
                                RenderEngine renderEngine = new RenderEngine()
                                {
                                    Identifier = 0,
                                    RenderEngineName = "DeFault NIS Engine",
                                    URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                    IsActive = true,
                                    IsDeleted = false,
                                    InUse = false,
                                    NumberOfThread = 1,
                                    PriorityLevel = 1
                                };
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    ParallelOptions parallelOptions = new ParallelOptions();
                                    parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                    var parallelRequest = new List<DM_CustomerParallelRequest>();
                                    parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                    customers = customers.Skip(parallelThreadCount).ToList();
                                    DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                            }
                        }
                        else
                        {
                            RenderEngine renderEngine = new RenderEngine()
                            {
                                Identifier = 0,
                                RenderEngineName = "DeFault NIS Engine",
                                URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                IsActive = true,
                                IsDeleted = false,
                                InUse = false,
                                NumberOfThread = 1,
                                PriorityLevel = 1
                            };
                            for (int i = 0; customers.Count > 0; i++)
                            {
                                ParallelOptions parallelOptions = new ParallelOptions();
                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                var parallelRequest = new List<DM_CustomerParallelRequest>();
                                parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                customers = customers.Skip(parallelThreadCount).ToList();
                                DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                            }
                        }

                        var scheduleRunHistory = new List<ScheduleRunHistory>();
                        scheduleRunHistory.Add(new ScheduleRunHistory()
                        {
                            StartDate = scheduleRunStartTime,
                            ScheduleId = scheduleRecord.Identifier,
                            StatementId = statement.Identifier,
                            ScheduleLogId = scheduleLog.Identifier,
                            EndDate = DateTime.UtcNow,
                            StatementFilePath = CommonStatementZipFilePath
                        });
                        this.scheduleRepository.AddScheduleRunHistorys(scheduleRunHistory, tenantCode);

                        //update status for respective schedule log, schedule log details entities as well as update batch status if statement generation done for all customers of current batch
                        var logDetailsRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
                        {
                            ScheduleLogId = statementRawData.ScheduleLog.Identifier.ToString(),
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
                        }, tenantCode);
                        var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                        var _batchStatus = BatchStatus.Completed.ToString();

                        var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                        if (failedRecords != null && failedRecords.Count > 0)
                        {
                            scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                            _batchStatus = BatchStatus.Failed.ToString();
                        }

                        this.scheduleLogRepository.UpdateScheduleLogStatus(statementRawData.ScheduleLog.Identifier, scheduleLogStatus, tenantCode);
                        this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, _batchStatus, true, tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(statementRawData.ScheduleLog.ScheduleId, ScheduleStatus.Completed.ToString(), tenantCode);
                    }
                    else
                    {
                        this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                        this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(scheduleRecord.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generates the nedbank customer statements by schedule run time.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <param name="tenantConfiguration">The tenant configuration.</param>
        /// <param name="batch">The batch.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <param name="baseURL">The base URL.</param>
        /// <param name="outputLocation">The output location.</param>
        /// <param name="parallelThreadCount">The parallel thread count.</param>
        private void GenerateNedbankCustomerStatementsByScheduleRunTime(Statement statement, TenantConfiguration tenantConfiguration, BatchMaster batch, Schedule schedule, string tenantCode, string baseURL, string outputLocation, int parallelThreadCount)
        {
            try
            {
                var statementPageContents = this.statementManager.GenerateHtmlFormatOfNedbankStatement(statement, tenantCode, tenantConfiguration);
                if (statementPageContents.Count > 0)
                {
                    var statementPreviewData = this.statementManager.BindDataToCommonNedbankStatement(statement, statementPageContents, tenantCode);
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

                    var scheduleLog = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
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
                    }, tenantCode).ToList().FirstOrDefault();
                    scheduleLog.ScheduleId = schedule.Identifier;

                    var tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
                    var BatchDetails = this.tenantTransactionDataRepository.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);

                    var customers = this.tenantTransactionDataRepository.Get_DM_CustomerMasters(new CustomerSearchParameter()
                    {
                        BatchId = batch.Identifier,
                    }, tenantCode);
                    var scheduleRunStartTime = DateTime.UtcNow;
                    if (customers.Count > 0)
                    {
                        long CustomerCount = customers.Count;
                        var statementRawData = new GenerateStatementRawData()
                        {
                            Statement = statement,
                            ScheduleLog = scheduleLog,
                            StatementPageContents = statementPageContents,
                            Batch = batch,
                            BatchDetails = BatchDetails,
                            BaseURL = baseURL,
                            CustomerCount = CustomerCount,
                            OutputLocation = outputLocation,
                            TenantConfiguration = tenantConfiguration,
                            TenantEntities = tenantEntities,
                        };

                        //NIS engine implementation logic
                        bool IsWantToUseNisEngines = true;
                        if (ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES] != null)
                        {
                            bool.TryParse(ConfigurationManager.AppSettings[ModelConstant.IS_WANT_TO_USE_NIS_ENGINES], out IsWantToUseNisEngines);
                        }

                        if (IsWantToUseNisEngines)
                        {
                            var NisEngines = this.renderEngineRepository.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                            if (NisEngines.Count > 0)
                            {
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    var availableNisEngines = new List<RenderEngine>(NisEngines);
                                    ParallelOptions parallelOptions = new ParallelOptions();

                                    if (customers.Count > availableNisEngines.Count * parallelThreadCount)
                                    {
                                        parallelOptions.MaxDegreeOfParallelism = availableNisEngines.Count;
                                        var parallelRequest = new List<DM_CustomerParallelRequest>();
                                        int count = 0;
                                        for (int j = 1; availableNisEngines.Count > 0; j++)
                                        {
                                            parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines.FirstOrDefault() });
                                            customers = customers.Skip(parallelThreadCount).ToList();
                                            count += 1;
                                            availableNisEngines = availableNisEngines.Skip(count).ToList();
                                        }

                                        DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                    }
                                    else
                                    {
                                        parallelOptions.MaxDegreeOfParallelism = customers.ToList().Count % parallelThreadCount == 0 ? customers.ToList().Count / parallelThreadCount : customers.ToList().Count / parallelThreadCount + 1;
                                        var parallelRequest = new List<DM_CustomerParallelRequest>();
                                        int count = 0;

                                        for (int k = 0; customers.Count > 0; k++)
                                        {
                                            if (customers.Count > parallelThreadCount)
                                            {
                                                parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = customers.Skip(parallelThreadCount).ToList();
                                                count += 1;
                                            }
                                            else
                                            {
                                                parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.ToList(), RenderEngine = availableNisEngines[count] });
                                                customers = new List<DM_CustomerMaster>();
                                            }
                                        }

                                        DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                    }
                                }
                            }
                            else
                            {
                                RenderEngine renderEngine = new RenderEngine()
                                {
                                    Identifier = 0,
                                    RenderEngineName = "DeFault NIS Engine",
                                    URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                    IsActive = true,
                                    IsDeleted = false,
                                    InUse = false,
                                    NumberOfThread = 1,
                                    PriorityLevel = 1
                                };
                                for (int i = 0; customers.Count > 0; i++)
                                {
                                    ParallelOptions parallelOptions = new ParallelOptions();
                                    parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                    var parallelRequest = new List<DM_CustomerParallelRequest>();
                                    parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                    customers = customers.Skip(parallelThreadCount).ToList();
                                    DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                            }
                        }
                        else
                        {
                            RenderEngine renderEngine = new RenderEngine()
                            {
                                Identifier = 0,
                                RenderEngineName = "DeFault NIS Engine",
                                URL = ConfigurationManager.AppSettings[ModelConstant.DEFAULT_NIS_ENGINE_BASE_URL].ToString(),
                                IsActive = true,
                                IsDeleted = false,
                                InUse = false,
                                NumberOfThread = 1,
                                PriorityLevel = 1
                            };
                            for (int i = 0; customers.Count > 0; i++)
                            {
                                ParallelOptions parallelOptions = new ParallelOptions();
                                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                                var parallelRequest = new List<DM_CustomerParallelRequest>();
                                parallelRequest.Add(new DM_CustomerParallelRequest { DM_Customers = customers.Take(parallelThreadCount).ToList(), RenderEngine = renderEngine });
                                customers = customers.Skip(parallelThreadCount).ToList();
                                DM_Customer_ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                            }
                        }

                        var scheduleRunHistory = new List<ScheduleRunHistory>();
                        scheduleRunHistory.Add(new ScheduleRunHistory()
                        {
                            StartDate = scheduleRunStartTime,
                            ScheduleId = schedule.Identifier,
                            StatementId = statement.Identifier,
                            ScheduleLogId = scheduleLog.Identifier,
                            EndDate = DateTime.UtcNow,
                            StatementFilePath = CommonStatementZipFilePath
                        });
                        this.scheduleRepository.AddScheduleRunHistorys(scheduleRunHistory, tenantCode);

                        //update status for respective schedule log, schedule log details entities as well as update batch status if statement generation done for all customers of current batch
                        var logDetailsRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
                        {
                            ScheduleLogId = statementRawData.ScheduleLog.Identifier.ToString(),
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
                        }, tenantCode);
                        var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                        var _batchStatus = BatchStatus.Completed.ToString();

                        var failedRecords = logDetailsRecords.Where(item => item.Status == ScheduleLogStatus.Failed.ToString())?.ToList();
                        if (failedRecords != null && failedRecords.Count > 0)
                        {
                            scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                            _batchStatus = BatchStatus.Failed.ToString();
                        }

                        this.scheduleLogRepository.UpdateScheduleLogStatus(statementRawData.ScheduleLog.Identifier, scheduleLogStatus, tenantCode);
                        this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, _batchStatus, true, tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(statementRawData.ScheduleLog.ScheduleId, ScheduleStatus.Completed.ToString(), tenantCode);
                    }
                    else
                    {
                        this.scheduleRepository.UpdateBatchStatus(batch.Identifier, BatchStatus.BatchDataNotAvailable.ToString(), false, tenantCode);
                        this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, ScheduleLogStatus.BatchDataNotAvailable.ToString(), tenantCode);
                        this.scheduleRepository.UpdateScheduleStatus(schedule.Identifier, ScheduleStatus.BatchDataNotAvailable.ToString(), tenantCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public class CustomerParallelRequest
        {
            /// <summary>
            /// Gets or sets the customers.
            /// </summary>
            /// <value>
            /// The customers.
            /// </value>
            public List<CustomerMaster> Customers { get; set; }
            /// <summary>
            /// Gets or sets the render engine.
            /// </summary>
            /// <value>
            /// The render engine.
            /// </value>
            public RenderEngine RenderEngine { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class DM_CustomerParallelRequest
        {
            /// <summary>
            /// Gets or sets the dm customers.
            /// </summary>
            /// <value>
            /// The dm customers.
            /// </value>
            public List<DM_CustomerMaster> DM_Customers { get; set; }
            /// <summary>
            /// Gets or sets the render engine.
            /// </summary>
            /// <value>
            /// The render engine.
            /// </value>
            public RenderEngine RenderEngine { get; set; }
        }
    }
}
