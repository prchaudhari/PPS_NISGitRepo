// <copyright file="SQLArchivalProcessRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    using Microsoft.Practices.ObjectBuilder2;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.IO.Compression;
    using System.Threading.Tasks;
    #endregion

    public class SQLArchivalProcessRepository : IArchivalProcessRepository
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
        /// The Asset repository.
        /// </summary>
        private IStatementSearchRepository statementSearchRepository = null;

        #endregion

        #region Constructor

        public SQLArchivalProcessRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.statementRepository = this.unityContainer.Resolve<IStatementRepository>();
            this.statementSearchRepository = this.unityContainer.Resolve<IStatementSearchRepository>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method to convert HTML statements into the PDF statement files and archieve related to log and metadata.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ParallelThreadCount"></param>
        /// <param name="MinimumArchivalPeriodDays"></param>
        /// <param name="pdfStatementFilepath"></param>
        /// <param name="htmlStatementFilepath"></param>
        /// <param name="tenantConfiguration"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the archive process runs successfully, false otherwise
        /// </returns>
        public bool RunArchivalProcess(Client client, int ParallelThreadCount, int MinimumArchivalPeriodDays, string pdfStatementFilepath, string htmlStatementFilepath, TenantConfiguration tenantConfiguration, string tenantCode)
        {
            bool IsArchivalProcessDone = false;
            string outputlocation = string.Empty;

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                if (tenantConfiguration.ArchivalPeriod != null && tenantConfiguration.ArchivalPeriod > MinimumArchivalPeriodDays)
                {
                    var archivalPeriod = tenantConfiguration.ArchivalPeriod ?? 0;
                    var statementmetadatarecords = new List<StatementMetadataRecord>();
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        var dueDate = DateTime.UtcNow.AddDays(-archivalPeriod);
                        statementmetadatarecords = nISEntitiesDataContext.StatementMetadataRecords.Where(item => item.StatementDate < dueDate).ToList();
                    }

                    if (statementmetadatarecords.Count > 0)
                    {
                        var statementmetadatarecord = new StatementMetadataRecord();
                        var customerRecord = new CustomerMasterRecord();
                        var statement = new Statement();
                        IList<StatementPageContent> statementPageContents = new List<StatementPageContent>();
                        var customerIds = new List<long>();
                        var results = statementmetadatarecords.GroupBy(p => p.StatementId, (key, g) => new { StatementId = key, GroupedStatementMetadataRecords = g.ToList() }).ToList();
                        if (results.Count > 0)
                        {
                            results.ForEach(res =>
                            {
                                if (res.GroupedStatementMetadataRecords.Count > 0)
                                {
                                    tenantCode = res.GroupedStatementMetadataRecords.FirstOrDefault().TenantCode;
                                    var scheduleLogId = res.GroupedStatementMetadataRecords.FirstOrDefault().ScheduleLogId;
                                    var schedulelog = new ScheduleLogRecord();
                                    var batchmaster = new BatchMasterRecord();
                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                    {
                                        schedulelog = nISEntitiesDataContext.ScheduleLogRecords.Where(item => item.Id == scheduleLogId && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                                        batchmaster = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.Id == schedulelog.BatchId && item.TenantCode == tenantCode).ToList()?.FirstOrDefault();
                                    }

                                    //if batch is present and it's status is approved then only schedule log and statement metadata records should be archived.
                                    if (batchmaster != null && batchmaster.Status == BatchStatus.Approved.ToString())
                                    {
                                        StatementSearchParameter statementSearchParameter = new StatementSearchParameter
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
                                        };
                                        var statements = this.statementRepository.GetStatements(statementSearchParameter, tenantCode);
                                        if (statements.Count > 0)
                                        {
                                            statement = statements.FirstOrDefault();
                                            statementPageContents = this.statementRepository.GenerateHtmlFormatOfStatement(statement, tenantCode, tenantConfiguration);
                                            customerIds = res.GroupedStatementMetadataRecords.Select(item => item.CustomerId).Distinct().ToList();

                                            //To insert archive schedule log record
                                            var scheduleLogArchiveRecord = new ScheduleLogArchiveRecord();
                                            //var tempmetadata = res.GroupedStatementMetadataRecords.FirstOrDefault();
                                            using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                            {
                                                scheduleLogArchiveRecord.ScheduleId = schedulelog.ScheduleId;
                                                scheduleLogArchiveRecord.ScheduleName = schedulelog.ScheduleName;
                                                scheduleLogArchiveRecord.BatchId = schedulelog.BatchId;
                                                scheduleLogArchiveRecord.BatchName = schedulelog.BatchName;
                                                scheduleLogArchiveRecord.LogCreationDate = schedulelog.CreationDate;
                                                scheduleLogArchiveRecord.LogFilePath = schedulelog.LogFilePath;
                                                scheduleLogArchiveRecord.NumberOfRetry = schedulelog.NumberOfRetry;
                                                scheduleLogArchiveRecord.Status = schedulelog.Status;
                                                scheduleLogArchiveRecord.TenantCode = schedulelog.TenantCode;
                                                scheduleLogArchiveRecord.ArchivalDate = DateTime.UtcNow;
                                                nISEntitiesDataContext.ScheduleLogArchiveRecords.Add(scheduleLogArchiveRecord);
                                                nISEntitiesDataContext.SaveChanges();
                                            }

                                            if (customerIds.Count > 0)
                                            {
                                                //customerIds.ForEach(customerid =>
                                                //{
                                                //    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                                //    {
                                                //        customerRecord = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.Id == customerid && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                                                //        statementmetadatarecord = nISEntitiesDataContext.StatementMetadataRecords.Where(item => item.CustomerId == customerid && item.TenantCode == tenantCode && item.StatementId == statement.Identifier).ToList().FirstOrDefault();
                                                //    }
                                                //    if (customerRecord != null && statementmetadatarecord != null)
                                                //    {
                                                //        this.RunArchivalForIndivualRecord(statement, statementmetadatarecord, customerRecord, statementPageContents, tenantConfiguration, pdfStatementFilepath, htmlStatementFilepath, client, tenantCode, scheduleLogArchiveRecord, schedulelog);
                                                //    }
                                                //});

                                                ParallelOptions parallelOptions = new ParallelOptions();
                                                parallelOptions.MaxDegreeOfParallelism = ParallelThreadCount;
                                                Parallel.ForEach(customerIds, parallelOptions, customerid =>
                                                {
                                                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                                    {
                                                        customerRecord = nISEntitiesDataContext.CustomerMasterRecords.Where(item => item.Id == customerid && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                                                        statementmetadatarecord = nISEntitiesDataContext.StatementMetadataRecords.Where(item => item.CustomerId == customerid && item.TenantCode == tenantCode && item.StatementId == statement.Identifier).ToList().FirstOrDefault();
                                                    }
                                                    if (customerRecord != null && statementmetadatarecord != null)
                                                    {
                                                        this.RunArchivalForIndivualRecord(statement, statementmetadatarecord, customerRecord, statementPageContents, tenantConfiguration, pdfStatementFilepath, htmlStatementFilepath, client, tenantCode, scheduleLogArchiveRecord, schedulelog);
                                                    }
                                                });

                                                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                                                {
                                                    batchmaster.Status = BatchStatus.Archived.ToString();
                                                    nISEntitiesDataContext.ScheduleLogRecords.Remove(schedulelog);
                                                    nISEntitiesDataContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                            });
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
            bool result = false;
            var scheduleLogArchiveRecords = new List<ScheduleLogArchiveRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                scheduleLogArchives.ToList().ForEach(log =>
                {
                    scheduleLogArchiveRecords.Add(new ScheduleLogArchiveRecord()
                    {
                        BatchId = log.BatchId,
                        BatchName = log.BatchName,
                        ArchivalDate = DateTime.Now,
                        LogFilePath = log.LogFilePath,
                        NumberOfRetry = log.NumberOfRetry,
                        ScheduleId = log.ScheduleId,
                        ScheduleName = log.ScheduleName,
                        Status = log.Status,
                        TenantCode = tenantCode,
                        LogCreationDate = DateTime.UtcNow
                    });
                });
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleLogArchiveRecords.AddRange(scheduleLogArchiveRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
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
            IList<ScheduleLogArchive> scheduleLogs = new List<ScheduleLogArchive>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var scheduleLogRecords = nISEntitiesDataContext.ScheduleLogArchiveRecords.Where(item => item.BatchId == BatchId && item.ScheduleId == ScheduleId && item.TenantCode == tenantCode).ToList();
                    scheduleLogRecords.ForEach(log =>
                    {
                        scheduleLogs.Add(new ScheduleLogArchive()
                        {
                            Identifier = log.Id,
                            BatchId = log.BatchId,
                            BatchName = log.BatchName,
                            ArchivalDate = log.ArchivalDate,
                            LogCreationDate = log.LogCreationDate,
                            LogFilePath = log.LogFilePath,
                            NumberOfRetry = log.NumberOfRetry,
                            ScheduleId = log.ScheduleId,
                            ScheduleName = log.ScheduleName,
                            Status = log.Status,
                            TenantCode = log.TenantCode
                        });
                    });
                }

                return scheduleLogs;
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
            bool result = false;
            var scheduleLogDetailArchieveRecords = new List<ScheduleLogDetailArchiveRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                scheduleLogDetailArchieves.ToList().ForEach(log =>
                {
                    scheduleLogDetailArchieveRecords.Add(new ScheduleLogDetailArchiveRecord()
                    {
                        CustomerId = log.CustomerId,
                        CustomerName = log.CustomerName,
                        ArchivalDate = DateTime.Now,
                        LogMessage = log.LogMessage,
                        NumberOfRetry = log.NumberOfRetry,
                        RenderEngineId = log.RenderEngineId,
                        RenderEngineName = log.RenderEngineName,
                        RenderEngineURL = log.RenderEngineURL,
                        ScheduleId = log.ScheduleId,
                        ScheduleLogArchiveId = log.ScheduleLogArchiveId,
                        PdfStatementPath = log.PdfStatementPath,
                        Status = log.Status,
                        TenantCode = tenantCode,
                        LogDetailCreationDate = DateTime.UtcNow
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.ScheduleLogDetailArchiveRecords.AddRange(scheduleLogDetailArchieveRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
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
            bool result = false;
            var StatementMetadataArchieveRecords = new List<StatementMetadataArchiveRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                statementMetadataArchives.ToList().ForEach(data =>
                {
                    StatementMetadataArchieveRecords.Add(new StatementMetadataArchiveRecord()
                    {
                        AccountNumber = data.AccountNumber,
                        AccountType = data.AccountType,
                        CustomerId = data.CustomerId,
                        CustomerName = data.CustomerName,
                        ScheduleId = data.ScheduleId,
                        ScheduleLogArchiveId = data.ScheduleLogArchiveId,
                        StatementDate = data.StatementDate,
                        StatementId = data.StatementId,
                        StatementPeriod = data.StatementPeriod,
                        StatementURL = data.StatementURL,
                        TenantCode = tenantCode,
                        ArchivalDate = DateTime.UtcNow
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.StatementMetadataArchiveRecords.AddRange(StatementMetadataArchieveRecords);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        #endregion

        #region Private Methods

        private bool RunArchivalForIndivualRecord(Statement statement, StatementMetadataRecord statementmetadatarecord, CustomerMasterRecord customerrecord, IList<StatementPageContent> statementPageContents, TenantConfiguration tenantConfiguration, string pdfStatementFilepath, string htmlStatementFilepath, Client client, string tenantCode, ScheduleLogArchiveRecord scheduleLogArchiveRecord, ScheduleLogRecord schedulelog)
        {
            var tempDir = string.Empty;

            try
            {
                var batchrecord = new BatchMasterRecord();
                var schedulerecord = new ScheduleRecord();
                var batchDetails = new List<BatchDetailRecord>();

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    schedulerecord = nISEntitiesDataContext.ScheduleRecords.Where(item => item.Id == statementmetadatarecord.ScheduleId && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                    batchrecord = nISEntitiesDataContext.BatchMasterRecords.Where(item => item.ScheduleId == schedulerecord.Id && item.Id == customerrecord.BatchId && item.TenantCode == tenantCode).ToList().FirstOrDefault();
                    batchDetails = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == batchrecord.Id && item.TenantCode == tenantCode)?.ToList();
                }

                //Create final output directory to save PDF statement of current customer
                var outputlocation = pdfStatementFilepath + "\\PDF_Statements" + "\\" + "ScheduleId_" + statementmetadatarecord.ScheduleId + "\\" + "BatchId_" + batchrecord.Id + "\\ArchiveData";
                if (!Directory.Exists(outputlocation))
                {
                    Directory.CreateDirectory(outputlocation);
                }

                tempDir = outputlocation + "\\temp_"+ DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                //Create temp output directory to save all neccessories files which requires to genearate PDF statement of current customer
                var samplefilespath = tempDir + "\\" + statementmetadatarecord.Id + "_" + customerrecord.Id +"_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_');
                if (!Directory.Exists(samplefilespath))
                {
                    Directory.CreateDirectory(samplefilespath);
                }

                //Copying all neccessories files which requires to genearate PDF statement of current customer
                this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\css", samplefilespath, false);
                this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\js", samplefilespath, false);
                this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\images", samplefilespath, false);
                this.utility.DirectoryCopy(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\fonts", samplefilespath, false);

                //Gernerate HTML statement of current customer
                this.statementSearchRepository.GenerateHtmlStatementForPdfGeneration(customerrecord, statement, statementPageContents, batchrecord, batchDetails, tenantCode, samplefilespath, client, tenantConfiguration);

                //To insert html statement file of current customer and all required files into the zip file
                var zipfilepath = tempDir + "\\tempzip";
                if (!Directory.Exists(zipfilepath))
                {
                    Directory.CreateDirectory(zipfilepath);
                }
                var zipFile = zipfilepath + "\\" + "StatementZip" + "_" + statementmetadatarecord.Id + "_" + statementmetadatarecord.ScheduleId + "_" + statementmetadatarecord.StatementId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".zip";
                ZipFile.CreateFromDirectory(samplefilespath, zipFile);

                //Convert HTML statement to PDF statement for current customer
                var pdfName = "Statement" + "_" + statementmetadatarecord.ScheduleLogId + "_" + statementmetadatarecord.ScheduleId + statementmetadatarecord.StatementId + "_" + statementmetadatarecord.CustomerId + "_" + DateTime.UtcNow.ToString().Replace("-", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".pdf";
                var result = this.utility.HtmlStatementToPdf(zipFile, outputlocation + "\\" + pdfName);
                if (result)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        //To insert archive schedule log detail records
                        var schedulelogdetails = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.ScheduleLogId == schedulelog.Id && item.ScheduleId == schedulelog.ScheduleId && item.CustomerId == statementmetadatarecord.CustomerId && item.TenantCode == schedulelog.TenantCode).ToList();
                        var scheduleDetailArchiveRecords = new List<ScheduleLogDetailArchiveRecord>();
                        schedulelogdetails.ForEach(logDetail =>
                        {
                            scheduleDetailArchiveRecords.Add(new ScheduleLogDetailArchiveRecord
                            {
                                CustomerId = logDetail.CustomerId,
                                ScheduleId = logDetail.ScheduleId,
                                ScheduleLogArchiveId = scheduleLogArchiveRecord.Id,
                                CustomerName = logDetail.CustomerName,
                                LogDetailCreationDate = logDetail.CreationDate,
                                NumberOfRetry = logDetail.NumberOfRetry,
                                LogMessage = logDetail.LogMessage,
                                PdfStatementPath = outputlocation + "\\" + pdfName,
                                RenderEngineId = logDetail.RenderEngineId,
                                RenderEngineName = logDetail.RenderEngineName,
                                RenderEngineURL = logDetail.RenderEngineURL,
                                Status = logDetail.Status,
                                TenantCode = logDetail.TenantCode,
                                ArchivalDate = DateTime.UtcNow
                            });
                        });
                        nISEntitiesDataContext.ScheduleLogDetailArchiveRecords.AddRange(scheduleDetailArchiveRecords);
                        nISEntitiesDataContext.SaveChanges();

                        //TO insert archive statement metadata records
                        var metadatas = nISEntitiesDataContext.StatementMetadataRecords.Where(item => item.CustomerId == customerrecord.Id && item.ScheduleId == statementmetadatarecord.ScheduleId && item.StatementId == statementmetadatarecord.StatementId && item.ScheduleLogId == statementmetadatarecord.ScheduleLogId && item.TenantCode == statementmetadatarecord.TenantCode).ToList();
                        var archiveMetadatas = new List<StatementMetadataArchiveRecord>();
                        metadatas.ForEach(data =>
                        {
                            archiveMetadatas.Add(new StatementMetadataArchiveRecord
                            {
                                AccountNumber = data.AccountNumber,
                                AccountType = data.AccountType,
                                CustomerId = data.CustomerId,
                                CustomerName = data.CustomerName,
                                ScheduleId = data.ScheduleId,
                                StatementId = data.StatementId,
                                StatementURL = outputlocation + "\\" + pdfName,
                                StatementDate = data.StatementDate,
                                StatementPeriod = data.StatementPeriod,
                                TenantCode = data.TenantCode,
                                ScheduleLogArchiveId = scheduleLogArchiveRecord.Id,
                                ArchivalDate = DateTime.UtcNow
                            });
                        });
                        nISEntitiesDataContext.StatementMetadataArchiveRecords.AddRange(archiveMetadatas);
                        nISEntitiesDataContext.SaveChanges();

                        //TO delete actual schedule log details, and statement metadata records
                        nISEntitiesDataContext.ScheduleLogDetailRecords.RemoveRange(schedulelogdetails);
                        nISEntitiesDataContext.StatementMetadataRecords.RemoveRange(metadatas);
                        nISEntitiesDataContext.SaveChanges();
                    }

                    //To delete actual HTML statement of currrent customer, once the PDF statement genearated
                    var htmlStatementDirPath = statementmetadatarecord.StatementURL.Substring(0, statementmetadatarecord.StatementURL.LastIndexOf("\\"));
                    DirectoryInfo directoryInfo = new DirectoryInfo(htmlStatementDirPath);
                    if (directoryInfo.Exists)
                    {
                        directoryInfo.Delete(true);
                    }
                }

                return true;
            }
            catch (IOException ioe)
            {
                throw ioe;
            }
            catch (Exception ex)
            {
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
        }

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
    }
}
