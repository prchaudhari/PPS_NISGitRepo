// <copyright file="RoleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedBankManager
{
    #region References

    using Microsoft.Practices.Unity;
    using NedbankModel;
    using NedbankRepository;
    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This class implements manager layer of customer manager.
    /// </summary>
    public class CustomerManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The customer repository.
        /// </summary>
        ICustomerRepository customerRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public CustomerManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.customerRepository = this.unityContainer.Resolve<ICustomerRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// This method helps to get specified customer.
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param> 
        /// <param name="investorId">
        /// The investor id
        /// </param> 
        /// <returns>
        /// Returns a list of customer if any found otherwise it will return enpty list.
        /// </returns>
        public IList<CustomerInformation> GetCustomersByInvesterId(string tenantCode, int investorId)
        {
            try
            {
                IList<CustomerInformation> customers = this.customerRepository.GetCustomersByInvesterId(tenantCode, investorId);

                return customers;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion

    }
}
