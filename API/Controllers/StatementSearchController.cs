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

        #endregion

        #region Constructor

        public StatementSearchController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.utility = new Utility();
            this.StatementSearchManager = new StatementSearchManager(this.unityContainer);
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);

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
                //HttpContext.Current.Response.AppendHeader("recordCount", this.StatementSearchManager.GetStatementSearchCount(StatementSearchSearchParameter, tenantCode).ToString());
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
                        var destinationFolder = batchURL + "\\" + tempStatement;
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
                        string zipFile = batchURL + "\\" + "tempStatementZip" + identifier + ".zip";
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
                                }
                                if (File.Exists(zipFile))
                                {
                                    File.Delete(zipFile);
                                }
                                if (Directory.Exists(destinationFolder))
                                {
                                    Directory.Delete(destinationFolder, true);
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
                outputpath = this.StatementSearchManager.GenerateStatement(identifier, tenantCode);
                zipFile = AppDomain.CurrentDomain.BaseDirectory + "\\Resources" + "\\" + "tempStatementZip" + identifier + ".zip";
                ZipFile.CreateFromDirectory(outputpath, zipFile);
                var pdfName = "Statement" + DateTime.Now.ToShortDateString().Replace(" - ", "_").Replace(":", "_").Replace(" ", "_").Replace('/', '_') + ".pdf";
                this.HtmlStatementToPdf(zipFile, outputpath + "\\" + pdfName);
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
                return httpResponseMessage;
            }
            catch (IOException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
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
                if (Directory.Exists(outputpath))
                {
                    Directory.Delete(outputpath, true);
                }
            }
        }

        private bool HtmlStatementToPdf(string htmlStatementPath, string outPdfPath)
        {
            var isPdfSuccess = false;
            try
            {
                var client = new pdfcrowd.HtmlToPdfClient("demo", "ce544b6ea52a5621fb9d55f8b542d14d");
                client.setPageWidth("12in");
                client.setPageHeight("10in");
                client.setRenderingMode("viewport");
                client.setSmartScalingMode("content-fit");
                client.setJpegQuality(80);
                client.setConvertImagesToJpeg("all");
                client.setImageDpi(340);
                client.convertFileToFile(htmlStatementPath, outPdfPath);
            }
            catch (pdfcrowd.Error why)
            {
                throw why;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isPdfSuccess;
        }

        #endregion
        #endregion
    }
}