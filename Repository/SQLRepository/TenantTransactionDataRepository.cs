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
                    //if (customerSearchParameter.PagingParameter.PageIndex > 0 && customerSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    customerMasterRecords = nISEntitiesDataContext.TTD_CustomerMasterRecord
                    //    .OrderBy(customerSearchParameter.SortParameter.SortColumn + " " + customerSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((customerSearchParameter.PagingParameter.PageIndex - 1) * customerSearchParameter.PagingParameter.PageSize)
                    //    .Take(customerSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    customerMasterRecords = nISEntitiesDataContext.TTD_CustomerMasterRecord
                    .Where(whereClause)
                    //.OrderBy(customerSearchParameter.SortParameter.SortColumn + " " + customerSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                                EmailId = item.Email,
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
                    //if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    subscriptionUsageRecords = nISEntitiesDataContext.TTD_SubscriptionUsageRecord
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    subscriptionUsageRecords = nISEntitiesDataContext.TTD_SubscriptionUsageRecord
                    .Where(whereClause)
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    // }

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
                    //if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    subscriptionSpendRecords = nISEntitiesDataContext.TTD_SubscriptionSpendRecord
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    subscriptionSpendRecords = nISEntitiesDataContext.TTD_SubscriptionSpendRecord
                    .Where(whereClause)
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    userSubscriptionRecords = nISEntitiesDataContext.TTD_UserSubscriptionsRecord
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    userSubscriptionRecords = nISEntitiesDataContext.TTD_UserSubscriptionsRecord
                    .Where(whereClause)
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    dataUsageRecords = nISEntitiesDataContext.TTD_DataUsageRecord
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    dataUsageRecords = nISEntitiesDataContext.TTD_DataUsageRecord
                    .Where(whereClause)
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    meetingUsageRecords = nISEntitiesDataContext.TTD_MeetingUsageRecord
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    meetingUsageRecords = nISEntitiesDataContext.TTD_MeetingUsageRecord
                    .Where(whereClause)
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    emailsBySubscriptionRecords = nISEntitiesDataContext.TTD_EmailsBySubscriptionRecord
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    emailsBySubscriptionRecords = nISEntitiesDataContext.TTD_EmailsBySubscriptionRecord
                    .Where(whereClause)
                    //    .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (customerSearchParameter.PagingParameter.PageIndex > 0 && customerSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    customerMasterRecords = nISEntitiesDataContext.CustomerMasterRecords
                    //    .OrderBy(customerSearchParameter.SortParameter.SortColumn + " " + customerSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((customerSearchParameter.PagingParameter.PageIndex - 1) * customerSearchParameter.PagingParameter.PageSize)
                    //    .Take(customerSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    customerMasterRecords = nISEntitiesDataContext.CustomerMasterRecords
                    .Where(whereClause)
                    //    .OrderBy(customerSearchParameter.SortParameter.SortColumn + " " + customerSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (accountSearchParameter.PagingParameter.PageIndex > 0 && accountSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    accountMasterRecords = nISEntitiesDataContext.AccountMasterRecords
                    //    .OrderBy(accountSearchParameter.SortParameter.SortColumn + " " + accountSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((accountSearchParameter.PagingParameter.PageIndex - 1) * accountSearchParameter.PagingParameter.PageSize)
                    //    .Take(accountSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    accountMasterRecords = nISEntitiesDataContext.AccountMasterRecords
                    .Where(whereClause)
                    //    .OrderBy(accountSearchParameter.SortParameter.SortColumn + " " + accountSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                    //if (accountSearchParameter.PagingParameter.PageIndex > 0 && accountSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    accountTransactionRecords = nISEntitiesDataContext.AccountTransactionRecords
                    //    .OrderBy(accountSearchParameter.SortParameter.SortColumn + " " + accountSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((accountSearchParameter.PagingParameter.PageIndex - 1) * accountSearchParameter.PagingParameter.PageSize)
                    //    .Take(accountSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    accountTransactionRecords = nISEntitiesDataContext.AccountTransactionRecords
                    .Where(whereClause)
                    //    .OrderBy(accountSearchParameter.SortParameter.SortColumn + " " + accountSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

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
                                TransactionDate = item.TransactionDate.ToShortDateString(),
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
                var savingTrendRecords = new List<SavingTrendRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    //if (accountSearchParameter.PagingParameter.PageIndex > 0 && accountSearchParameter.PagingParameter.PageSize > 0)
                    //{
                    //    savingTrendRecords = nISEntitiesDataContext.SavingTrendRecords
                    //    .OrderBy(accountSearchParameter.SortParameter.SortColumn + " " + accountSearchParameter.SortParameter.SortOrder.ToString())
                    //    .Where(whereClause)
                    //    .Skip((accountSearchParameter.PagingParameter.PageIndex - 1) * accountSearchParameter.PagingParameter.PageSize)
                    //    .Take(accountSearchParameter.PagingParameter.PageSize)
                    //    .ToList();
                    //}
                    //else
                    //{
                    savingTrendRecords = nISEntitiesDataContext.SavingTrendRecords
                    .Where(whereClause)
                    //    .OrderBy(accountSearchParameter.SortParameter.SortColumn + " " + accountSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                    .ToList();
                    //}

                    if (savingTrendRecords != null && savingTrendRecords.Count > 0)
                    {
                        savingTrendRecords.ForEach(item =>
                        {
                            savingTrends.Add(new SavingTrend()
                            {
                                Identifier = item.Id,
                                BatchId = item.BatchId,
                                CustomerId = item.CustomerId,
                                AccountId = item.AccountId,
                                Income = item.Income ?? 0,
                                IncomePercentage = item.IncomePercentage ?? 0,
                                Month = item.Month,
                                SpendAmount = item.SpendAmount,
                                SpendPercentage = item.SpendPercentage ?? 0,
                                TenantCode = item.TenantCode
                            });
                        });
                    }
                }
                return savingTrends;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            var condtionalOp = filterEntity.ConditionalOperator != null && filterEntity.ConditionalOperator != string.Empty ? filterEntity.ConditionalOperator : " and ";
            if (filterEntity.Operator == "EqualsTo")
            {
                queryString = queryString + condtionalOp + (string.Format(filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotEqualsTo")
            {
                queryString = queryString + condtionalOp + (string.Format("!" + filterEntity.FieldName + ".Equals(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "Contains")
            {
                queryString = queryString + condtionalOp + (string.Format(filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "NotContains")
            {
                queryString = queryString + condtionalOp + (string.Format("!" + filterEntity.FieldName + ".Contains(\"{0}\") ", filterEntity.Value));
            }
            else if (filterEntity.Operator == "LessThan")
            {
                queryString = queryString + condtionalOp + (string.Format(filterEntity.FieldName + " < " + filterEntity.Value + " "));
            }
            else if (filterEntity.Operator == "GreaterThan")
            {
                queryString = queryString + condtionalOp + (string.Format(filterEntity.FieldName + " > " + filterEntity.Value + " "));
            }
            return queryString;
        }

        #endregion


    }
}
