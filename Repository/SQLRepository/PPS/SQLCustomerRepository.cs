// <copyright file="SQLAnalyticsDataRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
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
    /// <seealso cref="ICustomerRepository" />
    public class SQLCustomerRepository : ICustomerRepository
    {

        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

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
        public SQLCustomerRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Functions

        ///// <summary>
        ///// Gets the customers by invester identifier.
        ///// </summary>
        ///// <param name="investorId">The investor identifier.</param>
        ///// <param name="tenantCode">The tenant code.</param>
        ///// <returns></returns>
        ///// <exception cref="NedBankException.RepositoryStoreNotAccessibleException"></exception>
        //public IList<CustomerInformation> GetCustomersByInvesterId(long investorId, string tenantCode)
        //{
        //    IList<CustomerInformation> customers = new List<CustomerInformation>();
        //    IList<NB_CustomerMaster> customerRecords = null;
        //    try
        //    {
        //        this.SetAndValidateConnectionString(tenantCode);
        //        using (NISEntities nedbankEntities = new NISEntities(this.connectionString))
        //        {
        //            string whereClause = this.WhereClauseGenerator(investorId, tenantCode);
        //            customerRecords = new List<NB_CustomerMaster>();
        //            customerRecords = nedbankEntities.NB_CustomerMaster.Where(whereClause).ToList();
        //        }
        //        IList<CustomerInformation> tempCustomers = new List<CustomerInformation>();
        //        customerRecords?.ToList().ForEach(customerRecord =>
        //        {
        //            tempCustomers.Add(new CustomerInformation()
        //            {
        //                Id = customerRecord.Id,
        //                BatchId = customerRecord.BatchId,
        //                CustomerId = customerRecord.CustomerId,
        //                InvestorId = customerRecord.InvestorId,
        //                BranchId = customerRecord.BranchId,
        //                Title = customerRecord.Title,
        //                FirstName = customerRecord.FirstName,
        //                SurName = customerRecord.SurName,
        //                AddressLine0 = customerRecord.AddressLine0,
        //                AddressLine1 = customerRecord.AddressLine1,
        //                AddressLine2 = customerRecord.AddressLine2,
        //                AddressLine3 = customerRecord.AddressLine3,
        //                AddressLine4 = customerRecord.AddressLine4,
        //                EmailAddress = customerRecord.EmailAddress,
        //                MaskCellNo = customerRecord.MaskCellNo,
        //                Barcode = customerRecord.Barcode,
        //                TenantCode = customerRecord.TenantCode,
        //            });
        //        });
        //        customers = tempCustomers;
        //    }
        //    catch (SqlException)
        //    {
        //        throw new RepositoryStoreNotAccessibleException(tenantCode);
        //    }
        //    catch (Exception exception)
        //    {
        //        throw exception;
        //    }
        //    return customers;
        //}
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
        /// <param name="investorId">The investor identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(long investorId, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
            {
                queryString.Append(string.Format(" TenantCode.Equals(\"{0}\") and", tenantCode));
            }
            queryString.Append(string.Format(" InvestorId.Equals({0})", investorId));

            return queryString.ToString();
        }

        #endregion
    }
}