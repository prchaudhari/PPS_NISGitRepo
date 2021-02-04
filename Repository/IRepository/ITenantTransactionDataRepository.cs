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
        /// This method gets the specified list of customer master from tenant transaction data repository.
        /// </summary>
        /// <param name="customerSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        //IList<CustomerMaster> Get_TTD_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of subscription master from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of statements
        ///// </returns>
        //IList<SubscriptionMaster> Get_TTD_SubscriptionMasters(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of subscription usage from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of subscription usage
        ///// </returns>
        //IList<SubscriptionUsage> Get_TTD_SubscriptionUsages(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of subscription summaries from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of subscription summeries
        ///// </returns>
        //IList<SubscriptionSummary> Get_TTD_SubscriptionSummaries(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of subscription spends from tenant transaction data repository.
        ///// </summary>
        ///// <param name="month">The month value</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of subscription spends
        ///// </returns>
        //IList<SubscriptionSpend> Get_TTD_SubscriptionSpends(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of user subscription from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of user subscriptions
        ///// </returns>
        //IList<UserSubscription> Get_TTD_UserSubscriptions(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of vendor subscription from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of vendor subscriptions
        ///// </returns>
        //IList<VendorSubscription> Get_TTD_VendorSubscriptions(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of data usages from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of data usages
        ///// </returns>
        //IList<DataUsage> Get_TTD_DataUsages(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of meeting usages from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of meeting usages
        ///// </returns>
        //IList<MeetingUsage> Get_TTD_MeetingUsages(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        ///// <summary>
        ///// This method gets the specified list of emails by subscription from tenant transaction data repository.
        ///// </summary>
        ///// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of emails by subscription
        ///// </returns>
        //IList<EmailsBySubscription> Get_TTD_EmailsBySubscription(TransactionDataSearchParameter subscriptionMasterSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer master from tenant transaction data repository.
        /// </summary>
        /// <param name="customerSearchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        IList<CustomerMaster> Get_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer account master from tenant transaction data repository.
        /// </summary>
        /// <param name="accountSearchParameter">The account search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account master
        /// </returns>
        IList<AccountMaster> Get_AccountMaster(CustomerAccountSearchParameter accountSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer account transaction from tenant transaction data repository.
        /// </summary>
        /// <param name="accountSearchParameter">The account search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account transaction
        /// </returns>
        IList<AccountTransaction> Get_AccountTransaction(CustomerAccountSearchParameter accountSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer account saving and spending trend from tenant transaction data repository.
        /// </summary>
        /// <param name="accountSearchParameter">The account search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account saving and spending trend
        /// </returns>
        IList<SavingTrend> Get_SavingTrend(CustomerAccountSearchParameter accountSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the batch details.
        /// </summary>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="StatementIdentifier">The statement id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of batch details
        /// </returns>
        IList<BatchDetail> GetBatchDetails(long BatchIdentifier, long StatementIdentifier, string tenantCode);

        /// <summary>
        /// This method gets the customer media details.
        /// </summary>
        /// <param name="CustomerIdentifier">The customer id</param>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="StatementIdentifier">The statement id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer media details
        /// </returns>
        IList<CustomerMedia> GetCustomerMediaList(long CustomerIdentifier, long BatchIdentifier, long StatementIdentifier, string tenantCode);

        /// <summary>
        /// This method gets the customer income sources.
        /// </summary>
        /// <param name="CustomerIdentifier">The customer id</param>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer income sources
        /// </returns>
        IList<IncomeSources> GetCustomerIncomeSources(long CustomerIdentifier, long BatchIdentifier, string tenantCode);

        /// <summary>
        /// This method gets the reminder and recommentations.
        /// </summary>
        /// <param name="CustomerIdentifier">The customer id</param>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of reminder and recommentations
        /// </returns>
        IList<ReminderAndRecommendation> GetReminderAndRecommendation(long CustomerIdentifier, long BatchIdentifier, string tenantCode);
    }
}
