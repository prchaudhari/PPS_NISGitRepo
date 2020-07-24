// <copyright file="StatementController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Unity;
    #endregion

    /// <summary>
    /// This class represent api controller for Statement
    /// </summary>
    public class StatementController: ApiController
    {
        #region Private Members

        /// <summary>
        /// The Statement manager object.
        /// </summary>
        private StatementManager StatementManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public StatementController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.StatementManager = new StatementManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to add Statements
        /// </summary>
        /// <param name="Statements"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<Statement> Statements)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.StatementManager.AddStatements(Statements, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update Statements.
        /// </summary>
        /// <param name="Statements"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<Statement> Statements)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.StatementManager.UpdateStatements(Statements, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete Statements.
        /// </summary>
        /// <param name="StatementIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(long StatementIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.StatementManager.DeleteStatements(StatementIdentifier, tenantCode);
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
        public IList<Statement> List(StatementSearchParameter StatementSearchParameter)
        {
            IList<Statement> Statements = new List<Statement>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                Statements = this.StatementManager.GetStatements(StatementSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.StatementManager.GetStatementCount(StatementSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Statements;
        }

        /// <summary>
        /// This method helps to publish Statement.
        /// </summary>
        /// <param name="StatementIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Publish(long StatementIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.StatementManager.PublishStatement(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to preview Statement.
        /// </summary>
        /// <param name="StatementIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public string Preview(long StatementIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                var baseURL = Url.Content("~/");
                return this.StatementManager.PreviewStatement(StatementIdentifier, baseURL, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to clone Statement.
        /// </summary>
        /// <param name="StatementIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Clone(long StatementIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.StatementManager.CloneStatement(StatementIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion
    }
}