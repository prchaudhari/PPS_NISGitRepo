// <copyright file="SQLAnalyticsDataRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace NedbankRepository
{
    using NedBankException;
    using NedbankModel;
    using NedbankUtility;
    #region References
    using NedBankValidationEngine;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents repository layer of accet library for crud operation.
    /// </summary>
    public class SQLCustomerRepository : ICustomerRepository
    {

        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        INedBankValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        public SQLCustomerRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new NedBankValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Functions

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

        #region Private Method

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
                this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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