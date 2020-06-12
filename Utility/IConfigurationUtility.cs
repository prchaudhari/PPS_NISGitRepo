
namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Mail;
    using Websym.Core.ConfigurationManager;

    #endregion

    public interface IConfigurationUtility
    {
        /// <summary>
        /// This method gets the configuration values from configuration manager component.
        /// </summary>
        /// <param name="configurationSearchParameter">The configuration search parameter.</param>
        /// <param name="configurationBaseURLKey">The configuration base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the list of configuration section
        /// </returns>
        IList<ConfigurationSection> GetConfigurationValues(ConfigurationSearchParameter configurationSearchParameter, string configurationBaseURLKey, string tenantKey, string tenantCode);

        /// <summary>
        /// This method gets the connection string from configuration manager as per the specified key.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="configurationBaseURLKey">The configuration base URL key.</param>
        /// <param name="tenantKey">The tenant key.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns the connection string for the specified configuration key(s).
        /// </returns>
        string GetConnectionString(string section, string configurationKey, string configurationBaseURLKey, string tenantKey, string tenantCode);

        #region other component api

        //IList<Websym.Core.EventManager.Event> AddUserNotificationSubscription(Websym.Core.EventManager.EventSearchParameter eventSearchParameter,Websym.Core.NotificationEngine.SubscrptionDeliveryMode deliveryMode, string userIdentifier, string contactNumber, string emailAddress, string tenantCode);

        //bool SendNotification(Websym.Core.EventManager.EventContext eventContext, Websym.Core.NotificationEngine.SubscrptionDeliveryMode deliveryMode, string tenantCode);


        #endregion
    }
}
