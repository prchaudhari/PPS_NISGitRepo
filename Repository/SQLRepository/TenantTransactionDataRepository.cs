// <copyright file="TenantTransactionDataRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    #endregion

    public class TenantTransactionDataRepository : ITenantTransactionDataRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        public TenantTransactionDataRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        #region Public methods for Financial Tenant

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
            IList<CustomerMaster> customerMasters = new List<CustomerMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomer(customerSearchParameter, tenantCode);
                var customerMasterRecords = new List<CustomerMasterRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    customerMasterRecords = nISEntitiesDataContext.CustomerMasterRecords.Where(whereClause).ToList();
                    if (customerMasterRecords != null && customerMasterRecords.Count > 0)
                    {
                        customerMasterRecords.ForEach(item =>
                        {
                            customerMasters.Add(new CustomerMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerCode = item.CustomerCode,
                                FirstName = item.FirstName,
                                MiddleName = item.MiddleName,
                                LastName = item.LastName,
                                AddressLine1 = item.AddressLine1,
                                AddressLine2 = item.AddressLine2,
                                City = item.City,
                                State = item.State,
                                Country = item.Country,
                                Zip = item.Zip,
                                StatementDate = item.StatementDate,
                                StatementPeriod = item.StatementPeriod,
                                RmName = item.RmName,
                                RmContactNumber = item.RmContactNumber,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return customerMasters;
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
            IList<AccountMaster> accountMasters = new List<AccountMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerAccount(accountSearchParameter, tenantCode);
                var accountMasterRecords = new List<AccountMasterRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    accountMasterRecords = nISEntitiesDataContext.AccountMasterRecords.ToList();
                    accountMasterRecords = nISEntitiesDataContext.AccountMasterRecords.Where(whereClause).ToList();
                    if (accountMasterRecords != null && accountMasterRecords.Count > 0)
                    {
                        accountMasterRecords.ForEach(item =>
                        {
                            accountMasters.Add(new AccountMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                AccountType = item.AccountType,
                                AccountNumber = item.AccountNumber,
                                Currency = item.Currency,
                                Balance = Convert.ToString(item.Balance),
                                TotalDeposit = Convert.ToString(item.TotalDeposit),
                                TotalSpend = Convert.ToString(item.TotalSpend),
                                ProfitEarned = Convert.ToString(item.ProfitEarned),
                                Indicator = item.Indicator,
                                FeesPaid = Convert.ToString(item.FeesPaid),
                                GrandTotal = Convert.ToString(item.GrandTotal),
                                Percentage = Convert.ToString(item.Percentage),
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return accountMasters;
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
            IList<AccountTransaction> accountTransactions = new List<AccountTransaction>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerAccount(accountSearchParameter, tenantCode);
                var accountTransactionRecords = new List<AccountTransactionRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    accountTransactionRecords = nISEntitiesDataContext.AccountTransactionRecords.ToList();
                    accountTransactionRecords = nISEntitiesDataContext.AccountTransactionRecords.Where(whereClause)?.OrderByDescending(it => it.TransactionDate).ToList();
                    if (accountTransactionRecords != null && accountTransactionRecords.Count > 0)
                    {
                        accountTransactionRecords.ForEach(item =>
                        {
                            accountTransactions.Add(new AccountTransaction()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                AccountId = item.AccountId,
                                AccountType = item.AccountType,
                                TransactionDate = item.TransactionDate.ToString("yyyy-dd-MM"),
                                TransactionType = item.TransactionType,
                                Narration = item.Narration,
                                FCY = Convert.ToString(item.FCY),
                                LCY = Convert.ToString(item.LCY),
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return accountTransactions;
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
            IList<SavingTrend> savingTrends = new List<SavingTrend>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerAccount(accountSearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var query = (from st in nISEntitiesDataContext.SavingTrendRecords
                                 join am in nISEntitiesDataContext.AccountMasterRecords on st.AccountId equals am.Id
                                 select new
                                 {
                                     st.Id,
                                     st.AccountId,
                                     st.CustomerId,
                                     am.AccountNumber,
                                     am.AccountType,
                                     st.Month,
                                     st.Income,
                                     st.IncomePercentage,
                                     st.SpendAmount,
                                     st.SpendPercentage,
                                     st.BatchId,
                                     st.TenantCode
                                 }).Where(whereClause).ToList();

                    query.ForEach(q =>
                    {
                        savingTrends.Add(new SavingTrend
                        {
                            Identifier = q.Id,
                            AccountId = q.AccountId,
                            CustomerId = q.CustomerId,
                            BatchId = q.BatchId,
                            AccountNumber = q.AccountNumber,
                            AccountType = q.AccountType,
                            Income = q.Income ?? 0,
                            IncomePercentage = q.IncomePercentage ?? 0,
                            Month = q.Month,
                            SpendAmount = q.SpendAmount,
                            SpendPercentage = q.SpendPercentage ?? 0,
                            TenantCode = q.TenantCode
                        });
                    });
                }
                return savingTrends;
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
            var BatchDetails = new List<BatchDetail>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var batchDetailRecords = nISEntitiesDataContext.BatchDetailRecords.Where(item => item.BatchId == BatchIdentifier && item.StatementId == StatementIdentifier && item.TenantCode == tenantCode).ToList();
                    batchDetailRecords.ForEach(record =>
                    {
                        BatchDetails.Add(new BatchDetail()
                        {
                            Identifier = record.Id,
                            BatchId = record.BatchId,
                            ImageURL = record.ImageURL,
                            PageId = record.PageId,
                            StatementId = record.StatementId,
                            TenantCode = record.TenantCode,
                            VideoURL = record.VideoURL,
                            WidgetId = record.WidgetId
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return BatchDetails;
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
            var _lstCustomerMedia = new List<CustomerMedia>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var CustomerMediaRecords = nISEntitiesDataContext.CustomerMediaRecords.Where(item => item.CustomerId == CustomerIdentifier && item.StatementId == StatementIdentifier && item.BatchId == BatchIdentifier && item.TenantCode == tenantCode)?.ToList();
                    CustomerMediaRecords.ForEach(record =>
                    {
                        _lstCustomerMedia.Add(new CustomerMedia()
                        {
                            Identifier = record.Id,
                            BatchId = record.BatchId,
                            ImageURL = record.ImageURL,
                            PageId = record.PageId,
                            StatementId = record.StatementId,
                            TenantCode = record.TenantCode,
                            VideoURL = record.VideoURL,
                            WidgetId = record.WidgetId,
                            CustomerId = record.CustomerId
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _lstCustomerMedia;
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
            var incomeSources = new List<IncomeSources>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var IncomeSourceRecords = nISEntitiesDataContext.Top4IncomeSourcesRecord.Where(item => item.CustomerId == CustomerIdentifier && item.BatchId == BatchIdentifier && item.TenantCode == tenantCode)?.ToList();
                    IncomeSourceRecords.ForEach(record =>
                    {
                        incomeSources.Add(new IncomeSources()
                        {
                            Identifier = record.Id,
                            BatchId = record.BatchId,
                            CustomerId = record.CustomerId,
                            AverageSpend = record.AverageSpend != null ? record.AverageSpend.ToString() : "0",
                            CurrentSpend = record.CurrentSpend != null ? record.CurrentSpend.ToString() : "0",
                            Source = record.Source,
                            TenantCode = record.TenantCode
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return incomeSources;
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
            var reminders = new List<ReminderAndRecommendation>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var ReminderREcords = nISEntitiesDataContext.ReminderAndRecommendationRecords.Where(item => item.CustomerId == CustomerIdentifier && item.BatchId == BatchIdentifier && item.TenantCode == tenantCode)?.ToList();
                    ReminderREcords.ForEach(record =>
                    {
                        reminders.Add(new ReminderAndRecommendation()
                        {
                            Identifier = record.Id,
                            BatchId = record.BatchId,
                            CustomerId = record.CustomerId,
                            Action = record.TargetURL,
                            Description = record.Description,
                            TenantCode = record.TenantCode,
                            Title = record.LabelText
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return reminders;
        }

        #endregion
        #region pps

        // pps details
        public IList<spIAA_PaymentDetail> Get_FSPDetails(string tenantCode)
        {
            //List<spIAA_PaymentDetail> spData = nISEntitiesDataContext.spIAA_PaymentDetail_fspstatement();

            try
            {
                IList<spIAA_PaymentDetail> ppsDetails = new List<spIAA_PaymentDetail>();
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    ppsDetails = nISEntitiesDataContext.spIAA_PaymentDetail_fspstatement();


                }
                return ppsDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<spIAA_Commission_Detail> Get_PPSDetails(string tenantCode)
        {
            //List<spIAA_PaymentDetail> spData = nISEntitiesDataContext.spIAA_PaymentDetail_fspstatement();

            try
            {
                IList<spIAA_Commission_Detail> ppsDetails1 = new List<spIAA_Commission_Detail>();
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    ppsDetails1 = nISEntitiesDataContext.spIAA_Commission_Detail();


                }
                return ppsDetails1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Private Methods

        #region Private methods for Financial Tenant
        private string WhereClauseGeneratorForCustomer(CustomerSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }
            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }
            if (validationEngine.IsValidText(searchParameter.CustomerCode))
            {
                queryString.Append(string.Format("CustomerCode.Equals(\"{0}\") and ", searchParameter.CustomerCode));
            }
            if (validationEngine.IsValidLong(searchParameter.BatchId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") ");
            }

            if (searchParameter.WidgetFilterSetting != null && searchParameter.WidgetFilterSetting != string.Empty)
            {
                var filterEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFilterEntity>>(searchParameter.WidgetFilterSetting);
                filterEntities.ForEach(filterEntity =>
                {
                    queryString.Append(this.QueryGenerator(filterEntity));
                });
            }

            queryString.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
            return queryString.ToString();
        }

        private string WhereClauseGeneratorForCustomerAccount(CustomerAccountSearchParameter searchParameter, string tenantCode)
        {
            try
            {
                StringBuilder queryString = new StringBuilder();

                //send account id value to this property when account master data fetching
                if (validationEngine.IsValidLong(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                //send account id value to this property when account transaction data fetching
                if (validationEngine.IsValidLong(searchParameter.AccountId))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.AccountId.ToString().Split(',').Select(item => string.Format("AccountId.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidLong(searchParameter.BatchId))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidLong(searchParameter.CustomerId) && validationEngine.IsValidText(searchParameter.AccountType))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidLong(searchParameter.CustomerId) && !validationEngine.IsValidText(searchParameter.AccountType))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") ");
                }
                if (validationEngine.IsValidText(searchParameter.AccountType))
                {
                    queryString.Append(string.Format("AccountType.Contains(\"{0}\") ", searchParameter.AccountType));
                }
                if (searchParameter.WidgetFilterSetting != null && searchParameter.WidgetFilterSetting != string.Empty)
                {
                    var filterEntities = JsonConvert.DeserializeObject<List<DynamicWidgetFilterEntity>>(searchParameter.WidgetFilterSetting);
                    filterEntities.ForEach(filterEntity =>
                    {
                        queryString.Append(this.QueryGenerator(filterEntity));
                    });
                }
                queryString.Append(string.Format(" and TenantCode.Equals(\"{0}\") ", tenantCode));
                return queryString.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        
        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string QueryGenerator(DynamicWidgetFilterEntity filterEntity)
        {
            var queryString = string.Empty;
            var condtionalOp = filterEntity.ConditionalOperator != null && filterEntity.ConditionalOperator != string.Empty && filterEntity.ConditionalOperator != "0" ? filterEntity.ConditionalOperator : " and ";
            if (filterEntity.Operator == "EqualsTo")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotEqualsTo")
            {
                queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "Contains")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotContains")
            {
                queryString = queryString + condtionalOp + " " + (string.Format("!" + filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "LessThan")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " < " + filterEntity.Value + " "));
            }
            else if (filterEntity.Operator == "GreaterThan")
            {
                queryString = queryString + condtionalOp + " " + (string.Format(filterEntity.FieldName + " > " + filterEntity.Value + " "));
            }
            return queryString;
        }
        #endregion


    }
}
#endregion