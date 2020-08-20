// <copyright file="AnalyticsDataController.cs" company="Websym Solutions Pvt Ltd">
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
    [RoutePrefix("AnalyticsData")]
    public class AnalyticsDataController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The asset library manager object.
        /// </summary>
        private AnalyticsDataManager AnalyticsDataManager = null;

        #endregion

        #region Constructor

        public AnalyticsDataController(IUnityContainer unityContainer)
        {
            this.AnalyticsDataManager = new AnalyticsDataManager(unityContainer);
        }

        #endregion

        #region Public Methods

        #region Analytics Data

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public IList<AnalyticsData> List()
        {
            IList<AnalyticsData> assetlibraries = new List<AnalyticsData>();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                assetlibraries = this.AnalyticsDataManager.GetAnalyticsDatas(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return assetlibraries;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="AnalyticsDataSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        [AllowAnonymous]
        public bool Save(IList<AnalyticsData> setting)
        {
            bool result;  
            try
            {

               // string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.AnalyticsDataManager.Save(setting,ModelConstant.DEFAULT_TENANT_CODE);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion


        #endregion

    }
}