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

                    query.ForEach(q => {
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

        #region Nedbank Tenant

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
                                DS_Investor_Name = item.DS_Investor_Name,
                                EmailAddress = item.EmailAddress,
                                Mask_Cell_No = item.Mask_Cell_No,
                                Barcode = item.Barcode,
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
                    var InvestmentTransactionRecords = nISEntitiesDataContext.DM_InvestmentTransactionRecord.Where(whereClause)?.OrderBy(it => it.TransactionDate).ToList();
                    if (InvestmentTransactionRecords != null && InvestmentTransactionRecords.Count > 0)
                    {
                        InvestmentTransactionRecords.ForEach(item =>
                        {
                            InvestmentTransactions.Add(new DM_InvestmentTransaction()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
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
                    var HomeLoanMasterRecords = nISEntitiesDataContext.DM_HomeLoanMasterRecord.Where(whereClause).ToList();
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
                    var HomeLoanTransactionRecords = nISEntitiesDataContext.DM_HomeLoanTransactionRecord.Where(whereClause)?.OrderBy(it => it.Posting_Date)?.ToList();
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
                                Effective_date = item.Effective_Date ?? DateTime.Now,
                                Posting_date = item.Posting_Date ?? DateTime.Now,
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
                    var HomeLoanArrearRecords = nISEntitiesDataContext.DM_HomeLoanArrearsRecord.Where(whereClause).ToList();
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
                    var HomeLoanSummaryRecords = nISEntitiesDataContext.DM_HomeLoanSummaryRecord.Where(whereClause).ToList();
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

        #endregion

        #endregion

        #region Private Methods

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

        private string WhereClauseGeneratorForDmCustomer(CustomerSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (validationEngine.IsValidLong(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }
            if (validationEngine.IsValidLong(searchParameter.CustomerId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.CustomerId.ToString().Split(',').Select(item => string.Format("CustomerId.Equals({0}) ", item))) + ") and ");
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
