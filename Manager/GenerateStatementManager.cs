// <copyright file="GenerateStatementManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.RegularExpressions;
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

        //private ICustomerRepository customerRepository = null;

        /// <summary>
        /// The tenant transaction data repository.
        /// </summary>
        private ITenantTransactionDataRepository tenantTransactionDataRepository = null;

       // private IMCARepository mcaDataRepository = null;

        //private ICorporateSaverRepository corporateSaverDataRepository = null;

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
                //this.customerRepository = this.unityContainer.Resolve<ICustomerRepository>();
                //this.mcaDataRepository = this.unityContainer.Resolve<IMCARepository>();
                //this.corporateSaverDataRepository = this.unityContainer.Resolve<ICorporateSaverRepository>();
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

        ///// <summary>
        ///// This method helps to create NedBank HTML statement for given customer list.
        ///// </summary>
        ///// <param name="statementRawData"> the raw data object requires for statement generate process</param>
        ///// <param name="tenantCode"></param>
        //public void CreateCustomerNedbankStatement(GenerateStatementRawData statementRawData, string tenantCode)
        //{
        //    IList<StatementMetadata> statementMetadataRecords = new List<StatementMetadata>();
        //    var customer = statementRawData.DM_Customer;

        //    try
        //    {
        //        //call to generate actual NedBank HTML statement file for current customer record
        //        var logDetailRecord = this.GenerateNedbankStatements(statementRawData, tenantCode);
        //        if (logDetailRecord != null)
        //        {
        //            //save schedule log details for current customer
        //            var logDetails = new List<ScheduleLogDetail>();
        //            logDetailRecord.ScheduleLogId = statementRawData.ScheduleLog.Identifier;
        //            logDetailRecord.CustomerId = customer.CustomerId;
        //            logDetailRecord.CustomerName = customer.FirstName?.Trim() + " " + customer.SurName?.Trim();
        //            logDetailRecord.ScheduleId = statementRawData.ScheduleLog.ScheduleId;
        //            logDetailRecord.RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0;
        //            logDetailRecord.RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "";
        //            logDetailRecord.RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "";
        //            logDetailRecord.NumberOfRetry = 1;
        //            logDetailRecord.CreateDate = DateTime.UtcNow;
        //            logDetails.Add(logDetailRecord);
        //            this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

        //            //if statement generated successfully, then save statement metadata with actual html statement file path
        //            if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()))
        //            {
        //                if (logDetailRecord.statementMetadata.Count > 0)
        //                {
        //                    logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
        //                    {
        //                        metarec.ScheduleLogId = statementRawData.ScheduleLog.Identifier;
        //                        metarec.ScheduleId = statementRawData.ScheduleLog.ScheduleId;
        //                        metarec.StatementDate = DateTime.UtcNow;
        //                        metarec.StatementURL = logDetailRecord.StatementFilePath;
        //                        metarec.TenantCode = tenantCode;
        //                        metarec.IsPasswordGenerated = false;
        //                        metarec.Password = "";
        //                        statementMetadataRecords.Add(metarec);
        //                    });
        //                    this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
        //                }
        //            }

        //            //If any error occurs during statement generation then delete all files from output directory of current customer html statement
        //            else if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
        //            {
        //                this.utility.DeleteUnwantedDirectory(statementRawData.Batch.Identifier, customer.CustomerId, statementRawData.OutputLocation);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //save schedule log details for current customer, if something went wrong while statement generation...
        //        var logDetails = new List<ScheduleLogDetail>();
        //        logDetails.Add(new ScheduleLogDetail()
        //        {
        //            ScheduleLogId = statementRawData.ScheduleLog.Identifier,
        //            CustomerId = customer.CustomerId,
        //            CustomerName = customer.FirstName.Trim() + " " + customer.SurName?.Trim(),
        //            ScheduleId = statementRawData.ScheduleLog.ScheduleId,
        //            RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0,
        //            RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "",
        //            RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "",
        //            NumberOfRetry = 1,
        //            CreateDate = DateTime.UtcNow,
        //            Status = ScheduleLogStatus.Failed.ToString(),
        //            LogMessage = "Something went wrong while generating statement: " + ex.Message
        //        });
        //        this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

        //        //write error log
        //        WriteToFile(ex.StackTrace.ToString());
        //        //throw ex;
        //    }
        //}


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

        ///// <summary>
        ///// This method helps to retry to generate HTML statement for failed nedbank customer list.
        ///// </summary>
        ///// <param name="statementRawData"> the raw data object requires for statement generate process</param>
        ///// <param name="tenantCode"></param>
        //public void RetryToCreateFailedNedbankCustomerStatements(GenerateStatementRawData statementRawData, string tenantCode)
        //{
        //    try
        //    {
        //        var scheduleLogDetail = statementRawData.ScheduleLogDetail;
        //        var customer = this.tenantTransactionDataRepository.Get_DM_CustomerMasters(new CustomerSearchParameter()
        //        {
        //            Identifier = scheduleLogDetail.CustomerId,
        //            BatchId = statementRawData.Batch.Identifier,
        //        }, tenantCode)?.FirstOrDefault();
        //        statementRawData.DM_Customer = customer;

        //        if (customer != null)
        //        {
        //            //call to generate actual HTML statement file for current customer record
        //            var logDetailRecord = this.GenerateNedbankStatements(statementRawData, tenantCode);
        //            if (logDetailRecord != null)
        //            {
        //                //delete un-neccessory files which are created during html statement generation in fail cases
        //                if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Failed.ToString().ToLower()))
        //                {
        //                    this.utility.DeleteUnwantedDirectory(statementRawData.Batch.Identifier, customer.Identifier, statementRawData.OutputLocation);
        //                }

        //                //update schedule log detail
        //                var scheduleLogDetails = new List<ScheduleLogDetail>();
        //                scheduleLogDetail.CustomerId = customer.CustomerId;
        //                scheduleLogDetail.CustomerName = customer.FirstName.Trim() + " " + customer.SurName?.Trim();
        //                scheduleLogDetail.RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0;
        //                scheduleLogDetail.RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : string.Empty;
        //                scheduleLogDetail.RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : string.Empty;
        //                scheduleLogDetail.LogMessage = logDetailRecord.LogMessage;
        //                scheduleLogDetail.Status = logDetailRecord.Status;
        //                scheduleLogDetail.NumberOfRetry++;
        //                scheduleLogDetail.StatementFilePath = logDetailRecord.StatementFilePath;
        //                scheduleLogDetails.Add(scheduleLogDetail);
        //                this.scheduleLogRepository.UpdateScheduleLogDetails(scheduleLogDetails, tenantCode);

        //                //save statement metadata if html statement generated successfully
        //                if (logDetailRecord.Status.ToLower().Equals(ScheduleLogStatus.Completed.ToString().ToLower()) && logDetailRecord.statementMetadata.Count > 0)
        //                {
        //                    var statementMetadataRecords = new List<StatementMetadata>();
        //                    logDetailRecord.statementMetadata.ToList().ForEach(metarec =>
        //                    {
        //                        metarec.ScheduleLogId = scheduleLogDetail.ScheduleLogId;
        //                        metarec.ScheduleId = scheduleLogDetail.ScheduleId;
        //                        metarec.StatementDate = DateTime.UtcNow;
        //                        metarec.StatementURL = scheduleLogDetail.StatementFilePath;
        //                        metarec.TenantCode = tenantCode;
        //                        metarec.IsPasswordGenerated = false;
        //                        metarec.Password = "";
        //                        statementMetadataRecords.Add(metarec);
        //                    });
        //                    this.scheduleLogRepository.SaveStatementMetadata(statementMetadataRecords, tenantCode);
        //                }

        //                var scheduleLogs = this.scheduleLogRepository.GetScheduleLogs(new ScheduleLogSearchParameter()
        //                {
        //                    ScheduleLogId = scheduleLogDetail.ScheduleLogId.ToString(),
        //                    BatchId = statementRawData.Batch.Identifier.ToString(),
        //                    PagingParameter = new PagingParameter
        //                    {
        //                        PageIndex = 0,
        //                        PageSize = 0,
        //                    },
        //                    SortParameter = new SortParameter()
        //                    {
        //                        SortOrder = SortOrder.Ascending,
        //                        SortColumn = "Id",
        //                    },
        //                    SearchMode = SearchMode.Equals
        //                }, tenantCode).ToList();
        //                scheduleLogs.ForEach(scheduleLog =>
        //                {
        //                    //get total no. of schedule log details for current schedule log
        //                    var _lstScheduleLogDetail = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
        //                    {
        //                        ScheduleLogId = scheduleLog.Identifier.ToString(),
        //                        PagingParameter = new PagingParameter
        //                        {
        //                            PageIndex = 0,
        //                            PageSize = 0,
        //                        },
        //                        SortParameter = new SortParameter()
        //                        {
        //                            SortOrder = SortOrder.Ascending,
        //                            SortColumn = "Id",
        //                        },
        //                        SearchMode = SearchMode.Equals
        //                    }, tenantCode);

        //                    //get no of success schedule log details of current schedule log
        //                    var successRecords = _lstScheduleLogDetail.Where(item => item.Status == ScheduleLogStatus.Completed.ToString())?.ToList();

        //                    var batchStatus = BatchStatus.Completed.ToString();
        //                    var isBatchCompleteExecuted = true;
        //                    var scheduleLogStatus = ScheduleLogStatus.Completed.ToString();

        //                    //check success schedule log details count is equal to total no. of schedule log details for current schedule log
        //                    //if equals then update schedule log and batch status as completed otherwise failed
        //                    if (successRecords != null && successRecords.Count != _lstScheduleLogDetail.Count)
        //                    {
        //                        scheduleLogStatus = ScheduleLogStatus.Failed.ToString();
        //                        batchStatus = BatchStatus.Failed.ToString();
        //                        isBatchCompleteExecuted = false;
        //                    }

        //                    //update schedule log and batch status
        //                    this.scheduleLogRepository.UpdateScheduleLogStatus(scheduleLog.Identifier, scheduleLogStatus, tenantCode);
        //                    this.scheduleRepository.UpdateBatchStatus(statementRawData.Batch.Identifier, batchStatus, isBatchCompleteExecuted, tenantCode);
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //save schedule log details for current customer, if something went wrong while statement generation...
        //        var logDetails = new List<ScheduleLogDetail>();
        //        logDetails.Add(new ScheduleLogDetail()
        //        {
        //            ScheduleLogId = statementRawData.ScheduleLog.Identifier,
        //            CustomerId = statementRawData.DM_Customer.CustomerId,
        //            CustomerName = statementRawData.DM_Customer.FirstName.Trim() + " " + statementRawData.DM_Customer.SurName?.Trim(),
        //            ScheduleId = statementRawData.ScheduleLog.ScheduleId,
        //            RenderEngineId = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.Identifier : 0,
        //            RenderEngineName = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.RenderEngineName : "",
        //            RenderEngineURL = statementRawData.RenderEngine != null ? statementRawData.RenderEngine.URL : "",
        //            NumberOfRetry = 1,
        //            CreateDate = DateTime.UtcNow,
        //            Status = ScheduleLogStatus.Failed.ToString(),
        //            LogMessage = "Something went wrong while generating statement: " + ex.Message
        //        });
        //        this.scheduleLogRepository.SaveScheduleLogDetails(logDetails, tenantCode);

        //        //write error log
        //        WriteToFile(ex.StackTrace.ToString());
        //        //throw ex;
        //    }
        //}

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

        ///// <summary>
        ///// This method helps to convert HTML statement to PDF statement and archive related data for the nedbank customer.
        ///// </summary>
        ///// <param name="archivalProcessRawData">The raw data object required for archival process</param>
        ///// <param name="tenantCode">The tenant code</param>
        //public bool RunArchivalForNedbankCustomerRecord(ArchivalProcessRawData archivalProcessRawData, string tenantCode)
        //{
        //    var tempDir = string.Empty;
        //    var runStatus = false;

        //    try
        //    {
        //        var pdfStatementFilepath = archivalProcessRawData.PdfStatementFilepath;
        //        var batch = archivalProcessRawData.BatchMaster;

        //        var customer = this.tenantTransactionDataRepository.Get_DM_CustomerMasters(new CustomerSearchParameter()
        //        {
        //            BatchId = batch.Identifier,
        //            CustomerId = archivalProcessRawData.CustomerId
        //        }, tenantCode).FirstOrDefault();
        //        var metadataRecords = this.statementSearchRepository.GetStatementSearchs(new StatementSearchSearchParameter()
        //        {
        //            CustomerId = archivalProcessRawData.CustomerId.ToString(),
        //            StatementId = archivalProcessRawData.Statement.Identifier.ToString(),
        //            PagingParameter = new PagingParameter
        //            {
        //                PageIndex = 0,
        //                PageSize = 0,
        //            },
        //            SortParameter = new SortParameter()
        //            {
        //                SortOrder = SortOrder.Ascending,
        //                SortColumn = "Id",
        //            },
        //            SearchMode = SearchMode.Equals
        //        }, tenantCode);

        //        if (customer != null && metadataRecords != null && metadataRecords.Count > 0)
        //        {
        //            var statementSearchRecord = metadataRecords.FirstOrDefault();

        //            //Create final output directory to save PDF statement of current customer
        //            var outputlocation = pdfStatementFilepath + "\\PDF_Statements" + "\\" + "ScheduleId_" + statementSearchRecord.ScheduleId + "\\" + "BatchId_" + batch.Identifier + "\\ArchiveData";
        //            if (!Directory.Exists(outputlocation))
        //            {
        //                Directory.CreateDirectory(outputlocation);
        //            }

        //            tempDir = outputlocation + "\\temp_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
        //            if (!Directory.Exists(tempDir))
        //            {
        //                Directory.CreateDirectory(tempDir);
        //            }

        //            //Create temp output directory to save all neccessories files which requires to genearate PDF statement of current customer
        //            var samplefilespath = tempDir + "\\" + statementSearchRecord.Identifier + "_" + customer.CustomerId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
        //            if (!Directory.Exists(samplefilespath))
        //            {
        //                Directory.CreateDirectory(samplefilespath);
        //            }

        //            //get actual HTML statement file path directory for current customer
        //            var htmlStatementDirPath = statementSearchRecord.StatementURL.Substring(0, statementSearchRecord.StatementURL.LastIndexOf("\\"));

        //            //get resource file path directory
        //            var resourceFilePath = htmlStatementDirPath.Substring(0, htmlStatementDirPath.LastIndexOf("\\")) + "\\common";
        //            if (!Directory.Exists(resourceFilePath))
        //            {
        //                resourceFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Resources";
        //            }

        //            //Copying all neccessories files which requires to genearate PDF statement of current customer
        //            this.utility.DirectoryCopy(resourceFilePath + "\\css", samplefilespath, false);
        //            this.utility.DirectoryCopy(resourceFilePath + "\\js", samplefilespath, false);
        //            this.utility.DirectoryCopy(resourceFilePath + "\\images", samplefilespath, false);
        //            this.utility.DirectoryCopy(resourceFilePath + "\\fonts", samplefilespath, false);

        //            //Gernerate HTML statement of current customer
        //            this.statementSearchManager.GenerateNedbankHtmlStatementForPdfGeneration(customer, archivalProcessRawData.Statement, archivalProcessRawData.StatementPageContents, batch, archivalProcessRawData.BatchDetails, tenantCode, samplefilespath, archivalProcessRawData.TenantConfiguration);

        //            //To insert html statement file of current customer and all required files into the zip file
        //            var zipfilepath = tempDir + "\\tempzip";
        //            if (!Directory.Exists(zipfilepath))
        //            {
        //                Directory.CreateDirectory(zipfilepath);
        //            }
        //            var zipFile = zipfilepath + "\\" + "StatementZip" + "_" + statementSearchRecord.Identifier + "_" + statementSearchRecord.ScheduleId + "_" + statementSearchRecord.StatementId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
        //            ZipFile.CreateFromDirectory(samplefilespath, zipFile);

        //            //Convert HTML statement to PDF statement for current customer
        //            var pdfName = "Statement" + "_" + statementSearchRecord.ScheduleLogId + "_" + statementSearchRecord.ScheduleId + statementSearchRecord.StatementId + "_" + statementSearchRecord.CustomerId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".pdf";
        //            string password = string.Empty;
        //            if (statementSearchRecord.IsPasswordGenerated)
        //            {
        //                password = this.cryptoManager.Decrypt(statementSearchRecord.Password);
        //            }
        //            var result = this.utility.HtmlStatementToPdf(zipFile, outputlocation + "\\" + pdfName, password);
        //            if (result)
        //            {
        //                //To insert archive schedule log detail records
        //                var scheduleLogDetailRecords = this.scheduleLogRepository.GetScheduleLogDetails(new ScheduleLogDetailSearchParameter()
        //                {
        //                    ScheduleLogId = archivalProcessRawData.ScheduleLog.Identifier.ToString(),
        //                    CustomerId = archivalProcessRawData.CustomerId.ToString(),
        //                    PagingParameter = new PagingParameter
        //                    {
        //                        PageIndex = 0,
        //                        PageSize = 0,
        //                    },
        //                    SortParameter = new SortParameter()
        //                    {
        //                        SortOrder = SortOrder.Ascending,
        //                        SortColumn = "Id",
        //                    },
        //                    SearchMode = SearchMode.Equals
        //                }, tenantCode);
        //                var scheduleDetailArchiveRecords = new List<ScheduleLogDetailArchieve>();
        //                scheduleLogDetailRecords.ToList().ForEach(logDetail =>
        //                {
        //                    scheduleDetailArchiveRecords.Add(new ScheduleLogDetailArchieve()
        //                    {
        //                        CustomerId = logDetail.CustomerId,
        //                        CustomerName = logDetail.CustomerName,
        //                        LogDetailCreationDate = logDetail.CreateDate,
        //                        LogMessage = logDetail.LogMessage,
        //                        NumberOfRetry = logDetail.NumberOfRetry,
        //                        RenderEngineId = archivalProcessRawData.RenderEngine.Identifier,
        //                        RenderEngineName = archivalProcessRawData.RenderEngine.RenderEngineName,
        //                        RenderEngineURL = archivalProcessRawData.RenderEngine.URL,
        //                        ScheduleId = archivalProcessRawData.Schedule.Identifier,
        //                        ScheduleLogArchiveId = archivalProcessRawData.ScheduleLogArchive.Identifier,
        //                        Status = logDetail.Status,
        //                        TenantCode = tenantCode,
        //                        ArchivalDate = DateTime.UtcNow,
        //                        PdfStatementPath = outputlocation + "\\" + pdfName
        //                    });
        //                });
        //                this.archivalProcessRepository.SaveScheduleLogDetailsArchieve(scheduleDetailArchiveRecords, tenantCode);

        //                //TO insert archive statement metadata records
        //                var metadataArchiveRecords = new List<StatementMetadataArchive>();
        //                metadataRecords.ToList().ForEach(record =>
        //                {
        //                    metadataArchiveRecords.Add(new StatementMetadataArchive()
        //                    {
        //                        AccountNumber = record.AccountNumber,
        //                        AccountType = record.AccountType,
        //                        CustomerId = record.CustomerId,
        //                        CustomerName = record.CustomerName,
        //                        ScheduleId = record.ScheduleId,
        //                        ScheduleLogArchiveId = archivalProcessRawData.ScheduleLogArchive.Identifier,
        //                        StatementDate = record.StatementDate,
        //                        StatementId = record.StatementId,
        //                        StatementPeriod = record.StatementPeriod,
        //                        StatementURL = outputlocation + "\\" + pdfName,
        //                        TenantCode = tenantCode,
        //                        IsPasswordGenerated = record.IsPasswordGenerated,
        //                        Password = record.Password,
        //                        ArchivalDate = DateTime.UtcNow
        //                    });
        //                });
        //                this.archivalProcessRepository.SaveStatementMetadataArchieve(metadataArchiveRecords, tenantCode);

        //                //TO delete actual schedule log details, and statement metadata records
        //                this.scheduleLogRepository.DeleteScheduleLogDetails(archivalProcessRawData.ScheduleLog.Identifier, archivalProcessRawData.CustomerId, tenantCode);
        //                this.scheduleLogRepository.DeleteStatementMetadata(archivalProcessRawData.ScheduleLog.Identifier, archivalProcessRawData.CustomerId, tenantCode);

        //                //To delete actual HTML statement of currrent customer, once the PDF statement genearated
        //                DirectoryInfo directoryInfo = new DirectoryInfo(htmlStatementDirPath);
        //                if (directoryInfo.Exists)
        //                {
        //                    directoryInfo.Delete(true);
        //                }

        //                runStatus = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToFile(ex.Message);
        //        WriteToFile(ex.StackTrace);
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //To delete temp files, once the PDF statement genearated
        //        DirectoryInfo directoryInfo = new DirectoryInfo(tempDir);
        //        if (directoryInfo.Exists)
        //        {
        //            directoryInfo.Delete(true);
        //        }
        //    }

        //    return runStatus;
        //}

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
                    var fspDetails = new List<spIAA_PaymentDetail>();
                    var savingaccountrecords = new List<AccountMaster>();
                    var curerntaccountrecords = new List<AccountMaster>();
                    var CustomerAcccountTransactions = new List<AccountTransaction>();
                    var CustomerSavingTrends = new List<SavingTrend>();
                  

                    var pages = statement.Pages.Where(item => item.PageTypeName == HtmlConstants.SAVING_ACCOUNT_PAGE || item.PageTypeName == HtmlConstants.CURRENT_ACCOUNT_PAGE).ToList();
                    IsSavingOrCurrentAccountPagePresent = pages.Count > 0 ? true : false;


                    fspDetails = this.tenantTransactionDataRepository.Get_PPSDetails(tenantCode)?.ToList();
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
                    //if (statement.Name == "Investment Wealth")
                    //{
                    //    clientlogo = "../common/images/NedBankLogoBlack.png";
                    //}
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
                        StatementPageContent statementPageContent = new StatementPageContent();
                        try
                        {                          
                            statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }
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
                                        case HtmlConstants.PAYMENT_SUMMARY_WIDGET_NAME:
                                            this.BindPaymentSummaryWidgetData(pageContent, customer, statement, page, widget, customerMedias, fspDetails, statementRawData.BatchDetails);
                                            break;
                                        case HtmlConstants.PRODUCT_SUMMARY_WIDGET_NAME:
                                            IsFailed = this.BindProductSummaryWidgetData(pageContent, ErrorMessages, fspDetails, page, widget);
                                            break;
                                        case HtmlConstants.DETAILED_TRANSACTIONS_WIDGET_NAME:
                                            IsFailed = this.BindDetailedTransactionsWidgetData(pageContent, ErrorMessages, fspDetails, page, widget);
                                            break;
                                        case HtmlConstants.PPS_HEADING_WIDGET_NAME:
                                            this.BindPpsHeadingWidgetData(pageContent, customer, statement, page, widget, customerMedias, fspDetails, statementRawData.BatchDetails);
                                            break;
                                        case HtmlConstants.PPS_DETAILS_WIDGET_NAME:
                                            this.BindPpsDetailsWidgetData(pageContent, customer, statement, page, widget, customerMedias, fspDetails, statementRawData.BatchDetails);
                                            break;
                                        case HtmlConstants.PPS_FOOTER1_WIDGET_NAME:
                                            this.BindPpsFooter1WidgetData(pageContent, customer, statement, page, widget, customerMedias, fspDetails, statementRawData.BatchDetails);
                                            break;
                                        case HtmlConstants.FOOTER_IMAGE_WIDGET_NAME:
                                            this.BindFooterImageWidgetData(pageContent, customer, statement, page, widget, customerMedias, fspDetails, statementRawData.BatchDetails);
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
                        string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, statementRawData.ScheduleLog.ScheduleName,batchMaster.BatchName, customer.Identifier, statementRawData.BaseURL, statementRawData.OutputLocation, true, statement.Pages[0].PageTypeName);

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
        //private ScheduleLogDetail GenerateNedbankStatements(GenerateStatementRawData statementRawData, string tenantCode)
        //{
        //    var logDetailRecord = new ScheduleLogDetail();
        //    var ErrorMessages = new StringBuilder();
        //    bool IsFailed = false;
        //    var statementMetadataRecords = new List<StatementMetadata>();

        //    try
        //    {
        //        var customer = statementRawData.DM_Customer;
        //        var statement = statementRawData.Statement;
        //        var batchMaster = statementRawData.Batch;

        //        if (statementRawData.StatementPageContents.Count > 0)
        //        {
        //            //collecting all media information which is required in html statement for some widgets like image, video and static customer information widgets
        //            var customerMedias = this.tenantTransactionDataRepository.GetCustomerMediaList(customer.CustomerId, batchMaster.Identifier, statement.Identifier, tenantCode);

        //            long BranchId = 0;
        //            var investmentMasters = new List<DM_InvestmentMaster>();
        //            var PersonalLoanAccounts = new List<DM_PersonalLoanMaster>();
        //            var HomeLoanAccounts = new List<DM_HomeLoanMaster>();
        //            var MCA = new List<DM_MCAMaster>();
        //            var CorporateSaver = new List<DM_CorporateSaverMaster>();
        //            var PersonalLoan = new List<DM_PersonalLoanMaster>();

        //            var AccountsSummaries = new List<DM_AccountsSummary>();
        //            var _lstAccountAnalysis = new List<DM_AccountAnanlysis>();
        //            var Reminders = new List<DM_CustomerReminderAndRecommendation>();
        //            var NewsAndAlerts = new List<DM_CustomerNewsAndAlert>();
        //            var GreenbackMaster = new DM_GreenbacksMaster();
        //            var GreenbacksRewardPoints = new List<DM_GreenbacksRewardPoints>();
        //            var GreenbacksRedeemedPoints = new List<DM_GreenbacksRewardPointsRedeemed>();
        //            var CustProductMonthwiseRewardPoints = new List<DM_CustomerProductWiseRewardPoints>();
        //            var CustRewardSpendByCategory = new List<DM_CustomerRewardSpendByCategory>();

        //            var customerSearchParameter = new CustomerSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier };

        //            var IsPortFolioPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.AT_A_GLANCE_PAGE_TYPE).ToList().Count > 0;
        //            var IsInvestmentPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH || it.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH || it.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN || it.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN).ToList().Count > 0;
        //            var IsPersonalLoanPageTypePresent = statement.Pages.Where(it => it.PageTypeName.Trim() == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList().Count > 0;
        //            var IsHomeLoanPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || it.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE).ToList().Count > 0;
        //            var IsRewardPageTypePresent = statement.Pages.Where(it => it.PageTypeName == HtmlConstants.GREENBACKS_PAGE_TYPE).ToList().Count > 0;
        //            var IsMCAPageTypePresent = statement.Pages.Where(it => it.PageTypeName.Trim() == HtmlConstants.MULTI_CURRENCY_FOR_CIB_PAGE_TYPE || it.PageTypeName.Trim() == HtmlConstants.MULTI_CURRENCY_FOR_WEA_PAGE_TYPE).ToList().Count > 0;
        //            var IsCorporateSaverPageTypePresent = statement.Pages.Where(it => it.PageTypeName.Trim() == HtmlConstants.CORPORATE_SAVER_ENG_PAGE_TYPE || it.PageTypeName.Trim() == HtmlConstants.CORPORATE_SAVER_AFR_PAGE_TYPE).ToList().Count > 0;
        //            // var IsMCAPageTypePresent = statement.Pages.Where(it => it.PageTypeName.Trim() == "").ToList().Count > 0;
        //            // var IsCorporateSaverPageTypePresent = statement.Pages.Where(it => it.PageTypeName.Trim() == "").ToList().Count > 0;
        //            if (IsPortFolioPageTypePresent)
        //            {
        //                AccountsSummaries = this.tenantTransactionDataRepository.GET_DM_AccountSummaries(customerSearchParameter, tenantCode)?.ToList();
        //                _lstAccountAnalysis = this.tenantTransactionDataRepository.GET_DM_AccountAnalysisDetails(customerSearchParameter, tenantCode)?.ToList();
        //                Reminders = this.tenantTransactionDataRepository.GET_DM_CustomerReminderAndRecommendations(customerSearchParameter, tenantCode)?.ToList();
        //                NewsAndAlerts = this.tenantTransactionDataRepository.GET_DM_CustomerNewsAndAlert(customerSearchParameter, tenantCode)?.ToList();
        //            }
        //            if (IsInvestmentPageTypePresent)
        //            {
        //                investmentMasters = this.tenantTransactionDataRepository.Get_NB_InvestmentMaster(new CustomerInvestmentSearchParameter() { InvestorId = customer.CustomerId, BatchId = batchMaster.Identifier }, tenantCode)?.ToList();

        //                if (investmentMasters != null && investmentMasters.Count > 0)
        //                {
        //                    //var totalAmount = 0.0m; var res = 0.0m;
        //                    investmentMasters.ForEach(invest =>
        //                    {
        //                        invest.investmentTransactions = this.tenantTransactionDataRepository.Get_DM_InvestmentTransaction(new CustomerInvestmentSearchParameter() { CustomerId = customer.CustomerId, BatchId = batchMaster.Identifier, InvestmentId = invest.InvestmentId, InvestorId = invest.InvestorId }, tenantCode)?.ToList();

        //                        //totalAmount = totalAmount + invest.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC)).Select(it => decimal.TryParse(it.WJXBFS4_Balance.Replace(",", "."), out res) ? res : 0).ToList().Sum(it => it);
        //                    });

        //                    BranchId = (investmentMasters != null && investmentMasters.Count > 0) ? investmentMasters[0].BranchId : 0;

        //                    //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Investment", TotalAmount = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, totalAmount) });
        //                }
        //            }
        //            if (IsPersonalLoanPageTypePresent)
        //            {
        //                //PersonalLoanAccounts = this.tenantTransactionDataRepository.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = 753, CustomerId = 151000262666 }, "94461633-b316-42c3-a188-db8d78075ef4")?.ToList();
        //                PersonalLoanAccounts = this.tenantTransactionDataRepository.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();

        //                //var totalAmount = 0.0m; var res = 0.0m;
        //                //totalAmount = PersonalLoanAccounts.Select(it => decimal.TryParse(it.CreditAdvance, out res) ? res : 0).ToList().Sum(it => it);
        //                //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Personal Loan", TotalAmount = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, totalAmount) });

        //                BranchId = (PersonalLoanAccounts != null && PersonalLoanAccounts.Count > 0) ? PersonalLoanAccounts[0].BranchId : 0;
        //            }
        //            if (IsHomeLoanPageTypePresent)
        //            {
        //                HomeLoanAccounts = this.tenantTransactionDataRepository.Get_DM_HomeLoanMaster(new CustomerHomeLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList();

        //                //var totalAmount = 0.0m; var res = 0.0m;
        //                //totalAmount = HomeLoanAccounts.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
        //                //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Home Loan", TotalAmount = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, totalAmount) });
        //            }
        //            if (IsRewardPageTypePresent)
        //            {
        //                //AccountsSummaries.Add(new DM_AccountsSummary() { AccountType = "Greenback reward points", TotalAmount = "432" });
        //                GreenbackMaster = this.tenantTransactionDataRepository.GET_DM_GreenbacksMasterDetails(tenantCode)?.ToList()?.FirstOrDefault();
        //                GreenbacksRewardPoints = this.tenantTransactionDataRepository.GET_DM_GreenbacksRewardPoints(customerSearchParameter, tenantCode)?.ToList();
        //                GreenbacksRedeemedPoints = this.tenantTransactionDataRepository.GET_DM_GreenbacksRewardPointsRedeemed(customerSearchParameter, tenantCode)?.ToList();
        //                CustProductMonthwiseRewardPoints = this.tenantTransactionDataRepository.GET_DM_CustomerProductWiseRewardPoints(customerSearchParameter, tenantCode)?.ToList();
        //                CustRewardSpendByCategory = this.tenantTransactionDataRepository.GET_DM_CustomerRewardSpendByCategory(customerSearchParameter, tenantCode)?.ToList();
        //            }
        //            if (IsMCAPageTypePresent)
        //            {
        //                MCA = this.mcaDataRepository.Get_DM_MCAMaster(new CustomerMCASearchParameter() { BatchId = batchMaster.Identifier, InvestorId = customer.InvestorId, CustomerId = customer.CustomerId }, tenantCode)?.ToList();
        //            }
        //            if (IsCorporateSaverPageTypePresent)
        //            {
        //                CorporateSaver = this.corporateSaverDataRepository.Get_DM_CorporateSaverMaster(new CustomerCorporateSaverSearchParameter() { BatchId = batchMaster.Identifier, InvestorId = customer.InvestorId }, tenantCode)?.ToList();
        //            }


        //            if (IsPersonalLoanPageTypePresent)
        //            {
        //                PersonalLoan = this.tenantTransactionDataRepository.Get_DM_PersonalLoanMaster(new CustomerPersonalLoanSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId, InvestorId = customer.InvestorId }, tenantCode)?.ToList();
        //            }

        //            var SpecialMessage = this.tenantTransactionDataRepository.Get_DM_SpecialMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier, CustomerId = customer.CustomerId }, tenantCode)?.ToList()?.FirstOrDefault();
        //            var ExplanatoryNotes = this.tenantTransactionDataRepository.Get_DM_ExplanatoryNotes(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();
        //            var _lstMessage = this.tenantTransactionDataRepository.Get_DM_MarketingMessages(new MessageAndNoteSearchParameter() { BatchId = batchMaster.Identifier }, tenantCode)?.ToList();

        //            var htmlbody = new StringBuilder();
        //            //htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
        //            //if (statement.Pages.Count() > 0)
        //            //{
        //            //    if (customer.Segment == "WEA" ? true : false)
        //            //    {
        //            //        htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{ImgHeight}}", "100").Replace("{{NedBankLogo}}", "../common/images/NedBankLogoBlack.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
        //            //    }
        //            //    else
        //            //    {
        //            //        htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{ImgHeight}}", "80").Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
        //            //    }
        //            //}
        //            //else
        //            //{
        //            //    htmlbody.Append(HtmlConstants.NEDBANK_STATEMENT_HEADER.Replace("{{ImgHeight}}", "80").Replace("{{eConfirmLogo}}", "../common/images/eConfirm.png").Replace("{{NedBankLogo}}", "../common/images/NEDBANKLogo.png").Replace("{{StatementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd)));
        //            //}

        //            //htmlbody.Append(statementRawData.StatementPageContents?.FirstOrDefault().PageHeaderContent);

        //            //this variable is used to bind all script to html statement, which helps to render data on chart and graph widgets
        //            var scriptHtmlRenderer = new StringBuilder();
        //            HttpClient httpClient = null;
        //            var NavItemList = new StringBuilder();

        //            var newStatementPageContents = new List<StatementPageContent>();
        //            statementRawData.StatementPageContents.ToList().ForEach(it => newStatementPageContents.Add(new StatementPageContent()
        //            {
        //                Id = it.Id,
        //                PageId = it.PageId,
        //                PageTypeId = it.PageTypeId,
        //                HtmlContent = it.HtmlContent,
        //                PageHeaderContent = it.PageHeaderContent,
        //                PageFooterContent = it.PageFooterContent,
        //                DisplayName = it.DisplayName,
        //                TabClassName = it.TabClassName,
        //                DynamicWidgets = it.DynamicWidgets
        //            }));

        //            long FirstPageId = statement.Pages[0].Identifier;
        //            var firstPage = statement.Pages[0];
        //            var widgets = new List<PageWidget>(firstPage.PageWidgets);
        //            for (int i = 0; i < statement.Pages.Count; i++)
        //            {
        //                var page = statement.Pages[i];
        //                var statementPageContent = newStatementPageContents.Where(item => item.PageTypeId == page.PageTypeId && item.Id == i).FirstOrDefault();

        //                var MarketingMessageCounter = 0;
        //                var Messages = _lstMessage?.Where(it => it.Type == page.PageTypeName)?.ToList();

        //                //sub page count under current page tab
        //                var PageHeaderContent = new StringBuilder(statementPageContent.PageHeaderContent);
        //                var dynamicWidgets = new List<DynamicWidget>(statementPageContent.DynamicWidgets);

        //                string tabClassName = Regex.Replace((statementPageContent.DisplayName + "-" + page.Identifier), @"\s+", "-");
        //                NavItemList.Append("<li class='nav-item" + (i != statement.Pages.Count - 1 ? " nav-rt-border" : string.Empty) + "'><a id='tab" + i + "-tab' data-toggle='tab' data-target='#" + tabClassName + "' role='tab' class='nav-link" + (i == 0 ? " active" : string.Empty) + "'> " + statementPageContent.DisplayName + " </a></li>");

        //                string ExtraClassName = string.Empty;
        //                PageHeaderContent.Replace("{{ExtraClass}}", ExtraClassName).Replace("{{DivId}}", tabClassName);

        //                var newPageContent = new StringBuilder();
        //                var pagewidgets = new List<PageWidget>(page.PageWidgets);
        //                var pageContent = new StringBuilder(statementPageContent.HtmlContent);

        //                for (int j = 0; j < pagewidgets.Count; j++)
        //                {
        //                    var widget = pagewidgets[j];
        //                    if (!widget.IsDynamicWidget)
        //                    {
        //                        switch (widget.WidgetName)
        //                        {
        //                            case HtmlConstants.CUSTOMER_DETAILS_WIDGET_NAME:
        //                                bool isShowCellNo = false;
        //                                string vatNo = string.Empty;
        //                                if (statement.Pages.Count == 1)
        //                                {
        //                                    if (page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE 
        //                                     || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE 
        //                                     || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE 
        //                                     || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE 
        //                                     || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE 
        //                                     || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE 
        //                                     || page.PageTypeName.Trim() == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE)
        //                                    {
        //                                        isShowCellNo = true;
        //                                    }
        //                                    if (page.PageTypeName.Trim() == HtmlConstants.MULTI_CURRENCY_FOR_CIB_PAGE_TYPE || page.PageTypeName.Trim() == HtmlConstants.MULTI_CURRENCY_FOR_WEA_PAGE_TYPE)
        //                                    {
        //                                        if (MCA != null && MCA.Count > 0)
        //                                        {
        //                                            vatNo = MCA[0].VatNo;
        //                                        }
        //                                    }
        //                                    this.BindCustomerDetailsWidgetData(pageContent, customer, CorporateSaver, page, widget, isShowCellNo, vatNo);
        //                                }
        //                                break;

        //                            case HtmlConstants.BRANCH_DETAILS_WIDGET_NAME:
        //                                if (page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE)
        //                                {
        //                                    if (statement.Pages.Count == 1)
        //                                    {
        //                                        this.BindBondDetailsWidgetData(pageContent, page, widget, HomeLoanAccounts, customer);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    this.BindBranchDetailsWidgetData(pageContent, BranchId, page, widget, tenantCode, customer);
        //                                }
        //                                break;

        //                            case HtmlConstants.IMAGE_WIDGET_NAME:
        //                                IsFailed = this.BindImageWidgetData(pageContent, ErrorMessages, customer.CustomerId, customerMedias, statementRawData.BatchDetails, statement, page, batchMaster, widget, tenantCode, statementRawData.OutputLocation);
        //                                break;

        //                            case HtmlConstants.VIDEO_WIDGET_NAME:
        //                                IsFailed = this.BindVideoWidgetData(pageContent, ErrorMessages, customer.CustomerId, customerMedias, statementRawData.BatchDetails, statement, page, batchMaster, widget, tenantCode, statementRawData.OutputLocation);
        //                                break;

        //                            case HtmlConstants.STATIC_SEGMENT_BASED_CONTENT_NAME:
        //                                IsFailed = this.BindSegmentBasedContentWidgetData(pageContent, customer, page, widget, statement, ErrorMessages, customer.Segment == "WEA" ? true : false);
        //                                break;

        //                            case HtmlConstants.STATIC_HTML_WIDGET_NAME:
        //                                IsFailed = this.BindStaticHtmlWidgetData(pageContent, customer, page, widget, statement, ErrorMessages);
        //                                break;

        //                            case HtmlConstants.PAGE_BREAK_WIDGET_NAME:
        //                                IsFailed = this.BindPageBreakWidgetData(pageContent, customer, page, widget, statement, ErrorMessages);
        //                                break;

        //                            case HtmlConstants.INVESTMENT_PORTFOLIO_STATEMENT_WIDGET_NAME:
        //                                this.BindInvestmentPortfolioStatementWidgetData(pageContent, customer, investmentMasters, page, widget);
        //                                break;

        //                            case HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME:
        //                                this.BindInvestorPerformanceWidgetData(pageContent, investmentMasters, page, widget, customer);
        //                                break;

        //                            case HtmlConstants.BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
        //                                this.BindBreakdownOfInvestmentAccountsWidgetData(pageContent, investmentMasters, page, widget);
        //                                break;

        //                            case HtmlConstants.EXPLANATORY_NOTES_WIDGET_NAME:
        //                                this.BindExplanatoryNotesWidgetData(pageContent, ExplanatoryNotes, page, widget);
        //                                break;

        //                            case HtmlConstants.SERVICE_WIDGET_NAME:
        //                                this.BindMarketingServiceWidgetData(pageContent, Messages, page, widget, MarketingMessageCounter, customer);
        //                                MarketingMessageCounter++;
        //                                break;

        //                            case HtmlConstants.PERSONAL_LOAN_DETAIL_WIDGET_NAME:
        //                                this.BindPersonalLoanDetailWidgetData(pageContent, PersonalLoan, page, widget, tenantCode);
        //                                break;

        //                            case HtmlConstants.PERSONAL_LOAN_TRANASCTION_WIDGET_NAME:
        //                                this.BindPersonalLoanTransactionWidgetData(pageContent, PersonalLoan, page, widget);
        //                                break;

        //                            case HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_WIDGET_NAME:
        //                                this.BindPersonalLoanPaymentDueWidgetData(pageContent, PersonalLoan, page, widget);
        //                                break;

        //                            case HtmlConstants.SPECIAL_MESSAGE_WIDGET_NAME:
        //                                this.BindSpecialMessageWidgetData(pageContent, SpecialMessage, page, widget, customer);
        //                                break;

        //                            case HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_WIDGET_NAME:
        //                                this.BindPersonalLoanInsuranceMessageWidgetData(pageContent, PersonalLoan, page, widget);
        //                                break;

        //                            case HtmlConstants.PERSONAL_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
        //                                this.BindPersonalLoanTotalAmountDetailWidgetData(pageContent, PersonalLoanAccounts, page, widget);
        //                                break;

        //                            case HtmlConstants.PERSONAL_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
        //                                this.BindPersonalLoanAccountsBreakdownWidgetData(pageContent, scriptHtmlRenderer, PersonalLoanAccounts, page, widget, batchMaster, customer, statementRawData.OutputLocation);
        //                                break;

        //                            case HtmlConstants.HOME_LOAN_TOTAL_AMOUNT_DETAIL_WIDGET_NAME:
        //                                this.BindHomeLoanTotalAmountDetailWidgetData(pageContent, HomeLoanAccounts, page, widget, customer);
        //                                break;

        //                            case HtmlConstants.HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
        //                                this.BindHomeLoanAccountsBreakdownWidgetData(pageContent, HomeLoanAccounts, page, widget, customer);
        //                                break;

        //                            case HtmlConstants.HOME_LOAN_SUMMARY_TAX_PURPOSE_WIDGET_NAME:
        //                                this.BindHomeLoanSummaryTaxPurposeWidgetData(pageContent, HomeLoanAccounts, widget);
        //                                break;

        //                            case HtmlConstants.HOME_LOAN_INSTALMENT_WIDGET_NAME:
        //                                IsFailed = this.BindHomeLoanInstalmentWidgetData(pageContent, HomeLoanAccounts, customer, page, widget, statement, ErrorMessages);
        //                                break;

        //                            case HtmlConstants.WEALTH_HOME_LOAN_TOTAL_AMOUNT_WIDGET_NAME:
        //                                this.BindHomeLoanWealthTotalAmountDetailWidgetData(pageContent, HomeLoanAccounts, page, widget, customer);
        //                                break;

        //                            case HtmlConstants.WEALTH_HOME_LOAN_ACCOUNTS_BREAKDOWN_WIDGET_NAME:
        //                                this.BindHomeLoanWealthAccountsBreakdownWidgetData(pageContent, scriptHtmlRenderer, HomeLoanAccounts, page, widget, batchMaster, customer, statementRawData.OutputLocation);
        //                                break;

        //                            case HtmlConstants.WEALTH_HOME_LOAN_SUMMARY_TAX_PURPOSE_WIDGET_NAME:
        //                                this.BindHomeLoanWealthSummaryTaxPurposeWidgetData(pageContent, HomeLoanAccounts, widget);
        //                                break;

        //                            case HtmlConstants.WEALTH_HOME_LOAN_INSTALMENT_WIDGET_NAME:
        //                                IsFailed = this.BindHomeLoanWealthInstalmentWidgetData(pageContent, HomeLoanAccounts, customer, page, widget, statement, ErrorMessages);
        //                                break;

        //                            case HtmlConstants.WEALTH_HOME_LOAN_BRANCH_DETAILS_WIDGET_NAME:
        //                                IsFailed = this.BindHomeLoanWealthBranchDetailsData(pageContent, page, widget, HomeLoanAccounts, ErrorMessages);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_DETAILS_WIDGET_NAME:
        //                                this.BindPortfolioCustomerDetailsWidgetData(pageContent, customer, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_CUSTOMER_ADDRESS_WIDGET_NAME:
        //                                this.BindPortfolioCustomerAddressDetailsWidgetData(pageContent, customer, page, widget);
        //                                break;

        //                            case HtmlConstants.CORPORATESAVER_AGENT_ADDRESS_NAME:
        //                                this.BindCorporateSaverAgentAddressDetailsWidgetData(pageContent, CorporateSaver[0], page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_CLIENT_CONTACT_DETAILS_WIDGET_NAME:
        //                                this.BindPortfolioClientContactDetailsWidgetData(pageContent, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_SUMMARY_WIDGET_NAME:
        //                                this.BindPortfolioAccountSummaryWidgetData(pageContent, AccountsSummaries, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_ACCOUNT_ANALYSIS_WIDGET_NAME:
        //                                this.BindPortfolioAccountAnalysisWidgetData(pageContent, scriptHtmlRenderer, _lstAccountAnalysis, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_REMINDERS_WIDGET_NAME:
        //                                this.BindPortfolioRemindersWidgetData(pageContent, Reminders, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PORTFOLIO_NEWS_ALERT_WIDGET_NAME:
        //                                this.BindPortfolioNewsAlertsWidgetData(pageContent, NewsAndAlerts, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_GREENBACKS_TOTAL_REWARDS_POINTS_WIDGET_NAME:
        //                                this.BindGreenbacksTotalRewardPointsWidgetData(pageContent, AccountsSummaries, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_YTD_REWARDS_POINTS_BAR_GRAPH_WIDGET_NAME:
        //                                this.BindGreenbacksYtdRewardsPointsGraphWidgetData(pageContent, scriptHtmlRenderer, GreenbacksRewardPoints, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_POINTS_REDEEMED_YTD_BAR_GRAPH_WIDGET_NAME:
        //                                this.BindGreenbacksPointsRedeemedYtdGraphWidgetData(pageContent, scriptHtmlRenderer, GreenbacksRedeemedPoints, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_WIDGET_NAME:
        //                                this.BindGreenbacksProductRelatedPonitsEarnedGraphWidgetData(pageContent, scriptHtmlRenderer, CustProductMonthwiseRewardPoints, page, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_CATEGORY_SPEND_REWARDS_PIE_CHART_WIDGET_NAME:
        //                                this.BindGreenbacksCategorySpendRewardPointsGraphWidgetData(pageContent, scriptHtmlRenderer, CustRewardSpendByCategory, page, widget);
        //                                break;

        //                            case HtmlConstants.WEALTH_BREAKDOWN_OF_INVESTMENT_ACCOUNTS_WIDGET_NAME:
        //                                this.BindDataToWealthBreakdownOfInvestmentAccountsWidget(pageContent, scriptHtmlRenderer, investmentMasters, page, widget, batchMaster, customer, statementRawData.OutputLocation);
        //                                break;
        //                            case HtmlConstants.INVESTMENT_WEALTH_PORTFOLIO_STATEMENT_WIDGET_NAME:
        //                                BindDataToWealthInvestmentPortfolioStatementWidget(pageContent, customer, investmentMasters, page, widget);
        //                                break;
        //                            case HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_NAME:
        //                                this.BindDataToWealthInvestorPerformanceStatementWidget(pageContent, investmentMasters, page, widget, customer);
        //                                break;
        //                            case HtmlConstants.WEALTH_EXPLANATORY_NOTES_WIDGET_NAME:
        //                                this.BindExplanatoryNotesWidgetData(pageContent, ExplanatoryNotes, page, widget);
        //                                break;
        //                            case HtmlConstants.NEDBANK_MCA_ACCOUNT_SUMMARY_WIDGET_NAME:
        //                                this.BindMCAAccountSummaryDetailWidgetData(pageContent, MCA, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_MCA_TRANSACTION_WIDGET_NAME:
        //                                this.BindMCATransactionDetailWidgetData(pageContent, MCA, widget, page);
        //                                break;

        //                            case HtmlConstants.NEDBANK_MCA_VAT_ANALYSIS_WIDGET_NAME:
        //                                this.BindMCAVATAnalysisDetailWidgetData(pageContent, MCA, widget, page);
        //                                break;

        //                            case HtmlConstants.NEDBANK_WEALTH_MCA_ACCOUNT_SUMMARY_WIDGET_NAME:
        //                                this.BindMCAAccountSummaryDetailWidgetData(pageContent, MCA, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_WEALTH_MCA_TRANSACTION_WIDGET_NAME:
        //                                this.BindMCATransactionDetailWidgetData(pageContent, MCA, widget, page);
        //                                break;

        //                            case HtmlConstants.NEDBANK_WEALTH_MCA_VAT_ANALYSIS_WIDGET_NAME:
        //                                this.BindMCAVATAnalysisDetailWidgetData(pageContent, MCA, widget, page);
        //                                break;

        //                            case HtmlConstants.NEDBANK_WEALTH_MCA_BRANCH_DETAILS_WIDGET_NAME:
        //                                this.BindBranchDetailsWidgetData(pageContent, BranchId, page, widget, tenantCode, customer);
        //                                break;
        //                            case HtmlConstants.CORPORATESAVER_AGENT_MESSAGE_WIDGET_NAME:
        //                                this.BindCorporateSaverAgentMessageWidgetData(pageContent, BranchId, page, widget, tenantCode, customer);
        //                                break;

        //                            case HtmlConstants.NEDBANK_CORPORATESAVER_TRANSACTION_WIDGET_NAME:
        //                                this.BindCorporateSaverTransactionDetailWidgetData(pageContent, CorporateSaver, widget, page, customer);
        //                                break;

        //                            case HtmlConstants.NETBANK_CORPORATESAVER_AGENTDETAILS_NAME:
        //                                this.BindCorporateTaxTotalData(pageContent, CorporateSaver, widget, page);
        //                                break;

        //                            case HtmlConstants.NEDBANK_CORPORATESAVER_CLIENTANDAGENT_DETAILS_NAME:
        //                                this.BindCorporateSaverClientDetailsWidgetData(pageContent, CorporateSaver, widget);
        //                                break;

        //                            case HtmlConstants.NEDBANK_CORPORATESAVER_LASTTOTAL_NAME:
        //                                this.BindCorporateSaverLastTotalWidgetData(pageContent, CorporateSaver, widget, page, customer);
        //                                break;

        //                            case HtmlConstants.NEDBANK_WEALTH_CORPORATESAVER_VAT_ANALYSIS_WIDGET_NAME:
        //                                this.BindMCAVATAnalysisDetailWidgetData(pageContent, MCA, widget, page);
        //                                break;

        //                            case HtmlConstants.NEDBANK_WEALTH_CORPORATESAVER_BRANCH_DETAILS_WIDGET_NAME:
        //                                this.BindBranchDetailsWidgetData(pageContent, BranchId, page, widget, tenantCode, customer);
        //                                break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var dynaWidgets = dynamicWidgets.Where(item => item.Identifier == widget.WidgetId).ToList();
        //                        if (dynaWidgets.Count > 0)
        //                        {
        //                            var dynawidget = dynaWidgets.FirstOrDefault();
        //                            var themeDetails = new CustomeTheme();
        //                            if (dynawidget.ThemeType == "Default")
        //                            {
        //                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(statementRawData.TenantConfiguration.WidgetThemeSetting);
        //                            }
        //                            else
        //                            {
        //                                themeDetails = JsonConvert.DeserializeObject<CustomeTheme>(dynawidget.ThemeCSS);
        //                            }

        //                            //Get data from database for widget
        //                            httpClient = new HttpClient();
        //                            httpClient.BaseAddress = new Uri(statementRawData.TenantConfiguration.BaseUrlForTransactionData);
        //                            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ModelConstant.APPLICATION_JSON_MEDIA_TYPE));
        //                            httpClient.DefaultRequestHeaders.Add(ModelConstant.TENANT_CODE_KEY, tenantCode);

        //                            //API search parameter
        //                            JObject searchParameter = new JObject();
        //                            searchParameter[ModelConstant.BATCH_ID] = batchMaster.Identifier;
        //                            searchParameter[ModelConstant.CUSTOEMR_ID] = customer.CustomerId;
        //                            searchParameter[ModelConstant.WIDGET_FILTER_SETTING] = dynawidget.WidgetFilterSettings;

        //                            switch (dynawidget.WidgetType)
        //                            {
        //                                case HtmlConstants.TABLE_DYNAMICWIDGET:
        //                                    this.BindDynamicTableWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
        //                                    break;
        //                                case HtmlConstants.FORM_DYNAMICWIDGET:
        //                                    this.BindDynamicFormWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
        //                                    break;
        //                                case HtmlConstants.LINEGRAPH_DYNAMICWIDGET:
        //                                    this.BindDynamicLineGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
        //                                    break;
        //                                case HtmlConstants.BARGRAPH_DYNAMICWIDGET:
        //                                    this.BindDynamicBarGraphWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails);
        //                                    break;
        //                                case HtmlConstants.PICHART_DYNAMICWIDGET:
        //                                    this.BindDynamicPieChartWidgetData(pageContent, scriptHtmlRenderer, page, widget, searchParameter, dynawidget, httpClient, themeDetails, tenantCode);
        //                                    break;
        //                                case HtmlConstants.HTML_DYNAMICWIDGET:
        //                                    this.BindDynamicHtmlWidgetData(pageContent, page, widget, searchParameter, dynawidget, httpClient);
        //                                    break;
        //                            }
        //                        }
        //                    }
        //                }

        //                newPageContent.Append(pageContent);
        //                statementPageContent.PageHeaderContent = PageHeaderContent.ToString();
        //                statementPageContent.HtmlContent = newPageContent.ToString();
        //            }

        //            //to add statement metadata records
        //            if (IsInvestmentPageTypePresent && investmentMasters != null && investmentMasters.Count > 0)
        //            {
        //                investmentMasters.ForEach(invest =>
        //                {
        //                    var InvestmentNo = Convert.ToString(invest.InvestorId) + Convert.ToString(invest.InvestmentId);
        //                    while (InvestmentNo.Length != 12)
        //                    {
        //                        InvestmentNo = "0" + InvestmentNo;
        //                    }

        //                    //to separate to string dates values into required date format -- 
        //                    //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
        //                    //1st try with string separator, if fails then try with char separator
        //                    var statementPeriod = string.Empty;
        //                    string[] stringSeparators = new string[] { " - ", "- ", " -" };
        //                    string[] dates = invest.StatementPeriod.Split(stringSeparators, StringSplitOptions.None);
        //                    if (dates.Length > 0)
        //                    {
        //                        if (dates.Length > 1)
        //                        {
        //                            statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[1]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
        //                        }
        //                        else
        //                        {
        //                            dates = investmentMasters[0].StatementPeriod.Split(new Char[] { ' ' });
        //                            if (dates.Length > 2)
        //                            {
        //                                statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[2]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        statementPeriod = investmentMasters[0].StatementPeriod;
        //                    }

        //                    statementMetadataRecords.Add(new StatementMetadata
        //                    {
        //                        AccountNumber = InvestmentNo,
        //                        AccountType = invest.ProductDesc,
        //                        CustomerId = customer.CustomerId,
        //                        CustomerName = customer.FirstName + " " + customer.SurName,
        //                        StatementPeriod = statementPeriod,
        //                        StatementId = statement.Identifier,
        //                    });
        //                });
        //            }
        //            if (IsPersonalLoanPageTypePresent && PersonalLoanAccounts != null && PersonalLoanAccounts.Count > 0)
        //            {
        //                PersonalLoanAccounts.ForEach(PersonalLoans =>
        //                {
        //                    statementMetadataRecords.Add(new StatementMetadata
        //                    {
        //                        AccountNumber = PersonalLoans.InvestorId.ToString(),
        //                        AccountType = (!string.IsNullOrEmpty(PersonalLoans.ProductType) ? PersonalLoans.ProductType : HtmlConstants.PERSONAL_LOAN_PAGE_TYPE),
        //                        CustomerId = customer.CustomerId,
        //                        CustomerName = customer.FirstName.Trim() + " " + customer.SurName?.Trim(),
        //                        StatementPeriod = PersonalLoans.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoans.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy),
        //                        StatementId = statement.Identifier,
        //                    });
        //                });
        //            }
        //            if (IsHomeLoanPageTypePresent && HomeLoanAccounts != null && HomeLoanAccounts.Count > 0)
        //            {
        //                HomeLoanAccounts.ForEach(HomeLoan =>
        //                {
        //                    statementMetadataRecords.Add(new StatementMetadata
        //                    {
        //                        AccountNumber = HomeLoan.InvestorId.ToString(),
        //                        AccountType = HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE,
        //                        CustomerId = customer.CustomerId,
        //                        CustomerName = customer.FirstName.Trim() + " " + customer.SurName?.Trim(),
        //                        StatementPeriod = "Monthly",
        //                        StatementId = statement.Identifier,
        //                    });
        //                });
        //            }

        //            //NAV bar will append to html statement, only if statement definition have more than 1 pages 
        //            if (statement.Pages.Count > 1)
        //            {
        //                htmlbody.Append(HtmlConstants.NEDBANK_NAV_BAR_HTML.Replace("{{Today}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MMM_yyyy)).Replace("{{NavItemList}}", NavItemList.ToString()));
        //            }

        //            htmlbody.Append(HtmlConstants.PAGE_TAB_CONTENT_HEADER);
        //            newStatementPageContents.ToList().ForEach(page =>
        //            {
        //                //htmlbody.Append(page.PageHeaderContent);
        //                htmlbody.Append(page.HtmlContent);
        //                //htmlbody.Append(page.PageFooterContent);
        //                //htmlbody.Append(HtmlConstants.PAGE_FOOTER_HTML);
        //            });
        //            htmlbody.Append(HtmlConstants.END_DIV_TAG); // end tab-content div

        //            //if (customer.Segment == "WEA" ? true : false)
        //            //{
        //            //    var footerContent = new StringBuilder(HtmlConstants.WEALTH_NEDBANK_STATEMENT_FOOTER);
        //            //    footerContent.Replace("{{NedbankSloganImage}}", "../common/images/Footer_Image.png");
        //            //    htmlbody.Append(footerContent.ToString());
        //            //}
        //            //else
        //            //{
        //            //    var footerContent = new StringBuilder(HtmlConstants.NEDBANK_STATEMENT_FOOTER);
        //            //    footerContent.Replace("{{NedbankSloganImage}}", "../common/images/See_money_differently.PNG");
        //            //    footerContent.Replace("{{NedbankNameImage}}", "../common/images/NEDBANK_Name.png");
        //            //    footerContent.Replace("{{FooterText}}", HtmlConstants.NEDBANK_STATEMENT_FOOTER_TEXT_STRING);
        //            //    footerContent.Replace("{{LastFooterText}}", string.Empty);
        //            //    htmlbody.Append(footerContent.ToString());
        //            //}

        //            //htmlbody.Append(statementRawData.StatementPageContents?.FirstOrDefault().PageFooterContent);

        //            //htmlbody.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER); // end of container-fluid div

        //            var finalHtml = new StringBuilder();
        //            finalHtml.Append(HtmlConstants.HTML_HEADER);
        //            finalHtml.Append(htmlbody.ToString());
        //            finalHtml.Append(HtmlConstants.HTML_FOOTER);

        //            //replace below variable with actual data in final html string
        //            finalHtml.Replace("{{ChartScripts}}", scriptHtmlRenderer.ToString());
        //            finalHtml.Replace("{{CustomerNumber}}", customer.CustomerId.ToString());
        //            finalHtml.Replace("{{StatementNumber}}", statement.Identifier.ToString());
        //            finalHtml.Replace("{{FirstPageId}}", FirstPageId.ToString());
        //            finalHtml.Replace("{{TenantCode}}", tenantCode);

        //            finalHtml = Translate(finalHtml, customer);

        //            //If has any error while rendering html statement, then assign status as failed and all collected errors message to log message variable..
        //            //Otherwise write html statement string to actual html file and store it at output location, then assign status as completed
        //            if (IsFailed)
        //            {
        //                logDetailRecord.Status = ScheduleLogStatus.Failed.ToString();
        //                logDetailRecord.LogMessage = "<ul class='pl-4 text-left'>" + ErrorMessages.ToString() + "</ul>";
        //            }
        //            else
        //            {
        //                string productPrefix = string.Empty;
        //                string documentName = string.Empty;
        //                string statementDate = string.Empty;
        //                string endDate = string.Empty;
        //                if (statement.Pages.Where(x => x.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE || x.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE ||
        //                                               x.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || x.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE ||
        //                                               x.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE || x.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE).Count() > 0)
        //                {
        //                    productPrefix = "H";
        //                    documentName = "Home Loan Statement";
        //                    statementDate = HomeLoanAccounts?.FirstOrDefault().StatementDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy3);
        //                }
        //                else if (statement.Pages.Where(x => x.PageTypeName == HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).Count() > 0)
        //                {
        //                    productPrefix = "P";
        //                    documentName = "Personal Loan Statement";
        //                    statementDate = PersonalLoanAccounts?.FirstOrDefault().ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy3);
        //                }
        //                else if (statement.Pages.Where(x => x.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH || x.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN ||
        //                                                    x.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH || x.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN).Count() > 0)
        //                {
        //                    productPrefix = "I";
        //                    documentName = "Investment Statement";
        //                    statementDate = investmentMasters?.FirstOrDefault().StatementDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy3);
        //                }
        //                else if (statement.Pages.Where(x => x.PageTypeName == HtmlConstants.MULTI_CURRENCY_FOR_CIB_PAGE_TYPE || x.PageTypeName == HtmlConstants.MULTI_CURRENCY_FOR_WEA_PAGE_TYPE).Count() > 0)
        //                {
        //                    productPrefix = "M";
        //                    documentName = "MCA Statement";
        //                    statementDate = MCA?.FirstOrDefault().StatementDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy3);
        //                }
        //                else if (statement.Pages.Where(x => x.PageTypeName == HtmlConstants.CORPORATE_SAVER_AFR_PAGE_TYPE || x.PageTypeName == HtmlConstants.CORPORATE_SAVER_ENG_PAGE_TYPE).Count() > 0)
        //                {
        //                    productPrefix = "C";
        //                    documentName = "Corporate Saver Statement";
        //                }

        //                string fileName = productPrefix + customer.CustomerId + " _ " + documentName + "  " + customer.Segment + " " + customer.Language + " " + statementDate + " " + DateTime.Now.ToString(ModelConstant.TIME_FORMAT_HH_MM_SS).Replace(".", " ").Replace(":", " ") + ".html";
        //                //string fileName = "Statement_" + customer.Identifier + "_" + statement.Identifier + "_" + DateTime.Now.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".html";
        //                string headerHtml = statement.Pages[0].HeaderHTML;
        //                string footerHtml = statement.Pages[0].FooterHTML;

        //                string filePath = this.utility.WriteToFile(finalHtml.ToString(), fileName, "",batchMaster.BatchName, customer.CustomerId, statementRawData.BaseURL, statementRawData.OutputLocation, printPdf: true, headerHtml: headerHtml, footerHtml: footerHtml, segment: statement.Pages[0].PageTypeName, language: customer.Language);

        //                logDetailRecord.StatementFilePath = filePath;
        //                logDetailRecord.Status = ScheduleLogStatus.Completed.ToString();
        //                logDetailRecord.LogMessage = "Statement generated successfully..!!";
        //                logDetailRecord.statementMetadata = statementMetadataRecords;
        //            }
        //        }

        //        return logDetailRecord;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

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

        private void BindPaymentSummaryWidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<spIAA_PaymentDetail> fspDetails, IList<BatchDetail> batchDetails)
        {
            pageContent.Replace("{{IntTotal}}", fspDetails.First().Earning_Amount);
            pageContent.Replace("{{Vat}}", fspDetails.First().VAT_Amount);
            pageContent.Replace("{{TotalDue}}", (Convert.ToDouble(fspDetails.First().Earning_Amount) +
                Convert.ToDouble(fspDetails.First().VAT_Amount)).ToString());
            pageContent.Replace("{{IntTotalDate}}", fspDetails.First().POSTED_DATE.ToString("MMMM yyyy"));
            // Format the date with a custom format
            string formattedOrdinalDate = FormatDateWithOrdinal(fspDetails.First().POSTED_DATE);
            pageContent.Replace("{{IntPostedDate}}", formattedOrdinalDate);
        }

        private bool BindProductSummaryWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, IList<spIAA_PaymentDetail> productSummary, Page page, PageWidget widget)
        {
            var IsFailed = false;
            try
            {
                if (productSummary != null && productSummary.Count > 0)
                {
                    StringBuilder productSummarySrc = new StringBuilder();
                    long index = 1;
                    productSummary.ToList().ForEach(item =>
                    {
                        productSummarySrc.Append("<tr><td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" + index + "</td><td class='fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td>" + "<td class='fsp-bdr-right fsp-bdr-bottom px-1'> " + (item.Prod_Group == "Service Fee" ? "Premium Under Advise Fee" : item.Prod_Group) + "</td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1'>R" + item.Display_Amount.ToString().Replace(',', '.') + "</td><td class='text-center fsp-bdr-bottom px-1'><a  href ='https://facebook.com' target='_blank'><img class='leftarrowlogo' src ='../common/images/leftarrowlogo.png' alt = 'Left Arrow'></a></td></tr>");
                        index++;
                    });
                    pageContent.Replace("{{ProductSummary}}", productSummarySrc.ToString());
                    pageContent.Replace("{{QueryBtn}}", "../common/images/IfQueryBtn.jpg");

                   String totalDue= productSummary.FirstOrDefault().Earning_Amount;
                    totalDue = totalDue.Replace('.', ',');
                    pageContent.Replace("{{TotalDue}}", "R" + totalDue);
                    String vatAmount = productSummary.FirstOrDefault().VAT_Amount;
                    vatAmount = vatAmount.Replace('.', ',');
                    pageContent.Replace("{{VATDue}}", "R" + vatAmount);
                    double grandTotalDue = (Convert.ToDouble(productSummary.FirstOrDefault().Earning_Amount) + Convert.ToDouble(productSummary.FirstOrDefault().VAT_Amount));
                    String grandTotalDueStr = grandTotalDue.ToString().Replace(',', '.');
                    pageContent.Replace("{{GrandTotalDue}}", "R" + grandTotalDueStr);
                    double ppsPayment = grandTotalDue;
                    pageContent.Replace("{{PPSPayment}}", "-R" + grandTotalDueStr);
                    String Balance=Convert.ToDouble((grandTotalDue - ppsPayment)).ToString().Replace(',', '.');

                    pageContent.Replace("{{Balance}}", "R" + Balance);

                }
                else
                {
                    ErrorMessages.Append("<li>Product master data is not available related to Product Summary widget..!!</li>");
                    IsFailed = true;
                }
                return IsFailed;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private bool BindDetailedTransactionsWidgetData(StringBuilder pageContent, StringBuilder ErrorMessages, IList<spIAA_PaymentDetail> transaction, Page page, PageWidget widget)
        {
            var IsFailed = false;
            double TotalPostedAmount = 0;
            try
            {
                if (transaction != null && transaction.Count > 0)
                {
                    StringBuilder detailedTransactionSrc = new StringBuilder();
                    var records = transaction.GroupBy(gptransactionitem => gptransactionitem.INT_EXT_REF).ToList();
                    records?.ForEach(transactionitem =>
                    {
                        detailedTransactionSrc.Append("<div class='px-50'><div class='prouct-table-block'><div class='text-left fsp-transaction-title font-weight-bold mb-3'>Intermediary:  " + transactionitem.FirstOrDefault().INT_EXT_REF + " " + transactionitem.FirstOrDefault().Int_Name + "</div><table width='100%' cellpadding='0' cellspacing='0'> <tr><th class='font-weight-bold text-white'>Client name</th> <th class='font-weight-bold text-white text-center pe-0 bdr-r-0'>Member<br /> number</th> <th class='font-weight-bold text-white text-center'>Will<br/> number</th> <th class='font-weight-bold text-white text-center'>Fiduciary fees</th> <th class='font-weight-bold text-white text-center'>Commission<br /> type</th> <th class='font-weight-bold text-white text-center'>Posted date</th> <th class='font-weight-bold text-white text-center'>Posted amount</th> <th class='font-weight-bold text-white'>Query</th> </tr> ");
                        pageContent.Replace("{{QueryBtnImgLink}}", "https://www.google.com/");
                        pageContent.Replace("{{QueryBtn}}", "../common/images/IfQueryBtn.jpg");
                        transaction.Where(witem => witem.INT_EXT_REF == transactionitem.FirstOrDefault().INT_EXT_REF).ToList().ForEach(item =>
                        {
                            detailedTransactionSrc.Append("<tr><td align = 'center' valign = 'center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'>" +
                                    item.Client_Name + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Member_Ref + "</td><td class= 'fsp-bdr-right fsp-bdr-bottom px-1'> " + item.Policy_Ref + "</td><td class= 'text-right fsp-bdr-right fsp-bdr-bottom px-1'>" + (item.Description == "Commission Service Fee" ? "Premium Under Advise Fee" : item.Description)  + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.Commission_Type + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'>" + item.POSTED_DATE.ToString("dd-MMM-yyyy") + "</td><td class= 'text-center fsp-bdr-right fsp-bdr-bottom px-1'> R" + item.Display_Amount + "</td><td class= 'text-center fsp-bdr-bottom px-1'><a href ='https://www.google.com/' target ='_blank'><img class='leftarrowlogo' src='../common/images/leftarrowlogo.png' alt='Left Arrow'></a></td></tr>");
                            TotalPostedAmount += ((item.TYPE == "Fiduciary_Data") && (item.Prod_Group != "VAT"))?  (Convert.ToDouble(item.Display_Amount)): 0.0;
                        });
                        string TotalPostedAmountR = (TotalPostedAmount == 0) ? "0" : ("R " + TotalPostedAmount.ToString());
                        detailedTransactionSrc.Append("<tr> <td align='center' valign='center' class='px-1 py-1 fsp-bdr-right fsp-bdr-bottom'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-right fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'><br /></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'></td> <td class='text-center fsp-bdr-right fsp-bdr-bottom px-1 py-1'>R" + TotalPostedAmountR + "</td> <td class='text-center fsp-bdr-bottom px-1'><a href='https://www.google.com/' target = '_blank' ><img src='../common/images/leftarrowlogo.png'></a></td> </tr></table><div class='text-right w-100 pt-3'><a href='https://www.google.com/' target = '_blank'><img src='../common/images/click-print-stmt-btn.jpg'></a></div></div></div></div>");
                        TotalPostedAmount = 0;
                    });
                    pageContent.Replace("{{detailedTransaction}}", detailedTransactionSrc.ToString());
                }
                else
                {
                    ErrorMessages.Append("<li>Product master data is not available related to Product Summary widget..!!</li>");
                    IsFailed = true;
                }
                return IsFailed;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void BindPpsHeadingWidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<spIAA_PaymentDetail> fspDetails, IList<BatchDetail> batchDetails)
        {
            pageContent.Replace("{{FSPName}}", fspDetails.FirstOrDefault().FSP_Name);
            pageContent.Replace("{{FSPTradingName}}", fspDetails.FirstOrDefault().FSP_Trading_Name);
        }

        private void BindPpsDetailsWidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<spIAA_PaymentDetail> fspDetails, IList<BatchDetail> batchDetails)
        {
            pageContent.Replace("{{FSPNumber}}", fspDetails.FirstOrDefault().FSP_Ext_Ref);
            pageContent.Replace("{{FSPAgreeNumber}}", fspDetails.FirstOrDefault().FSP_REF);
            pageContent.Replace("{{VATRegNumber}}", fspDetails.FirstOrDefault().FSP_VAT_Number);
        }

        private void BindPpsFooter1WidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<spIAA_PaymentDetail> fspDetails, IList<BatchDetail> batchDetails)
        {
            string middleText = "PPS Insurance is a registered Insurer and FSP";
            string pageText = "Page 1/2";
            pageContent.Replace("{{FSPFooterDetails}}", middleText);
            pageContent.Replace("{{FSPPage}}", pageText);
        }
             private void BindFooterImageWidgetData(StringBuilder pageContent, CustomerMaster customer, Statement statement, Page page, PageWidget widget, IList<CustomerMedia> customerMedias, IList<spIAA_PaymentDetail> fspDetails, IList<BatchDetail> batchDetails)
        {
            //string middleText = "PPS Insurance is a registered Insurer and FSP";
            //string pageText = "Page 1/2";
            //pageContent.Replace("{{FSPFooterDetails}}", middleText);
            //pageContent.Replace("{{FSPPage}}", pageText);
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
            try
            {
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
            catch(Exception ex)
            {
                throw ex;
            }
           
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

        //private bool BindSegmentBasedContentWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages, bool isWealthWidget)
        //{
        //    try
        //    {
        //        var content = HtmlConstants.SEGMENT_BASED_CONTENT_WIDGET_HTML;

        //        dynamic widgetSetting = JArray.Parse(widget.WidgetSetting);
        //        var html = "";
        //        if (widgetSetting[0].Html.ToString().Length > 0)
        //        {
        //            html = widgetSetting[0].Html; //TODO: ***Deepak: Remove hard coded line
        //        }

        //        var chanceToWin = string.Empty;
        //        switch (customer.Segment.ToLower())
        //        {
        //            case "consumer banking":
        //            case "pbvcm":
        //            case "cib":
        //            case "nbb":
        //                chanceToWin = "";
        //                break;
        //            case "ncb":
        //            case "prb":
        //            case "sbs":
        //                chanceToWin = isWealthWidget ? (customer.Language == "ENG" ? HtmlConstants.WEALTH_CHANCE_TO_WIN : HtmlConstants.WEALTH_CHANCE_TO_WIN_AFR) : (customer.Language == "ENG" ? HtmlConstants.CHANCE_TO_WIN : HtmlConstants.CHANCE_TO_WIN_AFR);
        //                break;
        //        }

        //        html = html.Replace("{{chanceToWin}}", chanceToWin);

        //        content = content.Replace("{{SegmentBasedContent}}", html);

        //        pageContent.Replace("{{SegmentBasedContent_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", content);

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Append("<li>Error occurred while configuring SegmentBasedContent widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
        //        return true;
        //    }
        //}

        //private bool BindStaticHtmlWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages)
        //{
        //    try
        //    {
        //        dynamic widgetSetting = JObject.Parse(widget.WidgetSetting);
        //        var html = "";
        //        if (widgetSetting.html.ToString().Length > 0)
        //        {
        //            html = widgetSetting.html;
        //        }

        //        var contactCenter = string.Empty;
        //        switch (customer.Segment.ToLower())
        //        {
        //            case "consumer banking":
        //            case "ncb":
        //            case "pbvcm":
        //                contactCenter = HtmlConstants.CONSUMER_BANKING;
        //                break;
        //            case "prb":
        //                contactCenter = HtmlConstants.INVESTMENT_PRIVATE_BANKING;
        //                break;
        //            case "sbs":
        //                contactCenter = HtmlConstants.INVESTMENT_SBS_BANKING;
        //                break;
        //            case "nbb":
        //                contactCenter = HtmlConstants.NBB_BANKING;
        //                break;
        //            case "cib":
        //                contactCenter = HtmlConstants.CORPORATE_BANKING;
        //                break;
        //            case "wea":
        //                contactCenter = HtmlConstants.WEA_BANKING;
        //                break;
        //        }

        //        if (page.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH || page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN || page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH || page.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN)
        //        {
        //            var isWealthWidget = page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN || page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH ? true : false;
        //            var chanceToWin = string.Empty;
        //            switch (customer.Segment.ToLower())
        //            {
        //                case "consumer banking":
        //                case "pbvcm":
        //                case "cib":
        //                case "nbb":
        //                    chanceToWin = "";
        //                    break;
        //                case "ncb":
        //                case "prb":
        //                case "sbs":
        //                    chanceToWin = isWealthWidget ? (customer.Language == "ENG" ? HtmlConstants.WEALTH_CHANCE_TO_WIN : HtmlConstants.WEALTH_CHANCE_TO_WIN_AFR) : (customer.Language == "ENG" ? HtmlConstants.CHANCE_TO_WIN : HtmlConstants.CHANCE_TO_WIN_AFR);
        //                    break;
        //            }
        //            html = html.Replace("{{chanceToWin}}", chanceToWin);
        //        }

        //        html = html.Replace("{{contactCenter}}", contactCenter);
        //        html = html.Replace("{{statementDate}}", DateTime.Now.ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd));
        //        pageContent.Replace("{{StaticHtml_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Append("<li>Error occurred while configuring StaticHtml widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
        //        return true;
        //    }
        //}

        //private bool BindPageBreakWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages)
        //{
        //    try
        //    {
        //        var html = "<div style=\"page-break-before:always\">&nbsp;</div>";

        //        pageContent.Replace("{{PageBreak_" + statement.Identifier + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Append("<li>Error occurred while configuring PageBreak widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
        //        return true;
        //    }
        //}

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

        //private void BindCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_CorporateSaverMaster> corporateSaverList, Page page, PageWidget widget, bool isShowCellNo = false, string vatNo = "")
        //{
        //    var CustomerDetails = "";
        //    if (page.PageTypeName == HtmlConstants.CORPORATE_SAVER_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.CORPORATE_SAVER_AFR_PAGE_TYPE)
        //    {
        //        //CustomerDetails = "<br><br><br><br>" +
        //        CustomerDetails = "<img src=\"" + corporateSaverList.FirstOrDefault().AGENT_LOGO + "\" style=\"height: 60px;\" /><br>" +
        //        (!string.IsNullOrEmpty(customer.Title) && customer.Title.ToLower() != "null" ? customer.Title + " " : string.Empty) + (!string.IsNullOrEmpty(customer.FirstName) && customer.FirstName.ToLower() != "null" ? customer.FirstName + " " : string.Empty) + (!string.IsNullOrEmpty(customer.SurName) && customer.SurName.ToLower() != "null" ? customer.SurName + " " : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty) + "<br>";
        //    }
        //    else
        //    {
        //        CustomerDetails = (!string.IsNullOrEmpty(customer.Title) && customer.Title.ToLower() != "null" ? customer.Title + " " : string.Empty) + (!string.IsNullOrEmpty(customer.FirstName) && customer.FirstName.ToLower() != "null" ? customer.FirstName + " " : string.Empty) + (!string.IsNullOrEmpty(customer.SurName) && customer.SurName.ToLower() != "null" ? customer.SurName + " " : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3) : string.Empty) + "<br>" +
        //        (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty) + "<br>";

        //        if (isShowCellNo && !string.IsNullOrWhiteSpace(customer.Mask_Cell_No))
        //        {
        //            CustomerDetails += "<br> Cell: " + customer.Mask_Cell_No;
        //        }

        //        if (page.PageTypeName.Trim() == HtmlConstants.MULTI_CURRENCY_FOR_CIB_PAGE_TYPE || page.PageTypeName.Trim() == HtmlConstants.MULTI_CURRENCY_FOR_WEA_PAGE_TYPE)
        //        {
        //            CustomerDetails += "<br> Vat no : " + vatNo;
        //        }
        //    }

        //    pageContent.Replace("{{CustomerDetails_" + page.Identifier + "_" + widget.Identifier + "}}", CustomerDetails);

        //}

        //private void BindBranchDetailsWidgetData(StringBuilder pageContent, long BranchId, Page page, PageWidget widget, string tenantCode, DM_CustomerMaster customer)
        //{
        //    try
        //    {
        //        var contactCenter = string.Empty;
        //        switch (customer.Segment.ToLower())
        //        {
        //            case "consumer banking":
        //            case "ncb":
        //            case "pbvcm":
        //            case "pml":
        //                contactCenter = HtmlConstants.CONSUMER_BANKING;
        //                break;
        //            case "prb":
        //                contactCenter = HtmlConstants.INVESTMENT_PRIVATE_BANKING;
        //                break;
        //            case "sbs":
        //                contactCenter = HtmlConstants.INVESTMENT_SBS_BANKING;
        //                break;
        //            case "nbb":
        //                contactCenter = HtmlConstants.NBB_BANKING;
        //                break;
        //            case "cib":
        //                contactCenter = HtmlConstants.CORPORATE_BANKING;
        //                break;
        //            case "wea":
        //                contactCenter = HtmlConstants.WEA_BANKING;
        //                break;
        //        }

        //        StringBuilder BranchDetail = new StringBuilder();
        //        BranchDetail.Append(HtmlConstants.BANK_DETAILS);
        //        BranchDetail.Replace("{{TodayDate}}<br>", string.Empty);
        //        if (page.PageTypeName == HtmlConstants.MULTI_CURRENCY_FOR_WEA_PAGE_TYPE || page.PageTypeName == HtmlConstants.MULTI_CURRENCY_FOR_CIB_PAGE_TYPE)
        //        {
        //            if (page.PageTypeName == HtmlConstants.MULTI_CURRENCY_FOR_WEA_PAGE_TYPE)
        //            {
        //                contactCenter = "<p class='mca_text_custom_color-w'>" + HtmlConstants.CORPORATE_BANKING + "</p>";
        //            }
        //            else
        //            {
        //                contactCenter = "<p class='text-success'>" + HtmlConstants.CORPORATE_BANKING + "</p>";
        //            }
        //            BranchDetail.Append(contactCenter);
        //            pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //        else if (page.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH || page.PageTypeName == HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN || page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH || page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN)
        //        {
        //            if (page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN || page.PageTypeName == HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH)
        //            {
        //                contactCenter = "<br><br><p class='text-success-w'>" + contactCenter + "</p>";
        //            }
        //            else
        //            {
        //                contactCenter = "<br><br><p class='text-success'>" + contactCenter + "</p>";
        //            }

        //            BranchDetail.Append(contactCenter);
        //            pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", contactCenter);
        //        }

        //        pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BranchDetail.ToString());
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        //private void BindCorporateSaverAgentMessageWidgetData(StringBuilder pageContent, long BranchId, Page page, PageWidget widget, string tenantCode, DM_CustomerMaster customer)
        //{
        //    try
        //    {
        //        var branchDetails = this.tenantTransactionDataRepository.Get_DM_BranchMaster(BranchId, tenantCode)?.FirstOrDefault();
        //        if (branchDetails != null)
        //        {
        //            var contactCenter = string.Empty;
        //            //switch (customer.Segment.ToLower())
        //            //{
        //            //    case "consumer banking":
        //            //    case "ncb":
        //            //    case "pbvcm":
        //            //        contactCenter = HtmlConstants.CONSUMER_BANKING;
        //            //        break;
        //            //    case "prb":
        //            //        contactCenter = HtmlConstants.INVESTMENT_PRIVATE_BANKING;
        //            //        break;
        //            //    case "sbs":
        //            //        contactCenter = HtmlConstants.INVESTMENT_SBS_BANKING;
        //            //        break;
        //            //    case "nbb":
        //            //        contactCenter = HtmlConstants.NBB_BANKING;
        //            //        break;
        //            //    case "cib":
        //            //        contactCenter = HtmlConstants.CORPORATE_BANKING;
        //            //        break;
        //            //    case "wea":
        //            //        contactCenter = HtmlConstants.WEA_BANKING;
        //            //        break;
        //            //}

        //            StringBuilder BranchDetail = new StringBuilder();
        //            BranchDetail.Append(HtmlConstants.BANK_DETAILS);
        //            BranchDetail.Replace("{{TodayDate}}", string.Empty);

        //            pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BranchDetail.ToString());
        //            pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", contactCenter);
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}


        //private void BindBondDetailsWidgetData(StringBuilder pageContent, Page page, PageWidget widget, List<DM_HomeLoanMaster> HomeLoans, DM_CustomerMaster customer)
        //{
        //    try
        //    {
        //        var BondDetails = new StringBuilder();
        //        if (HomeLoans.Count == 1)
        //        {
        //            BondDetails.Append("Bond No: " + HomeLoans?.FirstOrDefault().BondNo.ToString() + "<br>");
        //        }

        //        var date = string.Empty;
        //        if (HomeLoans != null && HomeLoans.Count > 0)
        //        {
        //            date = HomeLoans.FirstOrDefault().StatementDate != null ? Convert.ToDateTime(HomeLoans.FirstOrDefault().StatementDate).ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd) : "";
        //        }
        //        BondDetails.Append(date);

        //        var contactCenter = string.Empty;

        //        switch (customer.Segment.ToLower())
        //        {
        //            case "nbb":
        //                contactCenter = HtmlConstants.NBB_BANKING;
        //                break;
        //            case "ncb":
        //                contactCenter = HtmlConstants.CONSUMER_BANKING;
        //                break;
        //            case "pml":
        //                contactCenter = HtmlConstants.PML_BANKING;
        //                break;
        //            case "prb":
        //                contactCenter = HtmlConstants.HOME_LOAN_PRIVATE_BANKING;
        //                break;
        //            case "sbs":
        //                contactCenter = HtmlConstants.HOME_LOAN_SBS_BANKING;
        //                break;
        //            case "wea":
        //                contactCenter = HtmlConstants.WEA_BANKING;
        //                break;
        //        }

        //        //BondDetails.Append();
        //        pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", BondDetails.ToString());
        //        pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", contactCenter);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private void BindInvestmentPortfolioStatementWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        //{
        //    if (investmentMasters != null && investmentMasters.Count > 0)
        //    {
        //        var transactions = new List<DM_InvestmentTransaction>();
        //        var TotalClosingBalance = 0.0m;
        //        investmentMasters.ForEach(invest =>
        //        {
        //            transactions.AddRange(invest.investmentTransactions);
        //            var res = 0.0m;
        //            try
        //            {
        //                TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(
        //                    it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC)
        //                    || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)
        //                ).Select(it => decimal.TryParse(it.WJXBFS4_Balance.Replace(",", "."), out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch (Exception)
        //            {
        //                TotalClosingBalance = 0.0m;
        //            }
        //        });

        //        pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.FirstName);
        //        pageContent.Replace("{{SurName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.SurName);

        //        pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", !string.IsNullOrEmpty(customer.DS_Investor_Name) ? ": " + customer.DS_Investor_Name : string.Empty);
        //        pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalClosingBalance));
        //        pageContent.Replace("{{DayOfStatement_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].DayOfStatement);
        //        pageContent.Replace("{{InvestorID_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(investmentMasters[0].InvestorId));

        //        //to separate to string dates values into required date format -- 
        //        //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
        //        //1st try with string separator, if fails then try with char separator
        //        var statementPeriod = string.Empty;
        //        string[] stringSeparators = new string[] { " - ", "- ", " -" };


        //        DateTime minDate = transactions.Min(m => m.TransactionDate);
        //        DateTime maxDate = transactions.Max(m => m.TransactionDate);
        //        statementPeriod = minDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + (customer.Language == "ENG" ? " to " : " tot ") + maxDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);

        //        pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", statementPeriod);
        //        pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].StatementDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //    }
        //}

        //private void BindInvestorPerformanceWidgetData(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, DM_CustomerMaster customerData)
        //{
        //    if (investmentMasters != null && investmentMasters.Count > 0)
        //    {
        //        List<InvestorPerformanceWidgetData> investorPerformanceWidgetDatas = new List<InvestorPerformanceWidgetData>();
        //        List<DM_InvestmentTransaction> transactions = new List<DM_InvestmentTransaction>();

        //        investmentMasters.OrderBy(a => a.InvestmentId).ToList().ForEach(invest =>
        //        {
        //            InvestorPerformanceWidgetData item;
        //            var productType = invest.ProductType;
        //            if (investorPerformanceWidgetDatas.Any(m => m.ProductType == productType))
        //            {
        //                item = investorPerformanceWidgetDatas.FirstOrDefault(m => m.ProductType == productType);
        //                foreach (DM_InvestmentTransaction transaction in invest.investmentTransactions)
        //                {
        //                    transaction.ProductId = Convert.ToInt64(item.ProductId);
        //                }
        //            }
        //            else
        //            {
        //                item = new InvestorPerformanceWidgetData() { ProductType = productType, ProductId = invest.ProductId.ToString() };
        //                investorPerformanceWidgetDatas.Add(item);
        //            }

        //            transactions.AddRange(invest.investmentTransactions);
        //        });

        //        var html = "";

        //        int counter = 0;

        //        foreach (var item in investorPerformanceWidgetDatas)
        //        {
        //            item.OpeningBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && (m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC) || m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC_AFR))).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();
        //            item.ClosingBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && (m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC) || m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR))).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();

        //            item.OpeningBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.OpeningBalance));
        //            item.ClosingBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.ClosingBalance));

        //            var InvestorPerformanceHtmlWidget = HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_HTML;
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", item.ProductType);
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", item.OpeningBalance);
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", item.ClosingBalance);
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{WidgetId}}", "");

        //            if (counter != 0)
        //                InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("<div class='card-body-header pb-2'>Investor performance</div>", "");

        //            html += InvestorPerformanceHtmlWidget;
        //            counter++;
        //        }
        //        pageContent.Replace("{{" + HtmlConstants.INVESTOR_PERFORMANCE_WIDGET_NAME + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
        //    }
        //}

        //private void BindBreakdownOfInvestmentAccountsWidgetData(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        //{
        //    if (investmentMasters != null && investmentMasters.Count > 0)
        //    {
        //        //Create Nav tab if investment accounts is more than 1
        //        var NavTabs = new StringBuilder();
        //        var InvestmentAccountsCount = investmentMasters.Count;
        //        pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

        //        //create tab-content div if accounts is greater than 1, otherwise create simple div
        //        var TabContentHtml = new StringBuilder();
        //        var counter = 0;
        //        investmentMasters.OrderBy(m => m.InvestmentId).ToList().ForEach(acc =>
        //         {
        //             var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.INVESTMENT_ACCOUNT_DETAILS_HTML_SMT);

        //             #region Account Details
        //             InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
        //             InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", Convert.ToString(acc.InvestmentId));
        //             InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", string.Empty);

        //             var InvestmentNo = Convert.ToString(acc.InvestorId) + " " + Convert.ToString(acc.InvestmentId);
        //             //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
        //             while (InvestmentNo.Length != 13)
        //             {
        //                 InvestmentNo = "0" + InvestmentNo;
        //             }
        //             InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
        //             InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.AccountOpenDate != null ? acc.AccountOpenDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);
        //             var res = 0.0m;
        //             if (acc.CurrentInterestRate != null && decimal.TryParse(acc.CurrentInterestRate, out res))
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{InterestRate}}", decimal.Round(res, 2) + "% pa");
        //             }
        //             else
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{InterestRate}}", string.Empty);
        //             }

        //             InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate != null ? acc.ExpiryDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);
        //             InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc != null ? acc.InterestDisposalDesc : string.Empty);
        //             InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod != null ? acc.NoticePeriod : string.Empty);

        //             res = 0.0m;
        //             acc.AccuredInterest = acc.AccuredInterest.Replace(",", ".");
        //             if (acc.AccuredInterest != null && decimal.TryParse(acc.AccuredInterest, out res))
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{InterestDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(res))));
        //             }
        //             else
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{InterestDue}}", "R0.00");
        //             }

        //             if (acc.ProductType == "Fixed deposits" || acc.ProductType == "Linked Deposits" || acc.ProductType == "Vaste deposito's" || acc.ProductType == "Gekoppelde deposito's")
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{BonusIntrestLabel}}", "Bonus Interest:");
        //                 InvestmentAccountDetailHtml.Replace("{{BonusIntrestValue}}", (acc.BonusInterest + "% pa"));
        //             }
        //             else
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{BonusIntrestLabel}}", "");
        //                 InvestmentAccountDetailHtml.Replace("{{BonusIntrestValue}}", "");
        //             }

        //             var LastInvestmentTransaction = acc.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC) || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)).OrderByDescending(it => it.TransactionDate)?.ToList()?.FirstOrDefault();
        //             if (LastInvestmentTransaction != null)
        //             {
        //                 LastInvestmentTransaction.WJXBFS4_Balance = LastInvestmentTransaction.WJXBFS4_Balance.Replace(",", ".");
        //                 InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", LastInvestmentTransaction.TransactionDate.ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy));
        //                 if (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res))
        //                 {
        //                     InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", (LastInvestmentTransaction.WJXBFS4_Balance == "0" ? "-" : (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res) ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(LastInvestmentTransaction.WJXBFS4_Balance))) : "0")));
        //                 }
        //                 else
        //                 {
        //                     InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "0");
        //                 }
        //             }
        //             else
        //             {
        //                 InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "");
        //                 InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "");
        //             }

        //             #endregion Account Details

        //             #region Transactions
        //             var InvestmentTransactionRows = new StringBuilder();
        //             if (acc.investmentTransactions != null && acc.investmentTransactions.Count > 0)
        //             {
        //                 acc.investmentTransactions.OrderBy(it => it.TransactionDate).ToList().ForEach(trans =>
        //                 {
        //                     res = 0.0m;
        //                     var debit = trans.WJXBFS2_Debit.Replace(",", ".");
        //                     if (decimal.TryParse(debit, out res))
        //                     {
        //                         debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                     }
        //                     else
        //                     {
        //                         debit = "-";
        //                     }

        //                     res = 0.0m;
        //                     var credit = trans.WJXBFS3_Credit.Replace(",", ".");
        //                     if (decimal.TryParse(credit, out res))
        //                     {
        //                         credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                     }
        //                     else
        //                     {
        //                         credit = "-";
        //                     }

        //                     res = 0.0m;
        //                     var balance = trans.WJXBFS4_Balance.Replace(",", ".");
        //                     if (decimal.TryParse(balance, out res))
        //                     {
        //                         balance = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                     }
        //                     else
        //                     {
        //                         balance = "-";
        //                     }
        //                     var tr = new StringBuilder();
        //                     tr.Append("<tr class='ht-20'>");
        //                     tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>");
        //                     tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
        //                     tr.Append("<td class='w-15 text-right pt-1'>" + debit + "</td>");
        //                     tr.Append("<td class='w-15 text-right pt-1'>" + credit + "</td>");
        //                     tr.Append("<td class='w-15 text-right pt-1'>" + balance + "</td>");
        //                     tr.Append("</tr>");
        //                     InvestmentTransactionRows.Append(tr.ToString());
        //                 });

        //                 InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
        //             }

        //             TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
        //             #endregion Transactions

        //             counter++;
        //         });
        //        TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : string.Empty);

        //        pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
        //    }
        //    else
        //    {
        //        pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //    }
        //}

        //private void BindExplanatoryNotesWidgetData(StringBuilder pageContent, List<DM_ExplanatoryNote> ExplanatoryNotes, Page page, PageWidget widget)
        //{
        //    try
        //    {
        //        if (ExplanatoryNotes != null && ExplanatoryNotes.Count > 0)
        //        {
        //            var notes = new StringBuilder();
        //            notes.Append(string.IsNullOrEmpty(ExplanatoryNotes[0].Note1) ? string.Empty : "<span> " + Convert.ToString(ExplanatoryNotes[0].Note1) + " </span> <br/>");
        //            notes.Append(string.IsNullOrEmpty(ExplanatoryNotes[0].Note2) ? string.Empty : "<span> " + Convert.ToString(ExplanatoryNotes[0].Note2) + " </span> <br/>");
        //            notes.Append(string.IsNullOrEmpty(ExplanatoryNotes[0].Note3) ? string.Empty : "<span> " + Convert.ToString(ExplanatoryNotes[0].Note3) + " </span> <br/>");
        //            notes.Append(string.IsNullOrEmpty(ExplanatoryNotes[0].Note4) ? string.Empty : "<span> " + Convert.ToString(ExplanatoryNotes[0].Note4) + " </span> <br/>");
        //            notes.Append(string.IsNullOrEmpty(ExplanatoryNotes[0].Note5) ? string.Empty : "<span> " + Convert.ToString(ExplanatoryNotes[0].Note5) + " </span> <br/>");
        //            pageContent.Replace("{{Notes_" + page.Identifier + "_" + widget.Identifier + "}}", notes.ToString());
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private void BindMarketingServiceWidgetData(StringBuilder pageContent, List<DM_MarketingMessage> Messages, Page page, PageWidget widget, int MarketingMessageCounter, DM_CustomerMaster customer)
        //{
        //    if (page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE)
        //    {
        //        var specialMsgTxtData = string.Empty;
        //        switch (customer.Segment.ToLower())
        //        {
        //            case "nbb":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_NBB_NEDBANK_SERVICE_MESSAGE;
        //                break;
        //            case "ncb":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_NCB_NEDBANK_SERVICE_MESSAGE;
        //                break;
        //            case "pml":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_PML_NEDBANK_SERVICE_MESSAGE;
        //                break;
        //            case "prb":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_PRB_NEDBANK_SERVICE_MESSAGE;
        //                break;
        //            case "sbs":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_SBS_NEDBANK_SERVICE_MESSAGE;
        //                break;
        //            case "wea":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_WEA_NEDBANK_SERVICE_MESSAGE;
        //                break;
        //        }
        //        pageContent.Replace("{{ServiceMessageHeader_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", specialMsgTxtData);
        //    }
        //    else
        //    {
        //        if (Messages != null && Messages.Count > 0)
        //        {
        //            var ServiceMessage = Messages.Count > MarketingMessageCounter ? Messages[MarketingMessageCounter] : null;
        //            if (ServiceMessage != null)
        //            {
        //                var messageTxt = ((!string.IsNullOrEmpty(ServiceMessage.Message1)) ? "<p>" + ServiceMessage.Message1 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message2)) ? "<p>" + ServiceMessage.Message2 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message3)) ? "<p>" + ServiceMessage.Message3 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message4)) ? "<p>" + ServiceMessage.Message4 + "</p>" : string.Empty) + ((!string.IsNullOrEmpty(ServiceMessage.Message5)) ? "<p>" + ServiceMessage.Message5 + "</p>" : string.Empty);

        //                pageContent.Replace("{{ServiceMessageHeader_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", ServiceMessage.Header).Replace("{{ServiceMessageText_" + page.Identifier + "_" + widget.Identifier + "_" + MarketingMessageCounter + "}}", messageTxt);
        //            }
        //        }
        //    }
        //}

        //private void BindPersonalLoanDetailWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoanList, Page page, PageWidget widget, string tenantCode)
        //{
        //    try
        //    {
        //        if (PersonalLoanList != null && PersonalLoanList.Count > 0)
        //        {
        //            var PersonalLoan = PersonalLoanList[0];
        //            if (PersonalLoan != null)
        //            {
        //                var res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.CreditAdvance, out res))
        //                {
        //                    pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.OutstandingBalance, out res))
        //                {
        //                    pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.AmountDue, out res))
        //                {
        //                    pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //                }

        //                pageContent.Replace("{{AccountNumber_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.InvestorId.ToString());
        //                pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //                pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

        //                res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.Arrears, out res))
        //                {
        //                    pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //                }

        //                pageContent.Replace("{{AnnualRate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.AnnualRate + "% pa");

        //                res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
        //                {
        //                    pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //                }

        //                pageContent.Replace("{{Terms_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.Term);
        //                pageContent.Replace("{{DueByDate_" + page.Identifier + "_" + widget.Identifier + "}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //            }
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{AccountNumber_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{ArrearsAmount_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{AnnualRate_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{MonthlyInstallment_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{Terms_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{DueByDate_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private void BindPersonalLoanTransactionWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoanList, Page page, PageWidget widget)
        //{
        //    try
        //    {
        //        if (PersonalLoanList.Count > 0)
        //        {
        //            var transactions = PersonalLoanList[0].LoanTransactions;
        //            if (transactions != null && transactions.Count > 0)
        //            {
        //                var LoanTransactionRows = new StringBuilder();
        //                var tr = new StringBuilder();
        //                transactions.ForEach(trans =>
        //                {
        //                    tr = new StringBuilder();
        //                    tr.Append("<tr class='ht-20'>");
        //                    tr.Append("<td class='w-13 text-center'> " + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
        //                    tr.Append("<td class='w-15 text-center'> " + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " </td>");
        //                    tr.Append("<td class='w-35'> " + (!string.IsNullOrEmpty(trans.Description) ? trans.Description : ModelConstant.PAYMENT_THANK_YOU_TRANSACTION_DESC) + " </td>");

        //                    var res = 0.0m;
        //                    if (decimal.TryParse(trans.Debit, out res))
        //                    {
        //                        tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
        //                    }
        //                    else
        //                    {
        //                        tr.Append("<td class='w-12 text-right'> - </td>");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(trans.Credit, out res))
        //                    {
        //                        tr.Append("<td class='w-12 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
        //                    }
        //                    else
        //                    {
        //                        tr.Append("<td class='w-12 text-right'> - </td>");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(trans.OutstandingCapital, out res))
        //                    {
        //                        tr.Append("<td class='w-13 text-right'> " + (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-") + " </td>");
        //                    }
        //                    else
        //                    {
        //                        tr.Append("<td class='w-13 text-right'> - </td>");
        //                    }
        //                    tr.Append("</tr>");
        //                    LoanTransactionRows.Append(tr.ToString());
        //                });
        //                pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", LoanTransactionRows.ToString());
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr><td class='text-center' colspan='6'>No record found</td></tr>");
        //            }
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{PersonalLoanTransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr><td class='text-center' colspan='6'>No record found</td></tr>");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private void BindPersonalLoanPaymentDueWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoanList, Page page, PageWidget widget)
        //{
        //    try
        //    {
        //        if (PersonalLoanList.Count > 0)
        //        {
        //            var plArrears = PersonalLoanList[0].LoanArrears;
        //            if (plArrears != null)
        //            {
        //                var res = 0.0m;
        //                if (decimal.TryParse(plArrears.Arrears_120, out res))
        //                {
        //                    pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(plArrears.Arrears_90, out res))
        //                {
        //                    pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(plArrears.Arrears_60, out res))
        //                {
        //                    pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(plArrears.Arrears_30, out res))
        //                {
        //                    pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(plArrears.Arrears_0, out res))
        //                {
        //                    pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                }
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //                pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //            }
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{After120Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //            pageContent.Replace("{{After90Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //            pageContent.Replace("{{After60Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //            pageContent.Replace("{{After30Days_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //            pageContent.Replace("{{Current_" + page.Identifier + "_" + widget.Identifier + "}}", "R0.00");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        //private void BindSpecialMessageWidgetData(StringBuilder pageContent, SpecialMessage SpecialMessage, Page page, PageWidget widget, DM_CustomerMaster customer)
        //{
        //    if (page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE || page.PageTypeName == HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE)
        //    {
        //        var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
        //        var specialMsgTxtData = string.Empty;
        //        switch (customer.Segment.ToLower())
        //        {
        //            case "nbb":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_NBB_SPECIAL_MESSAGE;
        //                break;
        //            case "ncb":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_NCB_SPECIAL_MESSAGE;
        //                break;
        //            case "pml":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_PML_SPECIAL_MESSAGE;
        //                break;
        //            case "prb":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_PRB_SPECIAL_MESSAGE;
        //                break;
        //            case "sbs":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_SBS_SPECIAL_MESSAGE;
        //                break;
        //            case "wea":
        //                specialMsgTxtData = HtmlConstants.HOME_LOAN_WEA_SPECIAL_MESSAGE;
        //                break;
        //        }
        //        htmlWidget.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
        //        htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
        //        pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
        //    }
        //    else
        //    {
        //        if (SpecialMessage != null)
        //        {
        //            if (!string.IsNullOrEmpty(SpecialMessage.Header) || !string.IsNullOrEmpty(SpecialMessage.Message1) || !string.IsNullOrEmpty(SpecialMessage.Message2))
        //            {
        //                var htmlWidget = new StringBuilder(HtmlConstants.SPECIAL_MESSAGE_HTML);
        //                var specialMsgTxtData = (!string.IsNullOrEmpty(SpecialMessage.Header) ? "<div class='SpecialMessageHeader'> " + SpecialMessage.Header + " </div>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message1) ? "<p> " + SpecialMessage.Message1 + " </p>" : string.Empty) + (!string.IsNullOrEmpty(SpecialMessage.Message2) ? "<p> " + SpecialMessage.Message2 + " </p>" : string.Empty);
        //                htmlWidget.Replace("{{SpecialMessageTextData}}", specialMsgTxtData);
        //                htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
        //                pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            }
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{SpecialMessageTextDataDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //}

        //private void BindPersonalLoanInsuranceMessageWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoanList, Page page, PageWidget widget)
        //{
        //    if (PersonalLoanList != null && PersonalLoanList.Count() > 0)
        //    {
        //        var InsuranceMsg = PersonalLoanList[0].Messages;
        //        if (InsuranceMsg.Count > 0 && !string.IsNullOrWhiteSpace(InsuranceMsg[2]))
        //        {
        //            var htmlWidget = new StringBuilder(HtmlConstants.PERSONAL_LOAN_INSURANCE_MESSAGE_HTML);
        //            StringBuilder InsuranceMsgTxtData = new StringBuilder();
        //            foreach (var item in InsuranceMsg)
        //            {
        //                InsuranceMsgTxtData.Append(item);
        //            }
        //            htmlWidget.Replace("{{InsuranceMessages}}", InsuranceMsgTxtData.ToString());
        //            htmlWidget = htmlWidget.Replace("{{WidgetId}}", "PageWidgetId_" + widget.Identifier + "_Counter" + (new Random().Next(100)).ToString());
        //            pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", htmlWidget.ToString());
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //    else
        //    {
        //        pageContent.Replace("{{PersonalLoanInsuranceMessagesDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //    }
        //}

        //private void BindPersonalLoanTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_PersonalLoanMaster> PersonalLoans, Page page, PageWidget widget)
        //{
        //    try
        //    {
        //        var TotalLoanAmt = 0.0m;
        //        var TotalOutstandingAmt = 0.0m;
        //        var TotalLoanDueAmt = 0.0m;

        //        if (PersonalLoans != null && PersonalLoans.Count > 0)
        //        {
        //            var res = 0.0m;
        //            try
        //            {
        //                TotalLoanAmt = PersonalLoans.Select(it => decimal.TryParse(it.CreditAdvance, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalLoanAmt = 0.0m;
        //            }

        //            res = 0.0m;
        //            try
        //            {
        //                TotalOutstandingAmt = PersonalLoans.Select(it => decimal.TryParse(it.OutstandingBalance, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalOutstandingAmt = 0.0m;
        //            }

        //            res = 0.0m;
        //            try
        //            {
        //                TotalLoanDueAmt = PersonalLoans.Select(it => decimal.TryParse(it.AmountDue, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalLoanDueAmt = 0.0m;
        //            }
        //        }

        //        pageContent.Replace("{{TotalLoanAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
        //        pageContent.Replace("{{OutstandingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
        //        pageContent.Replace("{{DueAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanDueAmt));
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindPersonalLoanAccountsBreakdownWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_PersonalLoanMaster> PersonalLoans, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation)
        //{
        //    try
        //    {
        //        if (PersonalLoans != null && PersonalLoans.Count > 0)
        //        {
        //            //create tab-content div if accounts is greater than 1, otherwise create simple div
        //            var TabContentHtml = new StringBuilder();
        //            var counter = 0;
        //            TabContentHtml.Append((PersonalLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
        //            PersonalLoans.ForEach(PersonalLoan =>
        //            {
        //                var AccountNumber = PersonalLoan.InvestorId.ToString();
        //                string lastFourDigisOfAccountNumber = AccountNumber.Length > 4 ? AccountNumber.Substring(Math.Max(0, AccountNumber.Length - 4)) : AccountNumber;
        //                TabContentHtml.Append("<div id='PersonalLoan-" + lastFourDigisOfAccountNumber + counter + "'>");

        //                #region Loan Details
        //                var LoanDetailHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_DETAIL);
        //                LoanDetailHtml.Replace("{{AccountNumber}}", PersonalLoan.InvestorId.ToString());
        //                LoanDetailHtml.Replace("{{StatementDate}}", PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //                LoanDetailHtml.Replace("{{StatementPeriod}}", PersonalLoan.FromDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " - " + PersonalLoan.ToDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

        //                var res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.Arrears, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{ArrearsAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{ArrearsAmount}}", "R0.00");
        //                }

        //                LoanDetailHtml.Replace("{{AnnualRate}}", PersonalLoan.AnnualRate + "% pa");

        //                res = 0.0m;
        //                if (decimal.TryParse(PersonalLoan.MonthlyInstallment, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{MonthlyInstallment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{MonthlyInstallment}}", "R0.00");
        //                }

        //                LoanDetailHtml.Replace("{{Terms}}", PersonalLoan.Term);
        //                LoanDetailHtml.Replace("{{DueByDate}}", PersonalLoan.DueDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //                TabContentHtml.Append(LoanDetailHtml.ToString());
        //                #endregion Loan Details

        //                #region Loan Transaction
        //                var LoanTransactionHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_ACCOUNT_TRANSACTION_DETAIL_SMT);
        //                StringBuilder tableHTML = new StringBuilder();
        //                if (PersonalLoan.LoanTransactions != null && PersonalLoan.LoanTransactions.Count > 0)
        //                {
        //                    PersonalLoan.LoanTransactions.ForEach(trans =>
        //                    {
        //                        res = 0.0m;
        //                        if (decimal.TryParse(trans.Debit, out res))
        //                        {
        //                            trans.Debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                        }
        //                        else
        //                        {
        //                            trans.Debit = "-";
        //                        }

        //                        if (decimal.TryParse(trans.Credit, out res))
        //                        {
        //                            trans.Credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                        }
        //                        else
        //                        {
        //                            trans.Credit = "-";
        //                        }

        //                        if (decimal.TryParse(trans.OutstandingCapital, out res))
        //                        {
        //                            trans.OutstandingCapital = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                        }
        //                        else
        //                        {
        //                            trans.OutstandingCapital = "-";
        //                        }
        //                        tableHTML.Append("<tr class='ht-20'><td class='w-13 text-center'>" + trans.PostingDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy)
        //                            + "</td><td class='w-13 text-center'>" + trans.EffectiveDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td><td class='w-35'>"
        //                            + trans.Description + "</td><td class='w-12 text-right'>" + trans.Debit
        //                            + "</td><td class='w-12 text-right'>" + trans.Credit + "</td><td class='w-15 text-right'>"
        //                            + trans.OutstandingCapital + "</td></tr>");
        //                    });

        //                    LoanTransactionHtml.Replace("PersonalLoanTransactionTable", "PersonalLoanTransactionTable_" + PersonalLoan.InvestorId + "_" + page.Identifier);
        //                    LoanTransactionHtml.Replace("PersonalLoanTransactionTablePagination", "PersonalLoanTransactionTablePagination_" + PersonalLoan.InvestorId + "_" + page.Identifier);
        //                }

        //                LoanTransactionHtml.Replace("{{PersonalLoanTransactionsTableBody}}", tableHTML.ToString());
        //                TabContentHtml.Append(LoanTransactionHtml.ToString());
        //                #endregion Loan Transaction

        //                #region Loan arrear
        //                if (PersonalLoan.LoanArrears != null)
        //                {
        //                    var plArrears = PersonalLoan.LoanArrears;
        //                    var paddingClass = PersonalLoan.LoanTransactions.Count > 10 ? "pb-2 pt-5" : "py-2";
        //                    var LoanArrearHtml = new StringBuilder(HtmlConstants.PERSONAL_LOAN_PAYMENT_DUE_DETAIL).Replace("{{PaddingClass}}", paddingClass);
        //                    bool is120 = false, is90 = false, is60 = false, is30 = false, isCurrent = false;

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.Arrears_120, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                        is120 = res > 0;
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.Arrears_90, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                        is90 = res > 0;
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.Arrears_60, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                        is60 = res > 0;
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.Arrears_30, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                        is30 = res > 0;
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.Arrears_0, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{Current}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));

        //                        isCurrent = res > 0;
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{Current}}", "R0.00");
        //                    }

        //                    if (is30 || is60 || is90 || is120)
        //                    {
        //                        LoanArrearHtml.Append("<p style='font-family:Mark Pro Regular; font-size: 9pt;'>Your Nedbank personal loan is in arrears. According to your loan agreement with Nedbank, you are required to make regular monthly payments. Failure to do so results in extra interest being charged, and your arrear status and payment history being reported to the credit bureaus. This may have a negative impact on your ability to obtain credit. </p>");
        //                        LoanArrearHtml.Append("<p style='font-family:Mark Pro Regular; font-size: 9pt;'>Please settle the arrears by paying at any Nedbank Branch or by arranging a debit order through the Nedbank Contact Centre. If you cannot pay, please call 0860 103 117 urgently to discuss the options available to you.</p>");
        //                    }

        //                    if (decimal.TryParse(plArrears.Arrears_120, out res))
        //                    {
        //                        if (res > 0)
        //                        {
        //                            LoanArrearHtml.Append("<div style=\"font-size: 14pt;font-family: 'Mark Pro Bold';color: rgb(0, 91, 0) !important;\">Insurance</div>");
        //                            LoanArrearHtml.Append("<p style='font-family:Mark Pro Regular; font-size: 9pt;'>We would like to remind you of the credit life insurance benefits available to you through your Nedbank Insurance policy. When you pass away, Nedbank Insurance will cover your outstanding loan amount. If you are permanently employed, you will also enjoy cover for comprehensive disability and loss of income. The disability benefit will cover your monthly instalments if you cannot earn your usual income due to illness or bodily injury.</p>");
        //                            LoanArrearHtml.Append("<p style='font-family:Mark Pro Regular; font-size: 9pt;'>The loss-of-income benefit includes unemployment, retrenchment or any other event where you cannot earn an income. This benefit will cover your monthly instalments for up to 12 months. The disability and loss-of-income benefits end when you turn 65 years old. If you are a pensioner, self-employed, employed in the informal sector, employed by a family-owned business or receiving a social grant, you will be covered for the death benefit only.</p>");
        //                            LoanArrearHtml.Append("<p style='font-family:Mark Pro Regular; font-size: 9pt;'>Your policy document explains the provisions of your benefits, the claim events you are covered for and how the claims process works. If you need information about your policy or want to claim, please call us on 0860 333 111. Nedgroup Life Assurance Company Ltd is a licensed insurer FSP40915</p>");
        //                        }
        //                    }

        //                    TabContentHtml.Append(LoanArrearHtml.ToString());
        //                }
        //                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
        //                #endregion Loan arrear

        //                counter++;
        //            });

        //            TabContentHtml.Append((PersonalLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
        //            pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindHomeLoanTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget, DM_CustomerMaster customer)
        //{
        //    try
        //    {
        //        var TotalLoanAmt = 0.0m;
        //        var TotalOutstandingAmt = 0.0m;
        //        string instalmentLabel = string.Empty;
        //        if (HomeLoans != null && HomeLoans.Count > 0)
        //        {
        //            var res = 0.0m;
        //            try
        //            {
        //                TotalLoanAmt = HomeLoans.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalLoanAmt = 0.0m;
        //            }

        //            res = 0.0m;
        //            try
        //            {
        //                TotalOutstandingAmt = HomeLoans.Select(it => decimal.TryParse(it.Balance, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalOutstandingAmt = 0.0m;
        //            }

        //            var segmentType = HomeLoans?[0].SegmentType;

        //            switch (segmentType.ToLower())
        //            {
        //                case HtmlConstants.MONTHLY_SEGMENT_FREQUENCY:
        //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
        //                    break;
        //                case HtmlConstants.QUARTERLY_SEGMENT_FREQUENCY:
        //                    instalmentLabel = HtmlConstants.QUARTERLY_INSTALMENT_LABEL;
        //                    break;
        //                case HtmlConstants.ANNUAL_SEGMENT_FREQUENCY:
        //                    instalmentLabel = HtmlConstants.ANNUAL_INSTALMENT_LABEL;
        //                    break;
        //                default:
        //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
        //                    break;
        //            }
        //        }

        //        pageContent.Replace("{{InstalmentType_" + page.Identifier + "_" + widget.Identifier + "}}", instalmentLabel);
        //        pageContent.Replace("{{TotalHomeLoansAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
        //        pageContent.Replace("{{TotalHomeLoansBalanceOutstanding_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindHomeLoanWealthTotalAmountDetailWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget, DM_CustomerMaster customer)
        //{
        //    try
        //    {
        //        var TotalLoanAmt = 0.0m;
        //        var TotalOutstandingAmt = 0.0m;
        //        string instalmentLabel = string.Empty;
        //        if (HomeLoans != null && HomeLoans.Count > 0)
        //        {
        //            var res = 0.0m;
        //            try
        //            {
        //                TotalLoanAmt = HomeLoans.Select(it => decimal.TryParse(it.LoanAmount, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalLoanAmt = 0.0m;
        //            }

        //            res = 0.0m;
        //            try
        //            {
        //                TotalOutstandingAmt = HomeLoans.Select(it => decimal.TryParse(it.Balance, out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch
        //            {
        //                TotalOutstandingAmt = 0.0m;
        //            }

        //            var segmentType = HomeLoans?[0].SegmentType;

        //            switch (segmentType.ToLower())
        //            {
        //                case HtmlConstants.MONTHLY_SEGMENT_FREQUENCY:
        //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
        //                    break;
        //                case HtmlConstants.QUARTERLY_SEGMENT_FREQUENCY:
        //                    instalmentLabel = HtmlConstants.QUARTERLY_INSTALMENT_LABEL;
        //                    break;
        //                case HtmlConstants.ANNUAL_SEGMENT_FREQUENCY:
        //                    instalmentLabel = HtmlConstants.ANNUAL_INSTALMENT_LABEL;
        //                    break;
        //                default:
        //                    instalmentLabel = HtmlConstants.MONTHLY_INSTALMENT_LABEL;
        //                    break;
        //            }
        //        }

        //        pageContent.Replace("{{InstalmentType_" + page.Identifier + "_" + widget.Identifier + "}}", instalmentLabel);
        //        pageContent.Replace("{{TotalHomeLoansAmount_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalLoanAmt));
        //        pageContent.Replace("{{TotalHomeLoansBalanceOutstanding_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalOutstandingAmt));
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindHomeLoanAccountsBreakdownWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget, DM_CustomerMaster customer)
        //{
        //    try
        //    {
        //        if (HomeLoans != null && HomeLoans.Count > 0)
        //        {
        //            //create tab-content div if accounts is greater than 1, otherwise create simple div
        //            var TabContentHtml = new StringBuilder();
        //            var counter = 0;
        //            TabContentHtml.Append((HomeLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
        //            HomeLoans.ForEach(HomeLoan =>
        //            {
        //                var accNo = HomeLoan.BondNo.ToString();
        //                string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

        //                TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "' >");

        //                #region Loan Details
        //                var LoanDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_ACCOUNT_DETAIL_DIV_HTML);
        //                LoanDetailHtml.Replace("{{BondNumber}}", accNo);
        //                LoanDetailHtml.Replace("{{RegistrationDate}}", HomeLoan.RegisteredDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

        //                var secDesc1 = string.Empty;
        //                var secDesc2 = string.Empty;
        //                var secDesc3 = string.Empty;

        //                secDesc1 = HomeLoan.SecDescription1;
        //                secDesc2 = HomeLoan.SecDescription2;
        //                secDesc3 = HomeLoan.SecDescription3;

        //                LoanDetailHtml.Replace("{{SecDescription1}}", secDesc1);
        //                LoanDetailHtml.Replace("{{SecDescription2}}", secDesc2);
        //                LoanDetailHtml.Replace("{{SecDescription3}}", secDesc3);

        //                var res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.IntialDue, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{Instalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{Instalment}}", "R0.00");
        //                }

        //                LoanDetailHtml.Replace("{{InterestRate}}", HomeLoan.ChargeRate + "% pa");

        //                res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.ArrearStatus, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{Arrears}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{Arrears}}", "R0.00");
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.RegisteredAmount, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", "R0.00");
        //                }

        //                LoanDetailHtml.Replace("{{LoanTerms}}", HomeLoan.LoanTerm + " months");
        //                TabContentHtml.Append(LoanDetailHtml.ToString());

        //                #endregion  Loan Details

        //                #region Loan Transaction table
        //                var LoanTransactionHtml = new StringBuilder(HtmlConstants.HOME_LOAN_TRANSACTION_DETAIL_DIV_HTML);
        //                LoanTransactionHtml.Replace("HomeLoanTransactionTable", "HomeLoanTransactionTable_" + HomeLoan.InvestorId + "_" + page.Identifier);
        //                StringBuilder tableHTML = new StringBuilder();
        //                string debit, credit, remainingBalance = string.Empty;
        //                if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count > 0)
        //                {
        //                    HomeLoan.LoanTransactions.ForEach(trans =>
        //                    {
        //                        res = 0.0m;
        //                        if (decimal.TryParse(trans.Debit, out res))
        //                        {
        //                            trans.Debit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
        //                            debit = trans.Debit.Replace("R", "").Replace("$", "");
        //                        }
        //                        else
        //                        {
        //                            debit = trans.Debit;
        //                        }

        //                        if (decimal.TryParse(trans.Credit, out res))
        //                        {
        //                            trans.Credit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
        //                            credit = trans.Credit.Replace("R", "").Replace("$", "");
        //                        }
        //                        else
        //                        {
        //                            credit = trans.Credit;
        //                        }

        //                        if (decimal.TryParse(trans.RunningBalance, out res))
        //                        {
        //                            trans.RunningBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
        //                            remainingBalance = trans.RunningBalance.Replace("R", "").Replace("$", "");
        //                        }
        //                        else
        //                        {
        //                            remainingBalance = trans.RunningBalance;
        //                        }

        //                        tableHTML.Append("<tr class='ht-20'><td class='w-13 text-center'>" + trans.Posting_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td><td class='w-13 text-center'>" + trans.Effective_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td><td class='w-35'>" + trans.Description + "</td><td class='w-12 text-right'>" + debit + "</td><td class='w-12 text-right'>" + credit + "</td><td class='w-15 text-right'>" + remainingBalance + "</td></tr>");
        //                    });
        //                }
        //                LoanTransactionHtml.Replace("{{HomeLoanTransactionRow}}", tableHTML.ToString());
        //                TabContentHtml.Append(LoanTransactionHtml.ToString());
        //                #endregion Loan Transaction table

        //                #region Loan arrears
        //                var paddingClass = HomeLoan.LoanTransactions.Count > 10 ? "pb-2 pt-5" : "py-2";
        //                var LoanArrearHtml = new StringBuilder(HtmlConstants.HOME_LOAN_STATEMENT_OVERVIEW_AND_PAYMENT_DUE_DIV_HTML).Replace("{{PaddingClass}}", paddingClass);
        //                string statementDate = string.Empty;

        //                if (HomeLoan?.StatementDate != null)
        //                {
        //                    if (customer.Language == "AFR" && page.PageTypeName != HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE && page.PageTypeName != HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE)
        //                    {
        //                        statementDate = Convert.ToDateTime(HomeLoan?.StatementDate).ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy);
        //                    }
        //                    else
        //                    {
        //                        statementDate = Convert.ToDateTime(HomeLoan?.StatementDate).ToString(ModelConstant.DATE_FORMAT_yyyy_MM_dd);
        //                    }
        //                }

        //                LoanArrearHtml.Replace("{{StatementDate}}", statementDate);
        //                res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.Balance, out res))
        //                {
        //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", "R0.00");
        //                }

        //                if (HomeLoan.LoanArrear != null)
        //                {
        //                    var plArrears = HomeLoan.LoanArrear;
        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_120, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_90, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_60, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_30, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.CurrentDue, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
        //                    }
        //                }
        //                else
        //                {
        //                    LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
        //                }
        //                TabContentHtml.Append(LoanArrearHtml.ToString());

        //                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
        //                counter++;
        //            });

        //            TabContentHtml.Append((HomeLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
        //            pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindHomeLoanWealthAccountsBreakdownWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_HomeLoanMaster> HomeLoans, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation)
        //{
        //    try
        //    {
        //        if (HomeLoans != null && HomeLoans.Count > 0)
        //        {
        //            //create tab-content div if accounts is greater than 1, otherwise create simple div
        //            var TabContentHtml = new StringBuilder();
        //            var counter = 0;
        //            TabContentHtml.Append((HomeLoans.Count > 1) ? "<div class='tab-content'>" : string.Empty);
        //            HomeLoans.ForEach(HomeLoan =>
        //            {
        //                var accNo = HomeLoan.InvestorId.ToString();
        //                string lastFourDigisOfAccountNumber = accNo.Length > 4 ? accNo.Substring(Math.Max(0, accNo.Length - 4)) : accNo;

        //                TabContentHtml.Append("<div id='HomeLoan-" + lastFourDigisOfAccountNumber + "' >");

        //                #region Loan Details
        //                var LoanDetailHtml = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_ACCOUNT_DETAIL_DIV_HTML);
        //                LoanDetailHtml.Replace("{{BondNumber}}", accNo);
        //                LoanDetailHtml.Replace("{{RegistrationDate}}", HomeLoan.RegisteredDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));

        //                var secDesc1 = string.Empty;
        //                var secDesc2 = string.Empty;
        //                var secDesc3 = string.Empty;

        //                secDesc1 = HomeLoan.SecDescription1;
        //                secDesc2 = HomeLoan.SecDescription2;
        //                secDesc3 = HomeLoan.SecDescription3;

        //                LoanDetailHtml.Replace("{{SecDescription1}}", secDesc1);
        //                LoanDetailHtml.Replace("{{SecDescription2}}", secDesc2);
        //                LoanDetailHtml.Replace("{{SecDescription3}}", secDesc3);

        //                var res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.IntialDue, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{Instalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{Instalment}}", "R0.00");
        //                }

        //                LoanDetailHtml.Replace("{{InterestRate}}", HomeLoan.ChargeRate + "% pa");

        //                res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.ArrearStatus, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{Arrears}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{Arrears}}", "R0.00");
        //                }

        //                res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.RegisteredAmount, out res))
        //                {
        //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanDetailHtml.Replace("{{RegisteredAmount}}", "R0.00");
        //                }

        //                LoanDetailHtml.Replace("{{LoanTerms}}", HomeLoan.LoanTerm + " months");
        //                TabContentHtml.Append(LoanDetailHtml.ToString());

        //                #endregion  Loan Details

        //                #region Loan Transaction table
        //                var LoanTransactionHtml = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_TRANSACTION_DETAIL_DIV_HTML);
        //                LoanTransactionHtml.Replace("HomeLoanTransactionTable", "HomeLoanTransactionTable_" + HomeLoan.InvestorId + "_" + page.Identifier);
        //                StringBuilder tableHTML = new StringBuilder();
        //                string debit, credit, runningBalance = string.Empty;
        //                if (HomeLoan.LoanTransactions != null && HomeLoan.LoanTransactions.Count > 0)
        //                {
        //                    HomeLoan.LoanTransactions.ForEach(trans =>
        //                    {
        //                        res = 0.0m;
        //                        if (decimal.TryParse(trans.Debit, out res))
        //                        {
        //                            trans.Debit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
        //                            debit = trans.Debit.Replace("R", "").Replace("$", "");
        //                        }
        //                        else
        //                        {
        //                            debit = trans.Debit;
        //                        }

        //                        if (decimal.TryParse(trans.Credit, out res))
        //                        {
        //                            trans.Credit = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
        //                            credit = trans.Credit.Replace("R", "").Replace("$", "");
        //                        }
        //                        else
        //                        {
        //                            credit = trans.Credit;
        //                        }

        //                        if (decimal.TryParse(trans.RunningBalance, out res))
        //                        {
        //                            trans.RunningBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res);
        //                            runningBalance = trans.RunningBalance.Replace("R", "").Replace("$", "");
        //                        }
        //                        else
        //                        {
        //                            runningBalance = trans.RunningBalance;
        //                        }

        //                        tableHTML.Append("<tr class='ht-20'><td class='w-13 text-center'>" + trans.Posting_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td><td class='w-13 text-center'>" + trans.Effective_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td><td class='w-35'>" + trans.Description + "</td><td class='w-12 text-right'>" + debit + "</td><td class='w-12 text-right'>" + credit + "</td><td class='w-15 text-right'>" + runningBalance + "</td></tr>");
        //                    });
        //                }
        //                LoanTransactionHtml.Replace("{{HomeLoanTransactionRow}}", tableHTML.ToString());
        //                TabContentHtml.Append(LoanTransactionHtml.ToString());
        //                #endregion Loan Transaction table

        //                #region Loan arrears
        //                var paddingClass = HomeLoan.LoanTransactions.Count > 10 ? "pb-2 pt-5" : "py-2";
        //                var LoanArrearHtml = new StringBuilder(HtmlConstants.HOME_LOAN_WEALTH_STATEMENT_OVERVIEW_AND_PAYMENT_DUE_DIV_HTML).Replace("{{PaddingClass}}", paddingClass);
        //                LoanArrearHtml.Replace("{{StatementDate}}", (HomeLoan?.StatementDate != null ? Convert.ToDateTime(HomeLoan?.StatementDate).ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy) : ""));
        //                res = 0.0m;
        //                if (decimal.TryParse(HomeLoan.Balance, out res))
        //                {
        //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                }
        //                else
        //                {
        //                    LoanArrearHtml.Replace("{{BalanceOutstanding}}", "R0.00");
        //                }

        //                if (HomeLoan.LoanArrear != null)
        //                {
        //                    var plArrears = HomeLoan.LoanArrear;
        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_120, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After120Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_90, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After90Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_60, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After60Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.ARREARS_30, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{After30Days}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (decimal.TryParse(plArrears.CurrentDue, out res))
        //                    {
        //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
        //                    }
        //                }
        //                else
        //                {
        //                    LoanArrearHtml.Replace("{{After120Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{After90Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{After60Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{After30Days}}", "R0.00");
        //                    LoanArrearHtml.Replace("{{CurrentPaymentDue}}", "R0.00");
        //                }
        //                TabContentHtml.Append(LoanArrearHtml.ToString());

        //                #endregion  Loan arrears

        //                TabContentHtml.Append(HtmlConstants.END_DIV_TAG);
        //                counter++;
        //            });

        //            TabContentHtml.Append((HomeLoans.Count > 1) ? HtmlConstants.END_DIV_TAG : string.Empty);
        //            pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //            pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //private void BindHomeLoanSummaryTaxPurposeWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, PageWidget widget)
        //{
        //    if (HomeLoans != null && HomeLoans.Count > 0)
        //    {
        //        foreach (var item in HomeLoans)
        //        {
        //            var homeLoanSummary = item.LoanSummary;
        //            var res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Interest) && decimal.TryParse(homeLoanSummary.Annual_Interest, out res))
        //            {
        //                pageContent.Replace("{{Interest_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Interest_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Insurance) && decimal.TryParse(homeLoanSummary.Annual_Insurance, out res))
        //            {
        //                pageContent.Replace("{{Insurance_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Insurance_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Service_Fee) && decimal.TryParse(homeLoanSummary.Annual_Service_Fee, out res))
        //            {
        //                pageContent.Replace("{{Servicefee_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Servicefee_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Legal_Costs) && decimal.TryParse(homeLoanSummary.Annual_Legal_Costs, out res))
        //            {
        //                pageContent.Replace("{{Legalcosts_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Legalcosts_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Total_Recvd) && decimal.TryParse(homeLoanSummary.Annual_Total_Recvd, out res))
        //            {
        //                pageContent.Replace("{{AmountReceived_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{AmountReceived_" + widget.Identifier + "}}", "R0.00");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        pageContent.Replace("{{Interest_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{Insurance_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{Servicefee_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{Legalcosts_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{AmountReceived_" + widget.Identifier + "}}", "R0.00");
        //    }
        //}

        //private void BindHomeLoanWealthSummaryTaxPurposeWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, PageWidget widget)
        //{
        //    if (HomeLoans != null && HomeLoans.Count > 0)
        //    {
        //        foreach (var item in HomeLoans)
        //        {
        //            var homeLoanSummary = item.LoanSummary;
        //            var res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Interest) && decimal.TryParse(homeLoanSummary.Annual_Interest, out res))
        //            {
        //                pageContent.Replace("{{Interest_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Interest_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Insurance) && decimal.TryParse(homeLoanSummary.Annual_Insurance, out res))
        //            {
        //                pageContent.Replace("{{Insurance_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Insurance_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Service_Fee) && decimal.TryParse(homeLoanSummary.Annual_Service_Fee, out res))
        //            {
        //                pageContent.Replace("{{Servicefee_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Servicefee_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Legal_Costs) && decimal.TryParse(homeLoanSummary.Annual_Legal_Costs, out res))
        //            {
        //                pageContent.Replace("{{Legalcosts_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{Legalcosts_" + widget.Identifier + "}}", "R0.00");
        //            }

        //            res = 0.0m;
        //            if (!string.IsNullOrEmpty(homeLoanSummary.Annual_Total_Recvd) && decimal.TryParse(homeLoanSummary.Annual_Total_Recvd, out res))
        //            {
        //                pageContent.Replace("{{AmountReceived_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //            }
        //            else
        //            {
        //                pageContent.Replace("{{AmountReceived_" + widget.Identifier + "}}", "R0.00");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        pageContent.Replace("{{Interest_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{Insurance_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{Servicefee_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{Legalcosts_" + widget.Identifier + "}}", "R0.00");
        //        pageContent.Replace("{{AmountReceived_" + widget.Identifier + "}}", "R0.00");
        //    }
        //}

        //private bool BindHomeLoanInstalmentWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages)
        //{
        //    try
        //    {
        //        if (HomeLoans.Count > 0)
        //        {
        //            HomeLoans.ForEach(HomeLoan =>
        //            {
        //                var HomeLoanSummary = HomeLoan.LoanSummary;
        //                var res = 0.0m;
        //                decimal.TryParse(HomeLoanSummary.Total_Instalment, out res);
        //                if (HomeLoanSummary != null && !string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && HomeLoanSummary.Total_Instalment != "0" && HomeLoanSummary.Total_Instalment != "0.00" && res > 0)
        //                {
        //                    #region Installment details div
        //                    var htmlWidget = HtmlConstants.HOME_LOAN_INSTALMENT_DETAILS_HTML;
        //                    StringBuilder htmlForWidget = new StringBuilder();
        //                    htmlForWidget.Append(htmlWidget);
        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Basic_Instalment) && decimal.TryParse(HomeLoanSummary.Basic_Instalment, out res))
        //                    {
        //                        htmlForWidget.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{BasicInstalment}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Houseowner_Ins) && decimal.TryParse(HomeLoanSummary.Houseowner_Ins, out res))
        //                    {
        //                        htmlForWidget.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{HouseownerInsurance}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Loan_Protection) && decimal.TryParse(HomeLoanSummary.Loan_Protection, out res))
        //                    {
        //                        htmlForWidget.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{LoanProtectionAssurance}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Recovery_Fee_Debit) && decimal.TryParse(HomeLoanSummary.Recovery_Fee_Debit, out res))
        //                    {
        //                        htmlForWidget.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Capital_Redemption) && decimal.TryParse(HomeLoanSummary.Capital_Redemption, out res))
        //                    {
        //                        htmlForWidget.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{CapitalRedemption}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Service_Fee) && decimal.TryParse(HomeLoanSummary.Service_Fee, out res))
        //                    {
        //                        htmlForWidget.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{ServiceFee}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && decimal.TryParse(HomeLoanSummary.Total_Instalment, out res))
        //                    {
        //                        htmlForWidget.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{TotalInstalment}}", "R0.00");
        //                    }

        //                    htmlForWidget.Replace("{{InstalmentDate}}", (HomeLoan.EffectiveDate == null ? "" : "(effective from " + Convert.ToDateTime(HomeLoan.EffectiveDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + ")"));
        //                    pageContent.Replace("{{Home_Loan_Instalment_Details_" + widget.Identifier + "}}", htmlForWidget.ToString());

        //                    #endregion
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{Home_Loan_Instalment_Details_" + widget.Identifier + "}}", string.Empty);
        //                }
        //            });
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{Home_Loan_Instalment_Details_" + widget.Identifier + "}}", string.Empty);
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Append("<li>Error occurred while configuring StaticHtml widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
        //        return true;
        //    }
        //}

        //private bool BindHomeLoanWealthBranchDetailsData(StringBuilder pageContent, Page page, PageWidget widget, List<DM_HomeLoanMaster> HomeLoanMaster, StringBuilder ErrorMessages)
        //{
        //    try
        //    {
        //        StringBuilder htmlString = new StringBuilder();
        //        htmlString.Append(HtmlConstants.BANK_DETAILS);
        //        if (HomeLoanMaster != null && HomeLoanMaster.Count > 0)
        //        {
        //            htmlString.Replace("{{TodayDate}}", HomeLoanMaster.FirstOrDefault().StatementDate != null ? Convert.ToDateTime(HomeLoanMaster.FirstOrDefault().StatementDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : "");
        //        }
        //        else
        //        {
        //            htmlString.Replace("{{TodayDate}}", string.Empty);
        //        }
        //        pageContent.Replace("{{BranchDetails_" + page.Identifier + "_" + widget.Identifier + "}}", htmlString.ToString());
        //        pageContent.Replace("{{ContactCenter_" + page.Identifier + "_" + widget.Identifier + "}}", HtmlConstants.WEA_BANKING);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Append("<li>Error occurred while configuring StaticHtml widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
        //        return true;
        //    }
        //}

        //private bool BindHomeLoanWealthInstalmentWidgetData(StringBuilder pageContent, List<DM_HomeLoanMaster> HomeLoans, DM_CustomerMaster customer, Page page, PageWidget widget, Statement statement, StringBuilder ErrorMessages)
        //{
        //    try
        //    {
        //        if (HomeLoans.Count > 0)
        //        {
        //            HomeLoans.ForEach(HomeLoan =>
        //            {
        //                var HomeLoanSummary = HomeLoan.LoanSummary;
        //                if (HomeLoanSummary != null && !string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && HomeLoanSummary.Total_Instalment != "0" && HomeLoanSummary.Total_Instalment != "0.00")
        //                {
        //                    #region Installment details div
        //                    var htmlWidget = HtmlConstants.HOME_LOAN_WEALTH_INSTALMENT_DETAILS_HTML;
        //                    StringBuilder htmlForWidget = new StringBuilder();
        //                    htmlForWidget.Append(htmlWidget);
        //                    var res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Basic_Instalment) && decimal.TryParse(HomeLoanSummary.Basic_Instalment, out res))
        //                    {
        //                        htmlForWidget.Replace("{{BasicInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{BasicInstalment}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Houseowner_Ins) && decimal.TryParse(HomeLoanSummary.Houseowner_Ins, out res))
        //                    {
        //                        htmlForWidget.Replace("{{HouseownerInsurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{HouseownerInsurance}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Loan_Protection) && decimal.TryParse(HomeLoanSummary.Loan_Protection, out res))
        //                    {
        //                        htmlForWidget.Replace("{{LoanProtectionAssurance}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{LoanProtectionAssurance}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Recovery_Fee_Debit) && decimal.TryParse(HomeLoanSummary.Recovery_Fee_Debit, out res))
        //                    {
        //                        htmlForWidget.Replace("{{RecoveryOfFeeDebits}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{RecoveryOfFeeDebits}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Capital_Redemption) && decimal.TryParse(HomeLoanSummary.Capital_Redemption, out res))
        //                    {
        //                        htmlForWidget.Replace("{{CapitalRedemption}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{CapitalRedemption}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Service_Fee) && decimal.TryParse(HomeLoanSummary.Service_Fee, out res))
        //                    {
        //                        htmlForWidget.Replace("{{ServiceFee}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{ServiceFee}}", "R0.00");
        //                    }

        //                    res = 0.0m;
        //                    if (!string.IsNullOrEmpty(HomeLoanSummary.Total_Instalment) && decimal.TryParse(HomeLoanSummary.Total_Instalment, out res))
        //                    {
        //                        htmlForWidget.Replace("{{TotalInstalment}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //                    }
        //                    else
        //                    {
        //                        htmlForWidget.Replace("{{TotalInstalment}}", "R0.00");
        //                    }

        //                    htmlForWidget.Replace("{{InstalmentDate}}", (HomeLoan.EffectiveDate == null ? "" : "(effective from " + Convert.ToDateTime(HomeLoan.EffectiveDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + ")"));
        //                    pageContent.Replace("{{Home_Loan_Instalment_Details_" + widget.Identifier + "}}", htmlForWidget.ToString());

        //                    #endregion
        //                }
        //                else
        //                {
        //                    pageContent.Replace("{{Home_Loan_Instalment_Details_" + widget.Identifier + "}}", string.Empty);
        //                }
        //            });
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{Home_Loan_Instalment_Details_" + widget.Identifier + "}}", string.Empty);
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessages.Append("<li>Error occurred while configuring StaticHtml widget for Page: " + page.Identifier + " and Widget: " + widget.Identifier + ". error: " + ex.Message + "!!</li>");
        //        return true;
        //    }
        //}

        //private void BindPortfolioCustomerDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        //{
        //    pageContent.Replace("{{CustomerName_" + page.Identifier + "_" + widget.Identifier + "}}", (customer.Title + " " + customer.FirstName + " " + customer.SurName));
        //    pageContent.Replace("{{CustomerId_" + page.Identifier + "_" + widget.Identifier + "}}", customer.CustomerId.ToString());
        //    pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", customer.Mask_Cell_No);
        //    pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", customer.EmailAddress);
        //}

        //private void BindPortfolioCustomerAddressDetailsWidgetData(StringBuilder pageContent, DM_CustomerMaster customer, Page page, PageWidget widget)
        //{
        //    var custAddress = (!string.IsNullOrEmpty(customer.AddressLine0) ? (customer.AddressLine0 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(customer.AddressLine1) ? (customer.AddressLine1 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(customer.AddressLine2) ? (customer.AddressLine2 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(customer.AddressLine3) ? (customer.AddressLine3 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(customer.AddressLine4) ? customer.AddressLine4 : string.Empty);
        //    pageContent.Replace("{{CustomerAddress_" + page.Identifier + "_" + widget.Identifier + "}}", custAddress);
        //}
        //private void BindCorporateSaverAgentAddressDetailsWidgetData(StringBuilder pageContent, DM_CorporateSaverMaster CorporateSaver, Page page, PageWidget widget)
        //{
        //    var AgentAddress = (!string.IsNullOrEmpty(CorporateSaver.AgentClientAddress1) ? (CorporateSaver.AgentClientAddress1 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(CorporateSaver.AgentClientAddress2) ? (CorporateSaver.AgentClientAddress2 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(CorporateSaver.AgentClientAddress3) ? (CorporateSaver.AgentClientAddress3 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(CorporateSaver.AgentClientAddress4) ? (CorporateSaver.AgentClientAddress4 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(CorporateSaver.AgentClientAddress5) ? (CorporateSaver.AgentClientAddress5 + "<br>") : string.Empty) +
        //                        (!string.IsNullOrEmpty(CorporateSaver.AgentClientAddress6) ? (CorporateSaver.AgentClientAddress6 + "<br>") : string.Empty);
        //    pageContent.Replace("{{AgentAddress_" + page.Identifier + "_" + widget.Identifier + "}}", AgentAddress);
        //    pageContent.Replace("{{AgentContact_" + page.Identifier + "_" + widget.Identifier + "}}", CorporateSaver.AgentContactDetails);
        //}

        //private void BindPortfolioClientContactDetailsWidgetData(StringBuilder pageContent, Page page, PageWidget widget)
        //{
        //    pageContent.Replace("{{MobileNumber_" + page.Identifier + "_" + widget.Identifier + "}}", "0860 555 111");
        //    pageContent.Replace("{{EmailAddress_" + page.Identifier + "_" + widget.Identifier + "}}", "supportdesk@pps.com");
        //}

        //private void BindPortfolioAccountSummaryWidgetData(StringBuilder pageContent, List<DM_AccountsSummary> _AccountsSummaries, Page page, PageWidget widget)
        //{
        //    if (_AccountsSummaries.Count > 0)
        //    {
        //        var res = 0.0m;
        //        var accountSummaryRows = new StringBuilder();
        //        _AccountsSummaries.ForEach(acc =>
        //        {
        //            if (!acc.AccountType.ToLower().Contains("reward") || !acc.AccountType.ToLower().Contains("point"))
        //            {
        //                var tr = new StringBuilder();
        //                tr.Append("<tr class='ht-30'>");
        //                tr.Append("<td class='text-left'>" + acc.AccountType + " </td>");
        //                tr.Append("<td class='text-right'>" + utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (decimal.TryParse(acc.TotalAmount, out res) ? res : 0)) + " </td>");
        //                tr.Append("</tr>");
        //                accountSummaryRows.Append(tr.ToString());
        //            }
        //        });
        //        pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", accountSummaryRows.ToString());
        //    }
        //    else
        //    {
        //        pageContent.Replace("{{AccountSummaryRows_" + page.Identifier + "_" + widget.Identifier + "}}", "<tr class='ht-30'><td class='text-center' colspan='2'>No records found</td></tr>");
        //    }

        //    //To add reward points data
        //    var accSummary = _AccountsSummaries.Where(it => it.AccountType.ToLower().Contains("reward") || it.AccountType.ToLower().Contains("point"))?.ToList()?.FirstOrDefault();
        //    var rewardPointsDiv = new StringBuilder();
        //    if (accSummary != null)
        //    {
        //        rewardPointsDiv = new StringBuilder("<div class='pt-2'><table class='LoanTransactionTable customTable'><thead><tr class='ht-30'><th class='text-left'>" + accSummary.AccountType + " </th><th class='text-right'> " + accSummary.TotalAmount + " </th></tr></thead></table></div>");
        //    }
        //    pageContent.Replace("{{RewardPointsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", rewardPointsDiv.ToString());
        //}

        //private void BindPortfolioAccountAnalysisWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_AccountAnanlysis> _lstAccountAnalysis, Page page, PageWidget widget)
        //{
        //    var data = "[]";
        //    if (_lstAccountAnalysis != null && _lstAccountAnalysis.Count > 0)
        //    {
        //        data = JsonConvert.SerializeObject(_lstAccountAnalysis);
        //    }
        //    pageContent.Replace("HiddenAccountAnalysisGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
        //    scriptHtmlRenderer.Append(HtmlConstants.PORTFOLIO_ACCOUNT_ANALYSIS_BAR_GRAPH_SCRIPT.Replace("AccountAnalysisBarGraphcontainer", "AccountAnalysisBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenAccountAnalysisGraph", "HiddenAccountAnalysisGraph_" + page.Identifier + "_" + widget.Identifier));
        //}

        //private void BindPortfolioRemindersWidgetData(StringBuilder pageContent, List<DM_CustomerReminderAndRecommendation> Reminders, Page page, PageWidget widget)
        //{
        //    StringBuilder reminderstr = new StringBuilder();
        //    if (Reminders != null && Reminders.Count > 0)
        //    {
        //        Reminders.ToList().ForEach(item =>
        //        {
        //            if (!string.IsNullOrEmpty(item.reminderAndRecommendation.Description))
        //            {
        //                reminderstr.Append("<div class='row'><div class='col-lg-9 text-left'><p class='p-1' style='background-color: #dce3dc;'>" + item.reminderAndRecommendation.Description + " </p></div><div class='col-lg-3 text-left'><a href='" + item.reminderAndRecommendation.ActionUrl + "' target='_blank'><i class='fa fa-caret-left fa-3x float-left text-success'></i><span class='mt-2 d-inline-block ml-2'>" + item.reminderAndRecommendation.ActionTitle + "</span></a></div></div>");
        //            }
        //        });
        //    }
        //    pageContent.Replace("{{ReminderAndRecommendation_" + page.Identifier + "_" + widget.Identifier + "}}", reminderstr.ToString());
        //}

        //private void BindPortfolioNewsAlertsWidgetData(StringBuilder pageContent, List<DM_CustomerNewsAndAlert> NewsAndAlerts, Page page, PageWidget widget)
        //{
        //    var newsAlertStr = new StringBuilder();
        //    if (NewsAndAlerts != null && NewsAndAlerts.Count > 0)
        //    {
        //        NewsAndAlerts.ForEach(item =>
        //        {
        //            if (!string.IsNullOrEmpty(item.NewsAndAlert.Description))
        //            {
        //                newsAlertStr.Append("<p>" + item.NewsAndAlert.Description + "</p>");
        //            }
        //        });
        //    }
        //    pageContent.Replace("{{NewsAlert_" + page.Identifier + "_" + widget.Identifier + "}}", newsAlertStr.ToString());
        //}

        //private void BindGreenbacksTotalRewardPointsWidgetData(StringBuilder pageContent, List<DM_AccountsSummary> _AccountsSummaries, Page page, PageWidget widget)
        //{
        //    var accSummary = _AccountsSummaries.Where(it => it.AccountType.ToLower().Contains("reward") || it.AccountType.ToLower().Contains("point"))?.ToList()?.FirstOrDefault();
        //    pageContent.Replace("{{TotalRewardsPoints_" + page.Identifier + "_" + widget.Identifier + "}}", (accSummary != null ? accSummary.TotalAmount : "0"));
        //}

        //private void BindGreenbacksYtdRewardsPointsGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_GreenbacksRewardPoints> RewardPoints, Page page, PageWidget widget)
        //{
        //    var data = "[]";
        //    if (RewardPoints != null && RewardPoints.Count > 0)
        //    {
        //        data = JsonConvert.SerializeObject(RewardPoints);
        //    }
        //    pageContent.Replace("HiddenYTDRewardPointsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
        //    scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_YTD_REWARDS_POINTS_BAR_GRAPH_SCRIPT.Replace("YTDRewardPointsBarGraphcontainer", "YTDRewardPointsBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenYTDRewardPointsGraph", "HiddenYTDRewardPointsGraph_" + page.Identifier + "_" + widget.Identifier));
        //}

        //private void BindGreenbacksPointsRedeemedYtdGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_GreenbacksRewardPointsRedeemed> rewardPointsRedeemeds, Page page, PageWidget widget)
        //{
        //    var data = "[]";
        //    if (rewardPointsRedeemeds != null && rewardPointsRedeemeds.Count > 0)
        //    {
        //        data = JsonConvert.SerializeObject(rewardPointsRedeemeds);
        //    }
        //    pageContent.Replace("HiddenPointsRedeemedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
        //    scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_POINTS_REDEEMED_YTD_BAR_GRAPH_SCRIPT.Replace("PointsRedeemedYTDBarGraphcontainer", "PointsRedeemedYTDBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenPointsRedeemedGraph", "HiddenPointsRedeemedGraph_" + page.Identifier + "_" + widget.Identifier));
        //}

        //private void BindGreenbacksProductRelatedPonitsEarnedGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_CustomerProductWiseRewardPoints> productWiseRewardPoints, Page page, PageWidget widget)
        //{
        //    var data = "[]";
        //    if (productWiseRewardPoints != null && productWiseRewardPoints.Count > 0)
        //    {
        //        data = JsonConvert.SerializeObject(productWiseRewardPoints);
        //    }
        //    pageContent.Replace("HiddenProductRelatedPointsEarnedGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
        //    scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_PRODUCT_RELATED_POINTS_EARNED_BAR_GRAPH_SCRIPT.Replace("ProductRelatedPointsEarnedBarGraphcontainer", "ProductRelatedPointsEarnedBarGraphcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenProductRelatedPointsEarnedGraph", "HiddenProductRelatedPointsEarnedGraph_" + page.Identifier + "_" + widget.Identifier));
        //}

        //private void BindGreenbacksCategorySpendRewardPointsGraphWidgetData(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_CustomerRewardSpendByCategory> rewardSpendByCategories, Page page, PageWidget widget)
        //{
        //    var data = "[]";
        //    if (rewardSpendByCategories != null && rewardSpendByCategories.Count > 0)
        //    {
        //        data = JsonConvert.SerializeObject(rewardSpendByCategories);
        //    }
        //    pageContent.Replace("HiddenCategorySpendRewardsGraphValue_" + page.Identifier + "_" + widget.Identifier + "", data);
        //    scriptHtmlRenderer.Append(HtmlConstants.GREENBACKS_CATEGORY_SPEND_REWARD_POINTS_BAR_GRAPH_SCRIPT.Replace("CategorySpendRewardsPieChartcontainer", "CategorySpendRewardsPieChartcontainer_" + page.Identifier + "_" + widget.Identifier).Replace("HiddenCategorySpendRewardsGraph", "HiddenCategorySpendRewardsGraph_" + page.Identifier + "_" + widget.Identifier));
        //}

        //private void BindDataToWealthBreakdownOfInvestmentAccountsWidget(StringBuilder pageContent, StringBuilder scriptHtmlRenderer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, BatchMaster batchMaster, DM_CustomerMaster customer, string outputLocation)
        //{
        //    if (investmentMasters != null && investmentMasters.Count > 0)
        //    {
        //        //Create Nav tab if investment accounts is more than 1
        //        var NavTabs = new StringBuilder();
        //        var InvestmentAccountsCount = investmentMasters.Count;
        //        pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", NavTabs.ToString());

        //        pageContent = pageContent.Replace("<div class='card-body-header pb-2'>Breakdown of your investment accounts</div>", "<div class='card-body-header-w pb-2'>Breakdown of your investment accounts</div>");

        //        //create tab-content div if accounts is greater than 1, otherwise create simple div
        //        var TabContentHtml = new StringBuilder();
        //        var counter = 0;
        //        investmentMasters.OrderBy(m => m.InvestmentId).ToList().ForEach(acc =>
        //        {
        //            var InvestmentAccountDetailHtml = new StringBuilder(HtmlConstants.WEALTH_INVESTMENT_ACCOUNT_DETAILS_HTML_SMT);

        //            #region Account Details
        //            InvestmentAccountDetailHtml.Replace("{{ProductDesc}}", acc.ProductDesc);
        //            InvestmentAccountDetailHtml.Replace("{{InvestmentId}}", Convert.ToString(acc.InvestmentId));
        //            InvestmentAccountDetailHtml.Replace("{{TabPaneClass}}", string.Empty);

        //            var InvestmentNo = Convert.ToString(acc.InvestorId) + " " + Convert.ToString(acc.InvestmentId);
        //            //actual length is 12, due to space in between investor id and investment id we comparing for 13 characters
        //            while (InvestmentNo.Length != 13)
        //            {
        //                InvestmentNo = "0" + InvestmentNo;
        //            }
        //            InvestmentAccountDetailHtml.Replace("{{InvestmentNo}}", InvestmentNo);
        //            InvestmentAccountDetailHtml.Replace("{{AccountOpenDate}}", acc.AccountOpenDate != null ? acc.AccountOpenDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);
        //            var res = 0.0m;
        //            if (acc.CurrentInterestRate != null && decimal.TryParse(acc.CurrentInterestRate, out res))
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{InterestRate}}", decimal.Round(res, 2) + "% pa");
        //            }
        //            else
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{InterestRate}}", string.Empty);
        //            }
        //            InvestmentAccountDetailHtml.Replace("{{MaturityDate}}", acc.ExpiryDate != null ? acc.ExpiryDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) : string.Empty);
        //            InvestmentAccountDetailHtml.Replace("{{InterestDisposal}}", acc.InterestDisposalDesc != null ? acc.InterestDisposalDesc : string.Empty);
        //            InvestmentAccountDetailHtml.Replace("{{NoticePeriod}}", acc.NoticePeriod != null ? acc.NoticePeriod : string.Empty);

        //            res = 0.0m;
        //            acc.AccuredInterest = acc.AccuredInterest.Replace(",", ".");
        //            if (acc.AccuredInterest != null && decimal.TryParse(acc.AccuredInterest, out res))
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{InterestDue}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(res))));
        //            }
        //            else
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{InterestDue}}", "R0.00");
        //            }

        //            if (acc.ProductType == "Fixed deposit" || acc.ProductType == "Linked Deposit" || acc.ProductType == "Vaste deposito's" || acc.ProductType == "Gekoppelde deposito's")
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{BonusIntrestLabel}}", "Bonus Interest:");
        //                InvestmentAccountDetailHtml.Replace("{{BonusIntrestValue}}", (acc.BonusInterest + "% pa"));
        //            }
        //            else
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{BonusIntrestLabel}}", "");
        //                InvestmentAccountDetailHtml.Replace("{{BonusIntrestValue}}", "");
        //            }

        //            var LastInvestmentTransaction = acc.investmentTransactions.Where(it => it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC) || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)).OrderByDescending(it => it.TransactionDate)?.ToList()?.FirstOrDefault();
        //            if (LastInvestmentTransaction != null)
        //            {
        //                LastInvestmentTransaction.WJXBFS4_Balance = LastInvestmentTransaction.WJXBFS4_Balance.Replace(",", ".");
        //                InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", LastInvestmentTransaction.TransactionDate.ToString(ModelConstant.DATE_FORMAT_dd_MMMM_yyyy));
        //                if (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res))
        //                {
        //                    InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", (LastInvestmentTransaction.WJXBFS4_Balance == "0" ? "-" : (decimal.TryParse(LastInvestmentTransaction.WJXBFS4_Balance, out res) ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, (Convert.ToDecimal(LastInvestmentTransaction.WJXBFS4_Balance))) : "0")));
        //                }
        //                else
        //                {
        //                    InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "0");
        //                }
        //            }
        //            else
        //            {
        //                InvestmentAccountDetailHtml.Replace("{{LastTransactionDate}}", "");
        //                InvestmentAccountDetailHtml.Replace("{{BalanceOfLastTransactionDate}}", "");
        //            }

        //            #endregion Account Details

        //            #region Transactions
        //            var InvestmentTransactionRows = new StringBuilder();
        //            if (acc.investmentTransactions != null && acc.investmentTransactions.Count > 0)
        //            {
        //                acc.investmentTransactions.OrderBy(it => it.TransactionDate).ToList().ForEach(trans =>
        //                {
        //                    res = 0.0m;
        //                    var debit = trans.WJXBFS2_Debit.Replace(",", ".");
        //                    if (decimal.TryParse(debit, out res))
        //                    {
        //                        debit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                    }
        //                    else
        //                    {
        //                        debit = "-";
        //                    }

        //                    res = 0.0m;
        //                    var credit = trans.WJXBFS3_Credit.Replace(",", ".");
        //                    if (decimal.TryParse(credit, out res))
        //                    {
        //                        credit = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                    }
        //                    else
        //                    {
        //                        trans.WJXBFS3_Credit = "-";
        //                    }

        //                    res = 0.0m;
        //                    var balance = trans.WJXBFS4_Balance.Replace(",", ".");
        //                    if (decimal.TryParse(balance, out res))
        //                    {
        //                        balance = res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-";
        //                    }
        //                    else
        //                    {
        //                        balance = "0";
        //                    }
        //                    var tr = new StringBuilder();
        //                    tr.Append("<tr class='ht-20'>");
        //                    tr.Append("<td class='w-15 pt-1'>" + trans.TransactionDate + "</td>");
        //                    tr.Append("<td class='w-40 pt-1'>" + trans.TransactionDesc + "</td>");
        //                    tr.Append("<td class='w-15 text-right pt-1'>" + (debit) + "</td>");
        //                    tr.Append("<td class='w-15 text-right pt-1'>" + (credit) + "</td>");
        //                    tr.Append("<td class='w-15 text-right pt-1'>" + (balance) + "</td>");
        //                    tr.Append("</tr>");
        //                    InvestmentTransactionRows.Append(tr.ToString());
        //                });
        //                InvestmentAccountDetailHtml.Replace("{{InvestmentTransactionRows}}", InvestmentTransactionRows.ToString());
        //            }

        //            TabContentHtml.Append(InvestmentAccountDetailHtml.ToString());
        //            #endregion Transactions

        //            counter++;
        //        });
        //        TabContentHtml.Append((InvestmentAccountsCount > 1) ? "</div>" : string.Empty);

        //        if (customer.Language != "ENG")
        //            TabContentHtml.Replace("Date", "Datum");

        //        pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", TabContentHtml.ToString());
        //    }
        //    else
        //    {
        //        pageContent.Replace("{{NavTab_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //        pageContent.Replace("{{TabContentsDiv_" + page.Identifier + "_" + widget.Identifier + "}}", string.Empty);
        //    }
        //}

        //private void BindDataToWealthInvestmentPortfolioStatementWidget(StringBuilder pageContent, DM_CustomerMaster customer, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget)
        //{
        //    if (investmentMasters != null && investmentMasters.Count > 0)
        //    {
        //        var TotalClosingBalance = 0.0m;
        //        investmentMasters.ForEach(invest =>
        //        {
        //            var res = 0.0m;
        //            try
        //            {
        //                TotalClosingBalance = TotalClosingBalance + invest.investmentTransactions.Where(it =>
        //                it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC)
        //                || it.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR)
        //                ).Select(it => decimal.TryParse(it.WJXBFS4_Balance.Replace(",", "."), out res) ? res : 0).ToList().Sum(it => it);
        //            }
        //            catch (Exception)
        //            {
        //                TotalClosingBalance = 0.0m;
        //            }
        //        });

        //        pageContent.Replace("{{FirstName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.FirstName);
        //        pageContent.Replace("{{SurName_" + page.Identifier + "_" + widget.Identifier + "}}", customer.SurName);

        //        pageContent.Replace("{{DSName_" + page.Identifier + "_" + widget.Identifier + "}}", !string.IsNullOrEmpty(customer.DS_Investor_Name) ? ": " + customer.DS_Investor_Name : string.Empty);
        //        pageContent.Replace("{{TotalClosingBalance_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, TotalClosingBalance));
        //        pageContent.Replace("{{DayOfStatement_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].DayOfStatement);
        //        pageContent.Replace("{{InvestorID_" + page.Identifier + "_" + widget.Identifier + "}}", Convert.ToString(investmentMasters[0].InvestorId));

        //        //to separate to string dates values into required date format -- 
        //        //given date format for statement period is //2021-02-10 00:00:00.000 - 2021-03-09 23:00:00.000//
        //        //1st try with string separator, if fails then try with char separator
        //        var statementPeriod = string.Empty;
        //        string[] stringSeparators = new string[] { " - ", "- ", " -" };
        //        string[] dates = investmentMasters[0].StatementPeriod.Split(stringSeparators, StringSplitOptions.None);
        //        if (!string.IsNullOrWhiteSpace(investmentMasters[0].StatementPeriod) && dates.Length > 0)
        //        {
        //            if (dates.Length > 1)
        //            {
        //                statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[1]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
        //            }
        //            else
        //            {
        //                dates = investmentMasters[0].StatementPeriod.Split(new Char[] { ' ' });
        //                if (dates.Length > 2)
        //                {
        //                    statementPeriod = Convert.ToDateTime(dates[0]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(dates[2]).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var fromDate = investmentMasters.Select(x => x.investmentTransactions.OrderBy(z => z.TransactionDate).Select(y => y.TransactionDate).FirstOrDefault()).FirstOrDefault();
        //            var toDate = investmentMasters.Select(x => x.investmentTransactions.OrderByDescending(z => z.TransactionDate).Select(y => y.TransactionDate).FirstOrDefault()).FirstOrDefault();
        //            statementPeriod = Convert.ToDateTime(fromDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + " to " + Convert.ToDateTime(toDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy);
        //        }
        //        pageContent.Replace("{{StatementPeriod_" + page.Identifier + "_" + widget.Identifier + "}}", statementPeriod);
        //        pageContent.Replace("{{StatementDate_" + page.Identifier + "_" + widget.Identifier + "}}", investmentMasters[0].StatementDate?.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //    }
        //}

        //private void BindDataToWealthInvestorPerformanceStatementWidget(StringBuilder pageContent, List<DM_InvestmentMaster> investmentMasters, Page page, PageWidget widget, DM_CustomerMaster cust)
        //{
        //    if (investmentMasters != null && investmentMasters.Count > 0)
        //    {
        //        List<InvestorPerformanceWidgetData> investorPerformanceWidgetDatas = new List<InvestorPerformanceWidgetData>();
        //        List<DM_InvestmentTransaction> transactions = new List<DM_InvestmentTransaction>();

        //        investmentMasters.OrderBy(a => a.InvestmentId).ToList().ForEach(invest =>
        //        {
        //            InvestorPerformanceWidgetData item;
        //            var productType = invest.ProductType;
        //            if (investorPerformanceWidgetDatas.Any(m => m.ProductType == productType))
        //            {
        //                item = investorPerformanceWidgetDatas.FirstOrDefault(m => m.ProductType == productType);
        //                foreach (DM_InvestmentTransaction transaction in invest.investmentTransactions)
        //                {
        //                    transaction.ProductId = Convert.ToInt64(item.ProductId);
        //                }
        //            }
        //            else
        //            {
        //                item = new InvestorPerformanceWidgetData() { ProductType = productType, ProductId = invest.ProductId.ToString() };
        //                investorPerformanceWidgetDatas.Add(item);
        //            }

        //            transactions.AddRange(invest.investmentTransactions);
        //        });

        //        var html = "";

        //        int counter = 0;

        //        foreach (var item in investorPerformanceWidgetDatas)
        //        {
        //            item.OpeningBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && (m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC) || m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC_AFR))).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();
        //            item.ClosingBalance = transactions.Where(m => m.ProductId.ToString() == item.ProductId && (m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC) || m.TransactionDesc.ToLower().Contains(ModelConstant.BALANCE_CARRIED_FORWARD_TRANSACTION_DESC_AFR))).Sum(m => Convert.ToDecimal(m.WJXBFS4_Balance)).ToString();
        //            item.OpeningBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.OpeningBalance));
        //            item.ClosingBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(item.ClosingBalance));

        //            var InvestorPerformanceHtmlWidget = HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_HTML;
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ProductType}}", item.ProductType);
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{OpeningBalanceAmount}}", item.OpeningBalance);
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{ClosingBalanceAmount}}", item.ClosingBalance);
        //            InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("{{WidgetId}}", "");

        //            if (counter != 0)
        //                InvestorPerformanceHtmlWidget = InvestorPerformanceHtmlWidget.Replace("<div class='card-body-header-w pb-2'>Investor performance</div>", "");

        //            html += InvestorPerformanceHtmlWidget;
        //            counter++;
        //        }
        //        pageContent.Replace("{{" + HtmlConstants.WEALTH_INVESTOR_PERFORMANCE_WIDGET_NAME + "_" + page.Identifier + "_" + widget.Identifier + "}}", html);
        //    }
        //}

        //private void BindMCAAccountSummaryDetailWidgetData(StringBuilder pageContent, List<DM_MCAMaster> MCAMasterList, PageWidget widget)
        //{
        //    try
        //    {
        //        if (MCAMasterList != null && MCAMasterList.Count > 0)
        //        {
        //            var mcaMaster = MCAMasterList[0];
        //            pageContent.Replace("{{AccountNo_" + widget.Identifier + "}}", mcaMaster.CustomerId.ToString());
        //            pageContent.Replace("{{StatementNo_" + widget.Identifier + "}}", mcaMaster.StatementNo);
        //            pageContent.Replace("{{OverdraftLimit_" + widget.Identifier + "}}", (mcaMaster.OverdraftLimit != null ? Math.Round(decimal.Parse(mcaMaster.OverdraftLimit.ToString()), 2).ToString() : ""));
        //            pageContent.Replace("{{StatementDate_" + widget.Identifier + "}}", mcaMaster.StatementDate.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy));
        //            pageContent.Replace("{{Currency_" + widget.Identifier + "}}", mcaMaster.Currency);
        //            pageContent.Replace("{{Statementfrequency_" + widget.Identifier + "}}", mcaMaster.StatementFrequency);
        //            pageContent.Replace("{{FreeBalance_" + widget.Identifier + "}}", (mcaMaster.FreeBalance != null ? Math.Round(decimal.Parse(mcaMaster.FreeBalance.ToString()), 2).ToString() : ""));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        //private void BindCorporateSaverClientDetailsWidgetData(StringBuilder pageContent, List<DM_CorporateSaverMaster> CorporateSaverMasterList, PageWidget widget)
        //{
        //    try
        //    {
        //        if (CorporateSaverMasterList != null && CorporateSaverMasterList.Count > 0)
        //        {
        //            var CorporateSaverMaster = CorporateSaverMasterList[0];
        //            pageContent.Replace("{{TaxInvoiceNo_" + widget.Identifier + "}}", CorporateSaverMaster.TaxInvoiceNo);
        //            pageContent.Replace("{{ContactPerson_" + widget.Identifier + "}}", CorporateSaverMaster.AgentContactPerson);
        //            pageContent.Replace("{{EmailAddress_" + widget.Identifier + "}}", CorporateSaverMaster.AgentEmailAddress);
        //            pageContent.Replace("{{RegNo_" + widget.Identifier + "}}", CorporateSaverMaster.AgentRegNo);
        //            pageContent.Replace("{{VATRegNo_" + widget.Identifier + "}}", CorporateSaverMaster.AgentVATRegNo);
        //            pageContent.Replace("{{FSPLicNo_" + widget.Identifier + "}}", CorporateSaverMaster.AgentFSPLicNo);
        //            pageContent.Replace("{{AgentRefNo_" + widget.Identifier + "}}", CorporateSaverMaster.AgentReference);
        //            pageContent.Replace("{{StatementNo_" + widget.Identifier + "}}", CorporateSaverMaster.StatementNo);
        //            pageContent.Replace("{{AccountNo_" + widget.Identifier + "}}", Convert.ToString(CorporateSaverMaster.InvestorId));
        //            pageContent.Replace("{{Branchcode_" + widget.Identifier + "}}", CorporateSaverMaster.BranchCode);
        //            pageContent.Replace("{{Agentprofile_" + widget.Identifier + "}}", CorporateSaverMaster.AgentProfile);
        //            pageContent.Replace("{{CIFNo_" + widget.Identifier + "}}", CorporateSaverMaster.CIFNo);
        //            pageContent.Replace("{{ClientCode_" + widget.Identifier + "}}", CorporateSaverMaster.ClientCode);
        //            pageContent.Replace("{{RelationshipManager_" + widget.Identifier + "}}", CorporateSaverMaster.RelationshipManager);
        //            pageContent.Replace("{{VATCalculation_" + widget.Identifier + "}}", CorporateSaverMaster.VATCalculation);
        //            pageContent.Replace("{{ClientVATNo_" + widget.Identifier + "}}", CorporateSaverMaster.ClientVatNo);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        //private void BindMCATransactionDetailWidgetData(StringBuilder pageContent, List<DM_MCAMaster> MCAMasterList, PageWidget widget, Page page)
        //{
        //    StringBuilder tableHTML = new StringBuilder();
        //    if (MCAMasterList != null && MCAMasterList.Count > 0)
        //    {
        //        var MCAMaster = MCAMasterList[0];
        //        if (MCAMaster.MCATransactions != null && MCAMaster.MCATransactions.Count > 0)
        //        {
        //            var res = 0.0m;
        //            MCAMaster.MCATransactions.ForEach(trans =>
        //            {
        //                string debit = string.Empty;
        //                string credit = string.Empty;
        //                res = 0.0m;
        //                if (trans.Debit != null && decimal.TryParse(trans.Debit.ToString(), out res))
        //                {
        //                    var debitBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(res));
        //                    debit = debitBalance.Replace("R", "").Replace("$", "");
        //                }
        //                else
        //                {
        //                    debit = "";
        //                }

        //                if (trans.Credit != null && decimal.TryParse(trans.Credit.ToString(), out res))
        //                {
        //                    credit = res > 0 ? Math.Round(res, 2).ToString() : Math.Round(decimal.Parse(trans.Credit.ToString())).ToString();
        //                    var creditBalance = utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(res));
        //                    credit = creditBalance.Replace("R", "").Replace("$", "");
        //                }
        //                else
        //                {
        //                    credit = "";
        //                }

        //                tableHTML.Append("<tr class='ht-20'>" +
        //                                 "<td class='w-7 text-center'>" + trans.Transaction_Date.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
        //                                 "<td class='w-40 text-left'>" + trans.Description + "</td>" +
        //                                 "<td class='w-12 text-right'>" + (debit == "0" || debit == "0.00" ? "" : debit) + "</td>" +
        //                                 "<td class='w-12 text-right'>" + (credit == "0" || credit == "0.00" ? "" : credit) + "</td>" +
        //                                 "<td class='w-7 text-center'>" + (trans.Rate != null ? Math.Round(decimal.Parse(trans.Rate.ToString()), 2).ToString() : "") + "</td>" +
        //                                 "<td class='w-5 text-center'>" + (trans.Days != null && trans.Days != 0 ? trans.Days.ToString() : "") + "</td>" +
        //                                 "<td class='w-10 text-right'>" + (trans.AccuredInterest != null ? Math.Round(decimal.Parse(trans.AccuredInterest.ToString()), 2).ToString() : "") + "</td>" +
        //                                 "</tr>");
        //            });
        //        }
        //        pageContent.Replace("{{MCATransactionRow_" + page.Identifier + "_" + widget.Identifier + "}}", tableHTML.ToString());
        //    }
        //}
        //private void BindCorporateSaverLastTotalWidgetData(StringBuilder pageContent, List<DM_CorporateSaverMaster> DM_CorporateSaverMasterList, PageWidget widget, Page page, DM_CustomerMaster customerMaster)
        //{
        //    StringBuilder tableHTML = new StringBuilder();
        //    StringBuilder tableHTML2 = new StringBuilder();
        //    int tableHTMLCounter = 1, tableHTML2Counter = 1;
        //    if (DM_CorporateSaverMasterList != null && DM_CorporateSaverMasterList.Count > 0)
        //    {
        //        var CorporateSaverMaster = DM_CorporateSaverMasterList[0];
        //        // dynamic CorporateSaverTaxMatured = null; dynamic CorporateSaverTaxCurrent = null;
        //        var CorporateSaverTaxMatured = CorporateSaverMaster.CorporateSaverTax.Where(item => item.InvestType.Contains("Matured"))?.ToList();
        //        var CorporateSaverTaxCurrent = CorporateSaverMaster.CorporateSaverTax.Where(item => item.InvestType.Contains("Current"))?.ToList();



        //        CorporateSaverMaster.CorporateSaverTax.ForEach(invest =>
        //        {
        //            if (invest.InvestType.Contains("Current"))
        //            {
        //                if (tableHTMLCounter == 1)
        //                {
        //                    tableHTML.Append("<div class='CSTotalAmountDetailsDiv' style='height: 40px !important; text-align: center; padding: 6px !important;'><span class='fnt-14pt'>" + invest.InvestType + "</span ></div>");
        //                }

        //                tableHTML.Append("<table class= 'CScustomTable HomeLoanDetailDiv' border = '0' style = 'height: auto;margin-bottom:2%;' ><tbody>");
        //                tableHTML.Append("<tr><td colspan='2' class='w-25' style='font-weight: bold;padding-bottom: 8px !important;padding-top: 8px !important;'>" + invest.InterestDescription + "</td></tr>");
        //                tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest instruction</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>" + invest.InterestIntruction + "</td><td class='w-25' style='padding-bottom: 8px !important'>Date Invested</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + Convert.ToDateTime(invest.DateInvested).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy2) + "</td></tr>");
        //                tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Capital</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>"
        //                   + (invest.CapitalMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.CapitalMstr)) : "") +
        //                    "</td><td class='w-25' style='padding-bottom: 8px !important'>Agent fee deducted</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.AgentFeeDeducted != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.AgentFeeDeducted)) : "") + "</td></tr>");
        //                tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>" + (invest.InterestMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.InterestMstr)) : "") + "</td><td class='w-25' style='padding-bottom: 8px !important'>VAT on fee</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.VatOnFeeMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.VatOnFeeMstr)) : "") + "</td></tr>");
        //                tableHTML.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Agent fee structure</td><td class='w-25 text-right' style='padding-bottom: 8px !important;padding-right: 15px;'>" +
        //                    (invest.AGENT_FEE_STRUCTURE_1 != null ? String.Format("{0:0.00}", invest.AGENT_FEE_STRUCTURE_1) + "%" + " " + invest.AGENT_FEE_STRUCTURE_2 : String.Empty)
        //                    + "</td><td class='w-25' style='padding-bottom: 8px !important'>Interest (less agent fee and VAT)</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.VatOnFeeMstr0 != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.VatOnFeeMstr0)) : "") + "</td></tr>");
        //                tableHTML.Append("</tbody></table>");

        //                if (tableHTMLCounter == CorporateSaverTaxCurrent.Count)
        //                {

        //                    tableHTML.Append("<div class='d-flex flex-row' style='margin-top: -1.5%;'>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>Total capital</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>Total interest</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>Total agent fee (deducted)</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='font-weight: bold;margin-right:3px; margin-bottom:1px; '>VAT on fee</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-bottom:1px'>Interest (less agent fee & VAT)</div>");
        //                    tableHTML.Append("</div>");
        //                    tableHTML.Append("<div class='d-flex flex-row' style='margin-top: 2px !important;margin-bottom: 1%;'>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='margin-right:3px; margin-bottom:1px; '>" + (invest.TotalCapitalMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.TotalCapitalMstr)) : "") + "</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='margin-right:3px; margin-bottom:1px; '>" + (invest.TotalInterestMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.TotalInterestMstr)) : "") + "</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1' style='margin-right:3px; margin-bottom:1px; '>" + (invest.TotalAgentFeeMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.TotalAgentFeeMstr)) : "") + "</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1 ' style='margin-right:3px; margin-bottom:1px; '>" + (invest.VatOnFeeMstr0 != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.VatOnFeeMstr0)) : "") + "</div>");
        //                    tableHTML.Append("<div class='paymentDueHeaderBlock1' style='margin-bottom:1px; '>" + (invest.InterestAgentFeeMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.InterestAgentFeeMstr)) : "") + "</div>");
        //                    tableHTML.Append("</div>");
        //                }

        //                tableHTMLCounter = tableHTMLCounter + 1;
        //            }
        //            else if (invest.InvestType.Contains("Matured"))
        //            {
        //                if (tableHTML2Counter == 1)
        //                {
        //                    tableHTML2.Append("<div class='CSTotalAmountDetailsDiv' style='height: 40px !important; text-align: center; padding: 6px !important;'><span class='fnt-14pt'>" + invest.InvestType + "</span ></div>");
        //                }

        //                tableHTML2.Append("<table class= 'CScustomTable HomeLoanDetailDiv' border = '0' style = 'height: auto;margin-bottom:2%;' ><tbody>");
        //                tableHTML2.Append("<tr><td colspan='2' class='w-25' style='font-weight: bold;padding-bottom: 8px !important;padding-top: 8px !important;'>" + invest.InterestDescription + "</td></tr>");
        //                tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest instruction</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + invest.InterestIntruction + "</td><td class='w-25' style='padding-bottom: 8px !important'>Date Invested</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + Convert.ToDateTime(invest.DateInvested).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy2) + "</td></tr>");
        //                tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Capital</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>"
        //                   + (invest.CapitalMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.CapitalMstr)) : "") +
        //                    "</td><td class='w-25' style='padding-bottom: 8px !important'>Agent fee deducted</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.AgentFeeDeducted != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.AgentFeeDeducted)) : "") + "</td></tr>");
        //                tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Interest</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.InterestMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.InterestMstr)) : "") + "</td><td class='w-25' style='padding-bottom: 8px !important'>VAT on fee</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.VatOnFeeMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.VatOnFeeMstr)) : "") + "</td></tr>");
        //                tableHTML2.Append("<tr><td class='w-25' style='padding-bottom: 8px !important'>Agent fee structure</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" +
        //                  (invest.AGENT_FEE_STRUCTURE_1 != null ? String.Format("{0:0.00}", invest.AGENT_FEE_STRUCTURE_1) + "%" + " " + invest.AGENT_FEE_STRUCTURE_2 : String.Empty)
        //                    + "</td><td class='w-25' style='padding-bottom: 8px !important'>Interest (less agent fee and VAT)</td><td class='w-25 text-right pr-1' style='padding-bottom: 8px !important'>" + (invest.VatOnFeeMstr0 != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.VatOnFeeMstr0)) : "") + "</td></tr>");
        //                tableHTML2.Append("</tbody></table>");

        //                if (tableHTML2Counter == CorporateSaverTaxMatured.Count)
        //                {

        //                    tableHTML2.Append("<div class='d-flex flex-row' style='margin-top: -1.5%;'>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>Total capital</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>Total interest</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>Total agent fee (deducted)</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold;margin-right:3px; margin-bottom:1px;'>VAT on fee</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='font-weight: bold; margin-bottom:1px'>Interest (less agent fee & VAT)</div>");
        //                    tableHTML2.Append("</div>");
        //                    tableHTML2.Append("<div class='d-flex flex-row' style='margin-top: 2px !important;margin-bottom: 1%;'>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>" + (invest.TotalCapitalMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.TotalCapitalMstr)) : "") + "</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>" + (invest.TotalInterestMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.TotalInterestMstr)) : "") + "</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>" + (invest.TotalAgentFeeMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.TotalAgentFeeMstr)) : "") + "</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-right:3px; margin-bottom:1px;'>" + (invest.VatOnFeeMstr0 != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.VatOnFeeMstr0)) : "") + "</div>");
        //                    tableHTML2.Append("<div class='paymentDueHeaderBlock1'style='margin-bottom:1px;'>" + (invest.InterestAgentFeeMstr != null ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, Convert.ToDecimal(invest.InterestAgentFeeMstr)) : "") + "</div>");
        //                    tableHTML2.Append("</div>");
        //                }
        //                tableHTML2Counter = tableHTML2Counter + 1;

        //            }
        //        });
        //        tableHTML.Append(tableHTML2);
        //        // if (customerMaster.InvestorId == 9018933796)
        //        string importantMsg = "";
        //        importantMsg += "<div class='card border-0'><div class='card-body text-left'style='padding: 0;'><div class='card-body-header mt-3-2' style='font-family: \"Arial\";font-weight: 700;'>Important information</div> <div class='' style='font-size: 9pt; font-family: \"Arial\";'>";

        //        if (customerMaster.Language == "ENG")
        //        {
        //            importantMsg += "<p>Interest(less agent administration fee and VAT) is credited to your account in March.The agent administration fee and VAT are deducted in March and paid on your behalf to your agent, in accordance with the mandate held.</p>";
        //        }
        //        else
        //        {
        //            importantMsg += "<p>Rente (min agentadministrasiegelde en BTW) word in Maart op u rekening gekrediteer. Die agentadministrasiegelde en BTW word in Maart afgetrek en namens u aan u agent betaal, in ooreenstemming met die mandaat wat gehou word.</p>";
        //        }


        //        if (DM_CorporateSaverMasterList.FirstOrDefault().MESSAGE_INDICATOR)
        //        {
        //            if (DM_CorporateSaverMasterList.FirstOrDefault().MESSAGE_INDICATOR && !string.IsNullOrEmpty(DM_CorporateSaverMasterList.FirstOrDefault().SECTION86_MESSAGE))
        //            {
        //                importantMsg += "<p>" + DM_CorporateSaverMasterList.FirstOrDefault().SECTION86_MESSAGE + "</p>";
        //            }
        //        }
        //        importantMsg += "</div></div></div>";
        //        pageContent.Replace("{{dynamicMsg}}", importantMsg);


        //        pageContent.Replace("{{dynemicTables_" + page.Identifier + "_" + widget.Identifier + "}}", tableHTML.ToString());
        //    }

        //}


        //private void BindCorporateSaverTransactionDetailWidgetData(StringBuilder pageContent, List<DM_CorporateSaverMaster> CorporateSaverMasterList, PageWidget widget, Page page, DM_CustomerMaster customer)
        //{
        //    StringBuilder tableHTML = new StringBuilder();
        //    if (CorporateSaverMasterList != null && CorporateSaverMasterList.Count > 0)
        //    {
        //        var CorporateSaverMaster = CorporateSaverMasterList[0];
        //        int counter = 1;
        //        if (CorporateSaverMaster.CorporateSaverTransactions != null && CorporateSaverMaster.CorporateSaverTransactions.Count > 0)
        //        {
        //            var res = 0.0m;
        //            var firstTransaction = CorporateSaverMaster.CorporateSaverTransactions.OrderBy(m => m.FromDate).FirstOrDefault();
        //            var lastTransaction = CorporateSaverMaster.CorporateSaverTransactions.OrderByDescending(m => m.ToDate).FirstOrDefault();

        //            CorporateSaverMaster.CorporateSaverTransactions.ForEach(trans =>
        //            {
        //                if (counter == 1)
        //                {
        //                    pageContent.Replace("{{FromDate}}", Convert.ToDateTime(firstTransaction.FromDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy2));
        //                    pageContent.Replace("{{ToDate}}", Convert.ToDateTime(lastTransaction.FromDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy2));
        //                }

        //                tableHTML.Append("<tr class='ht-20 CorporateSaverTable'>");
        //                tableHTML.Append("<td class='w-12 text-center'>" + Convert.ToDateTime(trans.FromDate).ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>");
        //                tableHTML.Append("<td class='text-left'  style='width: 25%'>" + trans.PaymentDetails + "</td>");
        //                tableHTML.Append("<td class='text-left'  style='width: 25%'>" + trans.TransactionDescription + "</td>");

        //                if (decimal.TryParse(trans.Amount, out res))
        //                {
        //                    tableHTML.Append("<td class='w-15 text-right'> " + (res != 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : String.Empty) + "</td>");
        //                }
        //                else
        //                {
        //                    tableHTML.Append("<td class='w-15 text-center'></td>");
        //                }
        //                if (decimal.TryParse(trans.Rate, out res))
        //                {
        //                    // (res > 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : "-" + "%</td>");

        //                    tableHTML.Append("<td class='w-8 text-right'>" + (res != 0 ? String.Format("{0:0.00}", res) + "%" : " ") + "</td>");
        //                }
        //                else
        //                {
        //                    tableHTML.Append("<td class='w-8 text-center'></td>");
        //                }

        //                if (decimal.TryParse(trans.CapitalBalance, out res))
        //                {
        //                    tableHTML.Append("<td class='w-15 text-right'> " + (res != 0 ? utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res) : String.Empty) + "</td>");
        //                }
        //                else
        //                {
        //                    tableHTML.Append("<td class='w-15 text-center'></td>");
        //                }


        //                tableHTML.Append("</tr>");
        //                counter++;
        //            });
        //        }
        //        pageContent.Replace("{{CorporateSaverTransactions_" + widget.Identifier + "}}", tableHTML.ToString());
        //        if (CorporateSaverMasterList.FirstOrDefault().SHOWMESSAGE)
        //        {
        //            if (customer.Language == "ENG")
        //            {
        //                var msg = HtmlConstants.CORPORATESAVER_TRANS_DETAIL_MSG.Replace("{{TransMessageHeading}}", CorporateSaverMasterList.FirstOrDefault().AGENT_NAME);
        //                msg = msg.Replace("{{TransMessage}}", CorporateSaverMasterList.FirstOrDefault().AGENT_MESSAGE_ENGLISH);
        //                pageContent.Replace("{{AgentMessage_" + widget.Identifier + "}}", msg);
        //            }
        //            else
        //            {
        //                var msg = HtmlConstants.CORPORATESAVER_TRANS_DETAIL_MSG.Replace("{{TransMessageHeading}}", CorporateSaverMasterList.FirstOrDefault().AGENT_NAME);
        //                msg = msg.Replace("{{TransMessage}}", CorporateSaverMasterList.FirstOrDefault().AGENT_MESSAGE_AFRIKAANS);
        //                pageContent.Replace("{{AgentMessage_" + widget.Identifier + "}}", msg);
        //            }
        //        }
        //        else
        //        {
        //            pageContent.Replace("{{AgentMessage_" + widget.Identifier + "}}", "");
        //        }
        //    }
        //}

        //private void BindMCAVATAnalysisDetailWidgetData(StringBuilder pageContent, List<DM_MCAMaster> MCAMasterList, PageWidget widget, Page page)
        //{
        //    StringBuilder tableHTML = new StringBuilder();
        //    if (MCAMasterList != null && MCAMasterList.Count > 0)
        //    {
        //        var MCAMaster = MCAMasterList[0];
        //        if (MCAMaster.MCATransactions != null && MCAMaster.MCATransactions.Count > 0)
        //        {
        //            //MCAMaster.MCATransactions.ForEach(trans =>
        //            //{
        //            //    tableHTML.Append("<tr class='ht-20'>" +
        //            //                     "<td class='ip-w-25 text-left'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
        //            //                     "<td class='ip-w-25 text-left'>" + DateTime.Now.ToString(ModelConstant.DATE_FORMAT_dd_MM_yyyy) + "</td>" +
        //            //                     "<td class='ip-w-25 text-right'>" + (trans.Rate != null ? Math.Round(decimal.Parse(trans.Rate.ToString()), 2).ToString() : "0.00") + "</td>" +
        //            //                     "<td class='ip-w-25 text-right'>" + (trans.Credit != null ? Math.Round(decimal.Parse(trans.Credit.ToString()), 2).ToString() : "0.00") + "</td>" +
        //            //                         "</tr>");
        //            //});
        //        }
        //        pageContent.Replace("{{MCAVATTable_" + page.Identifier + "_" + widget.Identifier + "}}", tableHTML.ToString());
        //    }
        //}

        //private void BindCorporateTaxTotalData(StringBuilder pageContent, List<DM_CorporateSaverMaster> CorporateSaverMasterList,
        //    PageWidget widget, Page page)
        //{
        //    if (CorporateSaverMasterList != null && CorporateSaverMasterList.Count > 0)
        //    {
        //        var CorporateSaverMaster = CorporateSaverMasterList[0];
        //        if (CorporateSaverMaster != null)
        //        {
        //            decimal res;
        //            decimal.TryParse(CorporateSaverMaster.Interest, out res);
        //            pageContent.Replace("{{Interest_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));

        //            decimal.TryParse(CorporateSaverMaster.VatOnFee, out res);
        //            pageContent.Replace("{{VATonfee_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));

        //            decimal.TryParse(CorporateSaverMaster.AgentFeeDeducted, out res);
        //            pageContent.Replace("{{Agentfeededucted_" + page.Identifier + "_" + widget.Identifier + "}}", utility.CurrencyFormatting(ModelConstant.SA_COUNTRY_CULTURE_INFO_CODE, ModelConstant.DOT_AS_CURERNCY_DECIMAL_SEPARATOR, ModelConstant.CURRENCY_FORMAT_VALUE, res));
        //        }

        //    }
        //}

        //private StringBuilder Translate(StringBuilder inputStr, DM_CustomerMaster customer)
        //{
        //    //Check the language using customer.Language and then translate it
        //    List<TranslatedItem> list = new List<TranslatedItem>();
        //    if (customer.Language == "AFR")
        //    {
        //        var resourceItems = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

        //        foreach (DictionaryEntry item in resourceItems)
        //        {
        //            list.Add(new TranslatedItem() { Eng = item.Key.ToString(), Translated = item.Value.ToString(), StringLength = item.Key.ToString().Length });
        //        }

        //        var replaced = inputStr.ToString();
        //        foreach (var item in list.OrderByDescending(m => m.StringLength))
        //        {
        //            if (item.Eng.Contains('('))
        //            {
        //                replaced = replaced.Replace(item.Eng, item.Translated);
        //            }
        //            else
        //            {
        //                string pattern = @"\b(" + item.Eng + @")\b";
        //                replaced = Regex.Replace(replaced, pattern, item.Translated, RegexOptions.IgnoreCase);
        //            }
        //        }

        //        inputStr = new StringBuilder();
        //        inputStr.Length = 0;
        //        inputStr.Capacity = 0;
        //        inputStr.Append(replaced);
        //    }
        //    else
        //    {
        //        inputStr.Replace("After 120 days", "After 120 + days");
        //    }
        //    return inputStr;
        //}

        #endregion
        // pps
        static string FormatDateWithOrdinal(DateTime date)
        {
            // Get the day of the month
            int day = date.Day;

            // Create an ordinal suffix (e.g., "st", "nd", "rd", "th")
            string ordinalSuffix;
            if (day % 100 >= 11 && day % 100 <= 13)
            {
                ordinalSuffix = "th";
            }
            else
            {
                switch (day % 10)
                {
                    case 1:
                        ordinalSuffix = "st";
                        break;
                    case 2:
                        ordinalSuffix = "nd";
                        break;
                    case 3:
                        ordinalSuffix = "rd";
                        break;
                    default:
                        ordinalSuffix = "th";
                        break;
                }
            }

            // Format the date with the ordinal suffix
            string formattedDate = $"{day}<sup>{ordinalSuffix}</sup> {date:MMMM}";

            return formattedDate;
        }
        //#endregion
    }

    //public class InvestorPerformanceWidgetData
    //{
    //    public string ProductId { get; set; }
    //    public string ProductType { get; set; }
    //    public string OpeningBalance { get; set; }
    //    public string ClosingBalance { get; set; }
    //}

    //public class TranslatedItem
    //{
    //    public string Eng { get; set; }
    //    public string Translated { get; set; }
    //    public int StringLength { get; set; }
    //}
}
