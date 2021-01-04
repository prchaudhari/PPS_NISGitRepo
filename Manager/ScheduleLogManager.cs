// <copyright file="ScheduleLogManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Newtonsoft.Json;
    #region References
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
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
                var parallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"]);

                var scheduleLogDetailRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
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
                }, tenantCode);
                if (scheduleLogDetailRecords == null || scheduleLogDetailRecords.Count <= 0 || scheduleLogDetailRecords.Count() != string.Join(",", scheduleLogDetailRecords.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                {
                    throw new ScheduleLogDetailNotFoundException(tenantCode);
                }

                if (scheduleLogDetailRecords.Count > 0)
                {
                    var firstScheduleLogDetailRecord = scheduleLogDetailRecords.ToList().FirstOrDefault();
                    var scheduleLog = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
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
                    }, tenantCode).ToList().FirstOrDefault();
                    var batch = this.scheduleManager.GetBatches(new BatchSearchParameter()
                    {
                        Identifier = scheduleLog.BatchId.ToString()
                    }, tenantCode)?.FirstOrDefault();

                    if (batch != null)
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
                        var scheduleRecord = this.scheduleManager.GetSchedules(new ScheduleSearchParameter()
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
                        }, tenantCode)?.FirstOrDefault();
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
                        if (statements.Count > 0)
                        {
                            var statement = statements.ToList().FirstOrDefault();
                            var BatchDetails = this.tenantTransactionDataManager.GetBatchDetails(batch.Identifier, statement.Identifier, tenantCode);
                            
                            var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                            
                            var tenantEntities = this.dynamicWidgetManager.GetTenantEntities(tenantCode);

                            var statementRawData = new GenerateStatementRawData()
                            {
                                Statement = statement,
                                ScheduleLog = scheduleLog,
                                StatementPageContents = statementPageContents,
                                Batch = batch,
                                BatchDetails = BatchDetails,
                                BaseURL = baseURL,
                                OutputLocation = outputLocation,
                                TenantConfiguration = tenantConfiguration,
                                Client = client,
                                TenantEntities = tenantEntities,
                            };

                            //Render engine implementation logic
                            for (int i = 0; scheduleLogDetailRecords.Count > 0; i++)
                            {
                                var availableNisEngines = this.renderEngineManager.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                                ParallelOptions parallelOptions = new ParallelOptions();

                                if (scheduleLogDetailRecords.Count > availableNisEngines.Count * parallelThreadCount)
                                {
                                    parallelOptions.MaxDegreeOfParallelism = availableNisEngines.Count;
                                    var parallelRequest = new List<ScheduleLogDetailParallelRequest>();
                                    int count = 0;
                                    for (int j = 1; availableNisEngines.Count > 0; j++)
                                    {
                                        parallelRequest.Add(new ScheduleLogDetailParallelRequest { ScheduleLogDetails = scheduleLogDetailRecords.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines.FirstOrDefault() });
                                        scheduleLogDetailRecords = scheduleLogDetailRecords.Skip(parallelThreadCount).ToList();
                                        count += 1;
                                        availableNisEngines = availableNisEngines.Skip(count).ToList();
                                    }

                                    ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                                else
                                {
                                    parallelOptions.MaxDegreeOfParallelism = scheduleLogDetailRecords.ToList().Count % parallelThreadCount == 0 ? scheduleLogDetailRecords.ToList().Count / parallelThreadCount : scheduleLogDetailRecords.ToList().Count / parallelThreadCount + 1;
                                    var parallelRequest = new List<ScheduleLogDetailParallelRequest>();
                                    int count = 0;

                                    for (int k = 0; scheduleLogDetailRecords.Count > 0; k++)
                                    {
                                        if (scheduleLogDetailRecords.Count > parallelThreadCount)
                                        {
                                            parallelRequest.Add(new ScheduleLogDetailParallelRequest { ScheduleLogDetails = scheduleLogDetailRecords.Take(parallelThreadCount).ToList(), RenderEngine = availableNisEngines[count] });
                                            scheduleLogDetailRecords = scheduleLogDetailRecords.Skip(parallelThreadCount).ToList();
                                            count += 1;
                                        }
                                        else
                                        {
                                            parallelRequest.Add(new ScheduleLogDetailParallelRequest { ScheduleLogDetails = scheduleLogDetailRecords.ToList(), RenderEngine = availableNisEngines[count] });
                                            scheduleLogDetailRecords = new List<ScheduleLogDetail>();
                                        }
                                    }

                                    ParalllelProcessing(statementRawData, tenantCode, parallelOptions, parallelRequest, parallelThreadCount);
                                }
                            }
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

        /// <summary>
        /// This method adds the specified list of schedule log detail in the repository.
        /// </summary>
        /// <param name="scheduleLogDetails"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log details values are added successfully, false otherwise
        /// </returns>
        public bool SaveScheduleLogDetails(IList<ScheduleLogDetail> scheduleLogDetails, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.SaveScheduleLogDetails(scheduleLogDetails, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method adds the specified list of statement metadata in the repository.
        /// </summary>
        /// <param name="statementMetadata"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the statement metadata values are added successfully, false otherwise
        /// </returns>
        public bool SaveStatementMetadata(IList<StatementMetadata> statementMetadata, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.SaveStatementMetadata(statementMetadata, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to update schedule log status.
        /// </summary>
        /// <param name="ScheduleLogIdentifier"></param>
        /// <param name="Status"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True if success, otherwise false</returns>
        public bool UpdateScheduleLogStatus(long ScheduleLogIdentifier, string Status, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.UpdateScheduleLogStatus(ScheduleLogIdentifier, Status, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method update the specified list of schedule log detail in the repository.
        /// </summary>
        /// <param name="scheduleLogDetails"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log details values are updated successfully, false otherwise
        /// </returns>
        public bool UpdateScheduleLogDetails(IList<ScheduleLogDetail> scheduleLogDetails, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.UpdateScheduleLogDetails(scheduleLogDetails, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to remove schedule log records from repository.
        /// </summary>
        /// <param name="ScheduleLogId">The schedule log identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if records removed successfully..
        /// </returns>
        public bool DeleteScheduleLog(long ScheduleLogId, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.DeleteScheduleLog(ScheduleLogId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to remove schedule log details records from repository.
        /// </summary>
        /// <param name="ScheduleLogId">The schedule log identifier</param>
        /// <param name="CustomerId">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if records removed successfully..
        /// </returns>
        public bool DeleteScheduleLogDetails(long ScheduleLogId, long? CustomerId, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.DeleteScheduleLogDetails(ScheduleLogId, CustomerId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to remove statement metadata records from repository.
        /// </summary>
        /// <param name="ScheduleLogId">The schedule log identifier</param>
        /// <param name="CustomerId">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if records removed successfully..
        /// </returns>
        public bool DeleteStatementMetadata(long ScheduleLogId, long? CustomerId, string tenantCode)
        {
            try
            {
                return this.scheduleLogRepository.DeleteStatementMetadata(ScheduleLogId, CustomerId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method helps to process request in parallel
        /// </summary>
        /// <param name="statementRawData">raw data object requires in statement generate process</param>
        /// <param name="parallelOptions">parallel option object of threading</param>
        /// <param name="parallelRequests">the list of schedule log detail parallel request object</param>
        private void ParalllelProcessing(GenerateStatementRawData statementRawData, string tenantCode, ParallelOptions parallelOptions, List<ScheduleLogDetailParallelRequest> parallelRequests, int parallelThreadCount)
        {
            Parallel.ForEach(parallelRequests, parallelOptions, item =>
            {
                CallRetryToCreateFailedCustomerStatementsWebAPI(statementRawData, tenantCode, item, parallelThreadCount);
            });
        }

        /// <summary>
        /// This method helps to call web api of retry create statement for failed customer records
        /// </summary>
        /// <param name="statementRawData">raw data object requires in statement generate process</param>
        /// <param name="TenantCode">The tenant code</param>
        /// <param name="parallelRequest">the schedule log detail parallel request object</param>
        /// <param name="parallelThreadCount">the thread count to run request in parallel</param>
        private void CallRetryToCreateFailedCustomerStatementsWebAPI(GenerateStatementRawData statementRawData, string TenantCode, ScheduleLogDetailParallelRequest parallelRequest, int parallelThreadCount)
        {

            try
            {
                var renderEngine = parallelRequest.RenderEngine;
                string RenderEngineBaseUrl = string.IsNullOrEmpty(renderEngine?.URL) ? ConfigurationManager.AppSettings["DefaultGenerateStatementApiUrl"].ToString() : renderEngine?.URL;

                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;

                Parallel.ForEach(parallelRequest.ScheduleLogDetails, parallelOptions, logDetail =>
                {
                    var newStatementRawData = new GenerateStatementRawData()
                    {
                        Statement = statementRawData.Statement,
                        ScheduleLogDetail = logDetail,
                        StatementPageContents = statementRawData.StatementPageContents,
                        Batch = statementRawData.Batch,
                        BatchDetails = statementRawData.BatchDetails,
                        BaseURL = statementRawData.BaseURL,
                        CustomerCount = statementRawData.CustomerCount,
                        OutputLocation = statementRawData.OutputLocation,
                        TenantConfiguration = statementRawData.TenantConfiguration,
                        Client = statementRawData.Client,
                        TenantEntities = statementRawData.TenantEntities,
                        RenderEngine = parallelRequest.RenderEngine
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(RenderEngineBaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("TenantCode", TenantCode);
                    var response = client.PostAsync("GenerateStatement/RetryToCreateFailedCustomerStatements", new StringContent(JsonConvert.SerializeObject(newStatementRawData), Encoding.UTF8, "application/json")).Result;
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

        public class ScheduleLogDetailParallelRequest
        {
            public List<ScheduleLogDetail> ScheduleLogDetails { get; set; }
            public RenderEngine RenderEngine { get; set; }
        }
    }
}
