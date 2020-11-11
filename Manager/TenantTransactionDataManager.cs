// <copyright file="TenantTransactionDataManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    #endregion

    public class TenantTransactionDataManager
    {
        #region Private members
        
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The Tenant transaction data repository.
        /// </summary>
        ITenantTransactionDataRepository tenantTransactionDataRepository = null;

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for tenant transaction data manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="unityContainer">IUnity container implementation object.</param>
        public TenantTransactionDataManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.tenantTransactionDataRepository = this.unityContainer.Resolve<ITenantTransactionDataRepository>();
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
        /// This method gets the specified list of customer master from tenant transaction data repository.
        /// </summary>
        /// <param name="customerSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        public IList<CustomerMaster> Get_TTD_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_CustomerMasters(customerSearchParameter, tenantCode);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of subscription master from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        public IList<SubscriptionMaster> Get_TTD_SubscriptionMasters(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_SubscriptionMasters(subscriptionMasterSearchParameter, tenantCode);
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription usage
        /// </returns>
        public IList<SubscriptionUsage> Get_TTD_SubscriptionUsages(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_SubscriptionUsages(subscriptionMasterSearchParameter, tenantCode);
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription summeries
        /// </returns>
        public IList<SubscriptionSummary> Get_TTD_SubscriptionSummaries(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_SubscriptionSummaries(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of subscription spends from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription spends
        /// </returns>
        public IList<SubscriptionSpend> Get_TTD_SubscriptionSpends(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_SubscriptionSpends(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of user subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of user subscriptions
        /// </returns>
        public IList<UserSubscription> Get_TTD_UserSubscriptions(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_UserSubscriptions(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of vendor subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of vendor subscriptions
        /// </returns>
        public IList<VendorSubscription> Get_TTD_VendorSubscriptions(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_VendorSubscriptions(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of data usages from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of data usages
        /// </returns>
        public IList<DataUsage> Get_TTD_DataUsages(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_DataUsages(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of meeting usages from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of meeting usages
        /// </returns>
        public IList<MeetingUsage> Get_TTD_MeetingUsages(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_MeetingUsages(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of emails by subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of emails by subscription
        /// </returns>
        public IList<EmailsBySubscription> Get_TTD_EmailsBySubscription(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_TTD_EmailsBySubscription(subscriptionMasterSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer master from tenant transaction data repository.
        /// </summary>
        /// <param name="customerSearchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        public IList<CustomerMaster> Get_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_CustomerMasters(customerSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer account master from tenant transaction data repository.
        /// </summary>
        /// <param name="accountSearchParameter">The account search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account master
        /// </returns>
        public IList<AccountMaster> Get_AccountMaster(CustomerAccountSearchParameter accountSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_AccountMaster(accountSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer account transaction from tenant transaction data repository.
        /// </summary>
        /// <param name="accountSearchParameter">The account search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account transaction
        /// </returns>
        public IList<AccountTransaction> Get_AccountTransaction(CustomerAccountSearchParameter accountSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_AccountTransaction(accountSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer account saving and spending trend from tenant transaction data repository.
        /// </summary>
        /// <param name="accountSearchParameter">The account search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account saving and spending trend
        /// </returns>
        public IList<SavingTrend> Get_SavingTrend(CustomerAccountSearchParameter accountSearchParameter, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.Get_SavingTrend(accountSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
