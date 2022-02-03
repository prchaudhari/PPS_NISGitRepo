// <copyright file="UserController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace NedbankAPI.Controllers
{
    #region References

    using NedbankModel;
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Unity;
    using NedBankManager;

    #endregion

    /// <summary>
    /// This class represent api controller for NedBank
    /// </summary>
    [RoutePrefix("Customer")]
    public class CustomerController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The role manager object.
        /// </summary>
        private CustomerManager customerManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public CustomerController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.customerManager = new CustomerManager(this.unityContainer);
        }

        #endregion

        #region Get

        #region List

        /// <summary>
        /// This api call use to get single or list of customer
        /// </summary>
        /// <param name="customers">
        /// List of customers
        /// </param>
        /// <returns>Returns list of customers</returns>
        [HttpPost]
        [Route("List")]
        public IList<CustomerInformation> List(long investorId)
        {
            IList<CustomerInformation> customers = new List<CustomerInformation>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                customers = this.customerManager.GetCustomersByInvesterId(investorId, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return customers;
        }

        #endregion
        #endregion
    }
}
