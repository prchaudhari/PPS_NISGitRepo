// <copyright file="ICityRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    public interface ICityRepository
    {
        /// <summary>
        /// This method helps to adds the specified list of city in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are added successfully, else false.
        /// </returns>
        bool AddCities(IList<City> cities, string tenantCode);

        /// <summary>
        /// This method helps to update the specified list of cities in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are updated successfully, else false.
        /// </returns>
        bool UpdateCities(IList<City> cities, string tenantCode);

        /// <summary>
        /// This method helps to delete the specified list of cities in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are deleted successfully, else false.
        /// </returns>        
        bool DeleteCities(IList<City> cities, string tenantCode);

        /// <summary>
        /// This method helps to retrieve cities based on specified search condition from repository.
        /// </summary>
        /// <param name="citySearchParameter">The city search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of cities based on given search criteria
        /// </returns>
        IList<City> GetCities(CitySearchParameter citySearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get city count
        /// </summary>
        /// <param name="citySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Certifications count</returns>
        int GetCityCount(CitySearchParameter citySearchParameter, string tenantCode);


        /// <summary>
        /// This method helps to activate the cities
        /// </summary>
        /// <param name="cityIdentifier">The city identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if city activated successfully false otherwise</returns>
        bool ActivateCity(long cityIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the cities
        /// </summary>
        /// <param name="cityIdentifier">The city identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if city deactivated successfully false otherwise</returns>
        bool DeactivateCity(long cityIdentifier, string tenantCode);
    }
}
