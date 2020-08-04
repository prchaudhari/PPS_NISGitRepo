// <copyright file="TenantConfigurationController.cs" company="Websym Solutions Pvt Ltd">
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
    [RoutePrefix("TenantConfiguration")]
    public class TenantConfigurationController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The asset library manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        #endregion

        #region Constructor

        public TenantConfigurationController(IUnityContainer unityContainer)
        {
            this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);
        }

        #endregion

        #region Public Methods

        #region Tenant Configuration

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="tenantConfigurationSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public IList<TenantConfiguration> List()
        {
            IList<TenantConfiguration> tenantConfigurations = new List<TenantConfiguration>();
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                tenantConfigurations = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return tenantConfigurations;
        }

        /// <summary>
        /// This method helps to get asset libraries list based on the search parameters.
        /// </summary>
        /// <param name="tenantConfigurationSearchParameter"></param>
        /// <returns>List of asset libraries</returns>
        [HttpPost]
        public bool Save(IList<TenantConfiguration> setting)
        {
            bool result;  
            try
            {

                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.tenantConfigurationManager.Save(setting[0],tenantCode);
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