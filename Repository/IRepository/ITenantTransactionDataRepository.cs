// <copyright file="ITenantTransactionDataRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System.Collections.Generic;
    #endregion

    public interface ITenantTransactionDataRepository
    {
        /// <summary>
        /// This method gets the specified list of subscription master from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of statements
        /// </returns>
        IList<SubscriptionMaster> Get_TTD_SubscriptionMasters(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of subscription usage from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription usage
        /// </returns>
        IList<SubscriptionUsage> Get_TTD_SubscriptionUsages(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of subscription summaries from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription summeries
        /// </returns>
        IList<SubscriptionSummary> Get_TTD_SubscriptionSummaries(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of subscription spends from tenant transaction data repository.
        /// </summary>
        /// <param name="month">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription spends
        /// </returns>
        IList<SubscriptionSpend> Get_TTD_SubscriptionSpends(string month, string tenantCode);

        /// <summary>
        /// This method gets the specified list of user subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="employeeId">The employee id value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of user subscriptions
        /// </returns>
        IList<UserSubscription> Get_TTD_UserSubscriptions(string employeeId, string tenantCode);

        /// <summary>
        /// This method gets the specified list of vendor subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="VendorName">The vendor name value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of vendor subscriptions
        /// </returns>
        IList<VendorSubscription> Get_TTD_VendorSubscriptions(string VendorName, string tenantCode);

        /// <summary>
        /// This method gets the specified list of data usages from tenant transaction data repository.
        /// </summary>
        /// <param name="month">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of data usages
        /// </returns>
        IList<DataUsage> Get_TTD_DataUsages(string month, string tenantCode);

        /// <summary>
        /// This method gets the specified list of meeting usages from tenant transaction data repository.
        /// </summary>
        /// <param name="month">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of meeting usages
        /// </returns>
        IList<MeetingUsage> Get_TTD_MeetingUsages(string month, string tenantCode);

        /// <summary>
        /// This method gets the specified list of emails by subscription from tenant transaction data repository.
        /// </summary>
        /// <param name="subscription">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of emails by subscription
        /// </returns>
       IList<EmailsBySubscription> Get_TTD_EmailsBySubscription(string subscription, string tenantCode);
    }
}
