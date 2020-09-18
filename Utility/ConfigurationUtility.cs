

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Net.Mail;
    using System.Net.Http;

    using System.Configuration;
    using System.Reflection;
    using System.Net.Http.Headers;
    using Websym.Core.ConfigurationManager;
    using Unity;
    using Websym.Core.EventManager;
    using Websym.Core.EntityManager;
    using Websym.Core.TenantManager;
    #endregion

    public class ConfigurationUtility : IConfigurationUtility
    {
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        #region Constructore
        public ConfigurationUtility(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }
        #endregion

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
        public IList<Websym.Core.ConfigurationManager.ConfigurationSection> GetConfigurationValues(ConfigurationSearchParameter configurationSearchParameter, string configurationBaseURLKey, string tenantKey, string tenantCode)
        {
            IList<Websym.Core.ConfigurationManager.ConfigurationSection> configurationSectionList = null;
            try
            {
                #region DLL related changes

                //container = unityresolver.GetDependency();
                Websym.Core.ConfigurationManager.ConfigurationManager configmanager = null;
                configmanager = new Websym.Core.ConfigurationManager.ConfigurationManager(this.unityContainer);
                configurationSectionList = configmanager.GetConfigurations(configurationSearchParameter, tenantCode.ToString())?.ToList();

                #endregion

                return configurationSectionList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

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
        public string GetConnectionString(string section, string configurationKey, string configurationBaseURLKey, string tenantKey, string tenantCode)
        {
            string sqlConnectionString = string.Empty;
            try
            {
                //sqlConnectionString = "metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";
                //return sqlConnectionString;
                //return System.Configuration.ConfigurationManager.ConnectionStrings["FMSEntitiesDataContext"].ConnectionString;
                ConfigurationSearchParameter configurationSearchParameter = new ConfigurationSearchParameter();
                configurationSearchParameter.SectionName = section;
                configurationSearchParameter.ConfigurationKey = configurationKey;

                IList<Websym.Core.ConfigurationManager.ConfigurationSection> configurationSectionList = this.GetConfigurationValues(configurationSearchParameter, configurationBaseURLKey, tenantKey, tenantCode);
                if (configurationSectionList != null && configurationSectionList.Count > 0)
                {
                    if (configurationSectionList[0].ConfigurationItems != null && configurationSectionList[0].ConfigurationItems.Count > 0)
                    {
                        sqlConnectionString = configurationSectionList[0].ConfigurationItems[0].Value;
                    }
                }

                sqlConnectionString = sqlConnectionString.EndsWith(";") ? sqlConnectionString : sqlConnectionString + ";";
                // sqlConnectionString = "metadata=res://*/NISDataContext.csdl|res://*/NISDataContext.ssdl|res://*/NISDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;User ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";
                sqlConnectionString = @"metadata=res://*/NISDataContext.csdl|res://*/NISDataContext.ssdl|res://*/NISDataContext.msl;provider=System.Data.SqlClient;provider connection string=';" + sqlConnectionString + "multipleactiveresultsets=True;application name=EntityFramework';";



            }
            catch (Exception exception)
            {
                throw exception;
            }

            return sqlConnectionString;
        }

        /// <summary>
        /// THis method will call get method of entity manager.
        /// </summary>
        /// <param name="entitySearchParameter">The entity search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Return list of roleprivileges if exist other wise return null
        /// </returns>
        public IList<Entity> GetRolePrivileges(EntitySearchParameter entitySearchParameter, string tenantCode)
        {
            IList<Entity> entities = new List<Entity>();
            try
            {

                EntityManager manager = new EntityManager(this.unityContainer);

                entities = manager.GetEntities(entitySearchParameter, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entities;
        }

        /// <summary>
        /// THis method will call get method of entity manager.
        /// </summary>
        /// <param name="entitySearchParameter">The entity search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Return list of roleprivileges if exist other wise return null
        /// </returns>
        public IList<Tenant> GetTenant(TenantSearchParameter tenantSearchParameter)
        {
            IList<Tenant> tenants = new List<Tenant>();
            try
            {

                TenantManager manager = new TenantManager(this.unityContainer);

                tenants = manager.GetTenants(tenantSearchParameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tenants;
        }

        public bool ActivateTenant(IList<Tenant> tenants)
        {
            bool result = false;
            try
            {

                TenantManager manager = new TenantManager(this.unityContainer);

                result = manager.UpdateTenantStatus(tenants, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool AddTenant(IList<Tenant> tenants)
        {
            bool result = false;
            try
            {

                TenantManager manager = new TenantManager(this.unityContainer);

                result = manager.AddTenants(tenants, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool DeactivateTenant(IList<Tenant> tenants)
        {
            bool result = false;
            try
            {

                TenantManager manager = new TenantManager(this.unityContainer);

                result = manager.UpdateTenantStatus(tenants, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool DeleteTenant(IList<Tenant> tenants)
        {
            bool result = false;
            try
            {

                TenantManager manager = new TenantManager(this.unityContainer);

                result = manager.DeleteTenants(tenants);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public bool UpdateTenant(IList<Tenant> tenants)
        {
            bool result = false;
            try
            {

                TenantManager manager = new TenantManager(this.unityContainer);

                result = manager.UpdateTenants(tenants);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
