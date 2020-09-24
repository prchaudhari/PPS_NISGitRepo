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

        #region Asset Setting function
        /// <summary>
        /// This is responsible for get tenant configuration
        /// </summary>
        /// <param name="assetSettingSearchParameter"></param>
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
                        ArchivalPeriod = item.ArchivalPeriod!=null?(int)item.ArchivalPeriod:0,
                        ///ArchivalPeriodUnit = (ArchivalPeriod)(Enum.Parse(typeof(ArchivalPeriod), item.ArchivalPeriodUnit)),
                        DateFormat = item.DateFormat

                    }).ToList();
                    if (tenantConfigurations?.Count > 0)
                    {
                        AssetRecord assetRecord = nISEntitiesDataContext.AssetRecords.Where(item => item.IsDeleted == false)?.ToList()?.FirstOrDefault();
                        if (assetRecord != null)
                        {
                            tenantConfigurations[0].IsAssetPathEditable = false;
                        }
                        else
                        {
                            tenantConfigurations[0].IsAssetPathEditable = true;

                        }
                        ScheduleLogDetailRecord scheduleLogDetialsRecord = nISEntitiesDataContext.ScheduleLogDetailRecords.Where(item => item.TenantCode == tenantCode && item.Status == "Completed")?.ToList()?.FirstOrDefault();
                        if (scheduleLogDetialsRecord != null)
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
        /// <param name="assetSettingSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool Save(TenantConfiguration setting, string tenantCode)
        {
            TenantConfigurationRecord record = new TenantConfigurationRecord();
            bool result;
            try
            {

                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    record = nISEntitiesDataContext.TenantConfigurationRecords.Where(item => item.Id == setting.Identifier && item.TenantCode == tenantCode).FirstOrDefault();
                    record.Description = setting.Description;
                    record.InputDataSourcePath = setting.InputDataSourcePath;
                    record.OutputHTMLPath = setting.OutputHTMLPath;
                    record.OutputPDFPath = setting.OutputPDFPath;
                    record.Name = setting.Name;
                    record.AssetPath = setting.AssetPath;
                    record.ArchivalPath = setting.ArchivalPath;
                    record.ArchivalPeriod = (int)setting.ArchivalPeriod;
                    //record.ArchivalPeriodUnit = setting.ArchivalPeriodUnit.ToString();
                    record.DateFormat = setting.DateFormat;
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
