// <copyright file="ITenantConfigurationRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This interface represents reference to access accet library repository.
    /// </summary>
    public interface ITenantConfigurationRepository
    {
        #region  Tenant Configuration

        /// <summary>
        /// This method is used to get tenant configuaration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        IList<TenantConfiguration> GetTenantConfigurations(string tenantCode);

        /// <summary>
        /// This method is used to get tenant configuaration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        IList<TenantSubscription> GetTenantSubscriptions(string tenantCode);

        /// <summary>
        /// This method is used to get tenant configuaration
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        TenantSubscription GetTenantSubscription(string tenantCode);

        /// <summary>
        /// This method is used to save tenant configuaration
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool Save(TenantConfiguration setting, string tenantCode);

        /// <summary>
        /// This method is used to save tenant subscription
        /// </summary>
        /// <param name="tenantSubscriptions"></param>
        /// <param name="tenantcode"></param>
        /// <returns></returns>
        bool AddTenantSubscriptions(IList<TenantSubscription> tenantSubscriptions, string tenantcode);


        #endregion

    }
}

