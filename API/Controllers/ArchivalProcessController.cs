// <copyright file="ArchivalProcessController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
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
    /// This class represent api controller for archival process
    /// </summary>
    public class ArchivalProcessController: ApiController
    {
        #region Private Members

        /// <summary>
        /// The archival process manager object.
        /// </summary>
        private ArchivalProcessManager archivalProcessManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        /// <summary>
        /// The tenant config manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        #endregion

        #region Constructor

        public ArchivalProcessController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
            this.archivalProcessManager = new ArchivalProcessManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to run the schedule
        /// </summary>
        /// <returns>True if schedule runs successfully false otherwise</returns>
        [HttpPost]
        public bool RunArchivalProcess()
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var htmlStatementFilepath = Url.Content("~/");
                var pdfStatementFilepath = AppDomain.CurrentDomain.BaseDirectory;
                return this.archivalProcessManager.RunArchivalProcessNew(pdfStatementFilepath, htmlStatementFilepath, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}