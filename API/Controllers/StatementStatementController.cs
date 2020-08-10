// <copyright file="StatementSearchController.cs" company="Websym Solutions Pvt Ltd">
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
        #endregion
    }
}