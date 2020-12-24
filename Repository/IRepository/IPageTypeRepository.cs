// <copyright file="IPageTypeRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents interface that defines methods to access city repository.
    /// </summary>
    public interface IPageTypeRepository
    {
        /// <summary>
        /// This method helps to adds the specified list of city in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are added successfully, else false.
        /// </returns>
        bool AddPageTypes(IList<PageType> cities, string tenantCode);

        /// <summary>
        /// This method helps to update the specified list of cities in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are updated successfully, else false.
        /// </returns>
        bool UpdatePageTypes(IList<PageType> cities, string tenantCode);

        /// <summary>
        /// This method helps to delete the specified list of cities in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are deleted successfully, else false.
        /// </returns>        
        bool DeletePageTypes(IList<PageType> cities, string tenantCode);

        /// <summary>
        /// This method helps to retrieve cities based on specified search condition from repository.
        /// </summary>
        /// <param name="citySearchParameter">The city search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of cities based on given search criteria
        /// </returns>
        IList<PageType> GetPageTypes(PageTypeSearchParameter citySearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get city count
        /// </summary>
        /// <param name="citySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Certifications count</returns>
        int GetPageTypeCount(PageTypeSearchParameter citySearchParameter, string tenantCode);

    }
}
