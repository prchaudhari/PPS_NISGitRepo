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
        /// <param name="customerSearchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        [HttpPost]
        public IList<CustomerMaster> Get_CustomerMasters(CustomerSearchParameter customerSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_CustomerMasters(customerSearchParameter, tenantCode);
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
        /// <returns>
        /// Returns the list of customer account master
        /// </returns>
        [HttpPost]
        public IList<AccountMaster> Get_AccountMaster(CustomerAccountSearchParameter accountSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_AccountMaster(accountSearchParameter, tenantCode);
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
        /// <returns>
        /// Returns the list of customer account transaction
        /// </returns>
        [HttpPost]
        public IList<AccountTransaction> Get_AccountTransaction(CustomerAccountSearchParameter accountSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_AccountTransaction(accountSearchParameter, tenantCode);
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
        /// <returns>
        /// Returns the list of customer account saving and spending trend
        /// </returns>
        [HttpPost]
        public IList<SavingTrend> Get_SavingTrend(CustomerAccountSearchParameter accountSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_SavingTrend(accountSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Nedbank

        /// <summary>
        /// This method gets the specified list of customer master.
        /// </summary>
        /// <param name="customerSearchParameter">The customer search parameter</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        [HttpPost]
        public IList<DM_CustomerMaster> Get_DM_CustomerMasters(CustomerSearchParameter customerSearchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_CustomerMasters(customerSearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer investment master.
        /// </summary>
        /// <param name="searchParameter">The customer investment search parameter</param>
        /// <returns>
        /// Returns the list of customer investment master
        /// </returns>
        [HttpPost]
        public IList<DM_InvestmentMaster> Get_DM_InvestmasterMaster(CustomerInvestmentSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_InvestmasterMaster(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer investment transaction.
        /// </summary>
        /// <param name="searchParameter">The investment search parameter</param>
        /// <returns>
        /// Returns the list of customer investment transaction
        /// </returns>
        [HttpPost]
        public IList<DM_InvestmentTransaction> Get_DM_InvestmentTransaction(CustomerInvestmentSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_InvestmentTransaction(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of branch master.
        /// </summary>
        /// <param name="BranchId">The Branch Identifier</param>
        /// <returns>
        /// Returns the list of branch master
        /// </returns>
        [HttpPost]
        public IList<DM_BranchMaster> Get_DM_BranchMaster(long BranchId)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_BranchMaster(BranchId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of notes.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <returns>
        /// Returns the list of explanatory notes
        /// </returns>
        [HttpPost]
        public IList<DM_ExplanatoryNote> Get_DM_ExplanatoryNotes(MessageAndNoteSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_ExplanatoryNotes(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of message.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <returns>
        /// Returns the list of marketing message
        /// </returns>
        [HttpPost]
        public IList<DM_MarketingMessage> Get_DM_MarketingMessages(MessageAndNoteSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_MarketingMessages(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer personal loan master.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <returns>
        /// Returns the list of customer personal loan master
        /// </returns>
        [HttpPost]
        public IList<DM_PersonalLoanMaster> Get_DM_PersonalLoanMaster(CustomerPersonalLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_PersonalLoanMaster(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer personal loan transaction records.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <returns>
        /// Returns the list of customer personal loan transaction record
        /// </returns>
        [HttpPost]
        public IList<DM_PersonalLoanTransaction> Get_DM_PersonalLoanTransaction(CustomerPersonalLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_PersonalLoanTransaction(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer personal loan arrears.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <returns>
        /// Returns the list of customer personal loan arrears records
        /// </returns>
        [HttpPost]
        public IList<DM_PersonalLoanArrears> Get_DM_PersonalLoanArrears(CustomerPersonalLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_PersonalLoanArrears(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of special message.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <returns>
        /// Returns the list of special message
        /// </returns>
        [HttpPost]
        public IList<SpecialMessage> Get_DM_SpecialMessages(MessageAndNoteSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_SpecialMessages(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer home loan master from personal loan master repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <returns>
        /// Returns the list of customer home loan master
        /// </returns>
        [HttpPost]
        public IList<DM_HomeLoanMaster> Get_DM_HomeLoanMaster(CustomerHomeLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_HomeLoanMaster(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer home loan transaction records from personal loan transaction repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <returns>
        /// Returns the list of customer home loan transaction record
        /// </returns>
        [HttpPost]
        public IList<DM_HomeLoanTransaction> Get_DM_HomeLoanTransaction(CustomerHomeLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_HomeLoanTransaction(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer home loan arrears from personal loan arrear repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <returns>
        /// Returns the list of customer home loan arrears records
        /// </returns>
        [HttpPost]
        public IList<DM_HomeLoanArrear> Get_DM_HomeLoanArrears(CustomerHomeLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_HomeLoanArrears(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer home loan summary from personal loan arrear repository.
        /// </summary>
        /// <param name="searchParameter">The customer home loan search parameter</param>
        /// <returns>
        /// Returns the list of customer home loan summary records
        /// </returns>
        [HttpPost]
        public IList<DM_HomeLoanSummary> Get_DM_HomeLoanSummary(CustomerHomeLoanSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.Get_DM_HomeLoanSummary(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer account summaries from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account summary records
        /// </returns>
        [HttpPost]
        public IList<DM_AccountsSummary> GET_DM_AccountSummaries(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_AccountSummaries(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer account analysis records from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer account analysis records
        /// </returns>
        [HttpPost]
        public IList<DM_AccountAnanlysis> GET_DM_AccountAnalysisDetails(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_AccountAnalysisDetails(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of reminder and recommendation records from repository.
        /// </summary>
        /// <param name="ReminderId">The reminder identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of reminder and recommendation records
        /// </returns>
        [HttpPost]
        public IList<DM_ReminderAndRecommendation> GET_DM_ReminderAndRecommendations(long ReminderId)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_ReminderAndRecommendations(ReminderId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer reminder and recommendation records from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer reminder and recommendation records
        /// </returns>
        [HttpPost]
        public IList<DM_CustomerReminderAndRecommendation> GET_DM_CustomerReminderAndRecommendations(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_CustomerReminderAndRecommendations(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of news and alerts records from repository.
        /// </summary>
        /// <param name="NewsAndAlertId">The news/alert identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of news and alerts records
        /// </returns>
        [HttpPost]
        public IList<DM_NewsAndAlerts> GET_DM_NewsAndAlerts(long NewsAndAlertId)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_NewsAndAlerts(NewsAndAlertId, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer news and alerts records from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer news and alerts records
        /// </returns>
        [HttpPost]
        public IList<DM_CustomerNewsAndAlert> GET_DM_CustomerNewsAndAlert(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_CustomerNewsAndAlert(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the greenbacks master details from repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the greenbacks master details record
        /// </returns>
        [HttpPost]
        public IList<DM_GreenbacksMaster> GET_DM_GreenbacksMasterDetails()
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_GreenbacksMasterDetails(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the customer greenbacks reward points from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the customer greenbacks reward points records
        /// </returns>
        [HttpPost]
        public IList<DM_GreenbacksRewardPoints> GET_DM_GreenbacksRewardPoints(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_GreenbacksRewardPoints(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the customer redeemed greenbacks reward points from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the customer redeemed greenbacks reward points records
        /// </returns>
        [HttpPost]
        public IList<DM_GreenbacksRewardPointsRedeemed> GET_DM_GreenbacksRewardPointsRedeemed(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_GreenbacksRewardPointsRedeemed(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the customer's product monthwise reward points earned data from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the customer's product monthwise reward points earned records
        /// </returns>
        [HttpPost]
        public IList<DM_CustomerProductWiseRewardPoints> GET_DM_CustomerProductWiseRewardPoints(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_CustomerProductWiseRewardPoints(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the category wise customer's spend reward points from repository.
        /// </summary>
        /// <param name="searchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the category wise customer's spend reward points records
        /// </returns>
        [HttpPost]
        public IList<DM_CustomerRewardSpendByCategory> GET_DM_CustomerRewardSpendByCategory(CustomerSearchParameter searchParameter)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.tenantTransactionDataManager.GET_DM_CustomerRewardSpendByCategory(searchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion
    }
}