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
        public IList<AnalyticsData> GetAnalyticsData(string tenantCode)
        {
            IList<AnalyticsData> AnalyticsDatas = new List<AnalyticsData>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<AnalyticsDataRecord> AnalyticsDataRecords = new List<AnalyticsDataRecord>();
                    AnalyticsDataRecords = nISEntitiesDataContext.AnalyticsDataRecords.Where(item => item.TenantCode == tenantCode).ToList();
                    AnalyticsDatas = AnalyticsDataRecords.Select(item => new AnalyticsData
                    {
                        Identifier = item.Id,
                        StatementId = item.StatementId,
                        CustomerId = item.CustomerId,
                        AccountId = item.AccountId,
                        PageWidgetId = item.PageWidgetId == null ? 0 : (long)item.PageWidgetId,
                        PageId = item.PageId == null ? 0 : (long)item.PageId,
                        WidgetId = item.WidgetId == null ? 0 : (long)item.WidgetId,
                        EventDate = item.EventDate,
                        EventType = item.EventType,
                    }).ToList();
                }
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
        #endregion

    }
}
