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

        /// <summary>
        /// This method gets the batch details.
        /// </summary>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="StatementIdentifier">The statement id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of batch details
        /// </returns>
        public IList<BatchDetail> GetBatchDetails(long BatchIdentifier, long StatementIdentifier, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.GetBatchDetails(BatchIdentifier, StatementIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
        public IList<CustomerMedia> GetCustomerMediaList(long CustomerIdentifier, long BatchIdentifier, long StatementIdentifier, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.GetCustomerMediaList(CustomerIdentifier, BatchIdentifier, StatementIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the customer income sources.
        /// </summary>
        /// <param name="CustomerIdentifier">The customer id</param>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer income sources
        /// </returns>
        public IList<IncomeSources> GetCustomerIncomeSources(long CustomerIdentifier, long BatchIdentifier, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.GetCustomerIncomeSources(CustomerIdentifier, BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the reminder and recommentations.
        /// </summary>
        /// <param name="CustomerIdentifier">The customer id</param>
        /// <param name="BatchIdentifier">The batch id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of reminder and recommentations
        /// </returns>
        public IList<ReminderAndRecommendation> GetReminderAndRecommendation(long CustomerIdentifier, long BatchIdentifier, string tenantCode)
        {
            try
            {
                return this.tenantTransactionDataRepository.GetReminderAndRecommendation(CustomerIdentifier, BatchIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Nedbank

        ///// <summary>
        ///// This method gets the specified list of customer master from Dm customer master repository.
        ///// </summary>
        ///// <param name="customerSearchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer master
        ///// </returns>
        //public IList<DM_CustomerMaster> Get_DM_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_CustomerMasters(customerSearchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer investment master from investment master repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer investment search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer investment master
        ///// </returns>
        //public IList<DM_InvestmentMaster> Get_DM_InvestmasterMaster(CustomerInvestmentSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_InvestmasterMaster(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer investment transaction from Investment transaction repository.
        ///// </summary>
        ///// <param name="searchParameter">The investment search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer investment transaction
        ///// </returns>
        //public IList<DM_InvestmentTransaction> Get_DM_InvestmentTransaction(CustomerInvestmentSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_InvestmentTransaction(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of branch master from branch repository.
        ///// </summary>
        ///// <param name="BranchId">The Branch Identifier</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of branch master
        ///// </returns>
        //public IList<DM_BranchMaster> Get_DM_BranchMaster(long BranchId, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_BranchMaster(BranchId, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of notes from explanatory notes repository.
        ///// </summary>
        ///// <param name="searchParameter">The message or note search parameter object</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of explanatory notes
        ///// </returns>
        //public IList<DM_ExplanatoryNote> Get_DM_ExplanatoryNotes(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_ExplanatoryNotes(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of message from marketing message repository.
        ///// </summary>
        ///// <param name="searchParameter">The message or note search parameter object</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of marketing message
        ///// </returns>
        //public IList<DM_MarketingMessage> Get_DM_MarketingMessages(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_MarketingMessages(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer personal loan master from personal loan master repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer personal loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer personal loan master
        ///// </returns>
        //public IList<DM_PersonalLoanMaster> Get_DM_PersonalLoanMaster(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_PersonalLoanMaster(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer personal loan transaction records from personal loan transaction repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer personal loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer personal loan transaction record
        ///// </returns>
        //public IList<DM_PersonalLoanTransaction> Get_DM_PersonalLoanTransaction(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_PersonalLoanTransaction(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer personal loan arrears from personal loan arrear repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer personal loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer personal loan arrears records
        ///// </returns>
        //public IList<DM_PersonalLoanArrears> Get_DM_PersonalLoanArrears(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_PersonalLoanArrears(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of special message from special message repository.
        ///// </summary>
        ///// <param name="searchParameter">The message or note search parameter object</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of special message
        ///// </returns>
        //public IList<SpecialMessage> Get_DM_SpecialMessages(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_SpecialMessages(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer home loan master from personal loan master repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer home loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer home loan master
        ///// </returns>
        //public IList<DM_HomeLoanMaster> Get_DM_HomeLoanMaster(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_HomeLoanMaster(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer home loan transaction records from personal loan transaction repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer home loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer home loan transaction record
        ///// </returns>
        //public IList<DM_HomeLoanTransaction> Get_DM_HomeLoanTransaction(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_HomeLoanTransaction(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer home loan arrears from personal loan arrear repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer home loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer home loan arrears records
        ///// </returns>
        //public IList<DM_HomeLoanArrear> Get_DM_HomeLoanArrears(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_HomeLoanArrears(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer home loan summary from personal loan arrear repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer home loan search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer home loan summary records
        ///// </returns>
        //public IList<DM_HomeLoanSummary> Get_DM_HomeLoanSummary(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.Get_DM_HomeLoanSummary(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer account summaries from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer account summary records
        ///// </returns>
        //public IList<DM_AccountsSummary> GET_DM_AccountSummaries(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_AccountSummaries(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer account analysis records from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer account analysis records
        ///// </returns>
        //public IList<DM_AccountAnanlysis> GET_DM_AccountAnalysisDetails(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_AccountAnalysisDetails(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of reminder and recommendation records from repository.
        ///// </summary>
        ///// <param name="ReminderId">The reminder identifier</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of reminder and recommendation records
        ///// </returns>
        //public IList<DM_ReminderAndRecommendation> GET_DM_ReminderAndRecommendations(long ReminderId, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_ReminderAndRecommendations(ReminderId, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer reminder and recommendation records from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer reminder and recommendation records
        ///// </returns>
        //public IList<DM_CustomerReminderAndRecommendation> GET_DM_CustomerReminderAndRecommendations(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_CustomerReminderAndRecommendations(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of news and alerts records from repository.
        ///// </summary>
        ///// <param name="NewsAndAlertId">The news/alert identifier</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of news and alerts records
        ///// </returns>
        //public IList<DM_NewsAndAlerts> GET_DM_NewsAndAlerts(long NewsAndAlertId, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_NewsAndAlerts(NewsAndAlertId, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the specified list of customer news and alerts records from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the list of customer news and alerts records
        ///// </returns>
        //public IList<DM_CustomerNewsAndAlert> GET_DM_CustomerNewsAndAlert(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_CustomerNewsAndAlert(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the greenbacks master details from repository.
        ///// </summary>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the greenbacks master details record
        ///// </returns>
        //public IList<DM_GreenbacksMaster> GET_DM_GreenbacksMasterDetails(string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_GreenbacksMasterDetails(tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the customer greenbacks reward points from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the customer greenbacks reward points records
        ///// </returns>
        //public IList<DM_GreenbacksRewardPoints> GET_DM_GreenbacksRewardPoints(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_GreenbacksRewardPoints(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the customer redeemed greenbacks reward points from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the customer redeemed greenbacks reward points records
        ///// </returns>
        //public IList<DM_GreenbacksRewardPointsRedeemed> GET_DM_GreenbacksRewardPointsRedeemed(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_GreenbacksRewardPointsRedeemed(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the customer's product monthwise reward points earned data from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the customer's product monthwise reward points earned records
        ///// </returns>
        //public IList<DM_CustomerProductWiseRewardPoints> GET_DM_CustomerProductWiseRewardPoints(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_CustomerProductWiseRewardPoints(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// This method gets the category wise customer's spend reward points from repository.
        ///// </summary>
        ///// <param name="searchParameter">The customer search parameter</param>
        ///// <param name="tenantCode">The tenant code</param>
        ///// <returns>
        ///// Returns the category wise customer's spend reward points records
        ///// </returns>
        //public IList<DM_CustomerRewardSpendByCategory> GET_DM_CustomerRewardSpendByCategory(CustomerSearchParameter searchParameter, string tenantCode)
        //{
        //    try
        //    {
        //        return this.tenantTransactionDataRepository.GET_DM_CustomerRewardSpendByCategory(searchParameter, tenantCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        #endregion

        #endregion
    }
}
