// <copyright file="TenantTransactionDataController.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represent api controller for tenant transaction data
    /// </summary>
    public class TenantTransactionDataController: ApiController
    {
        #region Private Members

        /// <summary>
        /// The tenant transaction data manager object.
        /// </summary>
        private TenantTransactionDataManager tenantTransactionDataManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public TenantTransactionDataController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.tenantTransactionDataManager = new TenantTransactionDataManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method gets the specified list of customer master from tenant transaction data repository.
        /// </summary>
        /// <param name="customerSearchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        [HttpPost]
        public IList<CustomerMaster> Get_TTD_CustomerMasters(CustomerSearchParameter customerSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_CustomerMasters(customerSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of subscription master from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        [HttpPost]
        public IList<SubscriptionMaster> Get_TTD_SubscriptionMasters(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_SubscriptionMasters(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of subscription usage from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of subscription usage
        /// </returns>
        [HttpPost]
        public IList<SubscriptionUsage> Get_TTD_SubscriptionUsages(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_SubscriptionUsages(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of subscription summaries from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of subscription summeries
        /// </returns>
        [HttpPost]
        public IList<SubscriptionSummary> Get_TTD_SubscriptionSummaries(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_SubscriptionSummaries(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of subscription spends from tenant transaction data repository.
        /// </summary>
        /// <param name="searchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of subscription spends
        /// </returns>
        [HttpPost]
        public IList<SubscriptionSpend> Get_TTD_SubscriptionSpends(SubscriptionMasterSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_SubscriptionSpends(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of user subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="searchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of user subscriptions
        /// </returns>
        [HttpPost]
        public IList<UserSubscription> Get_TTD_UserSubscriptions(SubscriptionMasterSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_UserSubscriptions(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of vendor subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="searchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of vendor subscriptions
        /// </returns>
        [HttpPost]
        public IList<VendorSubscription> Get_TTD_VendorSubscriptions(SubscriptionMasterSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_VendorSubscriptions(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of data usages from tenant transaction data repository.
        /// </summary>
        /// <param name="searchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of data usages
        /// </returns>
        [HttpPost]
        public IList<DataUsage> Get_TTD_DataUsages(SubscriptionMasterSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_DataUsages(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of meeting usages from tenant transaction data repository.
        /// </summary>
        /// <param name="searchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of meeting usages
        /// </returns>
        [HttpPost]
        public IList<MeetingUsage> Get_TTD_MeetingUsages(SubscriptionMasterSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_MeetingUsages(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of emails by subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="searchParameter">The subscription master search parameter</param>
        /// <returns>
        /// Returns the list of emails by subscription
        /// </returns>
        [HttpPost]
        public IList<EmailsBySubscription> Get_TTD_EmailsBySubscription(SubscriptionMasterSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_TTD_EmailsBySubscription(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}