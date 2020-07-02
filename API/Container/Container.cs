// <copyright file = "Container.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//----------------------------------------------------------------------- 

namespace nIS
{
    #region References

    using Unity;

    #endregion

    /// <summary>
    /// Represents class for Dependency Resolver.
    /// </summary>
    public class Container
    {
        /// <summary>
        ///  This method use to create unity container
        /// </summary>
        /// <returns>Returns unity container object</returns>
        public static IUnityContainer GetUnityContainer()
        {
            IUnityContainer unityContainer = new UnityContainer();
            unityContainer.RegisterType<Websym.Core.ConfigurationManager.IConfigurationRepository, Websym.Core.ConfigurationManager.SQLTableConfigurationRepository>();
            unityContainer.RegisterType<Websym.Core.TenantManager.ITenantRepository, Websym.Core.TenantManager.SQLAzureTenantRepository>();
            unityContainer.RegisterType<Websym.Core.EntityManager.IEntityRepository, Websym.Core.EntityManager.SQLAzureEntityRepository>();
            unityContainer.RegisterType<Websym.Core.EventManager.IEventRepository, Websym.Core.EventManager.SQLAzureEventRepository>();

          
            unityContainer.RegisterType<IUtility, Utility>();
            unityContainer.RegisterType<IValidationEngine, ValidationEngine>();
            unityContainer.RegisterType<ICryptoManager, CryptoManager>();
            unityContainer.RegisterType<IUserRepository, SQLUserRepository>();
            unityContainer.RegisterType<IRoleRepository, SQLRoleRepository>();
            unityContainer.RegisterType<IAssetLibraryRepository, SQLAssetLibraryRepository>();
            unityContainer.RegisterType<IPageRepository, SQLPageRepository>();
            unityContainer.RegisterType<IWidgetRepository, SQLWidgetRepository>();
            unityContainer.RegisterType<IAssetSettingRepository, SQLAssetSettingRepository>();


            return unityContainer;
        }
    }
}