// <copyright file="ArchivalProcessManager.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class ArchivalProcessManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = null;

        /// <summary>
        /// The Client manager.
        /// </summary>
        private ClientManager clientManager = null;

        /// <summary>
        /// The archival process repository.
        /// </summary>
        private IArchivalProcessRepository archivalProcessRepository = null;

        /// <summary>
        /// The statement search manager.
        /// </summary>
        private StatementSearchManager statementSearchManager = null;

        /// <summary>
        /// The statement manager.
        /// </summary>
        private StatementManager statementManager = null;

        /// <summary>
        /// The schedule log manager.
        /// </summary>
        private ScheduleLogManager scheduleLogManager = null;

        /// <summary>
        /// The schedule manager.
        /// </summary>
        private ScheduleManager scheduleManager = null;

        /// <summary>
        /// The render engine manager.
        /// </summary>
        private RenderEngineManager renderEngineManager = null;

        /// <summary>
        /// The tenant transaction data manager.
        /// </summary>
        private TenantTransactionDataManager tenantTransactionDataManager = null;

        /// <summary>
        /// The tenant configuration manager.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public ArchivalProcessManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.archivalProcessRepository = this.unityContainer.Resolve<IArchivalProcessRepository>();
                this.clientManager = new ClientManager(unityContainer);
                this.statementSearchManager = new StatementSearchManager(unityContainer);
                this.statementManager = new StatementManager(unityContainer);
                this.scheduleManager = new ScheduleManager(unityContainer);
                this.scheduleLogManager = new ScheduleLogManager(unityContainer);
                this.renderEngineManager = new RenderEngineManager(unityContainer);
                this.tenantTransactionDataManager = new TenantTransactionDataManager(unityContainer);
                this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
                this.validationEngine = new ValidationEngine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        public bool RunArchivalProcess(string pdfStatementFilepath, string htmlStatementFilepath, TenantConfiguration tenantConfiguration, string tenantCode)
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
                var ParallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"] ?? "10");
                var MinimumArchivalPeriodDays = int.Parse(ConfigurationManager.AppSettings["MinimumArchivalPeriodDays"] ?? "30");
                return this.archivalProcessRepository.RunArchivalProcess(client, ParallelThreadCount, MinimumArchivalPeriodDays, pdfStatementFilepath, htmlStatementFilepath, tenantConfiguration, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method to convert HTML statements into the PDF statement files and archieve related to log and metadata.
        /// </summary>
        /// <param name="pdfStatementFilepath"></param>
        /// <param name="htmlStatementFilepath"></param>
        /// <param name="tenantConfiguration"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the archive process runs successfully, false otherwise
        /// </returns>
        public bool RunArchivalProcessNew(string pdfStatementFilepath, string htmlStatementFilepath, string tenantCode)
        {
            try
            {
                bool IsArchivalProcessDone = false;
                string outputlocation = string.Empty;
                TenantConfiguration tenantConfiguration = null;

                var ParallelThreadCount = int.Parse(ConfigurationManager.AppSettings["ThreadCountToGenerateStatementParallel"] ?? "10");
                var MinimumArchivalPeriodDays = int.Parse(ConfigurationManager.AppSettings["MinimumArchivalPeriodDays"] ?? "30");

                var clients = this.clientManager.GetClients(new ClientSearchParameter
                {
                    IsActive = true,
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
                }, tenantCode);
                if (clients.Count > 0)
                {
                    for (int idx = 0; idx < clients.Count; idx++)
                    {
                        if (clients[idx].IsTenantConfigured)
                        {
                            var client = clients[idx];
                            tenantCode = client.TenantCode;
                            tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                            if (tenantConfiguration != null)
                            {
                                if (!string.IsNullOrEmpty(tenantConfiguration.ArchivalPath))
                                {
                                    htmlStatementFilepath = tenantConfiguration.OutputHTMLPath;
                                    pdfStatementFilepath = tenantConfiguration.ArchivalPath;
                                }

                                if (tenantConfiguration.ArchivalPeriod != null && tenantConfiguration.ArchivalPeriod > MinimumArchivalPeriodDays)
                                {
                                    var archivalPeriod = tenantConfiguration.ArchivalPeriod ?? 0;
                                    var statementArchieveDate = DateTime.UtcNow.AddDays(-archivalPeriod);

                                    var statementSearchRecords = this.statementSearchManager.GetStatementSearchs(new StatementSearchSearchParameter()
                                    {
                                        StatementArchieveDate = statementArchieveDate,
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
                                    if (statementSearchRecords.Count > 0)
                                    {
                                        var customerIds = new List<long>();
                                        var results = statementSearchRecords.GroupBy(p => p.StatementId, (key, g) => new { StatementId = key, GroupedStatementMetadataRecords = g.ToList() }).ToList();
                                        if (results.Count > 0)
                                        {
                                            results.ForEach(res =>
                                            {
                                                if (res.GroupedStatementMetadataRecords.Count > 0)
                                                {
                                                    //get schedule log record
                                                    var scheduleLogId = res.GroupedStatementMetadataRecords.FirstOrDefault().ScheduleLogId;
                                                    var scheduleLog = this.scheduleLogManager.GetScheduleLogs(new ScheduleLogSearchParameter()
                                                    {
                                                        ScheduleLogId = scheduleLogId.ToString(),
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
                                                    }, tenantCode)?.FirstOrDefault();
                                                    if (scheduleLog != null)
                                                    {
                                                        var batchmaster = this.scheduleManager.GetBatches(new BatchSearchParameter()
                                                        {
                                                            Identifier = scheduleLog.BatchId.ToString(),
                                                            ScheduleId = scheduleLog.ScheduleId.ToString(),
                                                        }, tenantCode)?.FirstOrDefault();

                                                        //if batch is present and it's status is approved then only schedule log and statement metadata records should be archived.
                                                        if (batchmaster != null && batchmaster.Status == BatchStatus.Approved.ToString())
                                                        {
                                                            var statements = this.statementManager.GetStatements(new StatementSearchParameter
                                                            {
                                                                Identifier = res.StatementId,
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
                                                                var statementPageContents = this.statementManager.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                                                                customerIds = res.GroupedStatementMetadataRecords.Select(item => item.CustomerId).Distinct().ToList();

                                                                //insert schedule log into archive table
                                                                var scheduleLogArchive = new ScheduleLogArchive()
                                                                {
                                                                    ScheduleId = scheduleLog.ScheduleId,
                                                                    ScheduleName = scheduleLog.ScheduleName,
                                                                    BatchId = scheduleLog.BatchId,
                                                                    BatchName = scheduleLog.BatchName,
                                                                    ArchivalDate = DateTime.UtcNow,
                                                                    LogCreationDate = scheduleLog.CreateDate,
                                                                    LogFilePath = scheduleLog.LogFilePath,
                                                                    NumberOfRetry = scheduleLog.NumberOfRetry,
                                                                    Status = scheduleLog.ScheduleStatus,
                                                                    TenantCode = tenantCode
                                                                };
                                                                IList<ScheduleLogArchive> records = new List<ScheduleLogArchive>();
                                                                records.Add(scheduleLogArchive);
                                                                this.archivalProcessRepository.SaveScheduleLogArchieve(records, tenantCode);

                                                                //get schedule log archive
                                                                scheduleLogArchive = this.archivalProcessRepository.GetScheduleLogArchives(scheduleLog.ScheduleId, scheduleLog.BatchId, tenantCode)?.FirstOrDefault();

                                                                var BatchDetails = this.tenantTransactionDataManager.GetBatchDetails(batchmaster.Identifier, statement.Identifier, tenantCode);

                                                                if (customerIds.Count > 0)
                                                                {
                                                                    var schedule = this.scheduleManager.GetSchedules(new ScheduleSearchParameter()
                                                                    {
                                                                        Identifier = batchmaster.ScheduleId.ToString(),
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

                                                                    var archivalProcessRawData = new ArchivalProcessRawData()
                                                                    {
                                                                        Client = client,
                                                                        HtmlStatementFilepath = htmlStatementFilepath,
                                                                        PdfStatementFilepath = pdfStatementFilepath,
                                                                        ScheduleLog = scheduleLog,
                                                                        ScheduleLogArchive = scheduleLogArchive,
                                                                        Statement = statement,
                                                                        StatementPageContents = statementPageContents,
                                                                        TenantConfiguration = tenantConfiguration,
                                                                        BatchMaster = batchmaster,
                                                                        Schedule = schedule,
                                                                        BatchDetails = BatchDetails
                                                                    };

                                                                    //nIS engine implementation logic
                                                                    for (int i = 0; customerIds.Count > 0; i++)
                                                                    {
                                                                        var availableNisEngines = this.renderEngineManager.GetRenderEngine(tenantCode).Where(item => item.IsActive && !item.IsDeleted).ToList();
                                                                        ParallelOptions parallelOptions = new ParallelOptions();

                                                                        if (customerIds.Count > availableNisEngines.Count * ParallelThreadCount)
                                                                        {
                                                                            parallelOptions.MaxDegreeOfParallelism = availableNisEngines.Count;
                                                                            var parallelRequest = new List<CustomerParallelRequest>();
                                                                            int count = 0;
                                                                            for (int j = 1; availableNisEngines.Count > 0; j++)
                                                                            {
                                                                                parallelRequest.Add(new CustomerParallelRequest { Customers = customerIds.Take(ParallelThreadCount).ToList(), RenderEngine = availableNisEngines.FirstOrDefault() });
                                                                                customerIds = customerIds.Skip(ParallelThreadCount).ToList();
                                                                                count += 1;
                                                                                availableNisEngines = availableNisEngines.Skip(count).ToList();
                                                                            }

                                                                            ParalllelProcessing(archivalProcessRawData, tenantCode, parallelOptions, parallelRequest, ParallelThreadCount);
                                                                        }
                                                                        else
                                                                        {
                                                                            parallelOptions.MaxDegreeOfParallelism = customerIds.ToList().Count % ParallelThreadCount == 0 ? customerIds.ToList().Count / ParallelThreadCount : customerIds.ToList().Count / ParallelThreadCount + 1;
                                                                            var parallelRequest = new List<CustomerParallelRequest>();
                                                                            int count = 0;

                                                                            for (int k = 0; customerIds.Count > 0; k++)
                                                                            {
                                                                                if (customerIds.Count > ParallelThreadCount)
                                                                                {
                                                                                    parallelRequest.Add(new CustomerParallelRequest { Customers = customerIds.Take(ParallelThreadCount).ToList(), RenderEngine = availableNisEngines[count] });
                                                                                    customerIds = customerIds.Skip(ParallelThreadCount).ToList();
                                                                                    count += 1;
                                                                                }
                                                                                else
                                                                                {
                                                                                    parallelRequest.Add(new CustomerParallelRequest { Customers = customerIds.ToList(), RenderEngine = availableNisEngines[count] });
                                                                                    customerIds = new List<long>();
                                                                                }
                                                                            }

                                                                            ParalllelProcessing(archivalProcessRawData, tenantCode, parallelOptions, parallelRequest, ParallelThreadCount);
                                                                        }
                                                                    }

                                                                    this.scheduleManager.UpdateBatchStatus(batchmaster.Identifier, BatchStatus.Approved.ToString(), true, tenantCode);
                                                                    this.scheduleLogManager.DeleteScheduleLog(scheduleLog.Identifier, tenantCode);

                                                                    IsArchivalProcessDone = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    
                                                }
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                }

                return IsArchivalProcessDone;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method adds the specified list of schedule log archive in the repository.
        /// </summary>
        /// <param name="scheduleLogArchives"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log archive values are added successfully, false otherwise
        /// </returns>
        public bool SaveScheduleLogArchieve(IList<ScheduleLogArchive> scheduleLogArchives, string tenantCode)
        {
            try
            {
                return this.archivalProcessRepository.SaveScheduleLogArchieve(scheduleLogArchives, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of schedule log archive records from repository.
        /// </summary>
        /// <param name="ScheduleId">The schedule identifier</param>
        /// <param name="BatchId">The schedule identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of schedule log archive records
        /// </returns>
        public IList<ScheduleLogArchive> GetScheduleLogArchives(long ScheduleId, long BatchId, string tenantCode)
        {
            try
            {
                return this.archivalProcessRepository.GetScheduleLogArchives(ScheduleId, BatchId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method adds the specified list of schedule log detail archieve in the repository.
        /// </summary>
        /// <param name="scheduleLogDetailArchieves"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the schedule log details archieve values are added successfully, false otherwise
        /// </returns>
        public bool SaveScheduleLogDetailsArchieve(IList<ScheduleLogDetailArchieve> scheduleLogDetailArchieves, string tenantCode)
        {
            try
            {
                return this.archivalProcessRepository.SaveScheduleLogDetailsArchieve(scheduleLogDetailArchieves, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method adds the specified list of statement metadata archieve in the repository.
        /// </summary>
        /// <param name="statementMetadataArchives"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the statement metadata archieve values are added successfully, false otherwise
        /// </returns>
        public bool SaveStatementMetadataArchieve(IList<StatementMetadataArchive> statementMetadataArchives, string tenantCode)
        {
            try
            {
                return this.archivalProcessRepository.SaveStatementMetadataArchieve(statementMetadataArchives, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ParalllelProcessing(ArchivalProcessRawData archivalProcessRawData, string tenantCode, ParallelOptions parallelOptions, List<CustomerParallelRequest> parallelRequests, int parallelThreadCount)
        {
            Parallel.ForEach(parallelRequests, parallelOptions, item =>
            {
                CallGenearateStatementWebAPI(archivalProcessRawData, tenantCode, item, parallelThreadCount);
            });
        }

        /// <summary>
        /// This method helps to call web api of create customer statement file
        /// </summary>
        /// <param name="archivalProcessRawData">raw data object requires in archival process</param>
        /// <param name="TenantCode">The tenant code</param>
        /// <param name="parallelRequest">the customer parallel request object</param>
        /// <param name="parallelThreadCount">the thread count to run request in parallel</param>
        public void CallGenearateStatementWebAPI(ArchivalProcessRawData archivalProcessRawData, string TenantCode, CustomerParallelRequest parallelRequest, int parallelThreadCount)
        {
            try
            {
                var renderEngine = parallelRequest.RenderEngine;
                string RenderEngineBaseUrl = string.IsNullOrEmpty(renderEngine?.URL) ? ConfigurationManager.AppSettings["DefaultGenerateStatementApiUrl"].ToString() : renderEngine?.URL;

                ParallelOptions parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = parallelThreadCount;

                Parallel.ForEach(parallelRequest.Customers, parallelOptions, CustomerId =>
                {
                    var newArchivalProcessRawData = new ArchivalProcessRawData()
                    {
                        Statement = archivalProcessRawData.Statement,
                        ScheduleLog = archivalProcessRawData.ScheduleLog,
                        StatementPageContents = archivalProcessRawData.StatementPageContents,
                        TenantConfiguration = archivalProcessRawData.TenantConfiguration,
                        Client = archivalProcessRawData.Client,
                        CustomerId = CustomerId,
                        HtmlStatementFilepath = archivalProcessRawData.HtmlStatementFilepath,
                        PdfStatementFilepath = archivalProcessRawData.PdfStatementFilepath,
                        ScheduleLogArchive = archivalProcessRawData.ScheduleLogArchive,
                        RenderEngine = parallelRequest.RenderEngine,
                        BatchMaster = archivalProcessRawData.BatchMaster,
                        Schedule = archivalProcessRawData.Schedule,
                        BatchDetails = archivalProcessRawData.BatchDetails
                    };

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(RenderEngineBaseUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("TenantCode", TenantCode);
                    var response = client.PostAsync("GenerateStatement/RunArchivalForCustomerRecord", new StringContent(JsonConvert.SerializeObject(newArchivalProcessRawData), Encoding.UTF8, "application/json")).Result;
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
            public List<long> Customers { get; set;}
            public RenderEngine RenderEngine { get; set; }
        }

    }
}
