// <copyright file="IWidgetRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface IWidgetRepository
    {
        /// <summary>
        /// This method adds the specified list of widgets in widget repository.
        /// </summary>
        /// <param name="widgets">The list of widgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if widgets are added successfully, else false.
        /// </returns>
        bool AddWidgets(IList<Widget> widgets, string tenantCode);

        /// <summary>
        /// This method updates the specified list of widgets in widget repository.
        /// </summary>
        /// <param name="widgets">The list of widgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if widgets are updated successfully, else false.
        /// </returns>
        bool UpdateWidgets(IList<Widget> widgets, string tenantCode);

        /// <summary>
        /// This method deletes the specified list of widgets from widget repository.
        /// </summary>
        /// <param name="widgets">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if widgets are deleted successfully, else false.
        /// </returns>
        bool DeleteWidgets(IList<Widget> widgets, string tenantCode);

        /// <summary>
        /// This method gets the specified list of widgets from widget repository.
        /// </summary>
        /// <param name="widgetSearchParameter">The widget search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of widgets
        /// </returns>
        IList<Widget> GetWidgets(WidgetSearchParameter widgetSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get widget count
        /// </summary>
        /// <param name="widgetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Widget count</returns>
        int GetWidgetCount(WidgetSearchParameter widgetSearchParameter, string tenantCode);

    }
}
