// <copyright file = "IoCContainer.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//----------------------------------------------------------------------- 

namespace FinancialTenantService
{
    #region References

    using System.Web.Http.Dependencies;
    using Unity;

    #endregion

    /// <summary>
    /// Represents class for inversion of control Container.
    /// </summary>
    public class IoCContainer : UnityResolverService, IDependencyResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoCContainer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public IoCContainer(IUnityContainer container)
            : base(container) { }

        /// <summary>
        /// Starts a resolution scope.
        /// </summary>
        /// <returns>
        /// The dependency scope.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolverService(child);
        }
    }
}