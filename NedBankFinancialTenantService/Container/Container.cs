// <copyright file = "Container.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//----------------------------------------------------------------------- 

namespace FinancialTenantService
{
    #region References
    using Microsoft.Practices.Unity;
    using NedbankRepository;
    using NedbankUtility;
    using NedBankValidationEngine;
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
            unityContainer.RegisterType<ICustomerRepository, SQLCustomerRepository>();
            unityContainer.RegisterType<IConfigurationUtility, ConfigurationUtility>();
            unityContainer.RegisterType<INedBankValidationEngine, NedBankValidationEngine>();
            unityContainer.RegisterType<IBranchRepository, SQLBranchRepository>();
            return unityContainer;
        }
    }
}