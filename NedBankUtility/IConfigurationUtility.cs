
namespace NedbankUtility
{
    #region References

    using System.Collections.Generic;
    using Websym.Core.ConfigurationManager;
    using Websym.Core.EntityManager;
    using Websym.Core.TenantManager;

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

        /// <summary>
        /// THis method will call get method of entity manager.
        /// </summary>
        /// <param name="entitySearchParameter">The entity search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Return list of roleprivileges if exist other wise return null
        /// </returns>
        IList<Entity> GetRolePrivileges(EntitySearchParameter entitySearchParameter, string tenantCode);

        /// <summary>
        /// Return list of roleprivileges if exist other wise return null
        /// </summary>
        /// <param name="tenantSearchParameter"></param>
        /// <returns></returns>
        IList<Tenant> GetTenant(TenantSearchParameter tenantSearchParameter);


        /// <summary>
        /// Return list of roleprivileges if exist other wise return null
        /// </summary>
        /// <param name="tenantSearchParameter"></param>
        /// <returns></returns>
        bool AddTenant(IList<Tenant> tenants);

        /// <summary>
        /// Return list of roleprivileges if exist other wise return null
        /// </summary>
        /// <param name="tenantSearchParameter"></param>
        /// <returns></returns>
        bool UpdateTenant(IList<Tenant> tenants);

        /// <summary>
        /// Return list of roleprivileges if exist other wise return null
        /// </summary>
        /// <param name="tenantSearchParameter"></param>
        /// <returns></returns>
        bool DeleteTenant(IList<Tenant> tenants);


        /// <summary>
        /// Return list of roleprivileges if exist other wise return null
        /// </summary>
        /// <param name="tenantSearchParameter"></param>
        /// <returns></returns>
        bool ActivateTenant(IList<Tenant> tenants);


        /// <summary>
        /// Return list of roleprivileges if exist other wise return null
        /// </summary>
        /// <param name="tenantSearchParameter"></param>
        /// <returns></returns>
        bool DeactivateTenant(IList<Tenant> tenants);
    }
}
