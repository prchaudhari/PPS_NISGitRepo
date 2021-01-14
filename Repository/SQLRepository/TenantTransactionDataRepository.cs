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
        /// <param name="customerSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of customer master
        /// </returns>
        public IList<CustomerMaster> Get_TTD_CustomerMasters(CustomerSearchParameter searchParameter, string tenantCode)
        {
            IList<CustomerMaster> customerMasters = new List<CustomerMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGeneratorForCustomer(searchParameter, tenantCode);
                var customerMasterRecords = new List<TTD_CustomerMasterRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    customerMasterRecords = nISEntitiesDataContext.TTD_CustomerMasterRecord.Where(whereClause).ToList();
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
        /// This method gets the specified list of subscription master from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription master
        /// </returns>
        public IList<SubscriptionMaster> Get_TTD_SubscriptionMasters(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<SubscriptionMaster> subscriptionMasters = new List<SubscriptionMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var subscriptionMasterRecords = new List<TTD_SubscriptionMasterRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    subscriptionMasterRecords = nISEntitiesDataContext.TTD_SubscriptionMasterRecord.Where(whereClause).ToList();
                    if (subscriptionMasterRecords != null && subscriptionMasterRecords.Count > 0)
                    {
                        subscriptionMasterRecords.ForEach(item =>
                        {
                            var customermaster = nISEntitiesDataContext.CustomerMasterRecords.Where(it => it.Id == searchParameter.CustomerId && it.BatchId == searchParameter.BatchId).ToList().FirstOrDefault();
                            subscriptionMasters.Add(new SubscriptionMaster()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                CustomerCode = item.CustomerCode,
                                EmployeeID = item.CustomerCode,
                                EmployeeName = customermaster.FirstName + " " + customermaster.LastName,
                                VendorName = item.VendorName,
                                Subscription = item.Subscription,
                                Email = item.Email,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return subscriptionMasters;
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
        public IList<SubscriptionUsage> Get_TTD_SubscriptionUsages(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<SubscriptionUsage> subscriptionUsages = new List<SubscriptionUsage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var subscriptionUsageRecords = new List<TTD_SubscriptionUsageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    subscriptionUsageRecords = nISEntitiesDataContext.TTD_SubscriptionUsageRecord.Where(whereClause).ToList();
                    if (subscriptionUsageRecords != null && subscriptionUsageRecords.Count > 0)
                    {
                        subscriptionUsageRecords.ForEach(item =>
                        {
                            subscriptionUsages.Add(new SubscriptionUsage()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                VendorName = item.VendorName,
                                Subscription = item.Subscription,
                                Email = item.Email,
                                Usage = item.Usage,
                                Emails = item.Emails,
                                Meetings = item.Meetings,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return subscriptionUsages;
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
        public IList<SubscriptionSummary> Get_TTD_SubscriptionSummaries(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<SubscriptionSummary> subscriptionSummaries = new List<SubscriptionSummary>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var subscriptionSummaryRecords = new List<TTD_SubscriptionSummaryRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    subscriptionSummaryRecords = nISEntitiesDataContext.TTD_SubscriptionSummaryRecord.Where(whereClause).ToList();
                    if (subscriptionSummaryRecords != null && subscriptionSummaryRecords.Count > 0)
                    {
                        subscriptionSummaryRecords.ForEach(item =>
                        {
                            subscriptionSummaries.Add(new SubscriptionSummary()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                Vendor = item.VendorName,
                                Subscription = item.Subscription,
                                Total = item.Total,
                                AverageSpend = item.AverageSpend,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return subscriptionSummaries;
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
        public IList<SubscriptionSpend> Get_TTD_SubscriptionSpends(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<SubscriptionSpend> subscriptionSpends = new List<SubscriptionSpend>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var subscriptionSpendRecords = new List<TTD_SubscriptionSpendRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    subscriptionSpendRecords = nISEntitiesDataContext.TTD_SubscriptionSpendRecord.Where(whereClause).ToList();
                    if (subscriptionSpendRecords != null && subscriptionSpendRecords.Count > 0)
                    {
                        subscriptionSpendRecords.ForEach(item =>
                        {
                            subscriptionSpends.Add(new SubscriptionSpend()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                Month = item.Month,
                                Year = item.Year,
                                Microsoft = item.Microsoft,
                                Zoom = item.Zoom,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return subscriptionSpends;
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
        public IList<UserSubscription> Get_TTD_UserSubscriptions(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<UserSubscription> userSubscriptions = new List<UserSubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var userSubscriptionRecords = new List<TTD_UserSubscriptionsRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    userSubscriptionRecords = nISEntitiesDataContext.TTD_UserSubscriptionsRecord.Where(whereClause).ToList();
                    if (userSubscriptionRecords != null && userSubscriptionRecords.Count > 0)
                    {
                        userSubscriptionRecords.ForEach(item =>
                        {
                            userSubscriptions.Add(new UserSubscription()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                CountOfSubscription = item.CountOfSubscription,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return userSubscriptions;
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
        public IList<VendorSubscription> Get_TTD_VendorSubscriptions(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<VendorSubscription> vendorSubscriptions = new List<VendorSubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var vendorSubscriptionRecords = new List<TTD_VendorSubscriptionRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    vendorSubscriptionRecords = nISEntitiesDataContext.TTD_VendorSubscriptionRecord.Where(whereClause).ToList();
                    if (vendorSubscriptionRecords != null && vendorSubscriptionRecords.Count > 0)
                    {
                        vendorSubscriptionRecords.ForEach(item =>
                        {
                            vendorSubscriptions.Add(new VendorSubscription()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                VenderName = item.VenderName,
                                CountOfSubscription = item.CountOfSubscription,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return vendorSubscriptions;
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
        public IList<DataUsage> Get_TTD_DataUsages(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<DataUsage> dataUsages = new List<DataUsage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var dataUsageRecords = new List<TTD_DataUsageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    dataUsageRecords = nISEntitiesDataContext.TTD_DataUsageRecord.Where(whereClause).ToList();
                    if (dataUsageRecords != null && dataUsageRecords.Count > 0)
                    {
                        dataUsageRecords.ForEach(item =>
                        {
                            dataUsages.Add(new DataUsage()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                Month = item.Month,
                                Year = item.Year,
                                Microsoft = item.Microsoft,
                                Zoom = item.Zoom,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return dataUsages;
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
        public IList<MeetingUsage> Get_TTD_MeetingUsages(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<MeetingUsage> meetingUsages = new List<MeetingUsage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var meetingUsageRecords = new List<TTD_MeetingUsageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    meetingUsageRecords = nISEntitiesDataContext.TTD_MeetingUsageRecord.Where(whereClause).ToList();
                    if (meetingUsageRecords != null && meetingUsageRecords.Count > 0)
                    {
                        meetingUsageRecords.ForEach(item =>
                        {
                            meetingUsages.Add(new MeetingUsage()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                Month = item.Month,
                                Year = item.Year,
                                Microsoft = item.Microsoft,
                                Zoom = item.Zoom,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return meetingUsages;
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
        public IList<EmailsBySubscription> Get_TTD_EmailsBySubscription(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            IList<EmailsBySubscription> emailsBySubscriptions = new List<EmailsBySubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);
                var emailsBySubscriptionRecords = new List<TTD_EmailsBySubscriptionRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    emailsBySubscriptionRecords = nISEntitiesDataContext.TTD_EmailsBySubscriptionRecord.Where(whereClause).ToList();
                    if (emailsBySubscriptionRecords != null && emailsBySubscriptionRecords.Count > 0)
                    {
                        emailsBySubscriptionRecords.ForEach(item =>
                        {
                            emailsBySubscriptions.Add(new EmailsBySubscription()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                Emails = item.Emails,
                                Subscription = item.Subscription,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return emailsBySubscriptions;
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Role search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(TransactionDataSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            if (validationEngine.IsValidLong(searchParameter.BatchId))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.BatchId.ToString().Split(',').Select(item => string.Format("BatchId.Equals({0}) ", item))) + ") and ");
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
