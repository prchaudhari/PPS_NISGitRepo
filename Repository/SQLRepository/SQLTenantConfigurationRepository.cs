// <copyright file="SQLTenantConfigurationRepository.cs" company="Websym Solutions Pvt Ltd">
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
    using System.Security.Claims;


    #endregion

    /// <summary>
    /// This class represents repository layer of tenant configuration for crud operation.
    /// </summary>
    public class SQLTenantConfigurationRepository : ITenantConfigurationRepository
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
        public SQLTenantConfigurationRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Functions

        #region Tenant Configuration function

        /// <summary>
        /// This is responsible for get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantConfiguration> GetTenantConfigurations(string tenantCode)
        {
            IList<TenantConfiguration> tenantConfigurations = new List<TenantConfiguration>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<TenantConfigurationRecord> tenantConfigurationRecord = new List<TenantConfigurationRecord>();
                    tenantConfigurationRecord = nISEntitiesDataContext.TenantConfigurationRecords.Where(item => item.TenantCode == tenantCode).ToList();
                    tenantConfigurations = tenantConfigurationRecord.Select(item => new TenantConfiguration
                    {
                        Identifier = item.Id,
                        Description = item.Description,
                        InputDataSourcePath = item.InputDataSourcePath,
                        OutputHTMLPath = item.OutputHTMLPath,
                        OutputPDFPath = item.OutputPDFPath,
                        Name = item.Name,
                        AssetPath = item.AssetPath,
                        ArchivalPath = item.ArchivalPath,
                        ArchivalPeriod = item.ArchivalPeriod != null ? (int)item.ArchivalPeriod : 0,
                        ///ArchivalPeriodUnit = (ArchivalPeriod)(Enum.Parse(typeof(ArchivalPeriod), item.ArchivalPeriodUnit)),
                        DateFormat = item.DateFormat,
                        ApplicationTheme = item.ApplicationTheme,
                        WidgetThemeSetting = item.WidgetThemeSetting,
                        BaseUrlForTransactionData = item.BaseUrlForTransactionData,
                        TenantCode = item.TenantCode
                    }).ToList();

                    if (tenantConfigurations?.Count > 0)
                    {
                        //To check any asset record is present or all asset records deleted, if count grater than 0 then asset path should be non-editable otherwise editable
                        var assetRecords = nISEntitiesDataContext.AssetRecords.Where(item => item.IsDeleted == false)?.ToList();
                        if (assetRecords != null && assetRecords.Count > 0)
                        {
                            tenantConfigurations[0].IsAssetPathEditable = false;
                        }
                        else
                        {
                            tenantConfigurations[0].IsAssetPathEditable = true;
                        }

                        //To check any one schedule record run status is completed, if yes then output html path should be non-editable otherwise editable
                        var scheduleLogDetialsRecords = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.TenantCode == tenantCode && item.Status == ScheduleLogStatus.Completed.ToString())?.ToList();
                        if (scheduleLogDetialsRecords != null && scheduleLogDetialsRecords.Count > 0)
                        {
                            tenantConfigurations[0].IsOutputHTMLPathEditable = false;
                        }
                        else
                        {
                            tenantConfigurations[0].IsOutputHTMLPathEditable = true;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return tenantConfigurations;
        }

        /// <summary>
        /// This is responsible for get tenant configuration
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool Save(TenantConfiguration setting, string tenantCode)
        {
            TenantConfigurationRecord record = new TenantConfigurationRecord();
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    record = nISEntitiesDataContext.TenantConfigurationRecords.Where(item => item.Id == setting.Identifier && item.TenantCode == tenantCode).FirstOrDefault();
                    if (record == null)
                    {
                        record = new TenantConfigurationRecord();
                    }
                    record.Description = setting.Description ?? string.Empty;
                    record.InputDataSourcePath = setting.InputDataSourcePath ?? string.Empty;
                    record.OutputHTMLPath = setting.OutputHTMLPath ?? string.Empty;
                    record.OutputPDFPath = setting.OutputPDFPath ?? string.Empty;
                    record.Name = setting.Name;
                    record.AssetPath = setting.AssetPath ?? string.Empty;
                    record.ArchivalPath = setting.ArchivalPath ?? string.Empty;
                    record.ArchivalPeriod = setting.ArchivalPeriod;
                    //record.ArchivalPeriodUnit = setting.ArchivalPeriodUnit.ToString();
                    record.DateFormat = setting.DateFormat;
                    record.ApplicationTheme = setting.ApplicationTheme ?? string.Empty;
                    record.WidgetThemeSetting = setting.WidgetThemeSetting ?? string.Empty;
                    record.BaseUrlForTransactionData = setting.BaseUrlForTransactionData != null && setting.BaseUrlForTransactionData != string.Empty ? setting.BaseUrlForTransactionData : "";
                    if (record.TenantCode == null || record.TenantCode == string.Empty)
                    {
                        record.TenantCode = tenantCode;
                        nISEntitiesDataContext.TenantConfigurationRecords.Add(record);
                    }
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

        /// <summary>
        /// This is responsible for get tenant configuration
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddTenantSubscriptions(IList<TenantSubscription> tenantSubscriptions, string tenantcode)
        {
            TenantConfigurationRecord record = new TenantConfigurationRecord();
            bool result = false;
            try
            {
                int userId;
                var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
                int.TryParse(claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase)).Value, out userId);

                this.SetAndValidateConnectionString(tenantcode);
                IList<TenantSubscriptionRecord> tenantSubscriptionRecords = new List<TenantSubscriptionRecord>();
                tenantSubscriptions.ToList().ForEach(item =>
                {
                    tenantSubscriptionRecords.Add(new TenantSubscriptionRecord()
                    {
                        TenantCode = item.TenantCode,
                        SubscriptionEndDate = item.SubscriptionEndDate,
                        SubscriptionStartDate = DateTime.UtcNow,
                        LastModifiedBy = userId,
                        LastModifiedOn = DateTime.UtcNow,
                        SubscriptionKey = item.SubscriptionKey
                    });
                });
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.TenantSubscriptionRecords.AddRange(tenantSubscriptionRecords);
                    nISEntitiesDataContext.SaveChanges();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// This is responsible for get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantSubscription> GetTenantSubscriptions(string tenantCode)
        {
            IList<TenantSubscription> tenantConfigurations = new List<TenantSubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<View_TenantSubscriptionRecord> tenantConfigurationRecord = new List<View_TenantSubscriptionRecord>();
                    var data = nISEntitiesDataContext.View_TenantSubscriptionRecord.GroupBy(item => item.TenantCode);
                    var tenantData = data.Select(item => new
                    {
                        TenantCode = item.Key,
                        SubscriptionEndDate = item.OrderByDescending(i => i.LastModifiedOn).ToList().FirstOrDefault().SubscriptionEndDate,
                        LastModifiedBy = item.OrderByDescending(i => i.LastModifiedOn).ToList().FirstOrDefault().LastModifiedBy,
                        LastModifiedOn = item.OrderByDescending(i => i.LastModifiedOn).ToList().FirstOrDefault().LastModifiedOn,
                        SubscriptionStartDate = item.OrderByDescending(i => i.LastModifiedOn).ToList().FirstOrDefault().SubscriptionStartDate,
                        SubscriptionKey = item.OrderByDescending(i => i.LastModifiedOn).ToList().FirstOrDefault().SubscriptionKey,
                        LastModifiedName = item.OrderByDescending(i => i.LastModifiedOn).ToList().FirstOrDefault().LastModifiedName,

                        list = item.ToList()
                    });
                    foreach (var item in tenantData)
                    {
                        var tenantSubscription = item.list.FirstOrDefault();
                        TenantSubscription subscription = new TenantSubscription();
                        subscription.SubscriptionEndDate = DateTime.SpecifyKind(item.SubscriptionEndDate, DateTimeKind.Utc);
                        subscription.SubscriptionStartDate = DateTime.SpecifyKind(item.SubscriptionStartDate, DateTimeKind.Utc);
                        subscription.SubscriptionKey = item.SubscriptionKey;
                        subscription.LastModifiedBy = item.LastModifiedBy;
                        subscription.LastModifiedOn = item.LastModifiedOn;
                        subscription.LastModifiedName = item.LastModifiedName;
                        subscription.TenantCode = item.TenantCode;
                        tenantConfigurations.Add(subscription);
                    }
                    //tenantConfigurations = tenantData.Select(item => new TenantSubscription
                    //{
                    //    SubscriptionEndDate = item.SubscriptionEndDate,
                    //    SubscriptionStartDate = item.SubscriptionStartDate,
                    //    SubscriptionKey = item.SubscriptionKey,
                    //    LastModifiedBy=item.LastModifiedBy,
                    //    LastModifiedOn=item.LastModifiedOn,
                    //    TenantCode = item.TenantCode
                    //}).ToList();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return tenantConfigurations;
        }

        /// <summary>
        /// This is responsible for get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public TenantSubscription GetTenantSubscription(string tenantCode)
        {
            IList<TenantSubscription> tenantConfigurations = new List<TenantSubscription>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<View_TenantSubscriptionRecord> tenantConfigurationRecord = new List<View_TenantSubscriptionRecord>();
                    tenantConfigurationRecord = nISEntitiesDataContext.View_TenantSubscriptionRecord.Where(item => item.TenantCode.ToString() == tenantCode).OrderByDescending(item => item.LastModifiedOn).ToList();
                    tenantConfigurations = tenantConfigurationRecord.Select(item => new TenantSubscription
                    {
                        //Identifier = item.Id,
                        SubscriptionEndDate = item.SubscriptionEndDate,
                        SubscriptionStartDate = item.SubscriptionStartDate,
                        LastModifiedBy = item.LastModifiedBy,
                        LastModifiedOn = item.LastModifiedOn,
                        SubscriptionKey = item.SubscriptionKey,
                        TenantCode = item.TenantCode
                    }).ToList();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return tenantConfigurations.FirstOrDefault();
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
