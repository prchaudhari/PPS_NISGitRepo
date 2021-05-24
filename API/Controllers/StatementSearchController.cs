// <copyright file="StatementSearchController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.IO.Compression;
    using Unity;

    #endregion


    /// <summary>
    /// This class represent api controller for StatementSearch
    /// </summary>
    public class StatementSearchController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The StatementSearch manager object.
        /// </summary>
        private StatementSearchManager StatementSearchManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The asset library manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        /// <summary>
        /// The crypto manager
        /// </summary>
        private readonly ICryptoManager cryptoManager;

        /// <summary>
        /// The system activity history manager object.
        /// </summary>
        private SystemActivityHistoryManager systemActivityHistoryManager = null;

        #endregion

        #region Constructor

        public StatementSearchController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.utility = new Utility();
            this.StatementSearchManager = new StatementSearchManager(this.unityContainer);
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
            this.cryptoManager = this.unityContainer.Resolve<ICryptoManager>();
            this.systemActivityHistoryManager = new SystemActivityHistoryManager(unityContainer);

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to add StatementSearchs
        /// </summary>
        /// <param name="StatementSearchs"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<StatementSearch> StatementSearchs)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.StatementSearchManager.AddStatementSearchs(StatementSearchs, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get roles list based on the search parameters.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <returns>List of roles</returns>
        [HttpPost]
        public IList<StatementSearch> List(StatementSearchSearchParameter StatementSearchSearchParameter)
        {
            IList<StatementSearch> StatementSearchs = new List<StatementSearch>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                StatementSearchs = this.StatementSearchManager.GetStatementSearchs(StatementSearchSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.StatementSearchManager.GetStatementSearchCount(StatementSearchSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return StatementSearchs;
        }

        #region Download

        [HttpGet]
        [Route("StatementSearch/Download")]
        public HttpResponseMessage Download(string identifier)
        {
            string zipFile = string.Empty;
            string destinationFolder = string.Empty;
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                StatementSearch statement = this.StatementSearchManager.GetStatementSearchs(new StatementSearchSearchParameter()
                {
                    Identifier = identifier,
                    SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
                }, tenantCode).FirstOrDefault();

                string FileName = statement.StatementURL.Split('\\').ToList().LastOrDefault();
                if (File.Exists(statement.StatementURL))
                {
                    DirectoryInfo di = new DirectoryInfo(statement.StatementURL);
                    var customerId = di.Parent.Name;
                    var batchId = di.Parent.Parent.Name;
                    var batchFolderPath = di.Parent.Parent.FullName;
                    string batchURL = di.Parent.Parent.Parent.FullName + "\\" + batchId;
                    if (Directory.Exists(batchURL + "\\" + "common"))
                    {
                        string tempStatement = "tempStatement" + identifier;
                        var commonFolder = batchURL + "\\" + "common";
                        var customerFolder = batchURL + "\\" + customerId;
                        destinationFolder = batchURL + "\\" + tempStatement;
                        // If the destination directory doesn't exist, create it.
                        if (!Directory.Exists(destinationFolder))
                        {
                            Directory.CreateDirectory(destinationFolder);
                        }
                        if (!Directory.Exists(destinationFolder + "\\" + "common"))
                        {
                            Directory.CreateDirectory(destinationFolder + "\\" + "common");
                        }
                        if (!Directory.Exists(destinationFolder + "\\" + customerId))
                        {
                            Directory.CreateDirectory(destinationFolder + "\\" + customerId);
                        }
                        this.utility.DirectoryCopy(commonFolder, (destinationFolder + "\\" + "common"), true);
                        this.utility.DirectoryCopy(customerFolder, (destinationFolder + "\\" + customerId), true);
                        zipFile = batchURL + "\\" + "tempStatementZip" + identifier + ".zip";
                        ZipFile.CreateFromDirectory(destinationFolder, zipFile);
                        if (File.Exists(zipFile))
                        {
                            if (!File.Exists(zipFile))
                            {
                                throw new HttpResponseException(HttpStatusCode.NotFound);
                            }
                            try
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    using (FileStream file = new FileStream(zipFile, FileMode.Open, FileAccess.Read))
                                    {
                                        byte[] bytes = new byte[file.Length];
                                        file.Read(bytes, 0, (int)file.Length);
                                        ms.Write(bytes, 0, (int)file.Length);
                                        FileName = FileName.Replace(".html", ".zip");
                                        httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                                        httpResponseMessage.Content.Headers.Add("x-filename", FileName);
                                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
                                        httpResponseMessage.StatusCode = HttpStatusCode.OK;
                                    }

                                    IList<SystemActivityHistory> activityHistories = new List<SystemActivityHistory>();
                                    activityHistories.Add(new SystemActivityHistory()
                                    {
                                        Module = "Statement Search",
                                        EntityId = statement.Identifier,
                                        EntityName = "BatchId: " + statement.BatchId + "_BatchName: " + statement.BatchName,
                                        SubEntityId = statement.CustomerId,
                                        SubEntityName = statement.CustomerName,
                                        ActionTaken = "HtmlStatementDownload",
                                    });
                                    this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);
                                }
                                return httpResponseMessage;
                            }
                            catch (IOException)
                            {
                                throw new HttpResponseException(HttpStatusCode.InternalServerError);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }
                if (Directory.Exists(destinationFolder))
                {
                    Directory.Delete(destinationFolder, true);
                }
            }
            return httpResponseMessage;
        }

        [HttpGet]
        [Route("StatementSearch/ExportToPDF")]
        public HttpResponseMessage ExportHtmlToPDF(long identifier)
        {
            string zipFile = string.Empty;
            string outputpath = string.Empty;
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                StatementSearch statement = this.StatementSearchManager.GetStatementSearchs(new StatementSearchSearchParameter()
                {
                    Identifier = identifier.ToString(),
                    SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
                }, tenantCode).FirstOrDefault();

                outputpath = this.StatementSearchManager.GenerateStatementNew(statement, tenantCode);
                
                zipFile = AppDomain.CurrentDomain.BaseDirectory + "\\Resources" + "\\" + "tempStatementZip" + identifier + ".zip";
                ZipFile.CreateFromDirectory(outputpath, zipFile);
                var pdfName = "Statement_" + DateTime.Now.ToShortDateString().Replace(" - ", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".pdf";
                string password = string.Empty;
                if (statement.IsPasswordGenerated)
                {
                    password = this.cryptoManager.Decrypt(statement.Password);
                }
                var isPdfSuccess = this.utility.HtmlStatementToPdf(zipFile, outputpath + "\\" + pdfName,password);
                if (isPdfSuccess)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream((outputpath + "\\" + pdfName), FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", pdfName);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = pdfName;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                        }
                    }
                }

                IList<SystemActivityHistory> activityHistories = new List<SystemActivityHistory>();
                activityHistories.Add(new SystemActivityHistory()
                {
                    Module = "Statement Search",
                    EntityId = statement.Identifier,
                    EntityName = "BatchId: " + statement.BatchId + "_BatchName: " + statement.BatchName,
                    SubEntityId = statement.CustomerId,
                    SubEntityName = statement.CustomerName,
                    ActionTaken = "PdfStatementDownload",
                });
                this.systemActivityHistoryManager.SaveSystemActivityHistoryDetails(activityHistories, tenantCode);

                return httpResponseMessage;
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            finally
            {
                //if (File.Exists(zipFile))
                //{
                //    File.Delete(zipFile);
                //}
                //if (Directory.Exists(outputpath))
                //{
                //    Directory.Delete(outputpath, true);
                //}
            }
        }

        #endregion
        #endregion
    }
}