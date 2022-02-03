// <copyright file="SQLCustomerRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// ----------------------------------------------------------------------- 

namespace NedbankRepository
{
    #region References

    using System.Collections.Generic;
    using NedbankModel;
    using Microsoft.Practices.Unity;
    using System;
    using System.Data.SqlClient;
    using NedBankValidationEngine;
    using NedBankException;
    using NedBankUtility;

    #endregion
    public class SQLCustomerRepository : ICustomerRepository
    {
        #region Private Members

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private INedBankValidationEngine nedBankValidationEngine = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;
        #endregion

        #region Constructor

        public SQLCustomerRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.nedBankValidationEngine = new NedBankValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);

        }

        #endregion

        #region Get

        /// <summary>
        /// This method helps to retrieve list of customer.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <param name="investorId">The investor id</param>
        /// <returns></returns>
        public IList<CustomerInformation> GetCustomersByInvesterId(string tenantCode, int investorId)
        {
            IList<CustomerInformation> customers = new List<CustomerInformation>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return customers;
        }

        #endregion

        #region Get Connection String

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
                this.connectionString = nedBankValidationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.nedBankValidationEngine.IsValidText(this.connectionString))
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
