// <copyright file="AssetSettingManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion


    /// <summary>
    /// This class implements manager layer of asset library manager.
    /// </summary>
    public class AssetSettingManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IAssetSettingRepository assetSettingRepository = null;


        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for asset library manager, which initialise
        /// asset library repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public AssetSettingManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.assetSettingRepository = this.unityContainer.Resolve<IAssetSettingRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Asset Libraries Functions

        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="assetSettingSearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<AssetSetting> GetAssetSettings(string tenantCode)
        {
            IList<AssetSetting> assetLibraries = new List<AssetSetting>();
            try
            {

                assetLibraries = this.assetSettingRepository.GetAssetSettings(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return assetLibraries;
        }

        #endregion

  
        #endregion

        #region Private Methods

        

       

        #endregion
    }
}
