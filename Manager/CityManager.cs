// <copyright file="CityManager .cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class represents the manager layer for city.
    /// </summary>
    public class CityManager
    {
        #region Private Members

        /// <summary>
        /// The city repository
        /// </summary>
        private ICityRepository cityRepository = null;

        #endregion

        #region Constructor

        public CityManager(IUnityContainer unityContainer)
        {
            this.cityRepository = unityContainer.Resolve<ICityRepository>();
        }

        #endregion

        #region public Methods

        #region Add City

        /// <summary>
        /// This method helps to adds the specified list of city in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are added successfully, else false.
        /// </returns>
        public bool AddCities(IList<City> cities, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsDuplicateCity(cities, tenantCode);
                this.IsValidCities(cities, tenantCode);
                result = this.cityRepository.AddCities(cities, tenantCode);

            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }


        #endregion

        #region Update City

        /// <summary>
        /// This method helps to update the specified list of cities in city repository.
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are updated successfully, else false.
        /// </returns>
        public bool UpdateCities(IList<City> cities, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsDuplicateCity(cities, tenantCode);
                this.IsValidCities(cities, tenantCode);
                result = this.cityRepository.UpdateCities(cities, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Delete City

        /// <summary>
        /// This method helps to delete the specified list of cities .
        /// </summary>
        /// <param name="cities">The list of cities</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if cities are deleted successfully, else false.
        /// </returns>        
        public bool DeleteCities(IList<City> cities, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsDuplicateCity(cities, tenantCode);
                this.IsValidCities(cities, tenantCode);
                result = this.cityRepository.DeleteCities(cities, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Get City

        /// <summary>
        /// This method helps to retrieve cities based on specified search condition from repository.
        /// </summary>
        /// <param name="citySearchParameter">The city search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of cities based on given search criteria
        /// </returns>
        public IList<City> GetCities(CitySearchParameter citySearchParameter, string tenantCode)
        {
            IList<City> Cities = new List<City>();
            try
            {
                //InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                //try
                //{
                //    citySearchParameter.IsValid();
                //}
                //catch (Exception exception)
                //{
                //    invalidSearchParameterException.Data.Add(ModelConstant.INVALID_PAGING_PARAMETER, exception.Data);
                //}

                //if (invalidSearchParameterException.Data.Count > 0)
                //{
                //    throw invalidSearchParameterException;
                //}

                Cities = this.cityRepository.GetCities(citySearchParameter, tenantCode);

            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Cities;
        }

        #endregion

        #region Get City Count

        /// <summary>
        /// This method reference to get city count
        /// </summary>
        /// <param name="citySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Certifications count</returns>
        public int GetCityCount(CitySearchParameter citySearchParameter, string tenantCode)
        {
            int cityCount = 0;
            try
            {
                cityCount = this.cityRepository.GetCityCount(citySearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return cityCount;
        }

        #endregion

        #region Activate City

        /// <summary>
        /// This method helps to activate the cities
        /// </summary>
        /// <param name="cityIdentifier">The city identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if city activated successfully false otherwise</returns>
        public bool ActivateCity(long cityIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.cityRepository.ActivateCity(cityIdentifier, tenantCode); ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region DeActivate City

        /// <summary>
        /// This method helps to deactivate the cities
        /// </summary>
        /// <param name="stateIdentifier">The city identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if city deactivated successfully false otherwise</returns>
        public bool DeactivateCity(long cityIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.cityRepository.DeactivateCity(cityIdentifier, tenantCode); ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Validate Cities

        /// <summary>
        /// This method will be used to validate city.
        /// </summary>
        /// <param name="product cities">The list of cities</param>        
        private void IsValidCities(IList<City> cities, string tenantCode)
        {
            try
            {
                if (cities?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidCityException invalidCityException = new InvalidCityException(tenantCode);
                cities.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidCityException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidCityException.Data.Count > 0)
                {
                    throw invalidCityException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Check Duplicate Cities

        /// <summary>
        /// This method will be used to check duplicate cities in the list
        /// </summary>
        /// <param name="cities">The list of cities</param>        
        private void IsDuplicateCity(IList<City> cities, string tenantCode)
        {
            try
            {
                int duplicateCityCount = cities.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (duplicateCityCount > 0)
                {
                    throw new DuplicateCityFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #endregion
    }
}
