// <copyright file="StatementSearchManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class StatementSearchManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The StatementSearch repository.
        /// </summary>
        IStatementSearchRepository StatementSearchRepository = null;

        IPageRepository pageRepository = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public StatementSearchManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.StatementSearchRepository = this.unityContainer.Resolve<IStatementSearchRepository>();
                this.pageRepository = this.unityContainer.Resolve<IPageRepository>();
                this.validationEngine = new ValidationEngine();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will call add StatementSearchs method of repository.
        /// </summary>
        /// <param name="StatementSearchs">StatementSearchs are to be add.</param>
        /// <param name="tenantCode">Tenant code of StatementSearch.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddStatementSearchs(IList<StatementSearch> StatementSearchs, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.StatementSearchRepository.AddStatementSearchs(StatementSearchs, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      

        /// <summary>
        /// This method will call get StatementSearchs method of repository.
        /// </summary>
        /// <param name="StatementSearchSearchParameter">The StatementSearch search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns StatementSearchs if found for given parameters, else return null
        /// </returns>
        public IList<StatementSearch> GetStatementSearchs(StatementSearchSearchParameter StatementSearchSearchParameter, string tenantCode)
        {
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    StatementSearchSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

               
                return this.StatementSearchRepository.GetStatementSearchs(StatementSearchSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to get count of StatementSearchs.
        /// </summary>
        /// <param name="StatementSearchSearchParameter">The StatementSearch search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of StatementSearchs
        /// </returns>
        public int GetStatementSearchCount(StatementSearchSearchParameter StatementSearchSearchParameter, string tenantCode)
        {
            int roleCount = 0;
            try
            {
                roleCount = this.StatementSearchRepository.GetStatementSearchCount(StatementSearchSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }


        /// <summary>
        /// This method reference to generate html statement for export to pdf
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns>output location of html statmeent file</returns>
        public string GenerateStatement(long identifier, string tenantCode)
        {
            try
            {
                return this.StatementSearchRepository.GenerateStatement(identifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
