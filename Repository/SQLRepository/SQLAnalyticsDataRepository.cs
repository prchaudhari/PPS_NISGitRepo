// <copyright file="SQLAnalyticsDataRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region Referemces 

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Net.NetworkInformation;
    using System.Text;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents repository layer of accet library for crud operation.
    /// </summary>
    public class SQLAnalyticsDataRepository : IAnalyticsDataRepository
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
        /// The utility object
        /// </summary>
        private IUtility utility = null;

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

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        public SQLAnalyticsDataRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Functions

        #region Analytics Data function
        /// <summary>
        /// This is responsible for get asset library
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<AnalyticsData> GetAnalyticsData(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            IList<AnalyticsData> AnalyticsDatas = new List<AnalyticsData>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                IList<AnalyticsDataRecord> AnalyticsDataRecords = new List<AnalyticsDataRecord>();
                IList<PageRecord> pageRecords = new List<PageRecord>();
                IList<CustomerMasterRecord> customerMasterRecords = new List<CustomerMasterRecord>();
                IList<WidgetRecord> widgetRecords = new List<WidgetRecord>();

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string whereClause = this.WhereClauseGenerator(searchParameter, tenantCode);

                    if (string.IsNullOrEmpty(whereClause))
                    {
                        if (searchParameter.PagingParameter.PageIndex > 0 && searchParameter.PagingParameter.PageSize > 0)
                        {
                            AnalyticsDataRecords = nISEntitiesDataContext.AnalyticsDataRecords
                            .OrderBy(searchParameter.SortParameter.SortColumn + " " + searchParameter.SortParameter.SortOrder.ToString())
                            .Skip((searchParameter.PagingParameter.PageIndex - 1) * searchParameter.PagingParameter.PageSize)
                            .Take(searchParameter.PagingParameter.PageSize)
                            .ToList();
                        }
                        else
                        {
                            AnalyticsDataRecords = nISEntitiesDataContext.AnalyticsDataRecords
                            .OrderBy(searchParameter.SortParameter.SortColumn + " " + searchParameter.SortParameter.SortOrder.ToString().ToLower())
                            .ToList();
                        }
                    }
                    else
                    {
                        if (searchParameter.PagingParameter.PageIndex > 0 && searchParameter.PagingParameter.PageSize > 0)
                        {
                            AnalyticsDataRecords = nISEntitiesDataContext.AnalyticsDataRecords
                            .OrderBy(searchParameter.SortParameter.SortColumn + " " + searchParameter.SortParameter.SortOrder.ToString())
                            .Where(whereClause)
                            .Skip((searchParameter.PagingParameter.PageIndex - 1) * searchParameter.PagingParameter.PageSize)
                            .Take(searchParameter.PagingParameter.PageSize)
                            .ToList();
                        }
                        else
                        {
                            AnalyticsDataRecords = nISEntitiesDataContext.AnalyticsDataRecords
                            .Where(whereClause)
                            .OrderBy(searchParameter.SortParameter.SortColumn + " " + searchParameter.SortParameter.SortOrder.ToString().ToLower())
                            .ToList();
                        }
                    }

                    if (AnalyticsDataRecords?.Count() > 0)
                    {
                        StringBuilder pageIds = new StringBuilder();
                        pageIds.Append("(" + string.Join(" or ", AnalyticsDataRecords.Where(item => item.PageId != null && item.PageId > 0)
                            .Select(item => string.Format("Id.Equals({0})", item.PageId))) + ")");
                        pageRecords = nISEntitiesDataContext.PageRecords.Where(pageIds.ToString()).ToList();

                        StringBuilder customerIds = new StringBuilder();
                        customerIds.Append("(" + string.Join(" or ", AnalyticsDataRecords.Where(item => item.CustomerId > 0)
                            .Select(item => string.Format("Id.Equals({0})", item.CustomerId))) + ")");
                        customerMasterRecords = nISEntitiesDataContext.CustomerMasterRecords.Where(customerIds.ToString()).ToList();

                        StringBuilder widgetIds = new StringBuilder();
                        widgetIds.Append("(" + string.Join(" or ", AnalyticsDataRecords.Where(item => item.WidgetId != null && item.WidgetId > 0)
                            .Select(item => string.Format("Id.Equals({0})", item.WidgetId))) + ")");
                        widgetRecords = nISEntitiesDataContext.WidgetRecords.Where(widgetIds.ToString()).ToList();
                    }

                }
                AnalyticsDataRecords?.ToList().ForEach(item =>
                {
                    AnalyticsData data = new AnalyticsData();
                    data.Identifier = item.Id;
                    data.StatementId = item.StatementId;
                    data.CustomerId = item.CustomerId;
                    data.AccountId = item.AccountId;
                    data.PageWidgetId = item.PageWidgetId == null ? 0 : (long)item.PageWidgetId;
                    data.PageId = item.PageId == null ? 0 : (long)item.PageId;
                    data.WidgetId = item.WidgetId == null ? 0 : (long)item.WidgetId;
                    data.EventDate = DateTime.SpecifyKind((DateTime)item.EventDate, DateTimeKind.Utc);
                    data.EventType = item.EventType;
                    if (customerMasterRecords.Any(i => i.Id == data.CustomerId))
                    {
                        var customer = customerMasterRecords.Where(i => i.Id == data.CustomerId)?.FirstOrDefault();
                        data.CustomerName = customer.FirstName + " " + customer.FirstName;
                    }
                    if (data.PageId > 0 && pageRecords.Any(i => i.Id == data.PageId))
                    {
                        var customer = pageRecords.Where(i => i.Id == data.PageId)?.FirstOrDefault();
                        data.PageName = customer.DisplayName;
                    }
                    else
                    {
                        data.PageName = "";
                    }
                    if (data.WidgetId > 0 && widgetRecords.Any(i => i.Id == data.WidgetId))
                    {
                        var customer = widgetRecords.Where(i => i.Id == data.WidgetId)?.FirstOrDefault();
                        data.Widgetname = customer.DisplayName;
                    }
                    else
                    {
                        data.Widgetname = "";
                    }
                    AnalyticsDatas.Add(data);
                });
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return AnalyticsDatas;
        }

        /// <summary>
        /// This is responsible for get asset library
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool Save(IList<AnalyticsData> settings, string tenantCode)
        {
            IList<AnalyticsDataRecord> records = new List<AnalyticsDataRecord>();
            bool result;
            try
            {

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    settings.ToList().ForEach(setting =>
                    {
                        AnalyticsDataRecord record = new AnalyticsDataRecord();

                        record.StatementId = setting.StatementId;
                        record.CustomerId = setting.CustomerId;
                        record.AccountId = setting.AccountId;
                        record.PageWidgetId = setting.PageWidgetId;
                        record.PageId = setting.PageId;
                        record.WidgetId = setting.WidgetId;
                        record.EventDate = setting.EventDate;
                        record.EventType = setting.EventType;
                        record.TenantCode = "";
                        records.Add(record);

                    });

                    nISEntitiesDataContext.AnalyticsDataRecords.AddRange(records);
                    nISEntitiesDataContext.SaveChanges();
                    result = true;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        #endregion


        #endregion

        #region Private Methhod

        #region Get Connection String

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
                this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Role search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(AnalyticsSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            //if (searchParameter.SearchMode == SearchMode.Equals)
            //{
            //    if (validationEngine.IsValidLong(searchParameter.Identifier))
            //    {
            //        queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            //    }
            //    if (validationEngine.IsValidText(searchParameter.Name))
            //    {
            //        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", searchParameter.Name));
            //    }
            //}
            //if (searchParameter.SearchMode == SearchMode.Contains)
            //{
            //    if (validationEngine.IsValidText(searchParameter.Name))
            //    {
            //        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", searchParameter.Name));
            //    }
            //}
            //if (validationEngine.IsValidText(searchParameter.Status))
            //{
            //    queryString.Append(string.Format("Status.Equals(\"{0}\") and ", searchParameter.Status));
            //}


            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && !this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                queryString.Append("EventDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.EndDate) && !this.validationEngine.IsValidDate(searchParameter.StartDate))
            {
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);
                queryString.Append("EventDate <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }

            if (this.validationEngine.IsValidDate(searchParameter.StartDate) && this.validationEngine.IsValidDate(searchParameter.EndDate))
            {
                DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.StartDate), DateTimeKind.Utc);
                DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(searchParameter.EndDate), DateTimeKind.Utc);

                queryString.Append("EventDate >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                               "and EventDate <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
            }


            return queryString.ToString();
        }

        #endregion

    }
}
