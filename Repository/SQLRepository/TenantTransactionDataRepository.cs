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

    public class TenantTransactionDataRepository: ITenantTransactionDataRepository
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
        /// This method gets the specified list of subscription master from tenant transaction data repository.
        /// </summary>
        /// <param name="subscriptionMasterSearchParameter">The subscription master search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription master
        /// </returns>
        public IList<SubscriptionMaster> Get_TTD_SubscriptionMasters(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            IList<SubscriptionMaster> subscriptionMasters = new List<SubscriptionMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(subscriptionMasterSearchParameter, tenantCode);
                var subscriptionMasterRecords = new List<TTD_SubscriptionMasterRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    {
                        subscriptionMasterRecords = nISEntitiesDataContext.TTD_SubscriptionMasterRecord
                        .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                        .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else 
                    {
                        subscriptionMasterRecords = nISEntitiesDataContext.TTD_SubscriptionMasterRecord
                        .Where(whereClause)
                        .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (subscriptionMasterRecords != null && subscriptionMasterRecords.Count > 0)
                    {
                        subscriptionMasterRecords.ForEach(item =>
                        {
                            subscriptionMasters.Add(new SubscriptionMaster()
                            {
                                Identifier = item.Id,
                                VendorName = item.VendorName,
                                Subscription = item.Subscription,
                                EmployeeId = item.EmployeeID,
                                EmployeeName = item.EmployeeName,
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
        public IList<SubscriptionUsage> Get_TTD_SubscriptionUsages(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            IList<SubscriptionUsage> subscriptionUsages = new List<SubscriptionUsage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(subscriptionMasterSearchParameter, tenantCode);
                var subscriptionUsageRecords = new List<TTD_SubscriptionUsageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    {
                        subscriptionUsageRecords = nISEntitiesDataContext.TTD_SubscriptionUsageRecord
                        .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                        .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        subscriptionUsageRecords = nISEntitiesDataContext.TTD_SubscriptionUsageRecord
                        .Where(whereClause)
                        .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (subscriptionUsageRecords != null && subscriptionUsageRecords.Count > 0)
                    {
                        subscriptionUsageRecords.ForEach(item =>
                        {
                            subscriptionUsages.Add(new SubscriptionUsage()
                            {
                                Identifier = item.Id,
                                VendorName = item.VendorName,
                                Subscription = item.Subscription,
                                EmployeeId = item.EmployeeID,
                                EmployeeName = item.EmployeeName,
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
        public IList<SubscriptionSummary> Get_TTD_SubscriptionSummaries(SubscriptionMasterSearchParameter subscriptionMasterSearchParameter, string tenantCode)
        {
            IList<SubscriptionSummary> subscriptionSummaries = new List<SubscriptionSummary>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(subscriptionMasterSearchParameter, tenantCode);
                var subscriptionSummaryRecords = new List<TTD_SubscriptionSummaryRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (subscriptionMasterSearchParameter.PagingParameter.PageIndex > 0 && subscriptionMasterSearchParameter.PagingParameter.PageSize > 0)
                    {
                        subscriptionSummaryRecords = nISEntitiesDataContext.TTD_SubscriptionSummaryRecord
                        .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((subscriptionMasterSearchParameter.PagingParameter.PageIndex - 1) * subscriptionMasterSearchParameter.PagingParameter.PageSize)
                        .Take(subscriptionMasterSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        subscriptionSummaryRecords = nISEntitiesDataContext.TTD_SubscriptionSummaryRecord
                        .Where(whereClause)
                        .OrderBy(subscriptionMasterSearchParameter.SortParameter.SortColumn + " " + subscriptionMasterSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (subscriptionSummaryRecords != null && subscriptionSummaryRecords.Count > 0)
                    {
                        subscriptionSummaryRecords.ForEach(item =>
                        {
                            subscriptionSummaries.Add(new SubscriptionSummary()
                            {
                                Identifier = item.Id,
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
        /// <param name="month">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of subscription spends
        /// </returns>
        public IList<SubscriptionSpend> Get_TTD_SubscriptionSpends(string month, string tenantCode)
        {
            IList<SubscriptionSpend> subscriptionSpends = new List<SubscriptionSpend>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                var subscriptionSpendRecords = new List<TTD_SubscriptionSpendRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (month == null || month == string.Empty)
                    {
                        subscriptionSpendRecords = nISEntitiesDataContext.TTD_SubscriptionSpendRecord.Where(item => item.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        subscriptionSpendRecords = nISEntitiesDataContext.TTD_SubscriptionSpendRecord.Where(item => item.Month.Contains(month) && item.TenantCode == tenantCode).ToList();
                    }

                    if (subscriptionSpendRecords != null && subscriptionSpendRecords.Count > 0)
                    {
                        subscriptionSpendRecords.ForEach(item =>
                        {
                            subscriptionSpends.Add(new SubscriptionSpend()
                            {
                                Identifier = item.Id,
                                Month = item.Month,
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
        /// <param name="employeeId">The employee id value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of user subscriptions
        /// </returns>
        public IList<UserSubscription> Get_TTD_UserSubscriptions(string employeeId, string tenantCode)
        {
            IList<UserSubscription> userSubscriptions = new List<UserSubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                var userSubscriptionRecords = new List<TTD_UserSubscriptionsRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (employeeId == null || employeeId == string.Empty)
                    {
                        userSubscriptionRecords = nISEntitiesDataContext.TTD_UserSubscriptionsRecord.Where(item => item.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        userSubscriptionRecords = nISEntitiesDataContext.TTD_UserSubscriptionsRecord.Where(item => item.UserName.Contains(employeeId) && item.TenantCode == tenantCode).ToList();
                    }

                    if (userSubscriptionRecords != null && userSubscriptionRecords.Count > 0)
                    {
                        userSubscriptionRecords.ForEach(item =>
                        {
                            userSubscriptions.Add(new UserSubscription()
                            {
                                Identifier = item.Id,
                                UserName = item.UserName,
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
        /// <param name="VendorName">The vendor name value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of vendor subscriptions
        /// </returns>
        public IList<VendorSubscription> Get_TTD_VendorSubscriptions(string VendorName, string tenantCode)
        {
            IList<VendorSubscription> vendorSubscriptions = new List<VendorSubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                var vendorSubscriptionRecords = new List<TTD_VendorSubscriptionRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (VendorName == null || VendorName == string.Empty)
                    {
                        vendorSubscriptionRecords = nISEntitiesDataContext.TTD_VendorSubscriptionRecord.Where(item => item.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        vendorSubscriptionRecords = nISEntitiesDataContext.TTD_VendorSubscriptionRecord.Where(item => item.VenderName.Contains(VendorName) && item.TenantCode == tenantCode).ToList();
                    }

                    if (vendorSubscriptionRecords != null && vendorSubscriptionRecords.Count > 0)
                    {
                        vendorSubscriptionRecords.ForEach(item =>
                        {
                            vendorSubscriptions.Add(new VendorSubscription()
                            {
                                Identifier = item.Id,
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
        /// <param name="month">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of data usages
        /// </returns>
        public IList<DataUsage> Get_TTD_DataUsages(string month, string tenantCode)
        {
            IList<DataUsage> dataUsages = new List<DataUsage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                var dataUsageRecords = new List<TTD_DataUsageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (month == null || month == string.Empty)
                    {
                        dataUsageRecords = nISEntitiesDataContext.TTD_DataUsageRecord.Where(item => item.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        dataUsageRecords = nISEntitiesDataContext.TTD_DataUsageRecord.Where(item => item.Month.Contains(month) && item.TenantCode == tenantCode).ToList();
                    }

                    if (dataUsageRecords != null && dataUsageRecords.Count > 0)
                    {
                        dataUsageRecords.ForEach(item =>
                        {
                            dataUsages.Add(new DataUsage()
                            {
                                Identifier = item.Id,
                                Month = item.Month,
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
        /// <param name="month">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of meeting usages
        /// </returns>
        public IList<MeetingUsage> Get_TTD_MeetingUsages(string month, string tenantCode)
        {
            IList<MeetingUsage> meetingUsages = new List<MeetingUsage>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                var meetingUsageRecords = new List<TTD_MeetingUsageRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (month == null || month == string.Empty)
                    {
                        meetingUsageRecords = nISEntitiesDataContext.TTD_MeetingUsageRecord.Where(item => item.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        meetingUsageRecords = nISEntitiesDataContext.TTD_MeetingUsageRecord.Where(item => item.Month.Contains(month) && item.TenantCode == tenantCode).ToList();
                    }

                    if (meetingUsageRecords != null && meetingUsageRecords.Count > 0)
                    {
                        meetingUsageRecords.ForEach(item =>
                        {
                            meetingUsages.Add(new MeetingUsage()
                            {
                                Identifier = item.Id,
                                Month = item.Month,
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
        /// <param name="subscription">The month value</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of emails by subscription
        /// </returns>
        public IList<EmailsBySubscription> Get_TTD_EmailsBySubscription(string subscription, string tenantCode)
        {
            IList<EmailsBySubscription> emailsBySubscriptions = new List<EmailsBySubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                var emailsBySubscriptionRecords = new List<TTD_EmailsBySubscriptionRecord>();
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (subscription == null || subscription == string.Empty)
                    {
                        emailsBySubscriptionRecords = nISEntitiesDataContext.TTD_EmailsBySubscriptionRecord.Where(item => item.TenantCode == tenantCode).ToList();
                    }
                    else
                    {
                        emailsBySubscriptionRecords = nISEntitiesDataContext.TTD_EmailsBySubscriptionRecord.Where(item => item.Subscription.Contains(subscription) && item.TenantCode == tenantCode).ToList();
                    }

                    if (emailsBySubscriptionRecords != null && emailsBySubscriptionRecords.Count > 0)
                    {
                        emailsBySubscriptionRecords.ForEach(item =>
                        {
                            emailsBySubscriptions.Add(new EmailsBySubscription()
                            {
                                Identifier = item.Id,
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Role search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(SubscriptionMasterSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidLong(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.VendorName))
                {
                    queryString.Append(string.Format("VendorName.Equals(\"{0}\") and ", searchParameter.VendorName));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(searchParameter.VendorName))
                {
                    queryString.Append(string.Format("VendorName.Contains(\"{0}\") and ", searchParameter.VendorName));
                }
            }
            if (validationEngine.IsValidText(searchParameter.Subscription))
            {
                queryString.Append(string.Format("Subscription.Contains(\"{0}\") and ", searchParameter.Subscription));
            }
            if (validationEngine.IsValidText(searchParameter.EmployeeId))
            {
                queryString.Append(string.Format("EmployeeID.Contains(\"{0}\") and ", searchParameter.EmployeeId));
            }
            if (validationEngine.IsValidText(searchParameter.EmployeeName))
            {
                queryString.Append(string.Format("EmployeeName.Contains(\"{0}\") and ", searchParameter.EmployeeName));
            }
            if (validationEngine.IsValidText(searchParameter.EmailId))
            {
                queryString.Append(string.Format("Email.Contains(\"{0}\") and ", searchParameter.EmailId));
            }
            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
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

        #endregion


    }
}
