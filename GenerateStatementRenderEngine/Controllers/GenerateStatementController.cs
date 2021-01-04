// <copyright file="GenerateStatementController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace GenerateStatementRenderEngine
{
    #region References
    using nIS;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Unity;
    #endregion

    public class GenerateStatementController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The genearate statement manager object.
        /// </summary>
        private GenerateStatementManager generateStatementManager = null;

        #endregion

        #region Constructor

        public GenerateStatementController(IUnityContainer unityContainer)
        {
            this.generateStatementManager = new GenerateStatementManager(unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method generate respective customer's HTML statement file.
        /// </summary>
        /// <param name="GenerateStatementRawData">The raw data object required for statement generation process</param>
        [HttpPost]
        public void CreateCustomerStatement(GenerateStatementRawData statementRawData)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.generateStatementManager.CreateCustomerStatement(statementRawData, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to retry to generate HTML statement for failed customers.
        /// </summary>
        /// <param name="statementRawData">The raw data object required for statement generation process</param>
        [HttpPost]
        public void RetryToCreateFailedCustomerStatements(GenerateStatementRawData statementRawData)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.generateStatementManager.RetryToCreateFailedCustomerStatements(statementRawData, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to convert HTML statement to PDF statement and archive related data for the customer.
        /// </summary>
        /// <param name="archivalProcessRawData">The raw data object required for archival process</param>
        [HttpPost]
        public void RunArchivalForCustomerRecord(ArchivalProcessRawData archivalProcessRawData)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                this.generateStatementManager.RunArchivalForCustomerRecord(archivalProcessRawData, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}