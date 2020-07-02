// <copyright file="SQLAssetSettingRepository.cs" company="Websym Solutions Pvt Ltd">
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
    public class SQLAssetSettingRepository : IAssetSettingRepository
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
        public SQLAssetSettingRepository(IUnityContainer unityContainer)
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
        /// This is responsible for get asset library
        /// </summary>
        /// <param name="assetSettingSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<AssetSetting> GetAssetSettings(string tenantCode)
        {
            IList<AssetSetting> assetSettings = new List<AssetSetting>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<AssetSettingRecord> assetSettingRecords = new List<AssetSettingRecord>();
                    assetSettingRecords = nISEntitiesDataContext.AssetSettingRecords.Where(item => item.TenantCode == tenantCode).ToList();
                    assetSettings = assetSettingRecords.Select(item => new AssetSetting
                    {
                        Identifier = item.Id,
                        ImageFileExtension = item.ImageFileExtension,
                        ImageHeight=item.ImageHeight,
                        ImageWidth=item.ImageWidth,
                        ImageSize=item.ImageSize,
                        VideoFileExtension=item.VideoFileExtension,
                        VideoSize=item.VideoSize

                    }).ToList();
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return assetSettings;
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
