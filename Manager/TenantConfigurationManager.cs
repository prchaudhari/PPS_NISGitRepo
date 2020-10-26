// <copyright file="TenantConfigurationManager.cs" company="Websym Solutions Pvt. Ltd.">
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
    public class TenantConfigurationManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        ITenantConfigurationRepository tenantConfigurationRepository = null;


        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for asset library manager, which initialise
        /// asset library repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public TenantConfigurationManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.tenantConfigurationRepository = this.unityContainer.Resolve<ITenantConfigurationRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Tenant Configuration Functions

        /// <summary>
        /// This method is used to get tenant configuration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantConfiguration> GetTenantConfigurations(string tenantCode)
        {
            try
            {
                return this.tenantConfigurationRepository.GetTenantConfigurations(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// This method will call add save tenant configuration method of repository.
        /// </summary>
        /// <param name="setting">tenant configuration are to be add.</param>
        /// <param name="tenantCode">Tenant code of asset library.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool Save(TenantConfiguration setting, string tenantCode)
        {
            try
            {
                this.IsValidTenantConfiguration(setting, tenantCode);
                return tenantConfigurationRepository.Save(setting, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #endregion

        #region Private Methods


        #region Tenant Configuration


        /// <summary>
        /// This method is responsible for validate asset s.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        private void IsValidTenantConfiguration(TenantConfiguration item, string tenantCode)
        {
            try
            {

                InvalidAssetException invalidAssetException = new InvalidAssetException(tenantCode);

                try
                {
                    item.IsValid();
                }
                catch (Exception ex)
                {
                    invalidAssetException.Data.Add(item.Identifier, ex.Data);
                }


                if (invalidAssetException.Data.Count > 0)
                {
                    throw invalidAssetException;
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
