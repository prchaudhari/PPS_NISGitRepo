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

        #endregion
    }
}
