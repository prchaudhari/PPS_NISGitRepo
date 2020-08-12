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

        #endregion

        #region Constructor

        public StatementSearchController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.StatementSearchManager = new StatementSearchManager(this.unityContainer);
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
        [Route("ScheduleHistory/Download")]
        public HttpResponseMessage Download(string identifier)
        {
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                string path = string.Empty;
                StatementSearch statement = this.StatementSearchManager.GetStatementSearchs(new StatementSearchSearchParameter()
                {
                    Identifier = identifier,
                    SortParameter = new SortParameter() { SortColumn = ModelConstant.SORT_COLUMN }
                }, tenantCode).FirstOrDefault();


                string relativePath = HttpContext.Current.Server.MapPath("~") + ModelConstant.ASSETPATHSLASH;
                string FileName = statement.StatementURL.Split('\'').ToList().LastOrDefault();
                path = statement.StatementURL.Replace("\'", "/");
                path = relativePath + ModelConstant.ASSETPATHSLASH + path;

                if (!File.Exists(path))
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);

                            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                            httpResponseMessage.Content = new ByteArrayContent(bytes.ToArray());
                            httpResponseMessage.Content.Headers.Add("x-filename", FileName);
                            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                            httpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
                            httpResponseMessage.StatusCode = HttpStatusCode.OK;
                            return httpResponseMessage;
                        }
                    }
                }
                catch (IOException)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #endregion
    }
}