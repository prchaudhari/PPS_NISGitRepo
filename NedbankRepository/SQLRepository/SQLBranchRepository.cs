// <copyright file="SQLAnalyticsDataRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace NedbankRepository
{
    #region References
    using NedBankException;
    using NedbankModel;
    using NedbankUtility;
    using NedBankValidationEngine;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents repository layer of accet library for crud operation.
    /// </summary>
    /// <seealso cref="NedbankRepository.IBranchRepository" />
    public class SQLBranchRepository : IBranchRepository
    {

        #region Private Members

        NedbankEntities db;

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

        /// <summary>
        /// The configurationutility
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLBranchRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new NedBankValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);

            db = new NedbankEntities();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Get branch by id
        /// </summary>
        /// <param name="branchId"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BranchInformation GetBranchById(long branchId, string tenantCode)
        {
            var branchRecord = db.NB_BranchMaster.Where(m => m.Id == branchId && m.TenantCode == tenantCode).FirstOrDefault();

            return new BranchInformation()
            {
                AddressLine0 = branchRecord.AddressLine0,
                BranchId = branchRecord.BranchId,
                AddressLine1 = branchRecord.AddressLine1,
                AddressLine2 = branchRecord.AddressLine2,
                AddressLine3 = branchRecord.AddressLine3,
                AddressLine4 = branchRecord.AddressLine4,
                ContactNo = branchRecord.ContactNo,
                Id = branchRecord.Id,
                Name = branchRecord.Name,
                TenantCode = tenantCode,
                VatRegNo = branchRecord.VatRegNo
            };
        }


        #endregion

        #region Private Method

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <exception cref="NedBankException.ConnectionStringNotFoundException"></exception>
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

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="branchId">The investor identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(long branchId, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
            {
                queryString.Append(string.Format(" TenantCode.Equals(\"{0}\") and", tenantCode));
            }
            queryString.Append(string.Format(" BranchId.Equals({0})", branchId));

            return queryString.ToString();
        }

        #endregion
    }
}