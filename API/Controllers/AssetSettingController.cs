// <copyright file="AssetSettingController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for asset library
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("AssetSetting")]
    public class AssetSettingController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The asset library manager object.
        /// </summary>
        private AssetSettingManager assetSettingManager = null;

        #endregion

        #region Constructor

        public AssetSettingController(IUnityContainer unityContainer)
        {
            this.assetSettingManager = new AssetSettingManager(unityContainer);
        }

        #endregion

        #region Public Methods

        #region Asset Setting

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="assetSettingSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public IList<AssetSetting> List()
        {
            IList<AssetSetting> assetlibraries = new List<AssetSetting>();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                assetlibraries = this.assetSettingManager.GetAssetSettings(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assetlibraries;
        }

        #endregion


        #endregion

    }
}