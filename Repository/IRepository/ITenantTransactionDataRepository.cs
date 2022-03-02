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

        #region Nedbank

        /// <summary>
        /// This method gets the specified list of customer master from Dm customer master repository.
        /// </summary>
        /// <param name="customerSearchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        IList<DM_CustomerMaster> Get_DM_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer investment master from investment master repository.
        /// </summary>
        /// <param name="searchParameter">The customer investment search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer investment master
        /// </returns>
        IList<DM_InvestmentMaster> Get_DM_InvestmasterMaster(CustomerInvestmentSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer investment master from investment master repository.
        /// </summary>
        /// <param name="searchParameter">The customer investment search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer investment master
        /// </returns>
        IList<DM_InvestmentMaster> Get_NB_InvestmasterMaster(CustomerInvestmentSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer investment transaction from Investment transaction repository.
        /// </summary>
        /// <param name="searchParameter">The investment search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer investment transaction
        /// </returns>
        IList<DM_InvestmentTransaction> Get_DM_InvestmentTransaction(CustomerInvestmentSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of branch master from branch repository.
        /// </summary>
        /// <param name="BranchId">The Branch Identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of branch master
        /// </returns>
        IList<DM_BranchMaster> Get_DM_BranchMaster(long BranchId, string tenantCode);

        /// <summary>
        /// This method gets the specified list of notes from explanatory notes repository.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of explanatory notes
        /// </returns>
        IList<DM_ExplanatoryNote> Get_DM_ExplanatoryNotes(MessageAndNoteSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of message from marketing message repository.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of marketing message
        /// </returns>
        IList<DM_MarketingMessage> Get_DM_MarketingMessages(MessageAndNoteSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer personal loan master from personal loan master repository.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer personal loan master
        /// </returns>
        IList<DM_PersonalLoanMaster> Get_DM_PersonalLoanMaster(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer personal loan transaction records from personal loan transaction repository.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer personal loan transaction record
        /// </returns>
        IList<DM_PersonalLoanTransaction> Get_DM_PersonalLoanTransaction(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer personal loan arrears from personal loan arrear repository.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer personal loan arrears records
        /// </returns>
        IList<DM_PersonalLoanArrears> Get_DM_PersonalLoanArrears(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of special message from special message repository.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of special message
        /// </returns>
        IList<SpecialMessage> Get_DM_SpecialMessages(MessageAndNoteSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer home loan master from personal loan master repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan master
        /// </returns>
        IList<DM_HomeLoanMaster> Get_DM_HomeLoanMaster(CustomerHomeLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer home loan transaction records from personal loan transaction repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan transaction record
        /// </returns>
        IList<DM_HomeLoanTransaction> Get_DM_HomeLoanTransaction(CustomerHomeLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer home loan arrears from personal loan arrear repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan arrears records
        /// </returns>
        IList<DM_HomeLoanArrear> Get_DM_HomeLoanArrears(CustomerHomeLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer home loan summary from personal loan arrear repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan summary records
        /// </returns>
        IList<DM_HomeLoanSummary> Get_DM_HomeLoanSummary(CustomerHomeLoanSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer account summaries from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account summary records
        /// </returns>
        IList<DM_AccountsSummary> GET_DM_AccountSummaries(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer account analysis records from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account analysis records
        /// </returns>
        IList<DM_AccountAnanlysis> GET_DM_AccountAnalysisDetails(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of reminder and recommendation records from repository.
        /// </summary>
        /// <param name="ReminderId">The reminder identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of reminder and recommendation records
        /// </returns>
        IList<DM_ReminderAndRecommendation> GET_DM_ReminderAndRecommendations(long ReminderId, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer reminder and recommendation records from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer reminder and recommendation records
        /// </returns>
        IList<DM_CustomerReminderAndRecommendation> GET_DM_CustomerReminderAndRecommendations(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the specified list of news and alerts records from repository.
        /// </summary>
        /// <param name="NewsAndAlertId">The news/alert identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of news and alerts records
        /// </returns>
        IList<DM_NewsAndAlerts> GET_DM_NewsAndAlerts(long NewsAndAlertId, string tenantCode);

        /// <summary>
        /// This method gets the specified list of customer news and alerts records from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer news and alerts records
        /// </returns>
        IList<DM_CustomerNewsAndAlert> GET_DM_CustomerNewsAndAlert(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the greenbacks master details from repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the greenbacks master details record
        /// </returns>
        IList<DM_GreenbacksMaster> GET_DM_GreenbacksMasterDetails(string tenantCode);

        /// <summary>
        /// This method gets the customer greenbacks reward points from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the customer greenbacks reward points records
        /// </returns>
        IList<DM_GreenbacksRewardPoints> GET_DM_GreenbacksRewardPoints(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the customer redeemed greenbacks reward points from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the customer redeemed greenbacks reward points records
        /// </returns>
        IList<DM_GreenbacksRewardPointsRedeemed> GET_DM_GreenbacksRewardPointsRedeemed(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the customer's product monthwise reward points earned data from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the customer's product monthwise reward points earned records
        /// </returns>
        IList<DM_CustomerProductWiseRewardPoints> GET_DM_CustomerProductWiseRewardPoints(CustomerSearchParameter searchParameter, string tenantCode);

        /// <summary>
        /// This method gets the category wise customer's spend reward points from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the category wise customer's spend reward points records
        /// </returns>
        IList<DM_CustomerRewardSpendByCategory> GET_DM_CustomerRewardSpendByCategory(CustomerSearchParameter searchParameter, string tenantCode);

        #endregion
    }
}
