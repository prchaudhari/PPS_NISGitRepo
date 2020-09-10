// <copyright file="ICountryRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents interface that defines methods to access country repository.
    /// </summary>
    public interface ICountryRepository
    {
        /// <summary>
        /// This method helps to adds the specified list of country in country repository.
        /// </summary>
        /// <param name="countries">The list of countries</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if countries are added successfully, else false.
        /// </returns>
        bool AddCountries(IList<Country> countries, string tenantCode);

        /// <summary>
        /// This method helps to update the specified list of countries in country repository.
        /// </summary>
        /// <param name="countries">The list of countries</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if countries are updated successfully, else false.
        /// </returns>
        bool UpdateCountries(IList<Country> countries, string tenantCode);

        /// <summary>
        /// This method helps to delete the specified list of countries in country repository.
        /// </summary>
        /// <param name="countries">The list of countries</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if countries are deleted successfully, else false.
        /// </returns>        
        bool DeleteCountries(IList<Country> countries, string tenantCode);

        /// <summary>
        /// This method helps to retrieve countries based on specified search condition from repository.
        /// </summary>
        /// <param name="countrySearchParameter">The country search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of countries based on given search criteria
        /// </returns>
        IList<Country> GetCountries(CountrySearchParameter countrySearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get country count
        /// </summary>
        /// <param name="countrySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>country count</returns>
        int GetCountryCount(CountrySearchParameter countrySearchParameter, string tenantCode);
    }
}
