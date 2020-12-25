// <copyright file="ScheduleLogManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
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
    /// This class implements manager layer of schedule log manager.
    /// </summary>
    public class ScheduleLogManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The schedule log repository.
        /// </summary>
        IScheduleLogRepository scheduleLogRepository = null;

        /// <summary>
        /// The Client manager.
        /// </summary>
        private ClientManager clientManager = null;

        /// <summary>
        /// The tenant configuration manager.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The schedule manager.
        /// </summary>
        private ScheduleManager scheduleManager = null;

        /// <summary>
        /// The tenant transaction manager.
        /// </summary>
        private TenantTransactionDataManager tenantTransactionDataManager = null;

        /// <summary>
        /// The statement manager.
        /// </summary>
        private StatementManager statementManager = null;

        /// <summary>
        /// The render engine manager.
        /// </summary>
        private RenderEngineManager renderEngineManager = null;

        /// <summary>
        /// The dynamic widget manager.
        /// </summary>
        private DynamicWidgetManager dynamicWidgetManager = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;


        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for schedule log manager, which initialise
        /// schedule repository.
        /// </summary>
        /// <param name="unityContainer">IUnity container implementation object.</param>
        public ScheduleLogManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.scheduleLogRepository = this.unityContainer.Resolve<IScheduleLogRepository>();
                this.clientManager = this.unityContainer.Resolve<ClientManager>();
                this.tenantConfigurationManager = this.unityContainer.Resolve<TenantConfigurationManager>();
                this.scheduleManager = this.unityContainer.Resolve<ScheduleManager>();
                this.tenantTransactionDataManager = this.unityContainer.Resolve<TenantTransactionDataManager>();
                this.statementManager = this.unityContainer.Resolve<StatementManager>();
                this.renderEngineManager = this.unityContainer.Resolve<RenderEngineManager>();
                this.dynamicWidgetManager = this.unityContainer.Resolve<DynamicWidgetManager>();
                this.utility = new Utility();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    scheduleLogSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }
                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }
                scheduleLogs = this.scheduleLogRepository.GetScheduleLogs(scheduleLogSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return scheduleLogs;
        }

        /// <summary>
        /// This method reference to get schedule log count
        /// </summary>
        /// <param name="scheduleLogSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Schedule count</returns>
        public int GetScheduleLogsCount(ScheduleLogSearchParameter scheduleLogSearchParameter, string tenantCode)
        {
            int scheduleLogCount = 0;
            try
            {
                scheduleLogCount = this.scheduleLogRepository.GetScheduleLogsCount(scheduleLogSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
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
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    scheduleLogDetailSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }
                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }
                scheduleLogDetails = this.scheduleLogRepository.GetScheduleLogDetails(scheduleLogDetailSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
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
            try
            {
                scheduleLogDetailCount = this.scheduleLogRepository.GetScheduleLogDetailsCount(scheduleLogDetailSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
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
        public bool RetryStatementForFailedCustomerReocrds(IList<ScheduleLogDetail> scheduleLogDetails, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            bool retryStatementStatus = false;

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

                ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter = new ScheduleLogDetailSearchParameter()
                {
                    ScheduleLogDetailId = string.Join(",", scheduleLogDetails.Select(item => item.Identifier).Distinct()).ToString(),
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
                var scheduleLogDetailRecords = this.scheduleLogRepository.GetScheduleLogDetails(scheduleLogDetailSearchParameter, tenantCode);
                if (scheduleLogDetailRecords == null || scheduleLogDetailRecords.Count <= 0 || scheduleLogDetailRecords.Count() != string.Join(",", scheduleLogDetailRecords.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                {
                    throw new ScheduleLogDetailNotFoundException(tenantCode);
                }

                if (scheduleLogDetailRecords.Count > 0)
                {
                    var firstScheduleLogDetailRecord = scheduleLogDetailRecords.ToList().FirstOrDefault();

                    var scheduleSearchParameter = new ScheduleSearchParameter()
                    {
                        Identifier = firstScheduleLogDetailRecord.ScheduleId.ToString(),
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
                    var scheduleRecord = this.scheduleManager.GetSchedules(scheduleSearchParameter, tenantCode)?.FirstOrDefault();

                    ScheduleLogSearchParameter logSearchParameter = new ScheduleLogSearchParameter()
                    {
                        ScheduleLogId = firstScheduleLogDetailRecord.ScheduleLogId.ToString(),
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
                    var scheduleLog = this.scheduleLogRepository.GetScheduleLogs(logSearchParameter, tenantCode).ToList().FirstOrDefault();

                    BatchSearchParameter batchSearchParameter = new BatchSearchParameter()
                    {
                        Identifier = scheduleLog.BatchId.ToString()
                    };
                    var batch = this.scheduleManager.GetBatches(batchSearchParameter, tenantCode)?.FirstOrDefault();

                    if (batch != null)
                    {
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
                        if (statements.Count > 0)
                        {
                            var statement = statements.ToList().FirstOrDefault();
                            var BatchDetails = this.tenantTransactionDataManager.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);
                            
                            var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                            
                            var renderEngine = this.renderEngineManager.GetRenderEngine(tenantCode).FirstOrDefault();
                            var tenantEntities = this.dynamicWidgetManager.GetTenantEntities(tenantCode);

                            ParallelOptions parallelOptions = new ParallelOptions();
                            parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;
                            Parallel.ForEach(scheduleLogDetailRecords, parallelOptions, scheduleLogDetail =>
                            {
                                this.ReGenerateFailedCustomerStatements(scheduleLogDetail, statement, statementPageContents, batch, BatchDetails, baseURL, tenantCode, outputLocation, renderEngine, tenantEntities, tenantConfiguration, client);
                            });
                        }

                        retryStatementStatus = true;
                    }
                }

                return retryStatementStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to re run the schedule for failed customer records
        /// </summary>
        /// <param name="scheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="baseURL">The base URL</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if statements generates successfully runs successfully, false otherwise</returns>
        public bool ReRunScheduleForFailedCases(long scheduleLogIdentifier, string baseURL, string outputLocation, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            try
            {
                ScheduleLogDetailSearchParameter scheduleLogDetailSearchParameter = new ScheduleLogDetailSearchParameter()
                {
                    ScheduleLogId = scheduleLogIdentifier.ToString(),
                    Status = ScheduleLogStatus.Failed.ToString(),
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
                var failedScheduleLogDetailRecords = this.scheduleLogRepository.GetScheduleLogDetails(scheduleLogDetailSearchParameter, tenantCode);
                if (failedScheduleLogDetailRecords == null || failedScheduleLogDetailRecords.Count == 0)
                {
                    throw new ScheduleLogDetailNotFoundException(tenantCode);
                }

                return this.RetryStatementForFailedCustomerReocrds(failedScheduleLogDetailRecords, baseURL, outputLocation, tenantConfiguration, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to get error log message of schedule for failed customer records
        /// </summary>
        /// <param name="ScheduleLogIdentifier">The schedule log identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>list of schedule log error detail object</returns>
        public List<ScheduleLogErrorDetail> GetScheduleLogErrorDetails(long ScheduleLogIdentifier, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.GetScheduleLogErrorDetails(ScheduleLogIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to get dashboard data
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>dashboard data object</returns>
        public DashboardData GetDashboardData(string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.GetDashboardData(tenantCode);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void ReGenerateFailedCustomerStatements(ScheduleLogDetail scheduleLogDetail, Statement statement, IList<StatementPageContent> statementPageContents, BatchMaster batchMaster, IList<BatchDetail> batchDetails, string baseURL, string tenantCode, string outputLocation, RenderEngine renderEngine, IList<TenantEntity> tenantEntities, TenantConfiguration tenantConfiguration, Client client)
        {
            try
            {
                CustomerSearchParameter customerSearchParameter = new CustomerSearchParameter()
                {
                    Identifier = scheduleLogDetail.CustomerId,
                    BatchId = batchMaster.Identifier,
                };
                var customerMaster = this.tenantTransactionDataManager.Get_CustomerMasters(customerSearchParameter, tenantCode)?.FirstOrDefault();
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
                        TabClassName = it.TabClassName,
                        DynamicWidgets = it.DynamicWidgets
                    }));

                    var logDetailRecord = this.statementManager.GenerateStatements(customerMaster, statement, newStatementPageContents, batchMaster, batchDetails, baseURL, tenantCode, outputLocation, tenantConfiguration, client, tenantEntities);
                    if (logDetailRecord != null)
                    {
                        //delete un-neccessory files which are created during html statement generation in fail cases
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                        {
                            this.utility.DeleteUnwantedDirectory(batchMaster.Identifier, customerMaster.Identifier, outputLocation);
                        }

                        if (logDetailRecord != null)
                        {
                            //update schedule log detail
                            scheduleLogDetail.CustomerId = customerMaster.Identifier;
                            scheduleLogDetail.CustomerName = customerMaster.FirstName.Trim() + (customerMaster.MiddleName == string.Empty ? string.Empty : " " + customerMaster.MiddleName.Trim()) + " " + customerMaster.LastName.Trim();
                            scheduleLogDetail.RenderEngineId = renderEngine != null ? renderEngine.Identifier : 0; //To be change once render engine implmentation start
                            scheduleLogDetail.RenderEngineName = renderEngine != null ? renderEngine.RenderEngineName : string.Empty;
                            scheduleLogDetail.RenderEngineURL = renderEngine != null ? renderEngine.URL : string.Empty;
                            scheduleLogDetail.LogMessage = logDetailRecord.LogMessage;
                            scheduleLogDetail.Status = logDetailRecord.Status;
                            scheduleLogDetail.NumberOfRetry++;
                            scheduleLogDetail.StatementFilePath = logDetailRecord.StatementFilePath;
                            IList<ScheduleLogDetail> scheduleLogDetails = new List<ScheduleLogDetail>();
                            scheduleLogDetails.Add(scheduleLogDetail); 
                            this.scheduleLogRepository.UpdateScheduleLogDetails(scheduleLogDetails, tenantCode);

                            //save statement metadata if html statement generated successfully
                            if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()) && logDetailRecord.statementMetadata.Count > 0)
                            {
                                IList<StatementMetadata> statementMetadataRecords = new List<StatementMetadata>();
                                logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
                                {
                                    metarec.ScheduleLogId = scheduleLogDetail.ScheduleLogId;
                                    metarec.ScheduleId = scheduleLogDetail.ScheduleId;
                                    metarec.StatementDate = DateTime.UtcNow;
                                    metarec.StatementURL = scheduleLogDetail.StatementFilePath;
                                    metarec.TenantCode = tenantCode;
                                    statementMetadataRecords.Add(metarec);
                                });
                                this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
                            }

                            ScheduleLogSearchParameter logSearchParameter = new ScheduleLogSearchParameter()
                            {
                                ScheduleLogId = scheduleLogDetail.ScheduleLogId.ToString(),
                                BatchId = batchMaster.Identifier.ToString(),
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
                            var scheduleLogs = this.scheduleLogRepository.GetScheduleLogs(logSearchParameter, tenantCode).ToList();
                            scheduleLogs.ForEach(scheduleLog =>
                            {
                                //get total no. of schedule log details for current schedule log
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
                                var _lstScheduleLogDetail = this.scheduleLogRepository.GetScheduleLogDetails(scheduleLogDetailSearchParameter, tenantCode);

                                //get no of success schedule log details of current schedule log
                                var successRecords = _lstScheduleLogDetail.Where(item => item.Status == ScheduleLogStatus.Completed.ToString())?.ToList();

                                var batchStatus = BatchStatus.Completed.ToString();
                                var isBatchCompleteExecuted = true;
                                var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();
                                
                                //check success schedule log details count is equal to total no. of schedule log details for current schedule log
                                //if equals then update schedule log and batch status as completed otherwise failed
                                if (successRecords != null && successRecords.Count != _lstScheduleLogDetail.Count)
                                {
                                    scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
                                    batchStatus = BatchStatus.Failed.ToString();
                                    isBatchCompleteExecuted = false;
                                }

                                //update schedule log and batch status
                                this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, scheduleLogStatus, tenantCode);
                                this.scheduleManager.UpdateBatchStatus(batchMaster.Identifier, batchStatus, isBatchCompleteExecuted, tenantCode);
                            });

                        }
                    }
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
