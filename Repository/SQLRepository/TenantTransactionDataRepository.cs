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

        #region Public methods for Nedbank Tenant

        /// <summary>
        /// This method gets the specified list of customer master from Dm customer master repository.
        /// </summary>
        /// <param name="customerSearchParameter">The customer search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        public IList<DM_CustomerMaster> Get_DM_CustomerMasters(CustomerSearchParameter customerSearchParameter, string tenantCode)
        {
            IList<DM_CustomerMaster> customerMasters = new List<DM_CustomerMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(customerSearchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var customerMasterRecords = nISEntitiesDataContext.DM_CustomerMasterRecord.Where(whereClause).GroupBy(it => it.CustomerId).Select(it => it.FirstOrDefault()).ToList();
                    if (customerMasterRecords != null && customerMasterRecords.Count > 0)
                    {
                        customerMasterRecords.ForEach(item =>
                        {
                            customerMasters.Add(new DM_CustomerMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                Title = item.Title,
                                FirstName = item.FirstName,
                                SurName = item.SurName,
                                AddressLine0 = item.AddressLine0,
                                AddressLine1 = item.AddressLine1,
                                AddressLine2 = item.AddressLine2,
                                AddressLine3 = item.AddressLine3,
                                AddressLine4 = item.AddressLine4,
                                EmailAddress = item.EmailAddress,
                                Mask_Cell_No = item.MaskCellNo,
                                Barcode = item.Barcode,
                                TenantCode = item.TenantCode,
                                DS_Investor_Name = item.DS_Investor_Name,
                                Language = item.Language,
                                Segment = item.Segment
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
        /// This method gets the specified list of customer investment master from investment master repository.
        /// </summary>
        /// <param name="searchParameter">The customer investment search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer investment master
        /// </returns>
        public IList<DM_InvestmentMaster> Get_DM_InvestmasterMaster(CustomerInvestmentSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_InvestmentMaster> InvestmentMasters = new List<DM_InvestmentMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerInvestment(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var InvestmentMasterRecords = nISEntitiesDataContext.DM_InvestmentMasterRecord.Where(whereClause).ToList();
                    if (InvestmentMasterRecords != null && InvestmentMasterRecords.Count > 0)
                    {
                        InvestmentMasterRecords.ForEach(item =>
                        {
                            InvestmentMasters.Add(new DM_InvestmentMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestmentId = item.InvestmentId,
                                InvestorId = item.InvestorId,
                                AccountOpenDate = item.AccountOpenDate,
                                AccuredInterest = item.AccuredInterest,
                                BranchId = item.BranchId,
                                ClosingBalance = item.ClosingBalance,
                                CurrentInterestRate = item.CurrentInterestRate,
                                DayOfStatement = item.DayOfStatement,
                                ExpiryDate = item.ExpiryDate,
                                InterestDisposalDesc = item.InterestDisposalDesc,
                                NoticePeriod = item.NoticePeriod,
                                ProductDesc = item.ProductDesc,
                                ProductId = item.ProductId,
                                ProductType = item.ProductType,
                                StatementDate = item.StatementDate,
                                StatementPeriod = item.StatementPeriod,
                                Currenacy = item.Currency,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return InvestmentMasters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer investment master from investment master repository.
        /// </summary>
        /// <param name="searchParameter">The customer investment search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer investment master
        /// </returns>
        public IList<DM_InvestmentMaster> Get_NB_InvestmasterMaster(CustomerInvestmentSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_InvestmentMaster> InvestmentMasters = new List<DM_InvestmentMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerInvestment(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    //var InvestmentMasterRecords = nISEntitiesDataContext.NB_InvestmentMaster.Where(whereClause).ToList();
                    IList<NB_InvestmentMaster> query = nISEntitiesDataContext.NB_InvestmentMaster.Where(m => m.TenantCode == tenantCode).ToList();
                    if (searchParameter.InvestorId > 0)
                    {
                        query = query.Where(m => m.InvestorId == searchParameter.InvestorId).ToList();
                    }
                    if (searchParameter.BatchId > 0)
                    {
                        query = query.Where(m => m.BatchId == searchParameter.BatchId).ToList();
                    }

                    var InvestmentMasterRecords = query.OrderBy(m => m.ProductDesc).ToList();
                    if (InvestmentMasterRecords != null && InvestmentMasterRecords.Count > 0)
                    {
                        InvestmentMasterRecords.ForEach(item =>
                        {
                            InvestmentMasters.Add(new DM_InvestmentMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId.Value,
                                CustomerId = item.InvestorId.Value,
                                InvestmentId = item.InvestmentId.Value,
                                InvestorId = item.InvestorId.Value,
                                AccountOpenDate = item.AccountOpenDate,
                                AccuredInterest = item.AccuredInterest,
                                BranchId = item.BranchId.Value,
                                ClosingBalance = item.ClosingBalance,
                                CurrentInterestRate = item.CurrentInterestRate,
                                DayOfStatement = item.DayOfStatement.ToString(),
                                ExpiryDate = item.ExpiryDate,
                                InterestDisposalDesc = item.InterestDisposalDesc,
                                NoticePeriod = item.NoticePeriod,
                                ProductDesc = item.ProductDesc,
                                ProductId = item.ProductId.Value,
                                ProductType = item.ProductType,
                                StatementDate = item.StatementDate,
                                StatementPeriod = item.StatementPeriod,
                                Currenacy = item.Currency,
                                BonusInterest = item.BonusInterest,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return InvestmentMasters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// This method gets the specified list of customer investment transaction from Investment transaction repository.
        /// </summary>
        /// <param name="searchParameter">The investment search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer investment transaction
        /// </returns>
        public IList<DM_InvestmentTransaction> Get_DM_InvestmentTransaction(CustomerInvestmentSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_InvestmentTransaction> InvestmentTransactions = new List<DM_InvestmentTransaction>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerInvestment(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var query = nISEntitiesDataContext.NB_InvestmentTransaction.Where(m => m.TenantCode == tenantCode);
                    if (searchParameter.InvestorId > 0)
                    {
                        query = query.Where(m => m.InvestorId == searchParameter.InvestorId);
                    }
                    if (searchParameter.InvestmentId > 0)
                    {
                        query = query.Where(m => m.InvestmentId == searchParameter.InvestmentId);
                    }
                    if (searchParameter.BatchId > 0)
                    {
                        query = query.Where(m => m.BatchId == searchParameter.BatchId);
                    }

                    var InvestmentTransactionRecords = query.OrderBy(it => it.TransactionDate).ToList();
                    if (InvestmentTransactionRecords != null && InvestmentTransactionRecords.Count > 0)
                    {
                        InvestmentTransactionRecords.ForEach(item =>
                        {
                            InvestmentTransactions.Add(new DM_InvestmentTransaction()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.InvestorId,
                                ProductId = item.ProductId,
                                InvestmentId = item.InvestmentId,
                                InvestorId = item.InvestorId,
                                TransactionDate = item.TransactionDate,
                                TransactionDesc = item.TransactionDesc,
                                WJXBFS1 = item.WJXBFS1,
                                WJXBFS2_Debit = item.WJXBFS2_Debit,
                                WJXBFS3_Credit = item.WJXBFS3_Credit,
                                WJXBFS4_Balance = item.WJXBFS4_Balance,
                                WJXBFS5_TransId = item.WJXBFS5_TransId,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return InvestmentTransactions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of branch master from branch repository.
        /// </summary>
        /// <param name="BranchId">The Branch Identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of branch master
        /// </returns>
        public IList<DM_BranchMaster> Get_DM_BranchMaster(long BranchId, string tenantCode)
        {
            IList<DM_BranchMaster> _BranchMasters = new List<DM_BranchMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var BranchMasterRecords = nISEntitiesDataContext.DM_BranchMasterRecord.Where(it => it.Id == BranchId && it.TenantCode == tenantCode)?.ToList();
                    if (BranchMasterRecords == null || BranchMasterRecords.Count == 0)
                    {
                        BranchMasterRecords = nISEntitiesDataContext.DM_BranchMasterRecord.Where(it => it.Name.ToLower() == "UNKNOWN".ToLower() && it.TenantCode == tenantCode)?.ToList();
                    }

                    BranchMasterRecords.ForEach(item =>
                    {
                        _BranchMasters.Add(new DM_BranchMaster()
                        {
                            Identifier = item.Id,
                            BranchName = item.Name,
                            AddressLine0 = item.AddressLine0,
                            AddressLine1 = item.AddressLine1,
                            AddressLine2 = item.AddressLine2,
                            AddressLine3 = item.AddressLine3,
                            ContactNo = item.ContactNo,
                            VatRegNo = item.VatRegNo,
                            TenantCode = item.TenantCode
                        });
                    });

                }
                return _BranchMasters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of notes from explanatory notes repository.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of explanatory notes
        /// </returns>
        public IList<DM_ExplanatoryNote> Get_DM_ExplanatoryNotes(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_ExplanatoryNote> ExplanatoryNotes = new List<DM_ExplanatoryNote>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForMessageOrNoteSearch(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var ExplanatoryNoteRecords = nISEntitiesDataContext.DM_ExplanatoryNotesRecord.Where(whereClause)?.ToList();
                    if (ExplanatoryNoteRecords != null && ExplanatoryNoteRecords.Count > 0)
                    {
                        ExplanatoryNoteRecords.ForEach(item =>
                        {
                            ExplanatoryNotes.Add(new DM_ExplanatoryNote()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                ParentId = item.ParentId,
                                Note1 = item.Note1,
                                Note2 = item.Note2,
                                Note3 = item.Note3,
                                Note4 = item.Note4,
                                Note5 = item.Note5,
                                TenantCode = item.TenantCode
                            });
                        });
                    }

                }
                return ExplanatoryNotes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of message from marketing message repository.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of marketing message
        /// </returns>
        public IList<DM_MarketingMessage> Get_DM_MarketingMessages(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_MarketingMessage> MarketingMessages = new List<DM_MarketingMessage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForMessageOrNoteSearch(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var MarketingMessageRecords = nISEntitiesDataContext.DM_MarketingMessagesRecord.Where(whereClause)?.ToList();
                    if (MarketingMessageRecords != null && MarketingMessageRecords.Count > 0)
                    {
                        MarketingMessageRecords.ForEach(item =>
                        {
                            MarketingMessages.Add(new DM_MarketingMessage()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                ParentId = item.ParentId,
                                Header = item.Header,
                                Message1 = item.Message1,
                                Message2 = item.Message2,
                                Message3 = item.Message3,
                                Message4 = item.Message4,
                                Message5 = item.Message5,
                                Type = item.Type,
                                TenantCode = item.TenantCode
                            });
                        });
                    }

                }
                return MarketingMessages;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer personal loan master from personal loan master repository.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer personal loan master
        /// </returns>
        public IList<DM_PersonalLoanMaster> Get_DM_PersonalLoanMaster(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_PersonalLoanMaster> Records = new List<DM_PersonalLoanMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerPersonalLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var PersonalLoanMasterRecords = nISEntitiesDataContext.DM_PersonalLoanMasterRecord.Where(whereClause).ToList();
                    if (PersonalLoanMasterRecords != null && PersonalLoanMasterRecords.Count > 0)
                    {
                        PersonalLoanMasterRecords.ForEach(item =>
                        {
                            Records.Add(new DM_PersonalLoanMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                Currency = item.Currency,
                                AmountDue = item.AmountDue,
                                AnnualRate = item.AnnualRate,
                                Arrears = item.Arrears,
                                BranchId = item.BranchId != null ? Convert.ToInt32(item.BranchId) : 1,
                                CreditAdvance = item.CreditAdvance,
                                DueDate = item.DueDate ?? DateTime.Now,
                                FromDate = item.FromDate ?? DateTime.Now,
                                ToDate = item.ToDate ?? DateTime.Now,
                                Term = item.Term,
                                MonthlyInstallment = item.MonthlyInstallment,
                                OutstandingBalance = item.OutstandingBalance,
                                ProductType = item.ProductType,
                                TenantCode = item.TenantCode,
                                Messages = new List<string>
                                {
                                    item.InsuranceMessage1,
                                    item.InsuranceMessage2,
                                    item.InsuranceMessage3,
                                    item.InsuranceMessage4,
                                    item.InsuranceMessage5,
                                    item.InsuranceMessage6,
                                    item.InsuranceMessage7,
                                    item.InsuranceMessage8,
                                    item.InsuranceMessage9,
                                    item.InsuranceMessage10,
                                    item.InsuranceMessage11,
                                    item.InsuranceMessage12,
                                    item.InsuranceMessage13,
                                    item.InsuranceMessage14,
                                    item.InsuranceMessage15,
                                    item.InsuranceMessage16,
                                    item.InsuranceMessage17,
                                },
                                LoanTransactions = this.Get_DM_PersonalLoanTransaction(new CustomerPersonalLoanSearchParameter() { CustomerId = searchParameter.CustomerId, InvestorId = item.InvestorId, BatchId = searchParameter.BatchId }, tenantCode)?.ToList(),
                                LoanArrears = this.Get_DM_PersonalLoanArrears(new CustomerPersonalLoanSearchParameter() { CustomerId = searchParameter.CustomerId, InvestorId = item.InvestorId, BatchId = searchParameter.BatchId }, tenantCode)?.ToList()?.FirstOrDefault()
                            });
                        });
                    }
                }
                return Records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer personal loan transaction records from personal loan transaction repository.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer personal loan transaction record
        /// </returns>
        public IList<DM_PersonalLoanTransaction> Get_DM_PersonalLoanTransaction(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_PersonalLoanTransaction> Records = new List<DM_PersonalLoanTransaction>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerPersonalLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var PersonalLoanTransactionRecords = nISEntitiesDataContext.DM_PersonalLoanTransactionRecord.Where(whereClause)?.OrderBy(it => it.PostingDate)?.ToList();
                    if (PersonalLoanTransactionRecords != null && PersonalLoanTransactionRecords.Count > 0)
                    {
                        PersonalLoanTransactionRecords.ForEach(item =>
                        {
                            Records.Add(new DM_PersonalLoanTransaction()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                Credit = item.Credit,
                                Debit = item.Debit,
                                Description = item.Description,
                                EffectiveDate = item.EffectiveDate ?? DateTime.Now,
                                PostingDate = item.PostingDate ?? DateTime.Now,
                                OutstandingCapital = item.OutstandingCapital,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of customer personal loan arrears from personal loan arrear repository.
        /// </summary>
        /// <param name="searchParameter">The customer personal loan search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer personal loan arrears records
        /// </returns>
        public IList<DM_PersonalLoanArrears> Get_DM_PersonalLoanArrears(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_PersonalLoanArrears> Records = new List<DM_PersonalLoanArrears>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerPersonalLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var PersonalLoanArrearRecords = nISEntitiesDataContext.DM_PersonalLoanArrearsRecord.Where(whereClause).ToList();
                    if (PersonalLoanArrearRecords != null && PersonalLoanArrearRecords.Count > 0)
                    {
                        PersonalLoanArrearRecords.ForEach(item =>
                        {
                            Records.Add(new DM_PersonalLoanArrears()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                Arrears_120 = item.ARREARS_120,
                                Arrears_90 = item.ARREARS_90,
                                Arrears_60 = item.ARREARS_60,
                                Arrears_30 = item.ARREARS_30,
                                Arrears_0 = item.ARREARS_0,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method gets the specified list of special message from special message repository.
        /// </summary>
        /// <param name="searchParameter">The message or note search parameter object</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of special message
        /// </returns>
        public IList<SpecialMessage> Get_DM_SpecialMessages(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        {
            IList<SpecialMessage> SpecialMessages = new List<SpecialMessage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForMessageOrNoteSearch(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var SpecialMessageRecords = nISEntitiesDataContext.DM_SpecialMessagesRecord.Where(whereClause)?.ToList();
                    if (SpecialMessageRecords != null && SpecialMessageRecords.Count > 0)
                    {
                        SpecialMessageRecords.ForEach(item =>
                        {
                            SpecialMessages.Add(new SpecialMessage()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                ParentId = item.ParentId,
                                Message1 = item.Message1,
                                Message2 = item.Message2,
                                Message3 = item.Message3,
                                Message4 = item.Message4,
                                Message5 = item.Message5,
                                TenantCode = item.TenantCode
                            });
                        });
                    }

                }
                return SpecialMessages;
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan master
        /// </returns>
        public IList<DM_HomeLoanMaster> Get_DM_HomeLoanMaster(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_HomeLoanMaster> Records = new List<DM_HomeLoanMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerHomeLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var HomeLoanMasterRecords = nISEntitiesDataContext.NB_HomeLoanMaster.Where(whereClause).ToList();
                    if (HomeLoanMasterRecords != null && HomeLoanMasterRecords.Count > 0)
                    {
                        HomeLoanMasterRecords.ForEach(item =>
                        {
                            Records.Add(new DM_HomeLoanMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                Currency = item.Currency,
                                ArrearStatus = item.ArrearStatus,
                                Balance = item.Balance,
                                ChargeRate = item.ChargeRate,
                                IntialDue = item.IntialDue,
                                LoanAmount = item.LoanAmount,
                                LoanTerm = item.LoanTerm,
                                RegisteredAmount = item.RegisteredAmount,
                                RegisteredDate = item.RegisteredDate ?? DateTime.Now,
                                SecDescription1 = item.SecDescription1,
                                SecDescription2 = item.SecDescription2,
                                SecDescription3 = item.SecDescription3,
                                TenantCode = item.TenantCode,
                                SegmentType = item.SegmentType,
                                LoanTransactions = this.Get_DM_HomeLoanTransaction(new CustomerHomeLoanSearchParameter() { CustomerId = searchParameter.CustomerId, InvestorId = item.InvestorId, BatchId = searchParameter.BatchId }, tenantCode)?.ToList(),
                                LoanArrear = this.Get_DM_HomeLoanArrears(new CustomerHomeLoanSearchParameter() { CustomerId = searchParameter.CustomerId, InvestorId = item.InvestorId, BatchId = searchParameter.BatchId }, tenantCode)?.ToList()?.FirstOrDefault(),
                                LoanSummary = this.Get_DM_HomeLoanSummary(new CustomerHomeLoanSearchParameter() { CustomerId = searchParameter.CustomerId, InvestorId = item.InvestorId, BatchId = searchParameter.BatchId }, tenantCode)?.ToList()?.FirstOrDefault()
                            });
                        });
                    }
                }
                return Records;
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan transaction record
        /// </returns>
        public IList<DM_HomeLoanTransaction> Get_DM_HomeLoanTransaction(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_HomeLoanTransaction> Records = new List<DM_HomeLoanTransaction>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerHomeLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var HomeLoanTransactionRecords = nISEntitiesDataContext.NB_HomeLoanTransaction.Where(whereClause)?.OrderBy(it => it.Posting_Date)?.ToList();
                    if (HomeLoanTransactionRecords != null && HomeLoanTransactionRecords.Count > 0)
                    {
                        HomeLoanTransactionRecords.ForEach(item =>
                        {
                            Records.Add(new DM_HomeLoanTransaction()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                Credit = item.Credit,
                                Debit = item.Debit,
                                Description = item.Description,
                                Effective_Date = item.Effective_Date ?? DateTime.Now,
                                Posting_Date = item.Posting_Date ?? DateTime.Now,
                                RunningBalance = item.RunningBalance,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan arrears records
        /// </returns>
        public IList<DM_HomeLoanArrear> Get_DM_HomeLoanArrears(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_HomeLoanArrear> Records = new List<DM_HomeLoanArrear>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerHomeLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var HomeLoanArrearRecords = nISEntitiesDataContext.NB_HomeLoanArrears.Where(whereClause).ToList();
                    if (HomeLoanArrearRecords != null && HomeLoanArrearRecords.Count > 0)
                    {
                        HomeLoanArrearRecords.ForEach(item =>
                        {
                            Records.Add(new DM_HomeLoanArrear()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                ARREARS_120 = item.ARREARS_120,
                                ARREARS_90 = item.ARREARS_90,
                                ARREARS_60 = item.ARREARS_60,
                                ARREARS_30 = item.ARREARS_30,
                                CurrentDue = item.CurrentDue,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
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
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer home loan summary records
        /// </returns>
        public IList<DM_HomeLoanSummary> Get_DM_HomeLoanSummary(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_HomeLoanSummary> Records = new List<DM_HomeLoanSummary>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomerHomeLoan(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var HomeLoanSummaryRecords = nISEntitiesDataContext.NB_HomeLoanSummary.Where(whereClause).ToList();
                    if (HomeLoanSummaryRecords != null && HomeLoanSummaryRecords.Count > 0)
                    {
                        HomeLoanSummaryRecords.ForEach(item =>
                        {
                            Records.Add(new DM_HomeLoanSummary()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                InvestorId = item.InvestorId,
                                Annual_Insurance = item.Annual_Insurance,
                                Annual_Interest = item.Annual_Interest,
                                Annual_Legal_Costs = item.Annual_Legal_Costs,
                                Annual_Service_Fee = item.Annual_Service_Fee,
                                Annual_Total_Recvd = item.Annual_Total_Recvd,
                                Basic_Instalment = item.Basic_Instalment,
                                Capital_Redemption = item.Capital_Redemption,
                                Houseowner_Ins = item.Houseowner_Ins,
                                Loan_Protection = item.Loan_Protection,
                                Recovery_Fee_Debit = item.Recovery_Fee_Debit,
                                Service_Fee = item.Service_Fee,
                                Total_Instalment = item.Total_Instalment,
                                Message1 = item.Message1,
                                Message2 = item.Message2,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return Records;
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
        public IList<DM_AccountsSummary> GET_DM_AccountSummaries(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_AccountsSummary> records = new List<DM_AccountsSummary>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_AccountSummaryRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_AccountsSummary()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH,
                                TotalAmount = item.InvestmentAmt
                            });

                            records.Add(new DM_AccountsSummary()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.PERSONAL_LOAN_PAGE_TYPE,
                                TotalAmount = item.PersonalLoanAmt
                            });

                            records.Add(new DM_AccountsSummary()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE,
                                TotalAmount = item.HomeLoanAmt
                            });

                            records.Add(new DM_AccountsSummary()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = "Greenbacks rewards points",
                                TotalAmount = item.Greenbacks
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_AccountAnanlysis> GET_DM_AccountAnalysisDetails(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_AccountAnanlysis> records = new List<DM_AccountAnanlysis>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_AccountAnalysisRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_AccountAnanlysis()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH,
                                MonthwiseAmount = this.Get_MonthwiseAccountAnanlyses(item, HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH).ToList()
                            });

                            records.Add(new DM_AccountAnanlysis()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.PERSONAL_LOAN_PAGE_TYPE,
                                MonthwiseAmount = this.Get_MonthwiseAccountAnanlyses(item, HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList()
                            });

                            records.Add(new DM_AccountAnanlysis()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE,
                                MonthwiseAmount = this.Get_MonthwiseAccountAnanlyses(item, HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE).ToList()
                            });

                            //records.Add(new DM_AccountAnanlysis()
                            //{
                            //    CustomerId = item.CustomerId,
                            //    BatchId = item.BatchId,
                            //    AccountType = HtmlConstants.GREENBACKS_PAGE_TYPE,
                            //    MonthwiseAmount = this.Get_MonthwiseAccountAnanlyses(item, HtmlConstants.GREENBACKS_PAGE_TYPE).ToList()
                            //});
                        });
                    }
                }
                return records;
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
        public IList<DM_ReminderAndRecommendation> GET_DM_ReminderAndRecommendations(long ReminderId, string tenantCode)
        {
            IList<DM_ReminderAndRecommendation> records = new List<DM_ReminderAndRecommendation>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    List<DM_ReminderRecosRecord> dbRecords = new List<DM_ReminderRecosRecord>();
                    if (ReminderId > 0)
                    {
                        dbRecords = nISEntitiesDataContext.DM_ReminderRecosRecord.Where(it => it.Id == ReminderId && it.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        dbRecords = nISEntitiesDataContext.DM_ReminderRecosRecord.ToList();
                    }

                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_ReminderAndRecommendation()
                            {
                                Identifier = item.Id,
                                Description = item.Description,
                                IsGeneric = item.IsGeneric,
                                IsActionable = item.IsActionable,
                                ActionTitle = item.LabelText,
                                ActionUrl = item.TargetURL,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_CustomerReminderAndRecommendation> GET_DM_CustomerReminderAndRecommendations(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CustomerReminderAndRecommendation> records = new List<DM_CustomerReminderAndRecommendation>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_CustomerReminderRecosRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_CustomerReminderAndRecommendation()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                reminderAndRecommendation = this.GET_DM_ReminderAndRecommendations(item.ReminderId, tenantCode)?.FirstOrDefault()
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_NewsAndAlerts> GET_DM_NewsAndAlerts(long NewsAndAlertId, string tenantCode)
        {
            IList<DM_NewsAndAlerts> records = new List<DM_NewsAndAlerts>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    List<DM_NewsAndAlertsRecord> dbRecords = new List<DM_NewsAndAlertsRecord>();
                    if (NewsAndAlertId > 0)
                    {
                        dbRecords = nISEntitiesDataContext.DM_NewsAndAlertsRecord.Where(it => it.Id == NewsAndAlertId && it.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        dbRecords = nISEntitiesDataContext.DM_NewsAndAlertsRecord.ToList();
                    }

                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_NewsAndAlerts()
                            {
                                Identifier = item.Id,
                                Description = item.Description,
                                IsGeneric = item.IsGeneric,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_CustomerNewsAndAlert> GET_DM_CustomerNewsAndAlert(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CustomerNewsAndAlert> records = new List<DM_CustomerNewsAndAlert>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_CustomerNewsAndAlertsRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_CustomerNewsAndAlert()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                NewsAndAlert = this.GET_DM_NewsAndAlerts(item.NewsAndAlertId, tenantCode)?.FirstOrDefault()
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_GreenbacksMaster> GET_DM_GreenbacksMasterDetails(string tenantCode)
        {
            IList<DM_GreenbacksMaster> records = new List<DM_GreenbacksMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_GreenbacksMasterRecord.ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_GreenbacksMaster()
                            {
                                Identifier = item.Id,
                                JoinUsUrl = item.JoinUsUrl,
                                UseUsUrl = item.UseUsUrl,
                                ContactNumber = item.ContactNumber
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_GreenbacksRewardPoints> GET_DM_GreenbacksRewardPoints(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_GreenbacksRewardPoints> records = new List<DM_GreenbacksRewardPoints>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_CustomerRewardPointsRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        var res = 0.0m;
                        dbRecords.ForEach(item =>
                        {
                            if (!string.IsNullOrEmpty(item.Greenbacks1) && item.Greenbacks1 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Jan", RewardPoint = decimal.TryParse(item.Greenbacks1, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks2) && item.Greenbacks2 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Feb", RewardPoint = decimal.TryParse(item.Greenbacks2, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks3) && item.Greenbacks3 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Mar", RewardPoint = decimal.TryParse(item.Greenbacks3, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks4) && item.Greenbacks4 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Apr", RewardPoint = decimal.TryParse(item.Greenbacks4, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks5) && item.Greenbacks5 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "May", RewardPoint = decimal.TryParse(item.Greenbacks5, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks6) && item.Greenbacks6 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Jun", RewardPoint = decimal.TryParse(item.Greenbacks6, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks7) && item.Greenbacks7 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Jul", RewardPoint = decimal.TryParse(item.Greenbacks7, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks8) && item.Greenbacks8 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Aug", RewardPoint = decimal.TryParse(item.Greenbacks8, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks9) && item.Greenbacks9 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Sep", RewardPoint = decimal.TryParse(item.Greenbacks9, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks10) && item.Greenbacks10 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Oct", RewardPoint = decimal.TryParse(item.Greenbacks10, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks11) && item.Greenbacks11 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Nov", RewardPoint = decimal.TryParse(item.Greenbacks11, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks12) && item.Greenbacks12 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPoints() { Month = "Dec", RewardPoint = decimal.TryParse(item.Greenbacks12, out res) ? res : 0 });
                            }
                        });
                    }
                }
                return records;
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
        public IList<DM_GreenbacksRewardPointsRedeemed> GET_DM_GreenbacksRewardPointsRedeemed(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_GreenbacksRewardPointsRedeemed> records = new List<DM_GreenbacksRewardPointsRedeemed>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_CustomerRewardPointsRedeemedRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        var res = 0.0m;
                        dbRecords.ForEach(item =>
                        {
                            if (!string.IsNullOrEmpty(item.Greenbacks1) && item.Greenbacks1 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Jan", RedeemedPoints = decimal.TryParse(item.Greenbacks1, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks2) && item.Greenbacks2 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Feb", RedeemedPoints = decimal.TryParse(item.Greenbacks2, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks3) && item.Greenbacks3 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Mar", RedeemedPoints = decimal.TryParse(item.Greenbacks3, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks4) && item.Greenbacks4 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Apr", RedeemedPoints = decimal.TryParse(item.Greenbacks4, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks5) && item.Greenbacks5 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "May", RedeemedPoints = decimal.TryParse(item.Greenbacks5, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks6) && item.Greenbacks6 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Jun", RedeemedPoints = decimal.TryParse(item.Greenbacks6, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks7) && item.Greenbacks7 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Jul", RedeemedPoints = decimal.TryParse(item.Greenbacks7, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks8) && item.Greenbacks8 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Aug", RedeemedPoints = decimal.TryParse(item.Greenbacks8, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks9) && item.Greenbacks9 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Sep", RedeemedPoints = decimal.TryParse(item.Greenbacks9, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks10) && item.Greenbacks10 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Oct", RedeemedPoints = decimal.TryParse(item.Greenbacks10, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks11) && item.Greenbacks11 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Nov", RedeemedPoints = decimal.TryParse(item.Greenbacks11, out res) ? res : 0 });
                            }
                            if (!string.IsNullOrEmpty(item.Greenbacks12) && item.Greenbacks12 != "0")
                            {
                                records.Add(new DM_GreenbacksRewardPointsRedeemed() { Month = "Dec", RedeemedPoints = decimal.TryParse(item.Greenbacks12, out res) ? res : 0 });
                            }
                        });
                    }
                }
                return records;
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
        public IList<DM_CustomerProductWiseRewardPoints> GET_DM_CustomerProductWiseRewardPoints(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CustomerProductWiseRewardPoints> records = new List<DM_CustomerProductWiseRewardPoints>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_CustomerProductWiseRewardPointsRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_CustomerProductWiseRewardPoints()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH,
                                MonthwiseRewardPoints = this.Get_MonthwiseRewardPoints(item, HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH).ToList(),
                                TenantCode = item.TenantCode
                            });

                            records.Add(new DM_CustomerProductWiseRewardPoints()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE,
                                MonthwiseRewardPoints = this.Get_MonthwiseRewardPoints(item, HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE).ToList(),
                                TenantCode = item.TenantCode
                            });

                            records.Add(new DM_CustomerProductWiseRewardPoints()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                AccountType = HtmlConstants.PERSONAL_LOAN_PAGE_TYPE,
                                MonthwiseRewardPoints = this.Get_MonthwiseRewardPoints(item, HtmlConstants.PERSONAL_LOAN_PAGE_TYPE).ToList(),
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return records;
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
        public IList<DM_CustomerRewardSpendByCategory> GET_DM_CustomerRewardSpendByCategory(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<DM_CustomerRewardSpendByCategory> records = new List<DM_CustomerRewardSpendByCategory>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForDmCustomer(searchParameter, tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    var dbRecords = nISEntitiesDataContext.DM_CustomerRewardSpendByCategoryRecord.Where(whereClause).ToList();
                    if (dbRecords != null && dbRecords.Count > 0)
                    {
                        var res = 0.0m;
                        dbRecords.ForEach(item =>
                        {
                            records.Add(new DM_CustomerRewardSpendByCategory()
                            {
                                CustomerId = item.CustomerId,
                                BatchId = item.BatchId,
                                Category = item.Category,
                                SpendReward = decimal.TryParse(item.Points, out res) ? res : 0,
                                TenantCode = item.TenantCode,
                                Identifier = item.Id
                            });
                        });
                    }
                }
                return records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

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

        #endregion

        #region Private methods for Nedbank Tenant

        private string WhereClauseGeneratorForDmCustomer(CustomerSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id == {0} ", item))) + ") and ");
            }
            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId == {0} ", item))) + ") and ");
            }
            if (validationEngine.IsValidLong(searchParameter.BatchId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId == {0} ", item))) + ") ");
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

        private string WhereClauseGeneratorForCustomerInvestment(CustomerInvestmentSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            //send account id value to this property when account master data fetching
            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            //send account id value to this property when account transaction data fetching
            if (validationEngine.IsValidLong(searchParameter.InvestmentId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.InvestmentId.ToString().Split(',').Select(item => string.Format("InvestmentId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.BatchId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.InvestorId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.InvestorId.ToString().Split(',').Select(item => string.Format("InvestorId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") ");
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

        private string WhereClauseGeneratorForCustomerPersonalLoan(CustomerPersonalLoanSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            //send account id value to this property when account master data fetching
            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.InvestorId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.InvestorId.ToString().Split(',').Select(item => string.Format("InvestorId.Equals({0}) ", item))) + ") and ");
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

        private string WhereClauseGeneratorForCustomerHomeLoan(CustomerHomeLoanSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            //send account id value to this property when account master data fetching
            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.InvestorId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.InvestorId.ToString().Split(',').Select(item => string.Format("InvestorId.Equals({0}) ", item))) + ") and ");
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

        private string WhereClauseGeneratorForMessageOrNoteSearch(MessageAndNoteSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("ParentId.Equals({0}) ", item))) + ") and ");
            }

            if (validationEngine.IsValidText(searchParameter.Type))
            {
                queryString.Append(string.Format("Type.Equals(\"{0}\") and ", searchParameter.Type));
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

        private IList<DM_MonthwiseAccountAnanlysis> Get_MonthwiseAccountAnanlyses(DM_AccountAnalysisRecord _AccountAnanlysis, string dataFor)
        {
            IList<DM_MonthwiseAccountAnanlysis> records = new List<DM_MonthwiseAccountAnanlysis>();
            try
            {
                decimal res = 0.0m;
                switch (dataFor)
                {
                    case HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH:
                    case HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH:
                    case HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN:
                    case HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN:
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt1) && _AccountAnanlysis.InvestmentAmt1 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jan", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt2) && _AccountAnanlysis.InvestmentAmt2 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Feb", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt3) && _AccountAnanlysis.InvestmentAmt3 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Mar", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt4) && _AccountAnanlysis.InvestmentAmt4 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Apr", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt5) && _AccountAnanlysis.InvestmentAmt5 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "May", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt6) && _AccountAnanlysis.InvestmentAmt6 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jun", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt7) && _AccountAnanlysis.InvestmentAmt7 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jul", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt8) && _AccountAnanlysis.InvestmentAmt8 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Aug", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt9) && _AccountAnanlysis.InvestmentAmt9 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Sep", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt10) && _AccountAnanlysis.InvestmentAmt10 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Oct", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt11) && _AccountAnanlysis.InvestmentAmt11 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Nov", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.InvestmentAmt12) && _AccountAnanlysis.InvestmentAmt12 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Dec", Amount = decimal.TryParse(_AccountAnanlysis.InvestmentAmt12, out res) ? res : 0 });
                        }
                        break;

                    case HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE:
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt1) && _AccountAnanlysis.HomeLoanAmt1 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jan", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt2) && _AccountAnanlysis.HomeLoanAmt2 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Feb", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt3) && _AccountAnanlysis.HomeLoanAmt3 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Mar", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt4) && _AccountAnanlysis.HomeLoanAmt4 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Apr", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt5) && _AccountAnanlysis.HomeLoanAmt5 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "May", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt6) && _AccountAnanlysis.HomeLoanAmt6 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jun", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt7) && _AccountAnanlysis.HomeLoanAmt7 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jul", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt8) && _AccountAnanlysis.HomeLoanAmt8 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Aug", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt9) && _AccountAnanlysis.HomeLoanAmt9 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Sep", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt10) && _AccountAnanlysis.HomeLoanAmt10 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Oct", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt11) && _AccountAnanlysis.HomeLoanAmt11 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Nov", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.HomeLoanAmt12) && _AccountAnanlysis.HomeLoanAmt12 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Dec", Amount = decimal.TryParse(_AccountAnanlysis.HomeLoanAmt12, out res) ? res : 0 });
                        }
                        break;

                    case HtmlConstants.PERSONAL_LOAN_PAGE_TYPE:
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt1) && _AccountAnanlysis.PersonalLoanAmt1 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jan", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt2) && _AccountAnanlysis.PersonalLoanAmt2 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Feb", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt3) && _AccountAnanlysis.PersonalLoanAmt3 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Mar", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt4) && _AccountAnanlysis.PersonalLoanAmt4 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Apr", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt5) && _AccountAnanlysis.PersonalLoanAmt5 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "May", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt6) && _AccountAnanlysis.PersonalLoanAmt6 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jun", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt7) && _AccountAnanlysis.PersonalLoanAmt7 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jul", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt8) && _AccountAnanlysis.PersonalLoanAmt8 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Aug", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt9) && _AccountAnanlysis.PersonalLoanAmt9 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Sep", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt10) && _AccountAnanlysis.PersonalLoanAmt10 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Oct", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt11) && _AccountAnanlysis.PersonalLoanAmt11 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Nov", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.PersonalLoanAmt12) && _AccountAnanlysis.PersonalLoanAmt12 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Dec", Amount = decimal.TryParse(_AccountAnanlysis.PersonalLoanAmt12, out res) ? res : 0 });
                        }
                        break;

                    case HtmlConstants.GREENBACKS_PAGE_TYPE:
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks1) && _AccountAnanlysis.Greenbacks1 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jan", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks2) && _AccountAnanlysis.Greenbacks2 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Feb", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks3) && _AccountAnanlysis.Greenbacks3 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Mar", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks4) && _AccountAnanlysis.Greenbacks4 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Apr", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks5) && _AccountAnanlysis.Greenbacks5 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "May", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks6) && _AccountAnanlysis.Greenbacks6 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jun", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks7) && _AccountAnanlysis.Greenbacks7 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Jul", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks8) && _AccountAnanlysis.Greenbacks8 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Aug", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks9) && _AccountAnanlysis.Greenbacks9 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Sep", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks10) && _AccountAnanlysis.Greenbacks10 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Oct", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks11) && _AccountAnanlysis.Greenbacks11 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Nov", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(_AccountAnanlysis.Greenbacks12) && _AccountAnanlysis.Greenbacks12 != "0")
                        {
                            records.Add(new DM_MonthwiseAccountAnanlysis() { Month = "Dec", Amount = decimal.TryParse(_AccountAnanlysis.Greenbacks12, out res) ? res : 0 });
                        }
                        break;
                }
                return records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IList<DM_MonthwiseProductRewardPoints> Get_MonthwiseRewardPoints(DM_CustomerProductWiseRewardPointsRecord rp, string dataFor)
        {
            IList<DM_MonthwiseProductRewardPoints> records = new List<DM_MonthwiseProductRewardPoints>();
            try
            {
                decimal res = 0.0m;
                switch (dataFor)
                {
                    case HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_ENGLISH:
                    case HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_ENGLISH:
                    case HtmlConstants.INVESTMENT_PAGE_TYPE_OTHER_AFRICAN:
                    case HtmlConstants.WEALTH_INVESTMENT_PAGE_TYPE_WEALTH_AFRICAN:
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks1) && rp.InvestmentGreenbacks1 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jan", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks2) && rp.InvestmentGreenbacks2 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Feb", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks3) && rp.InvestmentGreenbacks3 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Mar", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks4) && rp.InvestmentGreenbacks4 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Apr", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks5) && rp.InvestmentGreenbacks5 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "May", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks6) && rp.InvestmentGreenbacks6 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jun", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks7) && rp.InvestmentGreenbacks7 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jul", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks8) && rp.InvestmentGreenbacks8 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Aug", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks9) && rp.InvestmentGreenbacks9 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Sep", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks10) && rp.InvestmentGreenbacks10 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Oct", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks11) && rp.InvestmentGreenbacks11 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Nov", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.InvestmentGreenbacks12) && rp.InvestmentGreenbacks12 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Dec", RewardPoint = decimal.TryParse(rp.InvestmentGreenbacks12, out res) ? res : 0 });
                        }
                        break;

                    case HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_ENG_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_OTHER_SEGMENT_AFR_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_ENG_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_PML_SEGMENT_AFR_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_ENG_PAGE_TYPE:
                    case HtmlConstants.HOME_LOAN_FOR_WEA_SEGMENT_AFR_PAGE_TYPE:
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks1) && rp.HomeLoanGreenbacks1 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jan", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks2) && rp.HomeLoanGreenbacks2 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Feb", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks3) && rp.HomeLoanGreenbacks3 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Mar", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks4) && rp.HomeLoanGreenbacks4 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Apr", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks5) && rp.HomeLoanGreenbacks5 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "May", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks6) && rp.HomeLoanGreenbacks6 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jun", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks7) && rp.HomeLoanGreenbacks7 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jul", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks8) && rp.HomeLoanGreenbacks8 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Aug", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks9) && rp.HomeLoanGreenbacks9 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Sep", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks10) && rp.HomeLoanGreenbacks10 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Oct", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks11) && rp.HomeLoanGreenbacks11 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Nov", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.HomeLoanGreenbacks12) && rp.HomeLoanGreenbacks12 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Dec", RewardPoint = decimal.TryParse(rp.HomeLoanGreenbacks12, out res) ? res : 0 });
                        }
                        break;

                    case HtmlConstants.PERSONAL_LOAN_PAGE_TYPE:
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks1) && rp.PersonalLoanGreenbacks1 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jan", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks1, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks2) && rp.PersonalLoanGreenbacks2 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Feb", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks2, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks3) && rp.PersonalLoanGreenbacks3 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Mar", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks3, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks4) && rp.PersonalLoanGreenbacks4 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Apr", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks4, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks5) && rp.PersonalLoanGreenbacks5 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "May", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks5, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks6) && rp.PersonalLoanGreenbacks6 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jun", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks6, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks7) && rp.PersonalLoanGreenbacks7 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Jul", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks7, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks8) && rp.PersonalLoanGreenbacks8 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Aug", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks8, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks9) && rp.PersonalLoanGreenbacks9 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Sep", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks9, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks10) && rp.PersonalLoanGreenbacks10 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Oct", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks10, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks11) && rp.PersonalLoanGreenbacks11 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Nov", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks11, out res) ? res : 0 });
                        }
                        if (!string.IsNullOrEmpty(rp.PersonalLoanGreenbacks12) && rp.PersonalLoanGreenbacks12 != "0")
                        {
                            records.Add(new DM_MonthwiseProductRewardPoints() { Month = "Dec", RewardPoint = decimal.TryParse(rp.PersonalLoanGreenbacks12, out res) ? res : 0 });
                        }
                        break;
                }
                return records;
            }
            catch (Exception ex)
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
