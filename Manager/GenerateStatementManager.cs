// <copyright file="GenerateStatementManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of generate statement manager.
    /// </summary>
    public class GenerateStatementManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = null;

        /// <summary>
        /// The dynamic widget repository object.
        /// </summary>
        private IDynamicWidgetRepository dynamicWidgetRepository = null;

        private IInvestmentRepository investmentRepository = null;
        private ICustomerRepository customerRepository = null;

        /// <summary>
        /// The tenant transaction data repository.
        /// </summary>
        private ITenantTransactionDataRepository tenantTransactionDataRepository = null;

        /// <summary>
        /// The schedule log repository.
        /// </summary>
        private IScheduleLogRepository scheduleLogRepository = null;

        /// <summary>
        /// The schedule repository.
        /// </summary>
        private IScheduleRepository scheduleRepository = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        private IAssetLibraryRepository assetLibraryRepository = null;

        /// <summary>
        /// The statement search manager.
        /// </summary>
        private StatementSearchManager statementSearchManager = null;

        /// <summary>
        /// The statement search repository.
        /// </summary>
        private IStatementSearchRepository statementSearchRepository = null;

        /// <summary>
        /// The archival process repository.
        /// </summary>
        private IArchivalProcessRepository archivalProcessRepository = null;

        /// <summary>
        /// The crypto manager
        /// </summary>
        private readonly ICryptoManager cryptoManager;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for generate statement manager, which initialise
        /// schedule repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public GenerateStatementManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.validationEngine = new ValidationEngine();
                this.utility = new Utility();
                this.statementSearchManager = this.unityContainer.Resolve<StatementSearchManager>();
                this.tenantTransactionDataRepository = this.unityContainer.Resolve<ITenantTransactionDataRepository>();
                this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
                this.archivalProcessRepository = this.unityContainer.Resolve<IArchivalProcessRepository>();
                this.scheduleRepository = this.unityContainer.Resolve<IScheduleRepository>();
                this.scheduleLogRepository = this.unityContainer.Resolve<IScheduleLogRepository>();
                this.dynamicWidgetRepository = this.unityContainer.Resolve<IDynamicWidgetRepository>();
                this.statementSearchRepository = this.unityContainer.Resolve<IStatementSearchRepository>();
                this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
                this.investmentRepository = this.unityContainer.Resolve<IInvestmentRepository>();
                this.customerRepository = this.unityContainer.Resolve<ICustomerRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to create HTML statement for given customer list.
        /// </summary>
        /// <param name="statementRawData"> the raw data object requires for statement generate process</param>
        /// <param name="tenantCode"></param>
        public void CreateCustomerStatement(GenerateStatementRawData statementRawData, string tenantCode)
        {
            IList<StatementMetadata> statementMetadataRecords = new List<StatementMetadata>();
            var customer = statementRawData.Customer;

            try
            {
                //call to generate actual HTML statement file for current customer record
                var logDetailRecord = this.GenerateStatements(statementRawData, tenantCode);
                if (logDetailRecord != null)
                {

                    //save schedule log details for current customer
                    var logDetails = new List<ScheduleLogDetail>();
                    logDetailRecord.ScheduleLogId = statementRawData.ScheduleLog.Identifier;
                    logDetailRecord.CustomerId = customer.Identifier;
                    logDetailRecord.CustomerName = customer.FirstName.Trim() + (customer.MiddleName == "" ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim();
                    logDetailRecord.ScheduleId = statementRawData.ScheduleLog.ScheduleId;
                    logDetailRecord.RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0;
                    logDetailRecord.RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "";
                    logDetailRecord.RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "";
                    logDetailRecord.NumberOfRetry = 1;
                    logDetailRecord.CreateDate = DateTime.UtcNow;
                    logDetails.Add(logDetailRecord);
                    this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                    //if statement generated successfully, then save statement metadata with actual html statement file path
                    if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()))
                    {
                        if (logDetailRecord.statementMetadata.Count > 0)
                        {
                            logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
                            {
                                metarec.ScheduleLogId = statementRawData.ScheduleLog.Identifier;
                                metarec.ScheduleId = statementRawData.ScheduleLog.ScheduleId;
                                metarec.StatementDate = DateTime.UtcNow;
                                metarec.StatementURL = logDetailRecord.StatementFilePath;
                                metarec.TenantCode = tenantCode;
                                metarec.IsPasswordGenerated = false;
                                metarec.Password = "";
                                statementMetadataRecords.Add(metarec);
                            });
                            this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
                        }
                    }

                    //If any error occurs during statement generation then delete all files from output directory of current customer html statement
                    else if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                    {
                        this.utility.DeleteUnwantedDirectory(statementRawData.Batch.Identifier, customer.Identifier, statementRawData.OutputLocation);
                    }
                }
            }
            catch (Exception ex)
            {
                //save schedule log details for current customer, if something went wrong while statement generation...
                var logDetails = new List<ScheduleLogDetail>();
                logDetails.Add(new ScheduleLogDetail()
                {
                    ScheduleLogId = statementRawData.ScheduleLog.Identifier,
                    CustomerId = customer.Identifier,
                    CustomerName = customer.FirstName.Trim() + (customer.MiddleName == "" ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim(),
                    ScheduleId = statementRawData.ScheduleLog.ScheduleId,
                    RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0,
                    RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "",
                    RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "",
                    NumberOfRetry = 1,
                    CreateDate = DateTime.UtcNow,
                    Status = ScheduleLogStatus.Failed.ToString(),
                    LogMessage = "Something went wrong while generating statement: " + ex.Message
                });
                this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                //write error log
                WriteToFile(ex.StackTrace.ToString());
                //throw ex;
            }
        }

        /// <summary>
        /// This method helps to create NedBank HTML statement for given customer list.
        /// </summary>
        /// <param name="statementRawData"> the raw data object requires for statement generate process</param>
        /// <param name="tenantCode"></param>
        public void CreateCustomerNedbankStatement(GenerateStatementRawData statementRawData, string tenantCode)
        {
            IList<StatementMetadata> statementMetadataRecords = new List<StatementMetadata>();
            var customer = statementRawData.DM_Customer;

            try
            {
                //call to generate actual NedBank HTML statement file for current customer record
                var logDetailRecord = this.GenerateNedbankStatements(statementRawData, tenantCode);
                if (logDetailRecord != null)
                {
                    //save schedule log details for current customer
                    var logDetails = new List<ScheduleLogDetail>();
                    logDetailRecord.ScheduleLogId = statementRawData.ScheduleLog.Identifier;
                    logDetailRecord.CustomerId = customer.CustomerId;
                    logDetailRecord.CustomerName = customer.FirstName.Trim() + " " + customer.SurName.Trim();
                    logDetailRecord.ScheduleId = statementRawData.ScheduleLog.ScheduleId;
                    logDetailRecord.RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0;
                    logDetailRecord.RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "";
                    logDetailRecord.RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "";
                    logDetailRecord.NumberOfRetry = 1;
                    logDetailRecord.CreateDate = DateTime.UtcNow;
                    logDetails.Add(logDetailRecord);
                    this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                    //if statement generated successfully, then save statement metadata with actual html statement file path
                    if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()))
                    {
                        if (logDetailRecord.statementMetadata.Count > 0)
                        {
                            logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
                            {
                                metarec.ScheduleLogId = statementRawData.ScheduleLog.Identifier;
                                metarec.ScheduleId = statementRawData.ScheduleLog.ScheduleId;
                                metarec.StatementDate = DateTime.UtcNow;
                                metarec.StatementURL = logDetailRecord.StatementFilePath;
                                metarec.TenantCode = tenantCode;
                                metarec.IsPasswordGenerated = false;
                                metarec.Password = "";
                                statementMetadataRecords.Add(metarec);
                            });
                            this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
                        }
                    }

                    //If any error occurs during statement generation then delete all files from output directory of current customer html statement
                    else if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                    {
                        this.utility.DeleteUnwantedDirectory(statementRawData.Batch.Identifier, customer.CustomerId, statementRawData.OutputLocation);
                    }
                }
            }
            catch (Exception ex)
            {
                //save schedule log details for current customer, if something went wrong while statement generation...
                var logDetails = new List<ScheduleLogDetail>();
                logDetails.Add(new ScheduleLogDetail()
                {
                    ScheduleLogId = statementRawData.ScheduleLog.Identifier,
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.FirstName.Trim() + " " + customer.SurName.Trim(),
                    ScheduleId = statementRawData.ScheduleLog.ScheduleId,
                    RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0,
                    RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "",
                    RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "",
                    NumberOfRetry = 1,
                    CreateDate = DateTime.UtcNow,
                    Status = ScheduleLogStatus.Failed.ToString(),
                    LogMessage = "Something went wrong while generating statement: " + ex.Message
                });
                this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                //write error log
                WriteToFile(ex.StackTrace.ToString());
                //throw ex;
            }
        }

        /// <summary>
        /// This method helps to retry to generate HTML statement for failed customer list.
        /// </summary>
        /// <param name="statementRawData"> the raw data object requires for statement generate process</param>
        /// <param name="tenantCode"></param>
        public void RetryToCreateFailedCustomerStatements(GenerateStatementRawData statementRawData, string tenantCode)
        {
            try
            {
                var scheduleLogDetail = statementRawData.ScheduleLogDetail;
                var customer = this.tenantTransactionDataRepository.Get_CustomerMasters(new CustomerSearchParameter()
                {
                    Identifier = scheduleLogDetail.CustomerId,
                    BatchId = statementRawData.Batch.Identifier,
                }, tenantCode)?.FirstOrDefault();
                statementRawData.Customer = customer;

                if (customer != null)
                {
                    //call to generate actual HTML statement file for current customer record
                    var logDetailRecord = this.GenerateStatements(statementRawData, tenantCode);
                    if (logDetailRecord != null)
                    {
                        //delete un-neccessory files which are created during html statement generation in fail cases
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                        {
                            this.utility.DeleteUnwantedDirectory(statementRawData.Batch.Identifier, customer.Identifier, statementRawData.OutputLocation);
                        }

                        //update schedule log detail
                        var scheduleLogDetails = new List<ScheduleLogDetail>();
                        scheduleLogDetail.CustomerId = customer.Identifier;
                        scheduleLogDetail.CustomerName = customer.FirstName.Trim() + (customer.MiddleName == "" ? "" : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim();
                        scheduleLogDetail.RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0;
                        scheduleLogDetail.RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : string.Empty;
                        scheduleLogDetail.RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : string.Empty;
                        scheduleLogDetail.LogMessage = logDetailRecord.LogMessage;
                        scheduleLogDetail.Status = logDetailRecord.Status;
                        scheduleLogDetail.NumberOfRetry++;
                        scheduleLogDetail.StatementFilePath = logDetailRecord.StatementFilePath;
                        scheduleLogDetails.Add(scheduleLogDetail);
                        this.scheduleLogRepository.UpdateScheduleLogDetails(scheduleLogDetails, tenantCode);

                        //save statement metadata if html statement generated successfully
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()) && logDetailRecord.statementMetadata.Count > 0)
                        {
                            var statementMetadataRecords = new List<StatementMetadata>();
                            logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
                            {
                                metarec.ScheduleLogId = scheduleLogDetail.ScheduleLogId;
                                metarec.ScheduleId = scheduleLogDetail.ScheduleId;
                                metarec.StatementDate = DateTime.UtcNow;
                                metarec.StatementURL = scheduleLogDetail.StatementFilePath;
                                metarec.TenantCode = tenantCode;
                                metarec.IsPasswordGenerated = false;
                                metarec.Password = "";
                                statementMetadataRecords.Add(metarec);
                            });
                            this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
                        }

                        var scheduleLogs = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
                        {
                            ScheduleLogId = scheduleLogDetail.ScheduleLogId.ToString(),
                            BatchId = statementRawData.Batch.Identifier.ToString(),
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
                        }, tenantCode).ToList();
                        scheduleLogs.ForEach(scheduleLog =>
                        {
                            //get total no. of schedule log details for current schedule log
                            var _lstScheduleLogDetail = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
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
                            }, tenantCode);

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
                            this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, batchStatus, isBatchCompleteExecuted, tenantCode);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //save schedule log details for current customer, if something went wrong while statement generation...
                var logDetails = new List<ScheduleLogDetail>();
                logDetails.Add(new ScheduleLogDetail()
                {
                    ScheduleLogId = statementRawData.ScheduleLog.Identifier,
                    CustomerId = statementRawData.Customer.Identifier,
                    CustomerName = statementRawData.Customer.FirstName.Trim() + (statementRawData.Customer.MiddleName == "" ? string.Empty : " " + statementRawData.Customer.MiddleName.Trim()) + " " + statementRawData.Customer.LastName.Trim(),
                    ScheduleId = statementRawData.ScheduleLog.ScheduleId,
                    RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0,
                    RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "",
                    RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "",
                    NumberOfRetry = 1,
                    CreateDate = DateTime.UtcNow,
                    Status = ScheduleLogStatus.Failed.ToString(),
                    LogMessage = "Something went wrong while generating statement: " + ex.Message
                });
                this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                //write error log
                WriteToFile(ex.StackTrace.ToString());
                //throw ex;
            }
        }

        /// <summary>
        /// This method helps to retry to generate HTML statement for failed nedbank customer list.
        /// </summary>
        /// <param name="statementRawData"> the raw data object requires for statement generate process</param>
        /// <param name="tenantCode"></param>
        public void RetryToCreateFailedNedbankCustomerStatements(GenerateStatementRawData statementRawData, string tenantCode)
        {
            try
            {
                var scheduleLogDetail = statementRawData.ScheduleLogDetail;
                var customer = this.tenantTransactionDataRepository.Get_DM_CustomerMasters(new CustomerSearchParameter()
                {
                    Identifier = scheduleLogDetail.CustomerId,
                    BatchId = statementRawData.Batch.Identifier,
                }, tenantCode)?.FirstOrDefault();
                statementRawData.DM_Customer = customer;

                if (customer != null)
                {
                    //call to generate actual HTML statement file for current customer record
                    var logDetailRecord = this.GenerateNedbankStatements(statementRawData, tenantCode);
                    if (logDetailRecord != null)
                    {
                        //delete un-neccessory files which are created during html statement generation in fail cases
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
                        {
                            this.utility.DeleteUnwantedDirectory(statementRawData.Batch.Identifier, customer.Identifier, statementRawData.OutputLocation);
                        }

                        //update schedule log detail
                        var scheduleLogDetails = new List<ScheduleLogDetail>();
                        scheduleLogDetail.CustomerId = customer.CustomerId;
                        scheduleLogDetail.CustomerName = customer.FirstName.Trim() + " " + customer.SurName.Trim();
                        scheduleLogDetail.RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0;
                        scheduleLogDetail.RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : string.Empty;
                        scheduleLogDetail.RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : string.Empty;
                        scheduleLogDetail.LogMessage = logDetailRecord.LogMessage;
                        scheduleLogDetail.Status = logDetailRecord.Status;
                        scheduleLogDetail.NumberOfRetry++;
                        scheduleLogDetail.StatementFilePath = logDetailRecord.StatementFilePath;
                        scheduleLogDetails.Add(scheduleLogDetail);
                        this.scheduleLogRepository.UpdateScheduleLogDetails(scheduleLogDetails, tenantCode);

                        //save statement metadata if html statement generated successfully
                        if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()) && logDetailRecord.statementMetadata.Count > 0)
                        {
                            var statementMetadataRecords = new List<StatementMetadata>();
                            logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
                            {
                                metarec.ScheduleLogId = scheduleLogDetail.ScheduleLogId;
                                metarec.ScheduleId = scheduleLogDetail.ScheduleId;
                                metarec.StatementDate = DateTime.UtcNow;
                                metarec.StatementURL = scheduleLogDetail.StatementFilePath;
                                metarec.TenantCode = tenantCode;
                                metarec.IsPasswordGenerated = false;
                                metarec.Password = "";
                                statementMetadataRecords.Add(metarec);
                            });
                            this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
                        }

                        var scheduleLogs = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
                        {
                            ScheduleLogId = scheduleLogDetail.ScheduleLogId.ToString(),
                            BatchId = statementRawData.Batch.Identifier.ToString(),
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
                        }, tenantCode).ToList();
                        scheduleLogs.ForEach(scheduleLog =>
                        {
                            //get total no. of schedule log details for current schedule log
                            var _lstScheduleLogDetail = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
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
                            }, tenantCode);

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
                            this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, batchStatus, isBatchCompleteExecuted, tenantCode);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //save schedule log details for current customer, if something went wrong while statement generation...
                var logDetails = new List<ScheduleLogDetail>();
                logDetails.Add(new ScheduleLogDetail()
                {
                    ScheduleLogId = statementRawData.ScheduleLog.Identifier,
                    CustomerId = statementRawData.DM_Customer.CustomerId,
                    CustomerName = statementRawData.DM_Customer.FirstName.Trim() + " " + statementRawData.DM_Customer.SurName.Trim(),
                    ScheduleId = statementRawData.ScheduleLog.ScheduleId,
                    RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0,
                    RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "",
                    RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "",
                    NumberOfRetry = 1,
                    CreateDate = DateTime.UtcNow,
                    Status = ScheduleLogStatus.Failed.ToString(),
                    LogMessage = "Something went wrong while generating statement: " + ex.Message
                });
                this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

                //write error log
                WriteToFile(ex.StackTrace.ToString());
                //throw ex;
            }
        }

        /// <summary>
        /// This method helps to convert HTML statement to PDF statement and archive related data for the customer.
        /// </summary>
        /// <param name="archivalProcessRawData">The raw data object required for archival process</param>
        /// <param name="tenantCode">The tenant code</param>
        public bool RunArchivalForCustomerRecord(ArchivalProcessRawData archivalProcessRawData, string tenantCode)
        {
            var tempDir = string.Empty;
            var runStatus = false;

            try
            {
                var pdfStatementFilepath = archivalProcessRawData.PdfStatementFilepath;
                var batch = archivalProcessRawData.BatchMaster;

                var customer = this.tenantTransactionDataRepository.Get_CustomerMasters(new CustomerSearchParameter()
                {
                    BatchId = batch.Identifier,
                    CustomerId = archivalProcessRawData.CustomerId
                }, tenantCode).FirstOrDefault();
                var metadataRecords = this.statementSearchRepository.GetStatementSearchs(new StatementSearchSearchParameter()
                {
                    CustomerId = archivalProcessRawData.CustomerId.ToString(),
                    StatementId = archivalProcessRawData.Statement.Identifier.ToString(),
                    IsPasswordRequired = true,
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

                if (customer != null && metadataRecords != null && metadataRecords.Count > 0)
                {
                    var statementSearchRecord = metadataRecords.FirstOrDefault();

                    //Create final output directory to save PDF statement of current customer
                    var outputlocation = pdfStatementFilepath + "\\PDF_Statements" + "\\" + "ScheduleId_" + statementSearchRecord.ScheduleId + "\\" + "BatchId_" + batch.Identifier + "\\ArchiveData";
                    if (!Directory.Exists(outputlocation))
                    {
                        Directory.CreateDirectory(outputlocation);
                    }

                    tempDir = outputlocation + "\\temp_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    //Create temp output directory to save all neccessories files which requires to genearate PDF statement of current customer
                    var samplefilespath = tempDir + "\\" + statementSearchRecord.Identifier + "_" + customer.Identifier + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                    if (!Directory.Exists(samplefilespath))
                    {
                        Directory.CreateDirectory(samplefilespath);
                    }

                    //get actual HTML statement file path directory for current customer
                    var htmlStatementDirPath = statementSearchRecord.StatementURL.Substring(0, statementSearchRecord.StatementURL.LastIndexOf("\\"));

                    //get resource file path directory
                    var resourceFilePath = htmlStatementDirPath.Substring(0, htmlStatementDirPath.LastIndexOf("\\")) + "\\common";
                    if (!Directory.Exists(resourceFilePath))
                    {
                        resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
                    }

                    //Update mark pro fonts url in ltr.css
                    var cssFIlePath = AppDomain.CurrentDomain.BaseDirectory + @"\Resources\css\ltr.css";
                    var css = File.ReadAllText(cssFIlePath);
                    css = css.Replace("../fonts/", "./");
                    File.WriteAllText(cssFIlePath, css);

                    cssFIlePath = AppDomain.CurrentDomain.BaseDirectory + @"\Resources\css\font-awesome.min.css";
                    css = File.ReadAllText(cssFIlePath);
                    css = css.Replace("../fonts/", "./");
                    File.WriteAllText(cssFIlePath, css);

                    //Copying all neccessories files which requires to genearate PDF statement of current customer
                    this.utility.DirectoryCopy(resourceFilePath + "\\css", samplefilespath, false);
                    this.utility.DirectoryCopy(resourceFilePath + "\\js", samplefilespath, false);
                    this.utility.DirectoryCopy(resourceFilePath + "\\images", samplefilespath, false);
                    this.utility.DirectoryCopy(resourceFilePath + "\\fonts", samplefilespath, false);

                    //Gernerate HTML statement of current customer
                    this.statementSearchManager.GenerateHtmlStatementForPdfGeneration(customer, archivalProcessRawData.Statement, archivalProcessRawData.StatementPageContents, batch, archivalProcessRawData.BatchDetails, tenantCode, samplefilespath, archivalProcessRawData.Client, archivalProcessRawData.TenantConfiguration);

                    //To insert html statement file of current customer and all required files into the zip file
                    var zipfilepath = tempDir + "\\tempzip";
                    if (!Directory.Exists(zipfilepath))
                    {
                        Directory.CreateDirectory(zipfilepath);
                    }
                    var zipFile = zipfilepath + "\\" + "StatementZip" + "_" + statementSearchRecord.Identifier + "_" + statementSearchRecord.ScheduleId + "_" + statementSearchRecord.StatementId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
                    ZipFile.CreateFromDirectory(samplefilespath, zipFile);

                    //Convert HTML statement to PDF statement for current customer
                    var pdfName = "Statement" + "_" + statementSearchRecord.ScheduleLogId + "_" + statementSearchRecord.ScheduleId + statementSearchRecord.StatementId + "_" + statementSearchRecord.CustomerId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".pdf";
                    string password = string.Empty;
                    if (statementSearchRecord.IsPasswordGenerated)
                    {
                        password = this.cryptoManager.Decrypt(statementSearchRecord.Password);
                    }
                    var result = this.utility.HtmlStatementToPdf(zipFile, outputlocation + "\\" + pdfName, password);
                    if (result)
                    {
                        //To insert archive schedule log detail records
                        var scheduleLogDetailRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
                        {
                            ScheduleLogId = archivalProcessRawData.ScheduleLog.Identifier.ToString(),
                            CustomerId = archivalProcessRawData.CustomerId.ToString(),
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
                        var scheduleDetailArchiveRecords = new List<ScheduleLogDetailArchieve>();
                        scheduleLogDetailRecords.ToList().ForEach(logDetail =>
                        {
                            scheduleDetailArchiveRecords.Add(new ScheduleLogDetailArchieve()
                            {
                                CustomerId = logDetail.CustomerId,
                                CustomerName = logDetail.CustomerName,
                                LogDetailCreationDate = logDetail.CreateDate,
                                LogMessage = logDetail.LogMessage,
                                NumberOfRetry = logDetail.NumberOfRetry,
                                RenderEngineId = archivalProcessRawData.RenderEngine.Identifier,
                                RenderEngineName = archivalProcessRawData.RenderEngine.RenderEngineName,
                                RenderEngineURL = archivalProcessRawData.RenderEngine.URL,
                                ScheduleId = archivalProcessRawData.Schedule.Identifier,
                                ScheduleLogArchiveId = archivalProcessRawData.ScheduleLogArchive.Identifier,
                                Status = logDetail.Status,
                                TenantCode = tenantCode,
                                ArchivalDate = DateTime.UtcNow,
                                PdfStatementPath = outputlocation + "\\" + pdfName
                            });
                        });
                        this.archivalProcessRepository.SaveScheduleLogDetailsArchieve(scheduleDetailArchiveRecords, tenantCode);

                        //TO insert archive statement metadata records
                        var metadataArchiveRecords = new List<StatementMetadataArchive>();
                        metadataRecords.ToList().ForEach(record =>
                        {
                            metadataArchiveRecords.Add(new StatementMetadataArchive()
                            {
                                AccountNumber = record.AccountNumber,
                                AccountType = record.AccountType,
                                CustomerId = record.CustomerId,
                                CustomerName = record.CustomerName,
                                ScheduleId = record.ScheduleId,
                                ScheduleLogArchiveId = archivalProcessRawData.ScheduleLogArchive.Identifier,
                                StatementDate = record.StatementDate,
                                StatementId = record.StatementId,
                                StatementPeriod = record.StatementPeriod,
                                StatementURL = outputlocation + "\\" + pdfName,
                                TenantCode = tenantCode,
                                IsPasswordGenerated = record.IsPasswordGenerated,
                                Password = record.Password,
                                ArchivalDate = DateTime.UtcNow
                            });
                        });
                        this.archivalProcessRepository.SaveStatementMetadataArchieve(metadataArchiveRecords, tenantCode);

                        //TO delete actual schedule log details, and statement metadata records
                        this.scheduleLogRepository.DeleteScheduleLogDetails(archivalProcessRawData.ScheduleLog.Identifier, archivalProcessRawData.CustomerId, tenantCode);
                        this.scheduleLogRepository.DeleteStatementMetadata(archivalProcessRawData.ScheduleLog.Identifier, archivalProcessRawData.CustomerId, tenantCode);

                        //To delete actual HTML statement of currrent customer, once the PDF statement genearated
                        DirectoryInfo directoryInfo = new DirectoryInfo(htmlStatementDirPath);
                        if (directoryInfo.Exists)
                        {
                            directoryInfo.Delete(true);
                        }

                        runStatus = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
                WriteToFile(ex.StackTrace);
                throw ex;
            }
            finally
            {
                //To delete temp files, once the PDF statement genearated
                DirectoryInfo directoryInfo = new DirectoryInfo(tempDir);
                if (directoryInfo.Exists)
                {
                    directoryInfo.Delete(true);
                }
            }

            return runStatus;
        }

        /// <summary>
        /// This method helps to convert HTML statement to PDF statement and archive related data for the nedbank customer.
        /// </summary>
        /// <param name="archivalProcessRawData">The raw data object required for archival process</param>
        /// <param name="tenantCode">The tenant code</param>
        public bool RunArchivalForNedbankCustomerRecord(ArchivalProcessRawData archivalProcessRawData, string tenantCode)
        {
            var tempDir = string.Empty;
            var runStatus = false;

            try
            {
                var pdfStatementFilepath = archivalProcessRawData.PdfStatementFilepath;
                var batch = archivalProcessRawData.BatchMaster;

                var customer = this.tenantTransactionDataRepository.Get_DM_CustomerMasters(new CustomerSearchParameter()
                {
                    BatchId = batch.Identifier,
                    CustomerId = archivalProcessRawData.CustomerId
                }, tenantCode).FirstOrDefault();
                var metadataRecords = this.statementSearchRepository.GetStatementSearchs(new StatementSearchSearchParameter()
                {
                    CustomerId = archivalProcessRawData.CustomerId.ToString(),
                    StatementId = archivalProcessRawData.Statement.Identifier.ToString(),
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

                if (customer != null && metadataRecords != null && metadataRecords.Count > 0)
                {
                    var statementSearchRecord = metadataRecords.FirstOrDefault();

                    //Create final output directory to save PDF statement of current customer
                    var outputlocation = pdfStatementFilepath + "\\PDF_Statements" + "\\" + "ScheduleId_" + statementSearchRecord.ScheduleId + "\\" + "BatchId_" + batch.Identifier + "\\ArchiveData";
                    if (!Directory.Exists(outputlocation))
                    {
                        Directory.CreateDirectory(outputlocation);
                    }

                    tempDir = outputlocation + "\\temp_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    //Create temp output directory to save all neccessories files which requires to genearate PDF statement of current customer
                    var samplefilespath = tempDir + "\\" + statementSearchRecord.Identifier + "_" + customer.CustomerId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                    if (!Directory.Exists(samplefilespath))
                    {
                        Directory.CreateDirectory(samplefilespath);
                    }

                    //get actual HTML statement file path directory for current customer
                    var htmlStatementDirPath = statementSearchRecord.StatementURL.Substring(0, statementSearchRecord.StatementURL.LastIndexOf("\\"));

                    //get resource file path directory
                    var resourceFilePath = htmlStatementDirPath.Substring(0, htmlStatementDirPath.LastIndexOf("\\")) + "\\common";
                    if (!Directory.Exists(resourceFilePath))
                    {
                        resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
                    }

                    //Copying all neccessories files which requires to genearate PDF statement of current customer
                    this.utility.DirectoryCopy(resourceFilePath + "\\css", samplefilespath, false);
                    this.utility.DirectoryCopy(resourceFilePath + "\\js", samplefilespath, false);
                    this.utility.DirectoryCopy(resourceFilePath + "\\images", samplefilespath, false);
                    this.utility.DirectoryCopy(resourceFilePath + "\\fonts", samplefilespath, false);

                    //Gernerate HTML statement of current customer
                    this.statementSearchManager.GenerateNedbankHtmlStatementForPdfGeneration(customer, archivalProcessRawData.Statement, archivalProcessRawData.StatementPageContents, batch, archivalProcessRawData.BatchDetails, tenantCode, samplefilespath, archivalProcessRawData.TenantConfiguration);

                    //To insert html statement file of current customer and all required files into the zip file
                    var zipfilepath = tempDir + "\\tempzip";
                    if (!Directory.Exists(zipfilepath))
                    {
                        Directory.CreateDirectory(zipfilepath);
                    }
                    var zipFile = zipfilepath + "\\" + "StatementZip" + "_" + statementSearchRecord.Identifier + "_" + statementSearchRecord.ScheduleId + "_" + statementSearchRecord.StatementId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
                    ZipFile.CreateFromDirectory(samplefilespath, zipFile);

                    //Convert HTML statement to PDF statement for current customer
                    var pdfName = "Statement" + "_" + statementSearchRecord.ScheduleLogId + "_" + statementSearchRecord.ScheduleId + statementSearchRecord.StatementId + "_" + statementSearchRecord.CustomerId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".pdf";
                    string password = string.Empty;
                    if (statementSearchRecord.IsPasswordGenerated)
                    {
                        password = this.cryptoManager.Decrypt(statementSearchRecord.Password);
                    }
                    var result = this.utility.HtmlStatementToPdf(zipFile, outputlocation + "\\" + pdfName, password);
                    if (result)
                    {
                        //To insert archive schedule log detail records
                        var scheduleLogDetailRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
                        {
                            ScheduleLogId = archivalProcessRawData.ScheduleLog.Identifier.ToString(),
                            CustomerId = archivalProcessRawData.CustomerId.ToString(),
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
                        var scheduleDetailArchiveRecords = new List<ScheduleLogDetailArchieve>();
                        scheduleLogDetailRecords.ToList().ForEach(logDetail =>
                        {
                            scheduleDetailArchiveRecords.Add(new ScheduleLogDetailArchieve()
                            {
                                CustomerId = logDetail.CustomerId,
                                CustomerName = logDetail.CustomerName,
                                LogDetailCreationDate = logDetail.CreateDate,
                                LogMessage = logDetail.LogMessage,
                                NumberOfRetry = logDetail.NumberOfRetry,
                                RenderEngineId = archivalProcessRawData.RenderEngine.Identifier,
                                RenderEngineName = archivalProcessRawData.RenderEngine.RenderEngineName,
                                RenderEngineURL = archivalProcessRawData.RenderEngine.URL,
                                ScheduleId = archivalProcessRawData.Schedule.Identifier,
                                ScheduleLogArchiveId = archivalProcessRawData.ScheduleLogArchive.Identifier,
                                Status = logDetail.Status,
                                TenantCode = tenantCode,
                                ArchivalDate = DateTime.UtcNow,
                                PdfStatementPath = outputlocation + "\\" + pdfName
                            });
                        });
                        this.archivalProcessRepository.SaveScheduleLogDetailsArchieve(scheduleDetailArchiveRecords, tenantCode);

                        //TO insert archive statement metadata records
                        var metadataArchiveRecords = new List<StatementMetadataArchive>();
                        metadataRecords.ToList().ForEach(record =>
                        {
                            metadataArchiveRecords.Add(new StatementMetadataArchive()
                            {
                                AccountNumber = record.AccountNumber,
                                AccountType = record.AccountType,
                                CustomerId = record.CustomerId,
                                CustomerName = record.CustomerName,
                                ScheduleId = record.ScheduleId,
                                ScheduleLogArchiveId = archivalProcessRawData.ScheduleLogArchive.Identifier,
                                StatementDate = record.StatementDate,
                                StatementId = record.StatementId,
                                StatementPeriod = record.StatementPeriod,
                                StatementURL = outputlocation + "\\" + pdfName,
                                TenantCode = tenantCode,
                                IsPasswordGenerated = record.IsPasswordGenerated,
                                Password = record.Password,
                                ArchivalDate = DateTime.UtcNow
                            });
                        });
                        this.archivalProcessRepository.SaveStatementMetadataArchieve(metadataArchiveRecords, tenantCode);

                        //TO delete actual schedule log details, and statement metadata records
                        this.scheduleLogRepository.DeleteScheduleLogDetails(archivalProcessRawData.ScheduleLog.Identifier, archivalProcessRawData.CustomerId, tenantCode);
                        this.scheduleLogRepository.DeleteStatementMetadata(archivalProcessRawData.ScheduleLog.Identifier, archivalProcessRawData.CustomerId, tenantCode);

                        //To delete actual HTML statement of currrent customer, once the PDF statement genearated
                        DirectoryInfo directoryInfo = new DirectoryInfo(htmlStatementDirPath);
                        if (directoryInfo.Exists)
                        {
                            directoryInfo.Delete(true);
                        }

                        runStatus = true;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
                WriteToFile(ex.StackTrace);
                throw ex;
            }
            finally
            {
                //To delete temp files, once the PDF statement genearated
                DirectoryInfo directoryInfo = new DirectoryInfo(tempDir);
                if (directoryInfo.Exists)
                {
                    directoryInfo.Delete(true);
                }
            }

            return runStatus;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method helps to write content into the file
        /// </summary>
        /// <param name="Message">content to write into the file</param>
        private void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\Log_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
        /// This method help to actaul color theme to show series data for dynamic line graph, bar graph, and pie chart widgets
        /// </summary>
        /// <param name="theme"> the widget theme </param>
        /// <returns>return new color theme for graph and chart widgets </returns>
        private string GetChartColorTheme(string theme)
        {
            string colorTheme = string.Empty;
            if (theme == "Theme1")
            {
                colorTheme = HtmlConstants.THEME1;
            }
            else if (theme == "Theme2")
            {
                colorTheme = HtmlConstants.THEME2;
            }
            else if (theme == "Theme3")
            {
                colorTheme = HtmlConstants.THEME3;
            }
            else if (theme == "Theme4")
            {
                colorTheme = HtmlConstants.THEME4;
            }
            else if (theme.ToLower() == "ChartTheme1".ToLower())
            {
                colorTheme = HtmlConstants.THEME1;
            }
            else if (theme.ToLower() == "ChartTheme2".ToLower())
            {
                colorTheme = HtmlConstants.THEME2;
            }
            else if (theme.ToLower() == "ChartTheme3".ToLower())
            {
                colorTheme = HtmlConstants.THEME4;
            }
            else if (theme.ToLower() == "ChartTheme4".ToLower())
            {
                colorTheme = HtmlConstants.THEME3;
            }

            return colorTheme;
        }

        /// <summary>
        /// This method help to bind data to financial tenant statement file for respective customer
        /// </summary>
        /// <param name="statementRawData"> the statement raw data object</param>
        /// <param name="tenantCode"> the tenant code </param>
        private ScheduleLogDetail GenerateStatements(GenerateStatementRawData statementRawData, string tenantCode)
        {
            var logDetailRecord = new ScheduleLogDetail();
            var ErrorMessages = new StringBuilder();
            bool IsFailed = false;
            bool IsSavingOrCurrentAccountPagePresent = false;
            var statementMetadataRecords = new List<StatementMetadata>();

            try
            {
                var customer = statementRawData.Customer;
                var statement = statementRawData.Statement;
                var batchMaster = statementRawData.Batch;

                if (statementRawData.StatementPageContents.Count > 0)
                {
                    string currency = string.Empty;
                    var accountrecords = new List<AccountMaster>();
                    var savingaccountrecords = new List<AccountMaster>();
                    var curerntaccountrecords = new List<AccountMaster>();
                    var CustomerAcccountTransactions = new List<AccountTransaction>();
                    var CustomerSavingTrends = new List<SavingTrend>();

                    var pages = statement.Pages.Where(item => item.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || item.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE).ToList();
                    IsSavingOrCurrentAccountPagePresent = pages.Count > 0 ? true : false;

                    //collecting all required transaction required for static widgets in financial tenant html statement
                    if (IsSavingOrCurrentAccountPagePresent)
                    {
                        var customerAccountSearchParameter = new CustomerAccountSearchParameter()
                        {
                            CustomerId = customer.Identifier,
                            BatchId = batchMaster.Identifier
                        };
                        accountrecords = this.tenantTransactionDataRepository.Get_AccountMaster(customerAccountSearchParameter, tenantCode)?.ToList();
                        if ((accountrecords == null || accountrecords.Count == 0))
                        {
                            ErrorMessages.Append("<li>Account master data is not available for this customer..!!</li>");
                            IsFailed = true;
                        }
                        else
                        {
                            var records = accountrecords.Where(item => item.AccountType.Equals(string.Empty)).ToList();
                            if (records.Count > 0)
                            {
                                ErrorMessages.Append("<li>Invalid account master data for this customer..!!</li>");
                                IsFailed = true;
                            }
                        }

                        //get customer account transaction details
                        CustomerAcccountTransactions = this.tenantTransactionDataRepository.Get_AccountTransaction(customerAccountSearchParameter, tenantCode)?.OrderBy(item => item.TransactionDate)?.ToList();

                        //get customer saving and spending trend details data
                        CustomerSavingTrends = this.tenantTransactionDataRepository.Get_SavingTrend(customerAccountSearchParameter, tenantCode)?.ToList();
                    }

                    //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
                    var customerMedias = this.tenantTransactionDataRepository.GetCustomerMediaList(customer.Identifier, batchMaster.Identifier, statement.Identifier, tenantCode);

                    var htmlbody = new StringBuilder();
                    currency = accountrecords.Count > 0 ? accountrecords[0].Currency : string.Empty;
                    string navbarHtml = HtmlConstants.NAVBAR_HTML_FOR_PREVIEW.Replace("{{logo}}", "../common/images/nisLogo.png");
                    navbarHtml = navbarHtml.Replace("{{Today}}", DateTime.UtcNow.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy)); //bind current date to html header

                    //get client logo in string format and pass it hidden input tag, so it will be render in right side of header of html statement
                    var clientlogo = statementRawData.Client.TenantLogo != null ? statementRawData.Client.TenantLogo : "";
                    if (statement.Name == "Investment Wealth")
                    {
                        clientlogo = "../common/images/NedBankLogoBlack.png";
                    }
                    navbarHtml = navbarHtml + "<input type='hidden' id='TenantLogoImageValue' value='" + clientlogo + "'>";
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);

                    //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                    var scriptHtmlRenderer = new StringBuilder();
                    var navbar = new StringBuilder();
                    int subPageCount = 0;
                    string accountNumber = string.Empty; //also use for Subscription
                    string accountType = string.Empty; //also use for vendor name
                    long accountId = 0;
                    HttpClient httpClient = null;

                    var newStatementPageContents = new List<StatementPageContent>();
                    statementRawData.StatementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
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

                    long FirstPageId = statement.Pages[0].Identifier;
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        StatementPageContent statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        //sub page count under current page tab
                        subPageCount = 1;
                        if (IsSavingOrCurrentAccountPagePresent)
                        {
                            //This will be applicable only for financial tenant
                            //if cusomer have 2 saving or current account, then 2 tabs will be render to current page in html statement
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                            {
                                savingaccountrecords = accountrecords.Where(item => item.AccountType.ToLower().Contains("saving"))?.ToList();
                                if (savingaccountrecords == null || savingaccountrecords.Count == 0)
                                {
                                    ErrorMessages.Append("<li>Saving account master data is not available for this customer..!!</li>");
                                    IsFailed = true;
                                }
                                subPageCount = savingaccountrecords.Count;
                            }
                            else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                curerntaccountrecords = accountrecords.Where(item => item.AccountType.ToLower().Contains("current"))?.ToList();
                                if (curerntaccountrecords == null || curerntaccountrecords.Count == 0)
                                {
                                    ErrorMessages.Append("<li>Current account master data is not available for this customer..!!</li>");
                                    IsFailed = true;
                                }
                                subPageCount = curerntaccountrecords.Count;
                            }
                        }

                        var SubTabs = new StringBuilder();
                        var PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = new List<DynamicWidget>(statementPageContent.DynamicWidgets);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        navbar.Append(" <li class='nav-item'><a class='nav-link pt-1 mainNav " + (i == 0 ? "active" : "") + " " + tabClassName + "' href='javascript:void(0);' >" + statementPageContent.DisplayName + "</a> </li> ");
                        string ExtraClassName = i > 0 ? "d-none " + tabClassName : tabClassName;
                        PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName).Replace("{{DivId}}", tabClassName);

                        var newPageContent = new StringBuilder();
                        newPageContent.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);

                        for (int x = 0; x < subPageCount; x++)
                        {
                            accountNumber = string.Empty;
                            accountType = string.Empty;

                            //Only for financial tenant
                            if (IsSavingOrCurrentAccountPagePresent)
                            {
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                {
                                    accountNumber = savingaccountrecords[x].AccountNumber;
                                    accountId = savingaccountrecords[x].Identifier;
                                    accountType = savingaccountrecords[x].AccountType;
                                }
                                else if (page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    accountNumber = curerntaccountrecords[x].AccountNumber;
                                    accountId = curerntaccountrecords[x].Identifier;
                                    accountType = curerntaccountrecords[x].AccountType;
                                }

                                //start creating sub tabs, append tab name with last 4 digits of account number
                                if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                                {
                                    string lastFourDigisOfAccountNumber = accountNumber.Length > 4 ? accountNumber.Substring(Math.Max(0, accountNumber.Length - 4)) : accountNumber;
                                    if (x == 0)
                                    {
                                        SubTabs.Append("<ul class='nav nav-tabs' style='margin-top:-20px;'>");
                                    }

                                    SubTabs.Append("<li class='nav-item " + (x == 0 ? "active" : "") + "'><a id='tab" + x + "-tab' data-toggle='tab' " + "data-target='#" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber + "-" + "AccountNumber-" + accountId + "' " + " role='tab' class='nav-link " + (x == 0 ? "active" : "") + "'> Account - " + lastFourDigisOfAccountNumber + "</a></li>");

                                    newPageContent.Append("<div id='" + (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE ? "Saving" : "Current") + "-" + lastFourDigisOfAccountNumber + "-" + "AccountNumber-" + accountId + "' >");

                                    if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE)
                                    {
                                        newPageContent.Append("<input type='hidden' id='SavingAccountId' name='SavingAccountId' value='" + accountId + "'>");
                                    }
                                    else
                                    {
                                        newPageContent.Append("<input type='hidden' id='CurrentAccountId' name='CurrentAccountId' value='" + accountId + "'>");
                                    }

                                    if (x == subPageCount - 1)
                                    {
                                        SubTabs.Append("</ul>");
                                    }
                                }
                            }

                            var pagewidgets = new List<PageWidget>(page.PageWidgets);
                            var pageContent = new StringBuilder(statementPageContent.HtmlContent);
                            for (int j = 0; j < pagewidgets.Count; j++)
                            {
                                var widget = pagewidgets[j];
                                if (!widget.IsDynamicWidget)
                                {
                                    switch (widget.WidgetName)
                                    {
                                        case HtmlConstants.CUSTOMER_INFORMATION_WIDGET_NAME:
                                            this.BindCustomerInformationWidgetData(pageContent, customer, statement, page, widget, customerMedias, statementRawData.BatchDetails);
                                            break;
                                        case HtmlConstants.ACCOUNT_INFORMATION_WIDGET_NAME:
                                            this.BindAccountInformationWidgetData(pageContent, customer, page, widget);
                                            break;
                                        case HtmlConstants.IMAGE_WIDGET_NAME:
                                            IsFailed = this.BindImageWidgetData(pageContent, ErrorMessages, customer.Identifier, customerMedias, statementRawData.BatchDetails, statement, page, batchMaster, widget, tenantCode, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.VIDEO_WIDGET_NAME:
                                            IsFailed = this.BindVideoWidgetData(pageContent, ErrorMessages, customer.Identifier, customerMedias, statementRawData.BatchDetails, statement, page, batchMaster, widget, tenantCode, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.SUMMARY_AT_GLANCE_WIDGET_NAME:
                                            IsFailed = this.BindSummaryAtGlanceWidgetData(pageContent, ErrorMessages, accountrecords, page, widget);
                                            break;
                                        case HtmlConstants.CURRENT_AVAILABLE_BALANCE_WIDGET_NAME:
                                            IsFailed = this.BindCurrentAvailBalanceWidgetData(pageContent, ErrorMessages, customer, batchMaster, accountId, accountrecords, page, widget, currency);
                                            break;
                                        case HtmlConstants.SAVING_AVAILABLE_BALANCE_WIDGET_NAME:
                                            IsFailed = this.BindSavingAvailBalanceWidgetData(pageContent, ErrorMessages, customer, batchMaster, accountId, accountrecords, page, widget, currency);
                                            break;
                                        case HtmlConstants.SAVING_TRANSACTION_WIDGET_NAME:
                                            this.BindSavingTransactionWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, CustomerAcccountTransactions, page, widget, accountId, tenantCode, currency, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.CURRENT_TRANSACTION_WIDGET_NAME:
                                            this.BindCurrentTransactionWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, CustomerAcccountTransactions, page, widget, accountId, tenantCode, currency, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.TOP_4_INCOME_SOURCE_WIDGET_NAME:
                                            this.BindTop4IncomeSourcesWidgetData(pageContent, customer, batchMaster, page, widget, tenantCode);
                                            break;
                                        case HtmlConstants.ANALYTICS_WIDGET_NAME:
                                            this.BindAnalyticsChartWidgetData(pageContent, scriptHtmlRenderer, customer, batchMaster, accountrecords, page, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.SAVING_TREND_WIDGET_NAME:
                                            IsFailed = this.BindSavingTrendChartWidgetData(pageContent, scriptHtmlRenderer, ErrorMessages, customer, batchMaster, CustomerSavingTrends, accountId, page, tenantCode, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.SPENDING_TREND_WIDGET_NAME:
                                            IsFailed = this.BindSpendingTrendChartWidgetData(pageContent, scriptHtmlRenderer, ErrorMessages, customer, batchMaster, CustomerSavingTrends, accountId, page, tenantCode, statementRawData.OutputLocation);
                                            break;
                                        case HtmlConstants.REMINDER_AND_RECOMMENDATION_WIDGET_NAME:
                                            this.BindReminderAndRecommendationWidgetData(pageContent, customer, batchMaster, page, widget, tenantCode);
                                            break;
                                    }
                                }
                                else
                                {
                                    var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                                    if (dynaWidgets.Count > 0)
                                    {
                                        var dynawidget = dynaWidgets.FirstOrDefault();
                                        var themeDetails = new CustomeTheme();
                                        if (dynawidget.ThemeType == "Default")
                                        {
                                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(statementRawData.TenantConfiguration.WidgetThemeSetting);
                                        }
                                        else
                                        {
                                            themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                        }

                                        //Get data from database for widget
                                        httpClient = new HttpClient();
                                        httpClient.BaseAddress = new Uri(statementRawData.TenantConfiguration.BaseUrlForTransactionData);
                                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
                                        httpClient.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, tenantCode);

                                        //API search parameter
                                        JObject searchParameter = new JObject();
                                        searchParameter[ModelConstant.BATCH_ID] = batchMaster.Identifier;
                                        searchParameter[ModelConstant.CUSTOEMR_ID] = customer.Identifier;
                                        searchParameter[ModelConstant.WIDGET_FILTER_SETTING] = dynawidget.WidgetFilterSettings;

                                        switch (dynawidget.WidgetType)
                                        {
                                            case HtmlConstants.TABLE_DYNAMICWIDGET:
                                                this.BindDynamicTableWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                                break;
                                            case HtmlConstants.FORM_DYNAMICWIDGET:
                                                this.BindDynamicFormWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                                break;
                                            case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
                                                this.BindDynamicLineGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                                break;
                                            case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
                                                this.BindDynamicBarGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                                break;
                                            case HtmlConstants.PICHART_DYNAMICWIDGET:
                                                this.BindDynamicPieChartWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails, tenantCode);
                                                break;
                                            case HtmlConstants.HTML_DYNAMICWIDGET:
                                                this.BindDynamicHtmlWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                                break;
                                        }
                                    }
                                }
                            }

                            //if account number variable is not empty means, financial tenant
                            if (accountNumber != string.Empty)
                            {
                                //generate statement metadata records in list format
                                statementMetadataRecords.Add(new StatementMetadata
                                {
                                    AccountNumber = accountNumber,
                                    AccountType = accountType,
                                    CustomerId = customer.Identifier,
                                    CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName,
                                    StatementPeriod = customer.StatementPeriod,
                                    StatementId = statement.Identifier,
                                });
                            }
                            else
                            {
                                //To add statement metadata records for subscription master tenant
                                //var subscriptionMasters = this.tenantTransactionDataRepository.Get_TTD_SubscriptionMasters(new TransactionDataSearchParameter()
                                //{
                                //    BatchId = batchMaster.Identifier,
                                //    CustomerId = customer.Identifier
                                //}, tenantCode);
                                //subscriptionMasters.ToList().ForEach(sub =>
                                //{
                                //    var records = statementMetadataRecords.Where(item => item.CustomerId == customer.Identifier && item.StatementId == statement.Identifier && item.AccountNumber == sub.Subscription && item.AccountType == sub.VendorName).ToList();
                                //    if (records.Count <= 0)
                                //    {
                                //        statementMetadataRecords.Add(new StatementMetadata
                                //        {
                                //            AccountNumber = sub.Subscription,
                                //            AccountType = sub.VendorName,
                                //            CustomerId = customer.Identifier,
                                //            CustomerName = customer.FirstName + (customer.MiddleName == string.Empty ? "" : " " + customer.MiddleName) + " " + customer.LastName,
                                //            StatementPeriod = customer.StatementPeriod,
                                //            StatementId = statement.Identifier,
                                //        });
                                //    }
                                //});
                            }

                            newPageContent.Append(pageContent);
                            if (page.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || page.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }

                            if (x == subPageCount - 1)
                            {
                                newPageContent.Append(HtmlConstants.END_DIV_TAG);
                            }
                        }

                        PageHeaderContent.Replace("{{SubTabs}}", SubTabs.ToString());
                        statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                        statementPageContent.HtmlContent = newPageContent.ToString();
                    }

                    newStatementPageContents.ToList().ForEach(page =>
                    {
                        htmlbody.Append(page.PageHeaderContent);
                        htmlbody.Append(page.HtmlContent);
                        htmlbody.Append(page.PageFooterContent);
                    });

                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);

                    navbarHtml = navbarHtml.Replace("{{NavItemList}}", navbar.ToString());

                    var finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(navbarHtml);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);
                    scriptHtmlRenderer.Append(HtmlConstants.TENANT_LOGO_SCRIPT);

                    //replace below variable with actual data in final html string
                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.Identifier.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("{{TenantCode}}", tenantCode);
                    finalHtml.Replace("{{TenantName}}", statementRawData.Client != null ? statementRawData.Client.TenantName : ModelConstant.CHILD_TENANT);

                    //If has any error while rendering html statement, then assign status as failed and all collected errors message to log message variable..
                    //Otherwise write html statement string to actual html file and store it at output location, then assign status as completed
                    if (IsFailed)
                    {
                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                        logDetailRecord.LogMessage = "<ul class='pl-4 text-left'>" + ErrorMessages.ToString() + "</ul>";
                    }
                    else
                    {
                        string fileName = "Statement_" + customer.Identifier + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                        string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, batchMaster.Identifier, customer.Identifier, statementRawData.BaseURL, statementRawData.OutputLocation);

                        logDetailRecord.StatementFilePath = filePath;
                        logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                        logDetailRecord.LogMessage = "Statement generated successfully..!!";
                        logDetailRecord.statementMetadata = statementMetadataRecords;
                    }
                }

                return logDetailRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method help to bind data to nedbank statement file for respective customer
        /// </summary>
        /// <param name="statementRawData"> the object of statement raw data</param>
        /// <param name="tenantCode"> the tenant code </param>
        private ScheduleLogDetail GenerateNedbankStatements(GenerateStatementRawData statementRawData, string tenantCode)
        {
            var logDetailRecord = new ScheduleLogDetail();
            var ErrorMessages = new StringBuilder();
            bool IsFailed = false;
            var statementMetadataRecords = new List<StatementMetadata>();

            try
            {
                var customer = statementRawData.DM_Customer;
                var statement = statementRawData.Statement;
                var batchMaster = statementRawData.Batch;

                if (statementRawData.StatementPageContents.Count > 0)
                {
                    //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
                    var customerMedias = this.tenantTransactionDataRepository.GetCustomerMediaList(customer.CustomerId, batchMaster.Identifier, statement.Identifier, tenantCode);

                    long BranchId = 0;
                    var investmentMasters = new List<DM_InvestmentMaster>();
                    var PersonalLoanAccounts = new List<DM_PersonalLoanMaster>();
                    var HomeLoanAccounts = new List<DM_HomeLoanMaster>();

                    var AccountsSummaries = new List<DM_AccountsSummary>();
                    var _lstAccountAnalysis = new List<DM_AccountAnanlysis>();
                    var Reminders = new List<DM_CustomerReminderAndRecommendation>();
                    var NewsAndAlerts = new List<DM_CustomerNewsAndAlert>();
                    var GreenbackMaster = new DM_GreenbacksMaster();
                    var GreenbacksRewardPoints = new List<DM_GreenbacksRewardPoints>();
                    var GreenbacksRedeemedPoints = new List<DM_GreenbacksRewardPointsRedeemed>();
                    var CustProductMonthwiseRewardPoints = new List<DM_CustomerProductWiseRewardPoints>();
                    var CustRewardSpendByCategory = new List<DM_CustomerRewardSpendByCategory>();

                    var customerSearchParameter = new CustomerSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier };

                    var IsPortFolioPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.AT_A_GLANCE_PAGE_TYPE).ToList().Count > 0;
                    var IsInvestmentPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE).ToList().Count > 0;
                    var IsPersonalLoanPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList().Count > 0;
                    var IsHomeLoanPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE).ToList().Count > 0;
                    var IsRewardPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.GREENBACKS_PAGE_TYPE).ToList().Count > 0;

                    if (IsPortFolioPageTypePresent)
                    {
                        AccountsSummaries = this.tenantTransactionDataRepository.GET_DM_AccountSummaries(customerSearchParameter, tenantCode)?.ToList();
                        _lstAccountAnalysis = this.tenantTransactionDataRepository.GET_DM_AccountAnalysisDetails(customerSearchParameter, tenantCode)?.ToList();
                        Reminders = this.tenantTransactionDataRepository.GET_DM_CustomerReminderAndRecommendations(customerSearchParameter, tenantCode)?.ToList();
                        NewsAndAlerts = this.tenantTransactionDataRepository.GET_DM_CustomerNewsAndAlert(customerSearchParameter, tenantCode)?.ToList();
                    }
                    if (IsInvestmentPageTypePresent)
                    {
                        investmentMasters = this.tenantTransactionDataRepository.Get_NB_InvestmasterMaster(new CustomerInvestmentSearchParameter() { InvestorId = customer.CustomerId, BatchId = batchMaster.Identifier }, tenantCode)?.ToList();

                        if (investmentMasters != null && investmentMasters.Count > 0)
                        {
                            //var totalAmount = 0.0m; var res = 0.0m;
                            investmentMasters.ForEach(invest =>
                            {
                                invest.investmentTransactions = this.tenantTransactionDataRepository.Get_DM_InvestmentTransaction(new CustomerInvestmentSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier, InvestmentId = invest.InvestmentId }, tenantCode)?.ToList();

                                //totalAmount = totalAmount + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC)).Select(it => decimal.TryParse(it.WJXBFS4_Balance.Replace(",", "."), out res) ? res : 0).ToList().Sum(it => it);
                            });

                            BranchId = (investmentMasters != null && investmentMasters.Count > 0) ? investmentMasters[0].BranchId : 0;

                            //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Investment", TotalAmount = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, totalAmount) });
                        }
                    }
                    if (IsPersonalLoanPageTypePresent)
                    {
                        PersonalLoanAccounts = this.tenantTransactionDataRepository.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();

                        //var totalAmount = 0.0m; var res = 0.0m;
                        //totalAmount = PersonalLoanAccounts.Select(it => decimal.TryParse(it.CreditAdvance, out res) ? res : 0).ToList().Sum(it => it);
                        //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Personal Loan", TotalAmount = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, totalAmount) });

                        BranchId = (PersonalLoanAccounts != null && PersonalLoanAccounts.Count > 0) ? PersonalLoanAccounts[0].BranchId : 0;
                    }
                    if (IsHomeLoanPageTypePresent)
                    {
                        HomeLoanAccounts = this.tenantTransactionDataRepository.Get_DM_HomeLoanMaster(new CustomerHomeLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();

                        //var totalAmount = 0.0m; var res = 0.0m;
                        //totalAmount = HomeLoanAccounts.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
                        //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Home Loan", TotalAmount = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, totalAmount) });
                    }
                    if (IsRewardPageTypePresent)
                    {
                        //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Greenback reward points", TotalAmount = "432" });
                        GreenbackMaster = this.tenantTransactionDataRepository.GET_DM_GreenbacksMasterDetails(tenantCode)?.ToList()?.FirstOrDefault();
                        GreenbacksRewardPoints = this.tenantTransactionDataRepository.GET_DM_GreenbacksRewardPoints(customerSearchParameter, tenantCode)?.ToList();
                        GreenbacksRedeemedPoints = this.tenantTransactionDataRepository.GET_DM_GreenbacksRewardPointsRedeemed(customerSearchParameter, tenantCode)?.ToList();
                        CustProductMonthwiseRewardPoints = this.tenantTransactionDataRepository.GET_DM_CustomerProductWiseRewardPoints(customerSearchParameter, tenantCode)?.ToList();
                        CustRewardSpendByCategory = this.tenantTransactionDataRepository.GET_DM_CustomerRewardSpendByCategory(customerSearchParameter, tenantCode)?.ToList();
                    }

                    var SpecialMessage = this.tenantTransactionDataRepository.Get_DM_SpecialMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();

                    var _lstMessage = this.tenantTransactionDataRepository.Get_DM_MarketingMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();

                    var htmlbody = new StringBuilder();
                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                    if (statement.Pages.Count() > 0)
                    {
                        var page = statement.Pages[0];
                        var pagewidgets = new List<PageWidget>(page.PageWidgets);
                        if (pagewidgets.Where(x => x.WidgetName.Contains("Wealth")).Count() > 0)
                        {
                            htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{ImgHeight}}", "100").Replace("{{NedBankLogo}}", "../common/images/NedBankLogoBlack.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
                        }
                        else
                        {
                            htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{ImgHeight}}", "80").Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
                        }
                    }
                    else
                    {
                        htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{ImgHeight}}", "80").Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
                    }
                    //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
                    var scriptHtmlRenderer = new StringBuilder();
                    HttpClient httpClient = null;
                    var NavItemList = new StringBuilder();

                    var newStatementPageContents = new List<StatementPageContent>();
                    statementRawData.StatementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
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

                    long FirstPageId = statement.Pages[0].Identifier;
                    var firstPage = statement.Pages[0];
                    var widgets = new List<PageWidget>(firstPage.PageWidgets);
                    for (int i = 0; i < statement.Pages.Count; i++)
                    {
                        var page = statement.Pages[i];
                        var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

                        var MarketingMessageCounter = 0;
                        var Messages = _lstMessage?.Where(it => it.Type == page.PageTypeName)?.ToList();

                        //sub page count under current page tab
                        var PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
                        var dynamicWidgets = new List<DynamicWidget>(statementPageContent.DynamicWidgets);

                        string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
                        NavItemList.Append("<li class='nav-item" + (i != statement.Pages.Count - 1 ? " nav-rt-border" : string.Empty) + "'><a id='tab" + i + "-tab' data-toggle='tab' data-target='#" + tabClassName + "' role='tab' class='nav-link" + (i == 0 ? " active" : string.Empty) + "'> " + statementPageContent.DisplayName + " </a></li>");

                        string ExtraClassName = string.Empty;
                        PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName).Replace("{{DivId}}", tabClassName);

                        var newPageContent = new StringBuilder();
                        var pagewidgets = new List<PageWidget>(page.PageWidgets);
                        var pageContent = new StringBuilder(statementPageContent.HtmlContent);
                        for (int j = 0; j < pagewidgets.Count; j++)
                        {
                            var widget = pagewidgets[j];
                            if (!widget.IsDynamicWidget)
                            {
                                switch (widget.WidgetName)
                                {
                                    case HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME:
                                        if (statement.Pages.Count == 1)
                                        {
                                            this.BindCustomerDetailsWidgetData(pageContent, customer, page, widget);
                                        }
                                        break;

                                    case HtmlConstants.BRANCH_DETAILS_WIDGET_NAME:
                                        if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
                                        {
                                            if (statement.Pages.Count == 1)
                                            {
                                                this.BindBondDetailsWidgetData(pageContent, page, widget, HomeLoanAccounts, customer);
                                            }
                                        }
                                        else
                                        {
                                            this.BindBranchDetailsWidgetData(pageContent, BranchId, page, widget, tenantCode, customer);
                                        }
                                        break;

                                    case HtmlConstants.IMAGE_WIDGET_NAME:
                                        IsFailed = this.BindImageWidgetData(pageContent, ErrorMessages, customer.CustomerId, customerMedias, statementRawData.BatchDetails, statement, page, batchMaster, widget, tenantCode, statementRawData.OutputLocation);
                                        break;

                                    case HtmlConstants.VIDEO_WIDGET_NAME:
                                        IsFailed = this.BindVideoWidgetData(pageContent, ErrorMessages, customer.CustomerId, customerMedias, statementRawData.BatchDetails, statement, page, batchMaster, widget, tenantCode, statementRawData.OutputLocation);
                                        break;

                                    case HtmlConstants.STATIC_SEGMENT_BASED_CONTENT_NAME:
                                        IsFailed = this.BindSegmentBasedContentWidgetData(pageContent, customer, page, widget, statement, ErrorMessages, widgets.Any(x => x.WidgetName.Contains("Wealth")));
                                        break;

                                    case HtmlConstants.STATIC_HTML_WIDGET_NAME:
                                        IsFailed = this.BindStaticHtmlWidgetData(pageContent, customer, page, widget, statement, ErrorMessages);
                                        break;

                                    case HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME:
                                        this.BindInvestmentPortfolioStatementWidgetData(pageContent, customer, investmentMasters, page, widget);
                                        break;

                                    case HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME:
                                        this.BindInvestorPerformanceWidgetData(pageContent, investmentMasters, page, widget, customer);
                                        break;

                                    case HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
                                        this.BindBreakdownOfInvestmentAccountsWidgetData(pageContent, scriptHtmlRenderer, investmentMasters, page, widget, batchMaster, customer, statementRawData.OutputLocation);
                                        break;

                                    case HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME:
                                        this.BindExplanatoryNotesWidgetData(pageContent, batchMaster, page, widget, tenantCode);
                                        break;

                                    case HtmlConstants.SERVICE_WIDGET_NAME:
                                        this.BindMarketingServiceWidgetData(pageContent, Messages, page, widget, MarketingMessageCounter);
                                        MarketingMessageCounter++;
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME:
                                        this.BindPersonalLoanDetailWidgetData(pageContent, batchMaster, customer, page, widget, tenantCode);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME:
                                        this.BindPersonalLoanTransactionWidgetData(pageContent, batchMaster, page, widget, customer, tenantCode);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME:
                                        this.BindPersonalLoanPaymentDueWidgetData(pageContent, batchMaster, page, widget, customer, tenantCode);
                                        break;

                                    case HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME:
                                        this.BindSpecialMessageWidgetData(pageContent, SpecialMessage, page, widget);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME:
                                        this.BindPersonalLoanInsuranceMessageWidgetData(pageContent, SpecialMessage, page, widget);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                        this.BindPersonalLoanTotalAmountDetailWidgetData(pageContent, PersonalLoanAccounts, page, widget);
                                        break;

                                    case HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                        this.BindPersonalLoanAccountsBreakdownWidgetData(pageContent, scriptHtmlRenderer, PersonalLoanAccounts, page, widget, batchMaster, customer, statementRawData.OutputLocation);
                                        break;

                                    case HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
                                        this.BindHomeLoanTotalAmountDetailWidgetData(pageContent, HomeLoanAccounts, page, widget);
                                        break;

                                    case HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
                                        this.BindHomeLoanAccountsBreakdownWidgetData(pageContent, scriptHtmlRenderer, HomeLoanAccounts, page, widget, batchMaster, customer, statementRawData.OutputLocation);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME:
                                        this.BindPortfolioCustomerDetailsWidgetData(pageContent, customer, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME:
                                        this.BindPortfolioCustomerAddressDetailsWidgetData(pageContent, customer, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME:
                                        this.BindPortfolioClientContactDetailsWidgetData(pageContent, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME:
                                        this.BindPortfolioAccountSummaryWidgetData(pageContent, AccountsSummaries, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME:
                                        this.BindPortfolioAccountAnalysisWidgetData(pageContent, scriptHtmlRenderer, _lstAccountAnalysis, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME:
                                        this.BindPortfolioRemindersWidgetData(pageContent, Reminders, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME:
                                        this.BindPortfolioNewsAlertsWidgetData(pageContent, NewsAndAlerts, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME:
                                        this.BindGreenbacksTotalRewardPointsWidgetData(pageContent, AccountsSummaries, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME:
                                        this.BindGreenbacksYtdRewardsPointsGraphWidgetData(pageContent, scriptHtmlRenderer, GreenbacksRewardPoints, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME:
                                        this.BindGreenbacksPointsRedeemedYtdGraphWidgetData(pageContent, scriptHtmlRenderer, GreenbacksRedeemedPoints, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME:
                                        this.BindGreenbacksProductRelatedPonitsEarnedGraphWidgetData(pageContent, scriptHtmlRenderer, CustProductMonthwiseRewardPoints, page, widget);
                                        break;

                                    case HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME:
                                        this.BindGreenbacksCategorySpendRewardPointsGraphWidgetData(pageContent, scriptHtmlRenderer, CustRewardSpendByCategory, page, widget);
                                        break;

                                    case HtmlConstants.WEALTH_BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
                                        this.BindDataToWealthBreakdownOfInvestmentAccountsWidget(pageContent, scriptHtmlRenderer, investmentMasters, page, widget, batchMaster, customer, statementRawData.OutputLocation, widgets.Any(x => x.WidgetName.Contains("Wealth")));
                                        break;
                                    case HtmlConstants.INVESTMENT_WEALTH_PORTFOLIO_STATEMENT_WIDGET_NAME:
                                        BindDataToWealthInvestmentPortfolioStatementWidget(pageContent, customer, investmentMasters, page, widget);
                                        break;
                                    case HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_NAME:
                                        this.BindDataToWealthInvestorPerformanceStatementWidget(pageContent, investmentMasters, page, widget, customer);
                                        break;
                                    case HtmlConstants.WEALTH_EXPLANATORY_NOTES_WIDGET_NAME:
                                        this.BindExplanatoryNotesWidgetData(pageContent, batchMaster, page, widget, tenantCode);
                                        break;
                                }
                            }
                            else
                            {
                                var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
                                if (dynaWidgets.Count > 0)
                                {
                                    var dynawidget = dynaWidgets.FirstOrDefault();
                                    var themeDetails = new CustomeTheme();
                                    if (dynawidget.ThemeType == "Default")
                                    {
                                        themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(statementRawData.TenantConfiguration.WidgetThemeSetting);
                                    }
                                    else
                                    {
                                        themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
                                    }

                                    //Get data from database for widget
                                    httpClient = new HttpClient();
                                    httpClient.BaseAddress = new Uri(statementRawData.TenantConfiguration.BaseUrlForTransactionData);
                                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
                                    httpClient.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, tenantCode);

                                    //API search parameter
                                    JObject searchParameter = new JObject();
                                    searchParameter[ModelConstant.BATCH_ID] = batchMaster.Identifier;
                                    searchParameter[ModelConstant.CUSTOEMR_ID] = customer.CustomerId;
                                    searchParameter[ModelConstant.WIDGET_FILTER_SETTING] = dynawidget.WidgetFilterSettings;

                                    switch (dynawidget.WidgetType)
                                    {
                                        case HtmlConstants.TABLE_DYNAMICWIDGET:
                                            this.BindDynamicTableWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                            break;
                                        case HtmlConstants.FORM_DYNAMICWIDGET:
                                            this.BindDynamicFormWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                            break;
                                        case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
                                            this.BindDynamicLineGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                            break;
                                        case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
                                            this.BindDynamicBarGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
                                            break;
                                        case HtmlConstants.PICHART_DYNAMICWIDGET:
                                            this.BindDynamicPieChartWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails, tenantCode);
                                            break;
                                        case HtmlConstants.HTML_DYNAMICWIDGET:
                                            this.BindDynamicHtmlWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
                                            break;
                                    }
                                }
                            }
                        }

                        newPageContent.Append(pageContent);
                        statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
                        statementPageContent.HtmlContent = newPageContent.ToString();
                    }

                    //to add statement metadata records
                    if (IsInvestmentPageTypePresent && investmentMasters != null && investmentMasters.Count > 0)
                    {
                        investmentMasters.ForEach(invest =>
                        {
                            var InvestmentNo = Convert.ToString(invest.InvestorId) + Convert.ToString(invest.InvestmentId);
                            while (InvestmentNo.Length != 12)
                            {
                                InvestmentNo = "0" + InvestmentNo;
                            }

                            //to separate to string dates values into required date format -- 
                            //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
                            //1st try with string separator, if fails then try with char separator
                            var statementPeriod = string.Empty;
                            string[] stringSeparators = new string[] { " - ", "- ", " -" };
                            string[] dates = invest.StatementPeriod.Split(stringSeparators, StringSplitOptions.None);
                            if (dates.Length > 0)
                            {
                                if (dates.Length > 1)
                                {
                                    statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[1]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
                                }
                                else
                                {
                                    dates = investmentMasters[0].StatementPeriod.Split(new Char[] { ' ' });
                                    if (dates.Length > 2)
                                    {
                                        statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[2]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
                                    }
                                }
                            }
                            else
                            {
                                statementPeriod = investmentMasters[0].StatementPeriod;
                            }

                            statementMetadataRecords.Add(new StatementMetadata
                            {
                                AccountNumber = InvestmentNo,
                                AccountType = invest.ProductDesc,
                                CustomerId = customer.CustomerId,
                                CustomerName = customer.FirstName + " " + customer.SurName,
                                StatementPeriod = statementPeriod,
                                StatementId = statement.Identifier,
                            });
                        });
                    }
                    if (IsPersonalLoanPageTypePresent && PersonalLoanAccounts != null && PersonalLoanAccounts.Count > 0)
                    {
                        PersonalLoanAccounts.ForEach(PersonalLoan =>
                        {
                            statementMetadataRecords.Add(new StatementMetadata
                            {
                                AccountNumber = PersonalLoan.InvestorId.ToString(),
                                AccountType = (!string.IsNullOrEmpty(PersonalLoan.ProductType) ? PersonalLoan.ProductType : HtmlConstants.PERSONAL_LOAN_PAGE_TYPE),
                                CustomerId = customer.CustomerId,
                                CustomerName = customer.FirstName.Trim() + " " + customer.SurName.Trim(),
                                StatementPeriod = PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy),
                                StatementId = statement.Identifier,
                            });
                        });
                    }
                    if (IsHomeLoanPageTypePresent && HomeLoanAccounts != null && HomeLoanAccounts.Count > 0)
                    {
                        HomeLoanAccounts.ForEach(HomeLoan =>
                        {
                            statementMetadataRecords.Add(new StatementMetadata
                            {
                                AccountNumber = HomeLoan.InvestorId.ToString(),
                                AccountType = HtmlConstants.HOME_LOAN_PAGE_TYPE,
                                CustomerId = customer.CustomerId,
                                CustomerName = customer.FirstName.Trim() + " " + customer.SurName.Trim(),
                                StatementPeriod = "Monthly",
                                StatementId = statement.Identifier,
                            });
                        });
                    }

                    //NAV bar will append to html statement, only if statement definition have more than 1 pages 
                    if (statement.Pages.Count > 1)
                    {
                        htmlbody.Append(HtmlConstants.NEDBANK_NAV_BAR_HTML.Replace("{{Today}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy)).Replace("{{NavItemList}}", NavItemList.ToString()));
                    }

                    htmlbody.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
                    newStatementPageContents.ToList().ForEach(page =>
                    {
                        htmlbody.Append(page.PageHeaderContent);
                        htmlbody.Append(page.HtmlContent);
                        htmlbody.Append(page.PageFooterContent);
                        htmlbody.Append(HtmlConstants.PAGE_FOOTER_HTML);
                    });
                    htmlbody.Append(HtmlConstants.END_DIV_TAG); // end tab-content div
                    
                    if (widgets.Where(x => x.WidgetName.Contains("Wealth")).Count() > 0)
                    {
                        var footerContent = new StringBuilder(HtmlConstants.WEALTH_NEDBANK_STATEMENT_FOOTER);
                        footerContent.Replace("{{NedbankSloganImage}}", "../common/images/Footer_Image.png");
                        htmlbody.Append(footerContent.ToString());
                    }
                    else
                    {
                        var footerContent = new StringBuilder(HtmlConstants.NEDBANK_STATEMENT_FOOTER);
                        footerContent.Replace("{{NedbankSloganImage}}", "../common/images/See_money_differently.PNG");
                        footerContent.Replace("{{NedbankNameImage}}", "../common/images/NEDBANK_Name.png");
                        footerContent.Replace("{{FooterText}}", HtmlConstants.NEDBANK_STATEMENT_FOOTER_TEXT_STRING);
                        footerContent.Replace("{{LastFooterText}}", string.Empty);
                        htmlbody.Append(footerContent.ToString());
                    }

                    htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER); // end of container-fluid div

                    var finalHtml = new StringBuilder();
                    finalHtml.Append(HtmlConstants.HTML_HEADER);
                    finalHtml.Append(htmlbody.ToString());
                    finalHtml.Append(HtmlConstants.HTML_FOOTER);

                    //replace below variable with actual data in final html string
                    finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
                    finalHtml.Replace("{{CustomerNumber}}", customer.CustomerId.ToString());
                    finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
                    finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
                    finalHtml.Replace("{{TenantCode}}", tenantCode);

                    finalHtml = Translate(finalHtml, customer);

                    //If has any error while rendering html statement, then assign status as failed and all collected errors message to log message variable..
                    //Otherwise write html statement string to actual html file and store it at output location, then assign status as completed
                    if (IsFailed)
                    {
                        logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
                        logDetailRecord.LogMessage = "<ul class='pl-4 text-left'>" + ErrorMessages.ToString() + "</ul>";
                    }
                    else
                    {
                        string fileName = "Statement_" + customer.CustomerId + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
                        string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, batchMaster.Identifier, customer.CustomerId, statementRawData.BaseURL, statementRawData.OutputLocation);

                        logDetailRecord.StatementFilePath = filePath;
                        logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
                        logDetailRecord.LogMessage = "Statement generated successfully..!!";
                        logDetailRecord.statementMetadata = statementMetadataRecords;
                    }
                }

                return logDetailRecord;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region These methods helps to bind Data to static widgets of Financial HTML Statement

        private void BindCustomerInformationWidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails)
        {
            pageContent.Replace("{{CustomerName}}", (customer.FirstName.Trim() + " " + (customer.MiddleName == string.Empty ? string.Empty : " " + customer.MiddleName.Trim()) + " " + customer.LastName.Trim()));
            pageContent.Replace("{{Address1}}", customer.AddressLine1);
            string address2 = (customer.AddressLine2 != "" ? customer.AddressLine2 + ", " : "") + (customer.City != "" ? customer.City + ", " : "") + (customer.State != "" ? customer.State + ", " : "") + (customer.Country != "" ? customer.Country + ", " : "") + (customer.Zip != "" ? customer.Zip : "");
            pageContent.Replace("{{Address2}}", address2);

            var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
            if (custMedia != null && custMedia.VideoURL != string.Empty)
            {
                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", custMedia.VideoURL);
            }
            else
            {
                var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                {
                    pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", batchDetail.VideoURL);
                }
            }
        }

        private void BindAccountInformationWidgetData(StringBuilder pageContent, CustomerMaster customer, Page page, PageWidget widget)
        {
            StringBuilder AccDivData = new StringBuilder();
            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Date" + "</div><label class='list-value mb-0'>" + Convert.ToDateTime(customer.StatementDate).ToShortDateString() + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Statement Period" + "</div><label class='list-value mb-0'>" + customer.StatementPeriod + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>Cusomer ID" + "</div><label class='list-value mb-0'>" + customer.CustomerCode + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Name" + "</div><label class='list-value mb-0'>" + customer.RmName + "</label></div></div>");

            AccDivData.Append("<div class='list-row-small ht70px'><div class='list-middle-row'> <div class='list-text'>RM Contact Number" + "</div><label class='list-value mb-0'>" + customer.RmContactNumber + "</label></div></div>");
            pageContent.Replace("{{AccountInfoData_" + page.Identifier + "_" + widget.Identifier + "}}", AccDivData.ToString());
        }

        private bool BindSummaryAtGlanceWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, IList<AccountMaster> accountrecords, Page page, PageWidget widget)
        {
            var IsFailed = false;
            if (accountrecords != null && accountrecords.Count > 0)
            {
                var accSummary = new StringBuilder();
                var accRecords = accountrecords.GroupBy(item => item.AccountType).ToList();
                accRecords.ForEach(acc =>
                {
                    accSummary.Append("<tr><td>" + acc.FirstOrDefault().AccountType + "</td><td>" + acc.FirstOrDefault().Currency + "</td><td>" + acc.Sum(it => Convert.ToDecimal(it.Balance)).ToString() + "</td></tr>");
                });
                pageContent.Replace("{{AccountSummary_" + page.Identifier + "_" + widget.Identifier + "}}", accSummary.ToString());
            }
            else
            {
                ErrorMessages.Append("<li>Account master data is not available related to Summary at Glance widget..!!</li>");
                IsFailed = true;
            }
            return IsFailed;
        }

        private bool BindCurrentAvailBalanceWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, long accountId, IList<AccountMaster> accountrecords, Page page, PageWidget widget, string currency)
        {
            var IsFailed = false;
            if (accountrecords != null && accountrecords.Count > 0)
            {
                var currentAccountRecords = accountrecords.Where(item => item.Identifier == accountId)?.ToList();
                if (currentAccountRecords != null && currentAccountRecords.Count > 0)
                {
                    var records = currentAccountRecords.GroupBy(item => item.AccountType).ToList();
                    records?.ForEach(acc =>
                    {
                        var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                        pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                        pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.GrandTotal)).ToString()));
                        pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalDeposit)).ToString()));
                        pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalSpend)).ToString()));
                        pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.ProfitEarned)).ToString()));
                    });
                }
                else
                {
                    ErrorMessages.Append("<li>Current Account master data is not available related to Current Available Balance widget..!!</li>");
                    IsFailed = true;
                }
            }
            else
            {
                ErrorMessages.Append("<li>Account master data is not available related to Current Available Balance widget..!!</li>");
                IsFailed = true;
            }
            return IsFailed;
        }

        private bool BindSavingAvailBalanceWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, long accountId, IList<AccountMaster> accountrecords, Page page, PageWidget widget, string currency)
        {
            var IsFailed = false;
            if (accountrecords != null && accountrecords.Count > 0)
            {
                var savingAccountRecords = accountrecords.Where(item => item.Identifier == accountId)?.ToList();
                if (savingAccountRecords != null && savingAccountRecords.Count > 0)
                {
                    var records = savingAccountRecords.GroupBy(item => item.AccountType).ToList();
                    records?.ForEach(acc =>
                    {
                        var accountIndicatorClass = acc.FirstOrDefault().Indicator.ToLower().Equals("up") ? "fa fa-sort-asc text-success" : "fa fa-sort-desc text-danger";
                        pageContent.Replace("{{AccountIndicatorClass}}", accountIndicatorClass);
                        pageContent.Replace("{{TotalValue_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.GrandTotal)).ToString()));
                        pageContent.Replace("{{TotalDeposit_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalDeposit)).ToString()));
                        pageContent.Replace("{{TotalSpend_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.TotalSpend)).ToString()));
                        pageContent.Replace("{{Savings_" + page.Identifier + "_" + widget.Identifier + "}}", (currency + " " + acc.Sum(it => Convert.ToDecimal(it.ProfitEarned)).ToString()));
                    });
                }
                else
                {
                    ErrorMessages.Append("<li>Saving Account master data is not available related to Saving Available Balance widget..!!</li>");
                    IsFailed = true;
                }
            }
            else
            {
                ErrorMessages.Append("<li>Account master data is not available related to Saving Available Balance widget..!!</li>");
                IsFailed = true;
            }
            return IsFailed;
        }

        private void BindSavingTransactionWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountTransaction> CustomerAcccountTransactions, Page page, PageWidget widget, long accountId, string tenantCode, string currency, string outputLocation)
        {
            var accountTransactions = CustomerAcccountTransactions.Where(item => item.AccountId == accountId)?.ToList();
            var transaction = new StringBuilder();
            var selectOption = new StringBuilder();

            if (accountTransactions != null && accountTransactions.Count > 0)
            {
                pageContent.Replace("{{Currency}}", currency);
                // convert it to json format string and store it as file at same directory of html statement file
                string savingtransactionjson = JsonConvert.SerializeObject(accountTransactions);
                if (savingtransactionjson != null && savingtransactionjson != string.Empty)
                {
                    var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                    distinctNaration.ForEach(item =>
                    {
                        selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                    });

                    var SavingTransactionGridJson = "savingtransactiondata" + accountId + page.Identifier + "=" + savingtransactionjson;
                    this.utility.WriteToJsonFile(SavingTransactionGridJson, "savingtransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtransactiondetail" + accountId + page.Identifier + ".json'></script>");

                    var scriptval = new StringBuilder(HtmlConstants.SAVING_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                    scriptval.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                    scriptval.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                    scriptval.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                    scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                    scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                    scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                    scriptval.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                    scriptHtmlRenderer.Append(scriptval);

                    pageContent.Replace("savingtransactiondata", "savingtransactiondata" + accountId + page.Identifier);
                    pageContent.Replace("savingShowAll", "savingShowAll" + accountId + page.Identifier);
                    pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                    pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                    pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                    pageContent.Replace("savingtransactionRadio", "savingtransactionRadio" + accountId + page.Identifier);
                    pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                    pageContent.Replace("SavingTransactionTable", "SavingTransactionTable" + accountId + page.Identifier);
                }
                else
                {
                    transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                }
            }
            else
            {
                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
            }

            pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
        }

        private void BindCurrentTransactionWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountTransaction> CustomerAcccountTransactions, Page page, PageWidget widget, long accountId, string tenantCode, string currency, string outputLocation)
        {
            var accountTransactions = CustomerAcccountTransactions.Where(item => item.AccountId == accountId)?.ToList();
            var transaction = new StringBuilder();
            var selectOption = new StringBuilder();
            if (accountTransactions != null && accountTransactions.Count > 0)
            {
                pageContent.Replace("{{Currency}}", currency);
                //convert it to json format string and store it as json file at same directory of html statement file
                string currenttransactionjson = JsonConvert.SerializeObject(accountTransactions);
                if (currenttransactionjson != null && currenttransactionjson != string.Empty)
                {
                    var distinctNaration = accountTransactions.Select(item => item.Narration).Distinct().ToList();
                    distinctNaration.ForEach(item =>
                    {
                        selectOption.Append("<option value='" + item + "'> " + item + "</option>");
                    });

                    var CurrentTransactionGridJson = "currenttransactiondata" + accountId + page.Identifier + "=" + currenttransactionjson;
                    this.utility.WriteToJsonFile(CurrentTransactionGridJson, "currenttransactiondetail" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
                    scriptHtmlRenderer.Append("<script type='text/javascript' src='./currenttransactiondetail" + accountId + page.Identifier + ".json'></script>");

                    var scriptval = new StringBuilder(HtmlConstants.CURRENT_TRANSACTION_DETAIL_GRID_WIDGET_SCRIPT);
                    scriptval.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                    scriptval.Replace("currenttransactiondata", "currenttransactiondata" + accountId + page.Identifier);
                    scriptval.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                    scriptval.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                    scriptval.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                    scriptval.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                    scriptval.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                    scriptHtmlRenderer.Append(scriptval);

                    pageContent.Replace("currentShowAll", "currentShowAll" + accountId + page.Identifier);
                    pageContent.Replace("filterStatus", "filterStatus" + accountId + page.Identifier);
                    pageContent.Replace("ResetGrid", "ResetGrid" + accountId + page.Identifier);
                    pageContent.Replace("PrintGrid", "PrintGrid" + accountId + page.Identifier);
                    pageContent.Replace("currenttransactionRadio", "currenttransactionRadio" + accountId + page.Identifier);
                    pageContent.Replace("{{SelectOption_" + page.Identifier + "_" + widget.Identifier + "}}", selectOption.ToString());
                    pageContent.Replace("CurrentTransactionTable", "CurrentTransactionTable" + accountId + page.Identifier);
                }
                else
                {
                    transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
                }
            }
            else
            {
                transaction.Append("<tr><td colspan='7' class='text-danger text-center'><span>No data available</span></td></tr>");
            }
            pageContent.Replace("{{AccountTransactionDetails_" + page.Identifier + "_" + widget.Identifier + "}}", transaction.ToString());
        }

        private void BindTop4IncomeSourcesWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var top4IncomeSources = this.tenantTransactionDataRepository.GetCustomerIncomeSources(customer.Identifier, batchMaster.Identifier, tenantCode)?.OrderByDescending(it => Convert.ToDecimal(it.CurrentSpend))?.Take(4)?.ToList();
                var incomeSources = new StringBuilder();
                if (top4IncomeSources != null && top4IncomeSources.Count > 0)
                {
                    top4IncomeSources.ForEach(src =>
                    {
                        var tdstring = string.Empty;
                        if (Convert.ToDecimal(src.CurrentSpend) > Convert.ToDecimal(src.AverageSpend))
                        {
                            tdstring = "<span class='fa fa-sort-desc fa-2x text-danger' aria-hidden='true'></span><span class='ml-2'>" + src.AverageSpend + "</span>";
                        }
                        else
                        {
                            tdstring = "<span class='fa fa-sort-asc fa-2x mt-1' aria-hidden='true' style='position:relative;top:6px;color:limegreen'></span><span class='ml-2'>" + src.AverageSpend + "</span>";
                        }
                        incomeSources.Append("<tr><td class='float-left'>" + src.Source + "</td>" + "<td> " + src.CurrentSpend + "</td><td>" + tdstring + "</td></tr>");
                    });
                }
                else
                {
                    incomeSources.Append("<tr><td colspan='3' class='text-danger text-center'><div style='margin-top: 20px;'>No data available</div></td></tr>");
                }
                pageContent.Replace("{{IncomeSourceList_" + page.Identifier + "_" + widget.Identifier + "}}", incomeSources.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindAnalyticsChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, CustomerMaster customer, BatchMaster batchMaster, IList<AccountMaster> accountrecords, Page page, string outputLocation)
        {
            var AnalyticsChartJson = string.Empty;
            if (accountrecords.Count > 0)
            {
                var accounts = new List<AccountMasterRecord>();
                var records = accountrecords.GroupBy(item => item.AccountType).ToList();

                //get analytics chart widget data, convert it into json string format and store it as json file at same directory of html statement file
                records?.ForEach(acc => accounts.Add(new AccountMasterRecord()
                {
                    AccountType = acc.FirstOrDefault().AccountType,
                    Percentage = acc.Average(item => item.Percentage == null || item.Percentage == string.Empty ? 0 : Convert.ToDecimal(item.Percentage))
                }));

                string accountsJson = JsonConvert.SerializeObject(accounts);
                if (accountsJson != null && accountsJson != string.Empty)
                {
                    AnalyticsChartJson = "analyticsdata=" + accountsJson;
                }
                else
                {
                    AnalyticsChartJson = "analyticsdata=[]";
                }
            }
            else
            {
                AnalyticsChartJson = "analyticsdata=[]";
            }

            this.utility.WriteToJsonFile(AnalyticsChartJson, "analyticschartdata.json", batchMaster.Identifier, customer.Identifier, outputLocation);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./analyticschartdata.json'></script>");
            pageContent.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier);
            scriptHtmlRenderer.Append(HtmlConstants.ANALYTICS_CHART_WIDGET_SCRIPT.Replace("analyticschartcontainer", "analyticschartcontainer" + page.Identifier));
        }

        private bool BindSavingTrendChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, IList<SavingTrend> CustomerSavingTrends, long accountId, Page page, string tenantCode, string outputLocation)
        {
            var SavingTrendChartJson = string.Empty;
            var IsFailed = false;

            var savingtrends = CustomerSavingTrends.Where(item => item.AccountId == accountId).ToList();
            if (savingtrends != null && savingtrends.Count > 0)
            {
                var currentMonth = DateTime.Now.Month;
                int mnth = currentMonth - 1 == 0 ? 12 : currentMonth - 1;  //To start month validation of consecutive month data from previous month
                for (int t = savingtrends.Count; t > 0; t--)
                {
                    string month = this.utility.getMonth(mnth);
                    var lst = savingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                    if (lst != null && lst.Count > 0)
                    {
                        lst[0].NumericMonth = mnth;
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Invalid consecutive month data for Saving trend widget..!!</li>");
                        IsFailed = true;
                    }
                    mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                }

                //get saving trend chart widget data, convert it into json string format and store it as json file at same directory of html statement file
                string savingtrendjson = JsonConvert.SerializeObject(savingtrends.OrderByDescending(item => item.NumericMonth).Take(6).ToList());
                if (savingtrendjson != null && savingtrendjson != string.Empty)
                {
                    SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=" + savingtrendjson;
                }
                else
                {
                    SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
                }
            }
            else
            {
                SavingTrendChartJson = "savingdata" + accountId + page.Identifier + "=[]";
            }

            this.utility.WriteToJsonFile(SavingTrendChartJson, "savingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./savingtrenddata" + accountId + page.Identifier + ".json'></script>");

            pageContent.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier);
            var scriptval = HtmlConstants.SAVING_TREND_CHART_WIDGET_SCRIPT.Replace("savingTrendscontainer", "savingTrendscontainer" + accountId + page.Identifier).Replace("savingdata", "savingdata" + accountId + page.Identifier);
            scriptHtmlRenderer.Append(scriptval);

            return IsFailed;
        }

        private bool BindSpendingTrendChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, StringBuilder ErrorMessages, CustomerMaster customer, BatchMaster batchMaster, IList<SavingTrend> CustomerSavingTrends, long accountId, Page page, string tenantCode, string outputLocation)
        {
            var IsFailed = false;
            var SpendingTrendChartJson = string.Empty;
            var spendingtrends = CustomerSavingTrends.Where(item => item.AccountId == accountId).ToList();
            if (spendingtrends != null && spendingtrends.Count > 0)
            {
                var currentMonth = DateTime.Now.Month;
                int mnth = currentMonth - 1 == 0 ? 12 : currentMonth - 1;  //To start month validation of consecutive month data from previous month
                for (int t = spendingtrends.Count; t > 0; t--)
                {
                    string month = this.utility.getMonth(mnth);
                    var lst = spendingtrends.Where(it => it.Month.ToLower().Contains(month.ToLower()))?.ToList();
                    if (lst != null && lst.Count > 0)
                    {
                        lst[0].NumericMonth = mnth;
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Invalid consecutive month data for Spending trend widget..!!</li>");
                        IsFailed = true;
                    }
                    mnth = mnth - 1 == 0 ? 12 : mnth - 1;
                }

                //get spending trend chart widget data, convert it into json string format and store it as json file at same directory of html statement file
                string spendingtrendjson = JsonConvert.SerializeObject(spendingtrends.OrderByDescending(item => item.NumericMonth).Take(6).ToList());
                if (spendingtrendjson != null && spendingtrendjson != string.Empty)
                {
                    SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=" + spendingtrendjson;
                }
                else
                {
                    SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
                }
            }
            else
            {
                SpendingTrendChartJson = "spendingdata" + accountId + page.Identifier + "=[]";
            }

            this.utility.WriteToJsonFile(SpendingTrendChartJson, "spendingtrenddata" + accountId + page.Identifier + ".json", batchMaster.Identifier, customer.Identifier, outputLocation);
            scriptHtmlRenderer.Append("<script type='text/javascript' src='./spendingtrenddata" + accountId + page.Identifier + ".json'></script>");

            pageContent.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier);
            var scriptval = HtmlConstants.SPENDING_TREND_CHART_WIDGET_SCRIPT.Replace("spendingTrendscontainer", "spendingTrendscontainer" + accountId + page.Identifier).Replace("spendingdata", "spendingdata" + accountId + page.Identifier);
            scriptHtmlRenderer.Append(scriptval);

            return IsFailed;
        }

        private void BindReminderAndRecommendationWidgetData(StringBuilder pageContent, CustomerMaster customer, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var reminderAndRecommendations = this.tenantTransactionDataRepository.GetReminderAndRecommendation(customer.Identifier, batchMaster.Identifier, tenantCode);
                var reminderstr = new StringBuilder();
                if (reminderAndRecommendations != null && reminderAndRecommendations.Count > 0)
                {
                    reminderstr.Append("<div class='row'><div class='col-lg-9'></div><div class='col-lg-3 text-left'><i class='fa fa-caret-left fa-3x float-left text-danger' aria-hidden='true'></i><span class='mt-2 d-inline-block ml-2'>Click</span></div> </div>");
                    reminderAndRecommendations.ToList().ForEach(item =>
                    {
                        string targetlink = item.Action != null && item.Action != string.Empty ? item.Action : "javascript:void(0)";
                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + targetlink + "' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-danger'></i><span class='mt-2 d-inline-block ml-2'>" + item.Title + "</span></a></div></div>");
                    });
                }
                else
                {
                    reminderstr.Append("<div class='row text-danger text-center' style='margin-top: 20px;'>No data available</div>");
                }
                pageContent.Replace("{{ReminderAndRecommdationDataList_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region These methods helps to bind data to Image and Video widgets

        private bool BindImageWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, long customerId, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails, Statement statement, Page page, BatchMaster batchMaster, PageWidget widget, string tenantCode, string outputLocation)
        {
            var imgHeight = "auto";
            var imgAlignment = "text-center";
            var imgAssetFilepath = string.Empty;
            var IsFailed = false;

            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.isPersonalize == false) //Is not dynamic image, then assign selected image from asset library
                {
                    var asset = this.assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var path = asset.FilePath.ToString();
                        var fileName = asset.Name;
                        var imagePath = outputLocation + "\\Statements\\" + batchMaster.Identifier + "\\common\\media";
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }
                        if (File.Exists(path) && !File.Exists(imagePath + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(imagePath, fileName));
                        }
                        imgAssetFilepath = "../common/media/" + fileName;
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Image asset file not found in asset library for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!</li>");
                        IsFailed = true;
                    }
                }
                else //Is dynamic image, then assign it from database 
                {
                    var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault(); //error if multiple records
                    if (custMedia != null && custMedia.ImageURL != string.Empty)
                    {
                        imgAssetFilepath = custMedia.ImageURL;
                    }
                    else
                    {
                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                        if (batchDetail != null && batchDetail.ImageURL != string.Empty)
                        {
                            imgAssetFilepath = batchDetail.ImageURL;
                        }
                        else
                        {
                            ErrorMessages.Append("<li>Image not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for image widget..!!</li>");
                            IsFailed = true;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Convert.ToString(widgetSetting.Height)) && Convert.ToString(widgetSetting.Height) != "0")
                {
                    imgHeight = widgetSetting.Height + "px";
                }

                if (widgetSetting.Align != null)
                {
                    imgAlignment = widgetSetting.Align == 1 ? "text-left" : widgetSetting.Align == 2 ? "text-right" : "text-center";
                }

                pageContent.Replace("{{ImgHeight_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgHeight);
                pageContent.Replace("{{ImgAlignmentClass_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAlignment);
                pageContent.Replace("{{ImageSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", imgAssetFilepath);
            }
            else
            {
                ErrorMessages.Append("<li>Image widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!</li>");
                IsFailed = true;
            }

            return IsFailed;
        }

        private bool BindVideoWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, long customerId, IList<CustomerMedia> customerMedias, IList<BatchDetail> batchDetails, Statement statement, Page page, BatchMaster batchMaster, PageWidget widget, string tenantCode, string outputLocation)
        {
            var vdoAssetFilepath = string.Empty;
            var IsFailed = false;

            if (widget.WidgetSetting != string.Empty && validationEngine.IsValidJson(widget.WidgetSetting))
            {
                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                if (widgetSetting.isEmbedded == true)//If embedded then assigned it it from widget config json source url
                {
                    vdoAssetFilepath = widgetSetting.SourceUrl;
                }
                else if (widgetSetting.isPersonalize == false && widgetSetting.isEmbedded == false) //If not dynamic video, then assign selected video from asset library
                {
                    var asset = this.assetLibraryRepository.GetAssets(new AssetSearchParameter { Identifier = widgetSetting.AssetId, SortParameter = new SortParameter { SortColumn = "Id" } }, tenantCode).ToList()?.FirstOrDefault();
                    if (asset != null)
                    {
                        var path = asset.FilePath.ToString();
                        var fileName = asset.Name;
                        var videoPath = outputLocation + "\\Statements\\" + batchMaster.Identifier + "\\common\\media";
                        if (!Directory.Exists(videoPath))
                        {
                            Directory.CreateDirectory(videoPath);
                        }
                        if (File.Exists(path) && !File.Exists(videoPath + "\\" + fileName))
                        {
                            File.Copy(path, Path.Combine(videoPath, fileName));
                        }
                        vdoAssetFilepath = "../common/media/" + fileName;
                    }
                    else
                    {
                        ErrorMessages.Append("<li>Video asset file not found in asset library for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!</li>");
                        IsFailed = true;
                    }
                }
                else //If dynamic video, then assign it from database 
                {
                    var custMedia = customerMedias.Where(item => item.PageId == page.Identifier && item.WidgetId == widget.Identifier)?.ToList()?.FirstOrDefault();
                    if (custMedia != null && custMedia.VideoURL != string.Empty)
                    {
                        vdoAssetFilepath = custMedia.VideoURL;
                    }
                    else
                    {
                        var batchDetail = batchDetails.Where(item => item.StatementId == statement.Identifier && item.WidgetId == widget.Identifier && item.PageId == page.Identifier)?.ToList()?.FirstOrDefault();
                        if (batchDetail != null && batchDetail.VideoURL != string.Empty)
                        {
                            vdoAssetFilepath = batchDetail.VideoURL;
                        }
                        else
                        {
                            ErrorMessages.Append("<li>Video not found for Page: " + page.Identifier + " and Widget: " + widget.Identifier + " for video widget..!!</li>");
                            IsFailed = true;
                        }
                    }
                }
                pageContent.Replace("{{VideoSource_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", vdoAssetFilepath);
            }
            else
            {
                ErrorMessages.Append("<li>Video widget configuration is missing for Page: " + page.Identifier + " and Widget: " + widget.Identifier + "!!</li>");
                IsFailed = true;
            }

            return IsFailed;
        }

        #endregion

        private bool BindSegmentBasedContentWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages, bool isWealthWidget)
        {
            try
            {
                var content = HtmlConstants.SEGMENT_BASED_CONTENT_WIDGET_HTML;

                dynamic widgetSetting = JArray.Parse(widget.WidgetSetting);
                var html = "";
                if (widgetSetting[0].Html.ToString().Length > 0)
                {
                    html = widgetSetting[0].Html; //TODO: ***Deepak: Remove hard coded line
                }

                var chanceToWin = string.Empty;
                switch (customer.Segment.ToLower())
                {
                    case "consumer banking":
                    case "pbvcm":
                    case "cib":
                    case "nbb":
                        chanceToWin = "";
                        break;
                    case "ncb":
                    case "prb":
                    case "sbs":
                        chanceToWin = isWealthWidget ? (customer.Language == "ENG" ? HtmlConstants.WEALTH_CHANCE_TO_WIN : HtmlConstants.WEALTH_CHANCE_TO_WIN_AFR) : (customer.Language == "ENG" ? HtmlConstants.CHANCE_TO_WIN : HtmlConstants.CHANCE_TO_WIN_AFR);
                        break;
                }

                html = html.Replace("{{chanceToWin}}", chanceToWin);

                content = content.Replace("{{SegmentBasedContent}}", html);

                pageContent.Replace("{{SegmentBasedContent_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", content);

                return false;
            }
            catch (Exception ex)
            {
                ErrorMessages.Append("<li>Error occurred while configuring SegmentBasedContent widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
                return true;
            }
        }

        private bool BindStaticHtmlWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages)
        {
            try
            {
                var content = HtmlConstants.STATIC_HTML_WIDGET_HTML;

                dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
                var html = "";
                if (widgetSetting.html.ToString().Length > 0)
                {
                    html = widgetSetting.html;
                }

                var contactCenter = string.Empty;
                switch (customer.Segment.ToLower())
                {
                    case "consumer banking":
                    case "ncb":
                    case "pbvcm":
                        contactCenter = HtmlConstants.CONSUMER_BANKING;
                        break;
                    case "prb":
                        contactCenter = HtmlConstants.PRIVATE_BANKING;
                        break;
                    case "sbs":
                        contactCenter = HtmlConstants.SBS_BANKING;
                        break;
                    case "nbb":
                        contactCenter = HtmlConstants.NBB_BANKING;
                        break;
                    case "cib":
                        contactCenter = HtmlConstants.CORPORATE_BANKING;
                        break;
                }

                html = html.Replace("{{contactCenter}}", contactCenter);

                content = content.Replace("{{StaticHtml}}", html);

                pageContent.Replace("{{StaticHtml_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", content);

                return false;
            }
            catch (Exception ex)
            {
                ErrorMessages.Append("<li>Error occurred while configuring StaticHtml widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
                return true;
            }
        }

        #region These methods helps to bind data to Dynamic widgets

        private void BindDynamicTableWidgetData(StringBuilder pageContent, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient)
        {
            try
            {
                var tableEntities = JsonConvert.DeserializeObject<List<DynamicWidgetTableEntity>>(dynawidget.WidgetSettings);
                var tr = new StringBuilder();

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        apiOutputArr.ToList().ForEach(op =>
                        {
                            tr.Append("<tr>");
                            tableEntities.ForEach(field =>
                            {
                                tr.Append("<td> " + op[field.FieldName] + " </td>");
                            });
                            tr.Append("</tr>");
                        });
                    }
                    else
                    {
                        tr.Append("<tr><td colspan='" + (tableEntities.Count + 1) + "'> No record found </td></tr>)");
                    }
                }
                else
                {
                    tr.Append("<tr><td colspan='" + (tableEntities.Count + 1) + "'> No record found </td></tr>");
                }
                pageContent.Replace("{{tableBody_" + page.Identifier + "_" + widget.Identifier + "}}", tr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicFormWidgetData(StringBuilder pageContent, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient)
        {
            try
            {
                var formEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFormEntity>>(dynawidget.WidgetSettings);
                var formdata = new StringBuilder();

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        apiOutputArr.ToList().ForEach(op =>
                        {
                            formEntities.ForEach(field =>
                            {
                                formdata.Append("<div class='row'><div class='col-sm-6'><label>" + field.DisplayName + "</label></div><div class='col-sm-6'>" + op[field.FieldName] + "</div></div>");
                            });
                        });
                    }
                    else
                    {
                        formdata.Append("<div class='row'> No record found </div>");
                    }
                }
                else
                {
                    formdata.Append("<div class='row'> No record found </div>");
                }

                pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", formdata.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicLineGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient, CustomeTheme themeDetails)
        {
            try
            {
                var chartDataVal = string.Empty;

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                        var chartData = new GraphChartData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get chart x-axis list
                        var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                        chartData.xAxis = xAxis;

                        //To get chart series data
                        var chartSeries = new List<ChartSeries>();
                        graphEntity.Details.ToList().ForEach(field =>
                        {
                            var series = new ChartSeries();
                            series.name = field.DisplayName;
                            var res = apiOutputArr.ToList().Select(item => item[field.FieldName]).ToList();
                            var seriesdata = new List<decimal>();
                            res.ForEach(r =>
                            {
                                seriesdata.Add(Convert.ToDecimal(r.ToString()));
                            });
                            series.data = seriesdata;
                            series.type = "line";
                            chartSeries.Add(series);
                        });
                        chartData.series = chartSeries;

                        //To get chart theme
                        string theme = string.Empty;
                        if (themeDetails != null)
                        {
                            if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                            {
                                theme = themeDetails.ChartColorTheme;
                            }
                            else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                            {
                                theme = themeDetails.ColorTheme;
                            }
                        }
                        chartData.color = this.GetChartColorTheme(theme);
                        chartDataVal = JsonConvert.SerializeObject(chartData);
                    }
                }

                pageContent.Replace("hiddenLineGraphValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                scriptHtmlRenderer.Append(HtmlConstants.LINE_GRAPH_WIDGET_SCRIPT.Replace("linechartcontainer", "lineGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenLineGraphData", "hiddenLineGraphData_" + page.Identifier + "_" + widget.Identifier));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicBarGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient, CustomeTheme themeDetails)
        {
            try
            {
                var chartDataVal = string.Empty;

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        var graphEntity = JsonConvert.DeserializeObject<DynamicWidgetLineGraph>(dynawidget.WidgetSettings);
                        var chartData = new GraphChartData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get chart x-axis list
                        var xAxis = apiOutputArr.ToList().Select(item => item[graphEntity.XAxis].ToString()).ToList();
                        chartData.xAxis = xAxis;

                        //To get chart series data
                        var chartSeries = new List<ChartSeries>();
                        graphEntity.Details.ToList().ForEach(field =>
                        {
                            var series = new ChartSeries();
                            series.name = field.DisplayName;
                            var res = apiOutputArr.ToList().Select(item => item[field.FieldName]).ToList();
                            var seriesdata = new List<decimal>();
                            res.ForEach(r =>
                            {
                                seriesdata.Add(Convert.ToDecimal(r.ToString()));
                            });
                            series.data = seriesdata;
                            series.type = "column";
                            chartSeries.Add(series);
                        });
                        chartData.series = chartSeries;

                        //To get chart theme
                        string theme = string.Empty;
                        if (themeDetails != null)
                        {
                            if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                            {
                                theme = themeDetails.ChartColorTheme;
                            }
                            else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                            {
                                theme = themeDetails.ColorTheme;
                            }
                        }
                        chartData.color = this.GetChartColorTheme(theme);
                        chartDataVal = JsonConvert.SerializeObject(chartData);
                    }
                }

                pageContent.Replace("hiddenBarGraphValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                scriptHtmlRenderer.Append(HtmlConstants.BAR_GRAPH_WIDGET_SCRIPT.Replace("barchartcontainer", "barGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenBarGraphData", "hiddenBarGraphData_" + page.Identifier + "_" + widget.Identifier));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicPieChartWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient, CustomeTheme themeDetails, string tenantCode)
        {
            try
            {
                var chartDataVal = string.Empty;

                //API call
                var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                    if (apiOutputArr.Count > 0)
                    {
                        var pieChartSetting = JsonConvert.DeserializeObject<PieChartSettingDetails>(dynawidget.WidgetSettings);
                        var entityFields = this.dynamicWidgetRepository.GetEntityFields(dynawidget.EntityId, tenantCode);
                        var seriesfor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieSeries))?.ToList()?.FirstOrDefault().Name;
                        var seriesdatafor = entityFields.Where(it => it.Identifier == Convert.ToInt32(pieChartSetting.PieValue))?.ToList()?.FirstOrDefault().Name;

                        var chartData = new PiChartGraphData();
                        chartData.title = new ChartTitle { text = dynawidget.Title };

                        //To get series data
                        var chartseries = new List<PieChartSeries>();
                        var datas = new List<PieChartData>();
                        apiOutputArr.ToList().ForEach(item =>
                        {
                            var pie = new PieChartData
                            {
                                name = item[seriesfor] != null ? item[seriesfor].ToString() : "",
                                y = Convert.ToDecimal(item[seriesdatafor] != null ? item[seriesdatafor] : 0)
                            };
                            datas.Add(pie);
                        });

                        PieChartSeries series = new PieChartSeries();
                        series.name = seriesfor;
                        series.data = datas;
                        chartseries.Add(series);
                        chartData.series = chartseries;

                        //To get chart theme
                        string theme = string.Empty;
                        if (themeDetails != null)
                        {
                            if (themeDetails.ChartColorTheme != null && themeDetails.ChartColorTheme != "")
                            {
                                theme = themeDetails.ChartColorTheme;
                            }
                            else if (themeDetails.ColorTheme != null && themeDetails.ColorTheme != "")
                            {
                                theme = themeDetails.ColorTheme;
                            }
                        }
                        chartData.color = this.GetChartColorTheme(theme);
                        chartDataVal = JsonConvert.SerializeObject(chartData);
                    }
                }

                pageContent.Replace("hiddenPieChartValue_" + page.Identifier + "_" + widget.Identifier + "", chartDataVal);
                scriptHtmlRenderer.Append(HtmlConstants.PIE_CHART_WIDGET_SCRIPT.Replace("pieChartcontainer", "pieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("hiddenPieChartData", "hiddenPieChartData_" + page.Identifier + "_" + widget.Identifier));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindDynamicHtmlWidgetData(StringBuilder pageContent, Page page, PageWidget widget, JObject searchParameter, DynamicWidget dynawidget, HttpClient httpClient)
        {
            try
            {
                var htmlWidgetContent = new StringBuilder(dynawidget.PreviewData);
                if (dynawidget.WidgetSettings != null)
                {
                    var _lstHtmlWidgetSettings = JsonConvert.DeserializeObject<List<HtmlWidgetSettings>>(dynawidget.WidgetSettings);
                    if (_lstHtmlWidgetSettings.Count > 0)
                    {
                        //API call
                        var response = httpClient.PostAsync(dynawidget.APIPath, new StringContent(JsonConvert.SerializeObject(searchParameter), Encoding.UTF8, ModelConstant.APPLICATION_JSON_MEDIA_TYPE)).Result;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var apiOutputArr = JArray.Parse(response.Content.ReadAsStringAsync().Result);
                            if (apiOutputArr.Count > 0)
                            {
                                var apidata = apiOutputArr.FirstOrDefault();
                                _lstHtmlWidgetSettings.ForEach(setting =>
                                {
                                    if (setting.Value != null && setting.Value != string.Empty && setting.Key != null && setting.Key != string.Empty && apidata[setting.Key] != null)
                                    {
                                        htmlWidgetContent.Replace(setting.Value, apidata[setting.Key].ToString());
                                    }
                                });
                            }
                        }
                    }
                }
                pageContent.Replace("{{FormData_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidgetContent.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region These methods helps to bind data to static widgets of Nedbank HTML statment

        private void BindCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            var CustomerDetails = (!string.IsNullOrEmpty(customer.Title) && customer.Title.ToLower() != "null" ? customer.Title + " " : string.Empty) + (!string.IsNullOrEmpty(customer.FirstName) && customer.FirstName.ToLower() != "null" ? customer.FirstName + " " : string.Empty) + (!string.IsNullOrEmpty(customer.SurName) && customer.SurName.ToLower() != "null" ? customer.SurName + " " : string.Empty) + "<br>" +
                (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
            pageContent.Replace("{{CustomerDetails_" + page.Identifier + "_" + widget.Identifier + "}}", CustomerDetails);
            //pageContent.Replace("{{MaskCellNo_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No != string.Empty ? "Cell: " + customer.Mask_Cell_No : string.Empty);
        }

        private void BindBranchDetailsWidgetData(StringBuilder pageContent, long BranchId, Page page, PageWidget widget, string tenantCode, DM_CustomerMaster customer)
        {
            try
            {
                var branchDetails = this.tenantTransactionDataRepository.Get_DM_BranchMaster(BranchId, tenantCode)?.FirstOrDefault();
                if (branchDetails != null)
                {
                    var contactCenter = string.Empty;
                    switch (customer.Segment.ToLower())
                    {
                        case "consumer banking":
                        case "ncb":
                        case "pbvcm":
                            contactCenter = HtmlConstants.CONSUMER_BANKING;
                            break;
                        case "prb":
                            contactCenter = HtmlConstants.PRIVATE_BANKING;
                            break;
                        case "sbs":
                            contactCenter = HtmlConstants.SBS_BANKING;
                            break;
                        case "nbb":
                            contactCenter = HtmlConstants.NBB_BANKING;
                            break;
                        case "cib":
                            contactCenter = HtmlConstants.CORPORATE_BANKING;
                            break;
                    }

                    var BranchDetail = "Nedbank" + "<br>" +
                        "135 Rivonia Road, Sandton 2196" + "<br>" +
                        "PO Box 1144, Johannesburg, 2000" + "<br>" +
                        "South Africa" + "<br>" +
                        "Bank VAT Reg No " + "4320116074" + "<br>" + "<br>" + "<br>" +
                        contactCenter;

                    pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BranchDetail);

                    pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "");
                }
                else
                {
                    pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindBondDetailsWidgetData(StringBuilder pageContent, Page page, PageWidget widget, List<DM_HomeLoanMaster> HomeLoans, DM_CustomerMaster customer)
        {
            try
            {
                var BondDetails = new StringBuilder();
                if (HomeLoans.Count == 1)
                {
                    BondDetails.Append("Bond No: " + HomeLoans[0].InvestorId.ToString() + "<br>");
                }
                BondDetails.Append(DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));

                pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BondDetails.ToString());
                pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", "Professional Banking 24/7 Contact centre " + " 0860 555 222");
            }
            catch (Exception)
            {
            }
        }

        private void BindInvestmentPortfolioStatementWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                var transactions = new List<DM_InvestmentTransaction>();
                var TotalClosingBalance = 0.0m;
                investmentMasters.ForEach(invest =>
                {
                    transactions.AddRange(invest.investmentTransactions);
                    var res = 0.0m;
                    try
                    {
                        TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(
                            it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC)
                            || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)
                        ).Select(it => decimal.TryParse(it.WJXBFS4_Balance.Replace(",", "."), out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch (Exception)
                    {
                        TotalClosingBalance = 0.0m;
                    }
                });

                pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.FirstName);
                pageContent.Replace("{{SurName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.SurName);

                pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", !string.IsNullOrEmpty(customer.DS_Investor_Name) ? ": " + customer.DS_Investor_Name : string.Empty);
                pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalClosingBalance));
                pageContent.Replace("{{DayOfStatement_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].DayOfStatement);
                pageContent.Replace("{{InvestorID_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(investmentMasters[0].InvestorId));

                //to separate to string dates values into required date format -- 
                //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
                //1st try with string separator, if fails then try with char separator
                var statementPeriod = string.Empty;
                string[] stringSeparators = new string[] { " - ", "- ", " -" };


                DateTime minDate = transactions.Min(m => m.TransactionDate);
                DateTime maxDate = transactions.Max(m => m.TransactionDate);
                statementPeriod = minDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + (customer.Language == "ENG" ? " to " : " tot ") + maxDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);

                pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", statementPeriod);
                pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].StatementDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
            }
        }

        private void BindInvestorPerformanceWidgetData(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, DM_CustomerMaster cust)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                List<InvestorPerformanceWidgetData> investorPerformanceWidgetDatas = new List<InvestorPerformanceWidgetData>();
                List<DM_InvestmentTransaction> transactions = new List<DM_InvestmentTransaction>();

                investmentMasters.OrderBy(a => a.InvestmentId).ToList().ForEach(invest =>
                {
                    InvestorPerformanceWidgetData item;
                    var productType = invest.ProductType;
                    if (investorPerformanceWidgetDatas.Any(m => m.ProductType == productType))
                    {
                        item = investorPerformanceWidgetDatas.FirstOrDefault(m => m.ProductType == productType);
                        foreach (DM_InvestmentTransaction transaction in invest.investmentTransactions)
                        {
                            transaction.ProductId = Convert.ToInt64(item.ProductId);
                        }
                    }
                    else
                    {
                        item = new InvestorPerformanceWidgetData() { ProductType = productType, ProductId = invest.ProductId.ToString() };
                        investorPerformanceWidgetDatas.Add(item);
                    }

                    transactions.AddRange(invest.investmentTransactions);
                });

                var html = "";

                int counter = 0;

                foreach (var item in investorPerformanceWidgetDatas)
                {
                    item.OpeningBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && m.TransactionDesc.ToLower().Contains(cust.Language == "ENG" ? ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC : ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC_AFR)).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();
                    item.ClosingBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && m.TransactionDesc.ToLower().Contains(cust.Language == "ENG" ? ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC : ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();

                    item.OpeningBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.OpeningBalance));
                    item.ClosingBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.ClosingBalance));

                    var InvestorPerformanceHtmlWidget = HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_HTML;
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", item.ProductType);
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", item.OpeningBalance);
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", item.ClosingBalance);
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{WidgetId}}", "");

                    if (counter != 0)
                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("<div class='card-body-header pb-2'>Investor performance</div>", "");

                    html += InvestorPerformanceHtmlWidget;
                    counter++;
                }
                pageContent.Replace("{{" + HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
            }
        }

        private void BindBreakdownOfInvestmentAccountsWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                //Create Nav tab if investment accounts is more than 1
                var NavTabs = new StringBuilder();
                var InvestmentAccountsCount = investmentMasters.Count;
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                //create tab-content div if accounts is greater than 1, otherwise create simple div
                var TabContentHtml = new StringBuilder();
                var counter = 0;
                investmentMasters.OrderBy(m => m.InvestmentId).ToList().ForEach(acc =>
                 {
                     var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML_SMT);

                     #region Account Details
                     InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                     InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", Convert.ToString(acc.InvestmentId));
                     InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", string.Empty);

                     var InvestmentNo = Convert.ToString(acc.InvestorId) + " " + Convert.ToString(acc.InvestmentId);
                     //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                     while (InvestmentNo.Length != 13)
                     {
                         InvestmentNo = "0" + InvestmentNo;
                     }
                     InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                     InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.AccountOpenDate != null ? acc.AccountOpenDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);

                     InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                     InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate != null ? acc.ExpiryDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);
                     InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc != null ? acc.InterestDisposalDesc : string.Empty);
                     InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod != null ? acc.NoticePeriod : string.Empty);

                     var res = 0.0m;
                     acc.AccuredInterest = acc.AccuredInterest.Replace(",", ".");
                     if (acc.AccuredInterest != null && decimal.TryParse(acc.AccuredInterest, out res))
                     {
                         InvestmentAccountDetailHtml.Replace("{{InterestDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(res))));
                     }
                     else
                     {
                         InvestmentAccountDetailHtml.Replace("{{InterestDue}}", "R0.00");
                     }

                     var LastInvestmentTransaction = acc.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC) || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)).OrderByDescending(it => it.TransactionDate)?.ToList()?.FirstOrDefault();
                     if (LastInvestmentTransaction != null)
                     {
                         LastInvestmentTransaction.WJXBFS4_Balance = LastInvestmentTransaction.WJXBFS4_Balance.Replace(",", ".");
                         InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", LastInvestmentTransaction.TransactionDate.ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy));
                         if (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res))
                         {
                             InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", (LastInvestmentTransaction.WJXBFS4_Balance == "0" ? "-" : (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res) ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(LastInvestmentTransaction.WJXBFS4_Balance))) : "0")));
                         }
                         else
                         {
                             InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "0");
                         }
                     }
                     else
                     {
                         InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "");
                         InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "");
                     }

                     #endregion Account Details

                     #region Transactions
                     if (acc.investmentTransactions != null && acc.investmentTransactions.Count > 0)
                     {
                         acc.investmentTransactions.OrderBy(it => it.TransactionDate).ToList().ForEach(trans =>
                         {
                             res = 0.0m;
                             trans.WJXBFS2_Debit = trans.WJXBFS2_Debit.Replace(",", ".");
                             if (decimal.TryParse(trans.WJXBFS2_Debit, out res))
                             {
                                 trans.WJXBFS2_Debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                             }
                             else
                             {
                                 trans.WJXBFS2_Debit = "-";
                             }

                             res = 0.0m;
                             trans.WJXBFS3_Credit = trans.WJXBFS3_Credit.Replace(",", ".");
                             if (decimal.TryParse(trans.WJXBFS3_Credit, out res))
                             {
                                 trans.WJXBFS3_Credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                             }
                             else
                             {
                                 trans.WJXBFS3_Credit = "-";
                             }

                             res = 0.0m;
                             trans.WJXBFS4_Balance = trans.WJXBFS4_Balance.Replace(",", ".");
                             if (decimal.TryParse(trans.WJXBFS4_Balance, out res))
                             {
                                 trans.WJXBFS4_Balance = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                             }
                             else
                             {
                                 trans.WJXBFS4_Balance = "-";
                             }
                         });

                         var Transactionjson = "InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier + "=" + JsonConvert.SerializeObject(acc.investmentTransactions);
                         this.utility.WriteToJsonFile(Transactionjson, "InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier + ".json", batchMaster.Identifier, customer.CustomerId, outputLocation);
                         scriptHtmlRenderer.Append("<script type='text/javascript' src='./InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier + ".json'></script>");

                         InvestmentAccountDetailHtml.Replace("InvestmentTransactionTable", "InvestmentTransactionTable_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);
                         InvestmentAccountDetailHtml.Replace("PersonalLoanTransactionTablePagination", "InvestmentTransactionTablePagination_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);

                         var scriptval = new StringBuilder(HtmlConstants.INVESTMENT_TRANSACTION_TABLE_VIEW_SCRIPT);
                         scriptval.Replace("InvestmentTransctiondata", "InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier);
                         scriptval.Replace("InvestmentTransactionTable", "InvestmentTransactionTable_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);
                         scriptval.Replace("InvestmentTransactionTablePagination", "InvestmentTransactionTablePagination_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);
                         scriptHtmlRenderer.Append(scriptval);
                     }

                     TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                     #endregion Transactions

                     counter++;
                 });
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : string.Empty);

                if (customer.Language != "ENG")
                    TabContentHtml.Replace("Date", "Datum");

                pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
            }
            else
            {
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
            }
        }

        private void BindExplanatoryNotesWidgetData(StringBuilder pageContent, BatchMaster batchMaster, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var ExPlanatoryNotes = this.tenantTransactionDataRepository.Get_DM_ExplanatoryNotes(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();
                if (ExPlanatoryNotes != null && ExPlanatoryNotes.Count > 0)
                {
                    var notes = new StringBuilder();
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note1) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note1) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note2) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note2) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note3) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note3) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note4) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note4) + " </span> <br/>");
                    notes.Append(string.IsNullOrEmpty(ExPlanatoryNotes[0].Note5) ? string.Empty : "<span> " + Convert.ToString(ExPlanatoryNotes[0].Note5) + " </span> <br/>");
                    pageContent.Replace("{{Notes_" + page.Identifier + "_" + widget.Identifier + "}}", notes.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindMarketingServiceWidgetData(StringBuilder pageContent, List<DM_MarketingMessage> Messages, Page page, PageWidget widget, int MarketingMessageCounter)
        {
            if (Messages != null && Messages.Count > 0)
            {
                var ServiceMessage = Messages.Count > MarketingMessageCounter ? Messages[MarketingMessageCounter] : null;
                if (ServiceMessage != null)
                {
                    var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.Message1)) ? "<p>" + ServiceMessage.Message1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message2)) ? "<p>" + ServiceMessage.Message2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message3)) ? "<p>" + ServiceMessage.Message3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message4)) ? "<p>" + ServiceMessage.Message4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message5)) ? "<p>" + ServiceMessage.Message5 + "</p>" : string.Empty);

                    pageContent.Replace("{{ServiceMessageHeader_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", ServiceMessage.Header).Replace("{{ServiceMessageText_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", messageTxt);
                }
            }
        }

        private void BindPersonalLoanDetailWidgetData(StringBuilder pageContent, BatchMaster batchMaster, DM_CustomerMaster customer, Page page, PageWidget widget, string tenantCode)
        {
            try
            {
                var PersonalLoan = this.tenantTransactionDataRepository.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();
                if (PersonalLoan != null)
                {
                    var res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.CreditAdvance, out res))
                    {
                        pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.OutstandingBalance, out res))
                    {
                        pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.AmountDue, out res))
                    {
                        pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    pageContent.Replace("{{AccountNumber_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.InvestorId.ToString());
                    pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                    pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.Arrears, out res))
                    {
                        pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    pageContent.Replace("{{AnnualRate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.AnnualRate + "% pa");

                    res = 0.0m;
                    if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                    {
                        pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }

                    pageContent.Replace("{{Terms_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.Term);
                    pageContent.Replace("{{DueByDate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindPersonalLoanTransactionWidgetData(StringBuilder pageContent, BatchMaster batchMaster, Page page, PageWidget widget, DM_CustomerMaster customer, string tenantCode)
        {
            try
            {
                var transactions = this.tenantTransactionDataRepository.Get_DM_PersonalLoanTransaction(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();
                if (transactions != null && transactions.Count > 0)
                {
                    var LoanTransactionRows = new StringBuilder();
                    var tr = new StringBuilder();
                    transactions.ForEach(trans =>
                    {
                        tr = new StringBuilder();
                        tr.Append("<tr class='ht-20'>");
                        tr.Append("<td class='w-13 text-center'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                        tr.Append("<td class='w-15 text-center'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
                        tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

                        var res = 0.0m;
                        if (decimal.TryParse(trans.Debit, out res))
                        {
                            tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-12 text-right'> - </td>");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(trans.Credit, out res))
                        {
                            tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-12 text-right'> - </td>");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(trans.OutstandingCapital, out res))
                        {
                            tr.Append("<td class='w-13 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
                        }
                        else
                        {
                            tr.Append("<td class='w-13 text-right'> - </td>");
                        }
                        tr.Append("</tr>");
                        LoanTransactionRows.Append(tr.ToString());
                    });
                    pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", LoanTransactionRows.ToString());
                }
                else
                {
                    pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr><td class='text-center' colspan='6'>No record found</td></tr>");
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindPersonalLoanPaymentDueWidgetData(StringBuilder pageContent, BatchMaster batchMaster, Page page, PageWidget widget, DM_CustomerMaster customer, string tenantCode)
        {
            try
            {
                var plArrears = this.tenantTransactionDataRepository.Get_DM_PersonalLoanArrears(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();
                if (plArrears != null)
                {
                    var res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_120, out res))
                    {
                        pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_90, out res))
                    {
                        pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_60, out res))
                    {
                        pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_30, out res))
                    {
                        pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }

                    res = 0.0m;
                    if (decimal.TryParse(plArrears.Arrears_0, out res))
                    {
                        pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                    }
                    else
                    {
                        pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    }
                }
                else
                {
                    pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                    pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
                }
            }
            catch (Exception)
            {
            }
        }

        private void BindSpecialMessageWidgetData(StringBuilder pageContent, SpecialMessage SpecialMessage, Page page, PageWidget widget)
        {
            if (page.PageTypeName == HtmlConstants.HOME_LOAN_PAGE_TYPE)
            {
                var jsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(jsonstr);
                if (SpecialMessage != null)
                {
                    var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
                    var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);
                    htmlWidget.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                    htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
                    pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
                }
                else
                {
                    pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            else
            {
                if (SpecialMessage != null)
                {
                    if (!string.IsNullOrEmpty(SpecialMessage.Header) || !string.IsNullOrEmpty(SpecialMessage.Message1) || !string.IsNullOrEmpty(SpecialMessage.Message2))
                    {
                        var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
                        var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);
                        htmlWidget.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
                        htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
                        pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
                    }
                    else
                    {
                        pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    }
                }
                else
                {
                    pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
        }

        private void BindPersonalLoanInsuranceMessageWidgetData(StringBuilder pageContent, SpecialMessage InsuranceMsg, Page page, PageWidget widget)
        {
            if (InsuranceMsg != null)
            {
                if (!string.IsNullOrEmpty(InsuranceMsg.Message3) || !string.IsNullOrEmpty(InsuranceMsg.Message4) || !string.IsNullOrEmpty(InsuranceMsg.Message5))
                {
                    var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_HTML);
                    var InsuranceMsgTxtData = (!string.IsNullOrEmpty(InsuranceMsg.Message3) ? "<p> " + InsuranceMsg.Message3 + " </p>" : string.Empty) +
                       (!string.IsNullOrEmpty(InsuranceMsg.Message4) ? "<p> " + InsuranceMsg.Message4 + " </p>" : string.Empty) +
                       (!string.IsNullOrEmpty(InsuranceMsg.Message5) ? "<p> " + InsuranceMsg.Message5 + " </p>" : string.Empty);
                    htmlWidget.Replace("{{InsuranceMessages}}", InsuranceMsgTxtData);
                    htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
                    pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
                }
                else
                {
                    pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            else
            {
                pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
            }
        }

        private void BindPersonalLoanTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoans, Page page, PageWidget widget)
        {
            try
            {
                var TotalLoanAmt = 0.0m;
                var TotalOutstandingAmt = 0.0m;
                var TotalLoanDueAmt = 0.0m;

                if (PersonalLoans != null && PersonalLoans.Count > 0)
                {
                    var res = 0.0m;
                    try
                    {
                        TotalLoanAmt = PersonalLoans.Select(it => decimal.TryParse(it.CreditAdvance, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalLoanAmt = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalOutstandingAmt = PersonalLoans.Select(it => decimal.TryParse(it.OutstandingBalance, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalOutstandingAmt = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalLoanDueAmt = PersonalLoans.Select(it => decimal.TryParse(it.AmountDue, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalLoanDueAmt = 0.0m;
                    }
                }

                pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
                pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanDueAmt));
            }
            catch
            {
            }
        }

        private void BindPersonalLoanAccountsBreakdownWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_PersonalLoanMaster> PersonalLoans, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation)
        {
            try
            {
                if (PersonalLoans != null && PersonalLoans.Count > 0)
                {
                    //Create Nav tab if customer has more than 1 personal loan accounts
                    var NavTabs = new StringBuilder();
                    var counter = 0;

                    if (PersonalLoans.Count > 1)
                    {
                        NavTabs.Append("<ul class='nav nav-tabs Personalloan-nav-tabs'>");
                        PersonalLoans.ToList().ForEach(PersonalLoan =>
                        {
                            var AccountNumber = PersonalLoan.InvestorId.ToString();
                            string lastFourDigisOfAccountNumber = AccountNumber.Length > 4 ? AccountNumber.Substring(Math.Max(0, AccountNumber.Length - 4)) : AccountNumber;
                            NavTabs.Append("<li class='nav-item " + (counter == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#PersonalLoan-" + lastFourDigisOfAccountNumber + counter + "' role='tab' class='nav-link " + (counter == 0 ? "active" : string.Empty) + "'> Personal Loan - " + lastFourDigisOfAccountNumber + "</a></li>");
                            counter++;
                        });
                        NavTabs.Append("</ul>");
                    }
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                    //create tab-content div if accounts is greater than 1, otherwise create simple div
                    var TabContentHtml = new StringBuilder();
                    counter = 0;
                    TabContentHtml.Append((PersonalLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                    PersonalLoans.ForEach(PersonalLoan =>
                    {
                        var AccountNumber = PersonalLoan.InvestorId.ToString();
                        string lastFourDigisOfAccountNumber = AccountNumber.Length > 4 ? AccountNumber.Substring(Math.Max(0, AccountNumber.Length - 4)) : AccountNumber;
                        TabContentHtml.Append("<div id='PersonalLoan-" + lastFourDigisOfAccountNumber + counter + "'>");

                        #region Loan Details
                        var LoanDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_DETAIL);
                        LoanDetailHtml.Replace("{{AccountNumber}}", PersonalLoan.InvestorId.ToString());
                        LoanDetailHtml.Replace("{{StatementDate}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        LoanDetailHtml.Replace("{{StatementPeriod}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                        var res = 0.0m;
                        if (decimal.TryParse(PersonalLoan.Arrears, out res))
                        {
                            LoanDetailHtml.Replace("{{ArrearsAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{ArrearsAmount}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{AnnualRate}}", PersonalLoan.AnnualRate + "% pa");

                        res = 0.0m;
                        if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
                        {
                            LoanDetailHtml.Replace("{{MonthlyInstallment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{MonthlyInstallment}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{Terms}}", PersonalLoan.Term);
                        LoanDetailHtml.Replace("{{DueByDate}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        TabContentHtml.Append(LoanDetailHtml.ToString());
                        #endregion Loan Details

                        #region Loan Transaction
                        var LoanTransactionHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_TRANSACTION_DETAIL_SMT);
                        if (PersonalLoan.LoanTransactions != null && PersonalLoan.LoanTransactions.Count > 0)
                        {
                            PersonalLoan.LoanTransactions.ForEach(trans =>
                            {
                                res = 0.0m;
                                if (decimal.TryParse(trans.Debit, out res))
                                {
                                    trans.Debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                                }
                                else
                                {
                                    trans.Debit = "-";
                                }

                                if (decimal.TryParse(trans.Credit, out res))
                                {
                                    trans.Credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                                }
                                else
                                {
                                    trans.Credit = "-";
                                }

                                if (decimal.TryParse(trans.OutstandingCapital, out res))
                                {
                                    trans.OutstandingCapital = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                                }
                                else
                                {
                                    trans.OutstandingCapital = "-";
                                }
                            });

                            var LoanTransactionjson = "PersonalLoanTransactiondata" + PersonalLoan.InvestorId + page.Identifier + "=" + JsonConvert.SerializeObject(PersonalLoan.LoanTransactions);
                            this.utility.WriteToJsonFile(LoanTransactionjson, "PersonalLoanTransactiondata" + PersonalLoan.InvestorId + page.Identifier + ".json", batchMaster.Identifier, customer.CustomerId, outputLocation);
                            scriptHtmlRenderer.Append("<script type='text/javascript' src='./PersonalLoanTransactiondata" + PersonalLoan.InvestorId + page.Identifier + ".json'></script>");

                            LoanTransactionHtml.Replace("PersonalLoanTransactionTable", "PersonalLoanTransactionTable_" + PersonalLoan.InvestorId + "_" + page.Identifier);
                            LoanTransactionHtml.Replace("PersonalLoanTransactionTablePagination", "PersonalLoanTransactionTablePagination_" + PersonalLoan.InvestorId + "_" + page.Identifier);

                            var scriptval = new StringBuilder(HtmlConstants.PERSONAL_LOAN_TRANSACTION_TABLE_VIEW_SCRIPT);
                            scriptval.Replace("PersonalLoanTransactiondata", "PersonalLoanTransactiondata" + PersonalLoan.InvestorId + page.Identifier);
                            scriptval.Replace("PersonalLoanTransactionTable", "PersonalLoanTransactionTable_" + PersonalLoan.InvestorId + "_" + page.Identifier);
                            scriptval.Replace("PersonalLoanTransactionTablePagination", "PersonalLoanTransactionTablePagination_" + PersonalLoan.InvestorId + "_" + page.Identifier);
                            scriptHtmlRenderer.Append(scriptval);
                        }

                        TabContentHtml.Append(LoanTransactionHtml.ToString());
                        #endregion Loan Transaction

                        #region Loan arrear
                        if (PersonalLoan.LoanArrears != null)
                        {
                            var plArrears = PersonalLoan.LoanArrears;
                            var paddingClass = PersonalLoan.LoanTransactions.Count > 10 ? "pb-2 pt-5" : "py-2";
                            var LoanArrearHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_DETAIL).Replace("{{PaddingClass}}", paddingClass);
                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_120, out res))
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_90, out res))
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_60, out res))
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_30, out res))
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.Arrears_0, out res))
                            {
                                LoanArrearHtml.Replace("{{Current}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{Current}}", "R0.00");
                            }

                            TabContentHtml.Append(LoanArrearHtml.ToString());
                        }
                        TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                        #endregion Loan arrear

                        counter++;
                    });

                    TabContentHtml.Append((PersonalLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
                }
                else
                {
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            catch
            {
            }
        }

        private void BindHomeLoanTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget)
        {
            try
            {
                var TotalLoanAmt = 0.0m;
                var TotalOutstandingAmt = 0.0m;

                if (HomeLoans != null && HomeLoans.Count > 0)
                {
                    var res = 0.0m;
                    try
                    {
                        TotalLoanAmt = HomeLoans.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalLoanAmt = 0.0m;
                    }

                    res = 0.0m;
                    try
                    {
                        TotalOutstandingAmt = HomeLoans.Select(it => decimal.TryParse(it.Balance, out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch
                    {
                        TotalOutstandingAmt = 0.0m;
                    }
                }

                pageContent.Replace("{{TotalHomeLoansAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
                pageContent.Replace("{{TotalHomeLoansBalanceOutstanding_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
            }
            catch
            {
            }
        }

        private void BindHomeLoanAccountsBreakdownWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation)
        {
            try
            {
                if (HomeLoans != null && HomeLoans.Count > 0)
                {
                    //Create Nav tab if customer has more than 1 personal loan accounts
                    var NavTabs = new StringBuilder();
                    if (HomeLoans.Count > 1)
                    {
                        NavTabs.Append("<ul class='nav nav-tabs Homeloan-nav-tabs'>");
                        var cnt = 0;
                        HomeLoans.ToList().ForEach(acc =>
                        {
                            var accNo = acc.InvestorId.ToString();
                            string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;
                            NavTabs.Append("<li class='nav-item " + (cnt == 0 ? "active" : string.Empty) + "'><a id='tab0-tab' data-toggle='tab' data-target='#HomeLoan-" + lastFourDigisOfAccountNumber + "' role='tab' class='nav-link " + (cnt == 0 ? "active" : string.Empty) + "'> Home Loan - " + lastFourDigisOfAccountNumber + "</a></li>");
                            cnt++;
                        });
                        NavTabs.Append("</ul>");
                    }
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                    //create tab-content div if accounts is greater than 1, otherwise create simple div
                    var TabContentHtml = new StringBuilder();
                    var counter = 0;
                    TabContentHtml.Append((HomeLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
                    HomeLoans.ForEach(HomeLoan =>
                    {
                        var accNo = HomeLoan.InvestorId.ToString();
                        string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

                        TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "' >");

                        #region Loan Details
                        var LoanDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNT_DETAIL_DIV_HTML);
                        LoanDetailHtml.Replace("{{BondNumber}}", accNo);
                        LoanDetailHtml.Replace("{{RegistrationDate}}", HomeLoan.RegisteredDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                        var secDesc1 = string.Empty;
                        var secDesc2 = string.Empty;
                        var secDesc3 = string.Empty;
                        if (HomeLoan.SecDescription1.Length > 15 || ((HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2).Length > 25))
                        {
                            secDesc1 = HomeLoan.SecDescription1;
                            if ((HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3).Length > 25)
                            {
                                secDesc2 = HomeLoan.SecDescription2;
                                secDesc3 = HomeLoan.SecDescription3;
                            }
                            else
                            {
                                secDesc2 = HomeLoan.SecDescription2 + " " + HomeLoan.SecDescription3;
                            }
                        }
                        else
                        {
                            secDesc1 = HomeLoan.SecDescription1 + " " + HomeLoan.SecDescription2;
                            secDesc2 = HomeLoan.SecDescription3;
                        }

                        LoanDetailHtml.Replace("{{SecDescription1}}", secDesc1);
                        LoanDetailHtml.Replace("{{SecDescription2}}", secDesc2);
                        LoanDetailHtml.Replace("{{SecDescription3}}", secDesc3);

                        var res = 0.0m;
                        if (decimal.TryParse(HomeLoan.IntialDue, out res))
                        {
                            LoanDetailHtml.Replace("{{Instalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{Instalment}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{InterestRate}}", HomeLoan.ChargeRate + "% pa");

                        res = 0.0m;
                        if (decimal.TryParse(HomeLoan.ArrearStatus, out res))
                        {
                            LoanDetailHtml.Replace("{{Arrears}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{Arrears}}", "R0.00");
                        }

                        res = 0.0m;
                        if (decimal.TryParse(HomeLoan.RegisteredAmount, out res))
                        {
                            LoanDetailHtml.Replace("{{RegisteredAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanDetailHtml.Replace("{{RegisteredAmount}}", "R0.00");
                        }

                        LoanDetailHtml.Replace("{{LoanTerms}}", HomeLoan.LoanTerm);
                        TabContentHtml.Append(LoanDetailHtml.ToString());

                        #endregion  Loan Details

                        #region Loan Transaction table
                        var LoanTransactionHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML_SMT);
                        if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count > 0)
                        {
                            HomeLoan.LoanTransactions.ForEach(trans =>
                            {
                                res = 0.0m;
                                if (decimal.TryParse(trans.Debit, out res))
                                {
                                    trans.Debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                                }
                                else
                                {
                                    trans.Debit = "-";
                                }

                                if (decimal.TryParse(trans.Credit, out res))
                                {
                                    trans.Credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                                }
                                else
                                {
                                    trans.Credit = "-";
                                }

                                if (decimal.TryParse(trans.RunningBalance, out res))
                                {
                                    trans.RunningBalance = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                                }
                                else
                                {
                                    trans.RunningBalance = "-";
                                }
                            });

                            var LoanTransactionJson = "HomeLoanTransactiondata" + HomeLoan.InvestorId + page.Identifier + "=" + JsonConvert.SerializeObject(HomeLoan.LoanTransactions);
                            this.utility.WriteToJsonFile(LoanTransactionJson, "HomeLoanTransactiondata" + HomeLoan.InvestorId + page.Identifier + ".json", batchMaster.Identifier, customer.CustomerId, outputLocation);
                            scriptHtmlRenderer.Append("<script type='text/javascript' src='./HomeLoanTransactiondata" + HomeLoan.InvestorId + page.Identifier + ".json'></script>");

                            LoanTransactionHtml.Replace("HomeLoanTransactionTable", "HomeLoanTransactionTable_" + HomeLoan.InvestorId + "_" + page.Identifier);
                            LoanTransactionHtml.Replace("HomeLoanTransactionTablePagination", "HomeLoanTransactionTablePagination_" + HomeLoan.InvestorId + "_" + page.Identifier);

                            var scriptval = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_TABLE_VIEW_SCRIPT);
                            scriptval.Replace("HomeLoanTransactiondata", "HomeLoanTransactiondata" + HomeLoan.InvestorId + page.Identifier);
                            scriptval.Replace("HomeLoanTransactionTable", "HomeLoanTransactionTable_" + HomeLoan.InvestorId + "_" + page.Identifier);
                            scriptval.Replace("HomeLoanTransactionTablePagination", "HomeLoanTransactionTablePagination_" + HomeLoan.InvestorId + "_" + page.Identifier);
                            scriptHtmlRenderer.Append(scriptval);
                        }
                        TabContentHtml.Append(LoanTransactionHtml.ToString());
                        #endregion Loan Transaction table

                        #region Loan arrears
                        var paddingClass = HomeLoan.LoanTransactions.Count > 10 ? "pb-2 pt-5" : "py-2";
                        var LoanArrearHtml = new StringBuilder(HtmlConstants.HOME_LOAN_STATEMENT_OVERVIEW_AND_PAYMENT_DUE_DIV_HTML).Replace("{{PaddingClass}}", paddingClass);
                        LoanArrearHtml.Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
                        res = 0.0m;
                        if (decimal.TryParse(HomeLoan.Balance, out res))
                        {
                            LoanArrearHtml.Replace("{{BalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                        }
                        else
                        {
                            LoanArrearHtml.Replace("{{BalanceOutstanding}}", "R0.00");
                        }

                        if (HomeLoan.LoanArrear != null)
                        {
                            var plArrears = HomeLoan.LoanArrear;
                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_120, out res))
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_90, out res))
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_60, out res))
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.ARREARS_30, out res))
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                            }

                            res = 0.0m;
                            if (decimal.TryParse(plArrears.CurrentDue, out res))
                            {
                                LoanArrearHtml.Replace("{{CurrentPaymentDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                            }
                        }
                        else
                        {
                            LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
                            LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
                        }
                        TabContentHtml.Append(LoanArrearHtml.ToString());

                        #endregion  Loan arrears

                        var PaymentDueMessageDivHtml = new StringBuilder(HtmlConstants.HOME_LAON_PAYMENT_DUE_SPECIAL_MESSAGE_DIV_HTML);
                        var spjsonstr = HtmlConstants.HOME_LOAN_SPECIAL_MESSAGES_WIDGET_PREVIEW_JSON_STRING;
                        if (spjsonstr != string.Empty && validationEngine.IsValidJson(spjsonstr))
                        {
                            var SpecialMessage = JsonConvert.DeserializeObject<SpecialMessage>(spjsonstr);
                            if (SpecialMessage != null)
                            {
                                var PaymentDueMessage = (!string.IsNullOrEmpty(SpecialMessage.Message3) ? "<p> " + SpecialMessage.Message3 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message4) ? "<p> " + SpecialMessage.Message4 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message5) ? "<p> " + SpecialMessage.Message5 + " </p>" : string.Empty);

                                PaymentDueMessageDivHtml.Replace("{{PaymentDueSpecialMessage}}", PaymentDueMessage);
                                TabContentHtml.Append(PaymentDueMessageDivHtml.ToString());
                            }
                        }

                        #region Loan Summary and Instalment detail
                        var LoanSummaryForTaxPurposesHtml = new StringBuilder(HtmlConstants.HOME_LOAN_SERVICE_FOR_TAX_PURPOSES_DIV_HTML);
                        var LoanInstalmentHtml = new StringBuilder(HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_DIV_HTML);
                        var HomeLoanSummary = HomeLoan.LoanSummary;
                        if (HomeLoanSummary != null)
                        {
                            #region Summary for Tax purposes div
                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Interest) && decimal.TryParse(HomeLoanSummary.Annual_Interest, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Insurance) && decimal.TryParse(HomeLoanSummary.Annual_Insurance, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Service_Fee) && decimal.TryParse(HomeLoanSummary.Annual_Service_Fee, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Legal_Costs) && decimal.TryParse(HomeLoanSummary.Annual_Legal_Costs, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Annual_Total_Recvd) && decimal.TryParse(HomeLoanSummary.Annual_Total_Recvd, out res))
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");
                            }

                            #endregion

                            #region Installment details div

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Basic_Instalment) && decimal.TryParse(HomeLoanSummary.Basic_Instalment, out res))
                            {
                                LoanInstalmentHtml.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Houseowner_Ins) && decimal.TryParse(HomeLoanSummary.Houseowner_Ins, out res))
                            {
                                LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Loan_Protection) && decimal.TryParse(HomeLoanSummary.Loan_Protection, out res))
                            {
                                LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Recovery_Fee_Debit) && decimal.TryParse(HomeLoanSummary.Recovery_Fee_Debit, out res))
                            {
                                LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Capital_Redemption) && decimal.TryParse(HomeLoanSummary.Capital_Redemption, out res))
                            {
                                LoanInstalmentHtml.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Service_Fee) && decimal.TryParse(HomeLoanSummary.Service_Fee, out res))
                            {
                                LoanInstalmentHtml.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                            }

                            res = 0.0m;
                            if (!string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && decimal.TryParse(HomeLoanSummary.Total_Instalment, out res))
                            {
                                LoanInstalmentHtml.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
                            }
                            else
                            {
                                LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                            }

                            LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

                            #endregion
                        }
                        else
                        {
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInterest}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualInsurance}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualServiceFee}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualLegalCosts}}", "R0.00");
                            LoanSummaryForTaxPurposesHtml.Replace("{{AnnualTotalAmountReceived}}", "R0.00");

                            LoanInstalmentHtml.Replace("{{BasicInstalment}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{HouseownerInsurance}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{LoanProtectionAssurance}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{CapitalRedemption}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{ServiceFee}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{TotalInstalment}}", "R0.00");
                            LoanInstalmentHtml.Replace("{{InstalmentDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
                        }

                        TabContentHtml.Append(LoanSummaryForTaxPurposesHtml.ToString());
                        TabContentHtml.Append(LoanInstalmentHtml.ToString());
                        #endregion Loan Summary and Instalment detail

                        TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
                        counter++;
                    });

                    TabContentHtml.Append((HomeLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
                }
                else
                {
                    pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                    pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                }
            }
            catch
            {
            }
        }

        private void BindPortfolioCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            pageContent.Replace("{{CustomerName_" + page.Identifier + "_" + widget.Identifier + "}}", (customer.Title + " " + customer.FirstName + " " + customer.SurName));
            pageContent.Replace("{{CustomerId_" + page.Identifier + "_" + widget.Identifier + "}}", customer.CustomerId.ToString());
            pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No);
            pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", customer.EmailAddress);
        }

        private void BindPortfolioCustomerAddressDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        {
            var custAddress = (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
                                (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
            pageContent.Replace("{{CustomerAddress_" + page.Identifier + "_" + widget.Identifier + "}}", custAddress);
        }

        private void BindPortfolioClientContactDetailsWidgetData(StringBuilder pageContent, Page page, PageWidget widget)
        {
            pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", "0860 555 111");
            pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", "supportdesk@nedbank.com");
        }

        private void BindPortfolioAccountSummaryWidgetData(StringBuilder pageContent, List<DM_AccountsSummary> _AccountsSummaries, Page page, PageWidget widget)
        {
            if (_AccountsSummaries.Count > 0)
            {
                var res = 0.0m;
                var accountSummaryRows = new StringBuilder();
                _AccountsSummaries.ForEach(acc =>
                {
                    if (!acc.AccountType.ToLower().Contains("reward") || !acc.AccountType.ToLower().Contains("point"))
                    {
                        var tr = new StringBuilder();
                        tr.Append("<tr class='ht-30'>");
                        tr.Append("<td class='text-left'>" + acc.AccountType + " </td>");
                        tr.Append("<td class='text-right'>" + utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (decimal.TryParse(acc.TotalAmount, out res) ? res : 0)) + " </td>");
                        tr.Append("</tr>");
                        accountSummaryRows.Append(tr.ToString());
                    }
                });
                pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", accountSummaryRows.ToString());
            }
            else
            {
                pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr class='ht-30'><td class='text-center' colspan='2'>No records found</td></tr>");
            }

            //To add reward points data
            var accSummary = _AccountsSummaries.Where(it => it.AccountType.ToLower().Contains("reward") || it.AccountType.ToLower().Contains("point"))?.ToList()?.FirstOrDefault();
            var rewardPointsDiv = new StringBuilder();
            if (accSummary != null)
            {
                rewardPointsDiv = new StringBuilder("<div class='pt-2'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='text-left'>" + accSummary.AccountType + " </th><th class='text-right'> " + accSummary.TotalAmount + " </th></tr></thead></table></div>");
            }
            pageContent.Replace("{{RewardPointsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", rewardPointsDiv.ToString());
        }

        private void BindPortfolioAccountAnalysisWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_AccountAnanlysis> _lstAccountAnalysis, Page page, PageWidget widget)
        {
            var data = "[]";
            if (_lstAccountAnalysis != null && _lstAccountAnalysis.Count > 0)
            {
                data = JsonConvert.SerializeObject(_lstAccountAnalysis);
            }
            pageContent.Replace("HiddenAccountAnalysisGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT.Replace("AccountAnalysisBarGraphcontainer", "AccountAnalysisBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenAccountAnalysisGraph", "HiddenAccountAnalysisGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindPortfolioRemindersWidgetData(StringBuilder pageContent, List<DM_CustomerReminderAndRecommendation> Reminders, Page page, PageWidget widget)
        {
            StringBuilder reminderstr = new StringBuilder();
            if (Reminders != null && Reminders.Count > 0)
            {
                Reminders.ToList().ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.reminderAndRecommendation.Description))
                    {
                        reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.reminderAndRecommendation.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + item.reminderAndRecommendation.ActionUrl + "' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>" + item.reminderAndRecommendation.ActionTitle + "</span></a></div></div>");
                    }
                });
            }
            pageContent.Replace("{{ReminderAndRecommendation_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
        }

        private void BindPortfolioNewsAlertsWidgetData(StringBuilder pageContent, List<DM_CustomerNewsAndAlert> NewsAndAlerts, Page page, PageWidget widget)
        {
            var newsAlertStr = new StringBuilder();
            if (NewsAndAlerts != null && NewsAndAlerts.Count > 0)
            {
                NewsAndAlerts.ForEach(item =>
                {
                    if (!string.IsNullOrEmpty(item.NewsAndAlert.Description))
                    {
                        newsAlertStr.Append("<p>" + item.NewsAndAlert.Description + "</p>");
                    }
                });
            }
            pageContent.Replace("{{NewsAlert_" + page.Identifier + "_" + widget.Identifier + "}}", newsAlertStr.ToString());
        }

        private void BindGreenbacksTotalRewardPointsWidgetData(StringBuilder pageContent, List<DM_AccountsSummary> _AccountsSummaries, Page page, PageWidget widget)
        {
            var accSummary = _AccountsSummaries.Where(it => it.AccountType.ToLower().Contains("reward") || it.AccountType.ToLower().Contains("point"))?.ToList()?.FirstOrDefault();
            pageContent.Replace("{{TotalRewardsPoints_" + page.Identifier + "_" + widget.Identifier + "}}", (accSummary != null ? accSummary.TotalAmount : "0"));
        }

        private void BindGreenbacksYtdRewardsPointsGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_GreenbacksRewardPoints> RewardPoints, Page page, PageWidget widget)
        {
            var data = "[]";
            if (RewardPoints != null && RewardPoints.Count > 0)
            {
                data = JsonConvert.SerializeObject(RewardPoints);
            }
            pageContent.Replace("HiddenYTDRewardPointsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_YTD_REWARDS_POINTS_BAR_GRAPH_SCRIPT.Replace("YTDRewardPointsBarGraphcontainer", "YTDRewardPointsBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenYTDRewardPointsGraph", "HiddenYTDRewardPointsGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindGreenbacksPointsRedeemedYtdGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_GreenbacksRewardPointsRedeemed> rewardPointsRedeemeds, Page page, PageWidget widget)
        {
            var data = "[]";
            if (rewardPointsRedeemeds != null && rewardPointsRedeemeds.Count > 0)
            {
                data = JsonConvert.SerializeObject(rewardPointsRedeemeds);
            }
            pageContent.Replace("HiddenPointsRedeemedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_POINTS_REDEEMED_YTD_BAR_GRAPH_SCRIPT.Replace("PointsRedeemedYTDBarGraphcontainer", "PointsRedeemedYTDBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenPointsRedeemedGraph", "HiddenPointsRedeemedGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindGreenbacksProductRelatedPonitsEarnedGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_CustomerProductWiseRewardPoints> productWiseRewardPoints, Page page, PageWidget widget)
        {
            var data = "[]";
            if (productWiseRewardPoints != null && productWiseRewardPoints.Count > 0)
            {
                data = JsonConvert.SerializeObject(productWiseRewardPoints);
            }
            pageContent.Replace("HiddenProductRelatedPointsEarnedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_SCRIPT.Replace("ProductRelatedPointsEarnedBarGraphcontainer", "ProductRelatedPointsEarnedBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenProductRelatedPointsEarnedGraph", "HiddenProductRelatedPointsEarnedGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindGreenbacksCategorySpendRewardPointsGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_CustomerRewardSpendByCategory> rewardSpendByCategories, Page page, PageWidget widget)
        {
            var data = "[]";
            if (rewardSpendByCategories != null && rewardSpendByCategories.Count > 0)
            {
                data = JsonConvert.SerializeObject(rewardSpendByCategories);
            }
            pageContent.Replace("HiddenCategorySpendRewardsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
            scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_CATEGORY_SPEND_REWARD_POINTS_BAR_GRAPH_SCRIPT.Replace("CategorySpendRewardsPieChartcontainer", "CategorySpendRewardsPieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenCategorySpendRewardsGraph", "HiddenCategorySpendRewardsGraph_" + page.Identifier + "_" + widget.Identifier));
        }

        private void BindDataToWealthBreakdownOfInvestmentAccountsWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation, bool isWealthStatement)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                //Create Nav tab if investment accounts is more than 1
                var NavTabs = new StringBuilder();
                var InvestmentAccountsCount = investmentMasters.Count;
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

                pageContent = pageContent.Replace("<div class='card-body-header pb-2'>Breakdown of your investment accounts</div>", "<div class='card-body-header-w pb-2'>Breakdown of your investment accounts</div>");

                //create tab-content div if accounts is greater than 1, otherwise create simple div
                var TabContentHtml = new StringBuilder();
                var counter = 0;
                investmentMasters.OrderBy(m => m.InvestmentId).ToList().ForEach(acc =>
                {
                    var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.WEALTH_INVESTMENT_ACCOUNT_DETAILS_HTML_SMT);

                    #region Account Details
                    InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
                    InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", Convert.ToString(acc.InvestmentId));
                    InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", string.Empty);

                    var InvestmentNo = Convert.ToString(acc.InvestorId) + " " + Convert.ToString(acc.InvestmentId);
                    //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
                    while (InvestmentNo.Length != 13)
                    {
                        InvestmentNo = "0" + InvestmentNo;
                    }
                    InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
                    InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.AccountOpenDate != null ? acc.AccountOpenDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);

                    InvestmentAccountDetailHtml.Replace("{{InterestRate}}", acc.CurrentInterestRate + "% pa");
                    InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate != null ? acc.ExpiryDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);
                    InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc != null ? acc.InterestDisposalDesc : string.Empty);
                    InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod != null ? acc.NoticePeriod : string.Empty);

                    var res = 0.0m;
                    acc.AccuredInterest = acc.AccuredInterest.Replace(",", ".");
                    if (acc.AccuredInterest != null && decimal.TryParse(acc.AccuredInterest, out res))
                    {
                        InvestmentAccountDetailHtml.Replace("{{InterestDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(res))));
                    }
                    else
                    {
                        InvestmentAccountDetailHtml.Replace("{{InterestDue}}", "R0.00");
                    }

                    var LastInvestmentTransaction = acc.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC) || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)).OrderByDescending(it => it.TransactionDate)?.ToList()?.FirstOrDefault();
                    if (LastInvestmentTransaction != null)
                    {
                        LastInvestmentTransaction.WJXBFS4_Balance = LastInvestmentTransaction.WJXBFS4_Balance.Replace(",", ".");
                        InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", LastInvestmentTransaction.TransactionDate.ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy));
                        if (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res))
                        {
                            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", (LastInvestmentTransaction.WJXBFS4_Balance == "0" ? "-" : (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res) ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(LastInvestmentTransaction.WJXBFS4_Balance))) : "0")));
                        }
                        else
                        {
                            InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "0");
                        }
                    }
                    else
                    {
                        InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "");
                        InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "");
                    }

                    #endregion Account Details

                    #region Transactions
                    if (acc.investmentTransactions != null && acc.investmentTransactions.Count > 0)
                    {
                        acc.investmentTransactions.OrderBy(it => it.TransactionDate).ToList().ForEach(trans =>
                        {
                            res = 0.0m;
                            trans.WJXBFS2_Debit = trans.WJXBFS2_Debit.Replace(",", ".");
                            if (decimal.TryParse(trans.WJXBFS2_Debit, out res))
                            {
                                trans.WJXBFS2_Debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                            }
                            else
                            {
                                trans.WJXBFS2_Debit = "-";
                            }

                            res = 0.0m;
                            trans.WJXBFS3_Credit = trans.WJXBFS3_Credit.Replace(",", ".");
                            if (decimal.TryParse(trans.WJXBFS3_Credit, out res))
                            {
                                trans.WJXBFS3_Credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                            }
                            else
                            {
                                trans.WJXBFS3_Credit = "-";
                            }

                            res = 0.0m;
                            trans.WJXBFS4_Balance = trans.WJXBFS4_Balance.Replace(",", ".");
                            if (decimal.TryParse(trans.WJXBFS4_Balance, out res))
                            {
                                trans.WJXBFS4_Balance = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
                            }
                            else
                            {
                                trans.WJXBFS4_Balance = "-";
                            }
                        });

                        var Transactionjson = "InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier + "=" + JsonConvert.SerializeObject(acc.investmentTransactions);
                        this.utility.WriteToJsonFile(Transactionjson, "InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier + ".json", batchMaster.Identifier, customer.CustomerId, outputLocation);
                        scriptHtmlRenderer.Append("<script type='text/javascript' src='./InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier + ".json'></script>");

                        InvestmentAccountDetailHtml.Replace("InvestmentTransactionTable", "InvestmentTransactionTable_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);
                        InvestmentAccountDetailHtml.Replace("PersonalLoanTransactionTablePagination", "InvestmentTransactionTablePagination_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);

                        var scriptval = new StringBuilder(HtmlConstants.INVESTMENT_TRANSACTION_TABLE_VIEW_SCRIPT);
                        scriptval.Replace("InvestmentTransctiondata", "InvestmentTransctiondata" + acc.InvestorId + acc.InvestmentId + page.Identifier);
                        scriptval.Replace("InvestmentTransactionTable", "InvestmentTransactionTable_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);
                        scriptval.Replace("InvestmentTransactionTablePagination", "InvestmentTransactionTablePagination_" + acc.InvestorId + acc.InvestmentId + "_" + page.Identifier);
                        scriptHtmlRenderer.Append(scriptval);
                    }

                    TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
                    #endregion Transactions

                    counter++;
                });
                TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : string.Empty);

                if (customer.Language != "ENG")
                    TabContentHtml.Replace("Date", "Datum");

                pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
            }
            else
            {
                pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
                pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
            }
        }

        private void BindDataToWealthInvestmentPortfolioStatementWidget(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                var TotalClosingBalance = 0.0m;
                investmentMasters.ForEach(invest =>
                {
                    var res = 0.0m;
                    try
                    {
                        TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(it =>
                        it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC)
                        || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)
                        ).Select(it => decimal.TryParse(it.WJXBFS4_Balance.Replace(",", "."), out res) ? res : 0).ToList().Sum(it => it);
                    }
                    catch (Exception)
                    {
                        TotalClosingBalance = 0.0m;
                    }
                });

                pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.FirstName);
                pageContent.Replace("{{SurName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.SurName);

                pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", !string.IsNullOrEmpty(customer.DS_Investor_Name) ? ": " + customer.DS_Investor_Name : string.Empty);
                pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalClosingBalance));
                pageContent.Replace("{{DayOfStatement_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].DayOfStatement);
                pageContent.Replace("{{InvestorID_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(investmentMasters[0].InvestorId));

                //to separate to string dates values into required date format -- 
                //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
                //1st try with string separator, if fails then try with char separator
                var statementPeriod = string.Empty;
                string[] stringSeparators = new string[] { " - ", "- ", " -" };
                string[] dates = investmentMasters[0].StatementPeriod.Split(stringSeparators, StringSplitOptions.None);
                if (!string.IsNullOrWhiteSpace(investmentMasters[0].StatementPeriod) && dates.Length > 0)
                {
                    if (dates.Length > 1)
                    {
                        statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[1]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
                    }
                    else
                    {
                        dates = investmentMasters[0].StatementPeriod.Split(new Char[] { ' ' });
                        if (dates.Length > 2)
                        {
                            statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[2]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
                        }
                    }
                }
                else
                {
                    var fromDate = investmentMasters.Select(x => x.investmentTransactions.OrderBy(z => z.TransactionDate).Select(y => y.TransactionDate).FirstOrDefault()).FirstOrDefault();
                    var toDate = investmentMasters.Select(x => x.investmentTransactions.OrderByDescending(z => z.TransactionDate).Select(y => y.TransactionDate).FirstOrDefault()).FirstOrDefault();
                    statementPeriod = Convert.ToDateTime(fromDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(toDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
                }
                pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", statementPeriod);
                pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].StatementDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
            }
        }

        private void BindDataToWealthInvestorPerformanceStatementWidget(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, DM_CustomerMaster cust)
        {
            if (investmentMasters != null && investmentMasters.Count > 0)
            {
                List<InvestorPerformanceWidgetData> investorPerformanceWidgetDatas = new List<InvestorPerformanceWidgetData>();
                List<DM_InvestmentTransaction> transactions = new List<DM_InvestmentTransaction>();

                investmentMasters.OrderBy(a => a.InvestmentId).ToList().ForEach(invest =>
                {
                    InvestorPerformanceWidgetData item;
                    var productType = invest.ProductType;
                    if (investorPerformanceWidgetDatas.Any(m => m.ProductType == productType))
                    {
                        item = investorPerformanceWidgetDatas.FirstOrDefault(m => m.ProductType == productType);
                        foreach (DM_InvestmentTransaction transaction in invest.investmentTransactions)
                        {
                            transaction.ProductId = Convert.ToInt64(item.ProductId);
                        }
                    }
                    else
                    {
                        item = new InvestorPerformanceWidgetData() { ProductType = productType, ProductId = invest.ProductId.ToString() };
                        investorPerformanceWidgetDatas.Add(item);
                    }

                    transactions.AddRange(invest.investmentTransactions);
                });

                var html = "";

                int counter = 0;

                foreach (var item in investorPerformanceWidgetDatas)
                {
                    item.OpeningBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && m.TransactionDesc.ToLower().Contains(cust.Language == "ENG" ? ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC : ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC_AFR)).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();
                    item.ClosingBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && m.TransactionDesc.ToLower().Contains(cust.Language == "ENG" ? ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC : ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();

                    item.OpeningBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.OpeningBalance));
                    item.ClosingBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.ClosingBalance));

                    var InvestorPerformanceHtmlWidget = HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_HTML;
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", item.ProductType);
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", item.OpeningBalance);
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", item.ClosingBalance);
                    InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{WidgetId}}", "");

                    if (counter != 0)
                        InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("<div class='card-body-header-w pb-2'>Investor performance</div>", "");

                    html += InvestorPerformanceHtmlWidget;
                    counter++;
                }
                pageContent.Replace("{{" + HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_NAME + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
            }
        }

        private StringBuilder Translate(StringBuilder inputStr, DM_CustomerMaster customer)
        {
            //Check the language using customer.Language and then translate it
            List<TranslatedItem> list = new List<TranslatedItem>();
            if (customer.Language != "ENG")
            {
                var resourceItems = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

                foreach (DictionaryEntry item in resourceItems)
                {
                    list.Add(new TranslatedItem() { Eng = item.Key.ToString(), Translated = item.Value.ToString(), StringLength = item.Key.ToString().Length });
                }

                var replaced = inputStr.ToString();
                foreach (var item in list.OrderByDescending(m => m.StringLength))
                {
                    string pattern = @"\b(" + item.Eng + @")\b";

                    replaced = Regex.Replace(replaced, pattern, item.Translated);
                }

                inputStr = new StringBuilder();
                inputStr.Length = 0;
                inputStr.Capacity = 0;
                inputStr.Append(replaced);
            }
            return inputStr;
        }

        #endregion

        #endregion
    }

    public class InvestorPerformanceWidgetData
    {
        public string ProductId { get; set; }
        public string ProductType { get; set; }
        public string OpeningBalance { get; set; }
        public string ClosingBalance { get; set; }
    }

    public class TranslatedItem
    {
        public string Eng { get; set; }
        public string Translated { get; set; }
        public int StringLength { get; set; }
    }
}
