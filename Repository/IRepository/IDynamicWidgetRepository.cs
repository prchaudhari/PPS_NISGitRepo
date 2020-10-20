// <copyright file="IDynamicWidgetRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents interface that defines methods to access dynamicWidget repository.
    /// </summary>
    public interface IDynamicWidgetRepository
    {
        /// <summary>
        /// This method helps to adds the specified list of dynamicWidget in dynamicWidget repository.
        /// </summary>
        /// <param name="dynamicWidgets">The list of dynamicWidgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if dynamicWidgets are added successfully, else false.
        /// </returns>
        bool AddDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode);

        /// <summary>
        /// This method helps to update the specified list of dynamicWidgets in dynamicWidget repository.
        /// </summary>
        /// <param name="dynamicWidgets">The list of dynamicWidgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if dynamicWidgets are updated successfully, else false.
        /// </returns>
        bool UpdateDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode);

        /// <summary>
        /// This method helps to delete the specified list of dynamicWidgets in dynamicWidget repository.
        /// </summary>
        /// <param name="dynamicWidgets">The list of dynamicWidgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if dynamicWidgets are deleted successfully, else false.
        /// </returns>        
        bool DeleteDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode);

        /// <summary>
        /// This method helps to retrieve dynamicWidgets based on specified search condition from repository.
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter">The dynamicWidget search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of dynamicWidgets based on given search criteria
        /// </returns>
        IList<DynamicWidget> GetDynamicWidgets(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get dynamicWidget count
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>dynamicWidget count</returns>
        int GetDynamicWidgetCount(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode);

        IList<TenantEntity> GetTenantEntities(string tenantCode);

        IList<EntityFieldMap> GetEntityFields(long entityIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to publish page
        /// </summary>
        /// <param name="widgetIdnetifier"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool PublishDynamicWidget(long widgetIdnetifier, string tenantCode);

    }
}
