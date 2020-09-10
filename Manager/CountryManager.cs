// <copyright file="CountryManager.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// This class implements manager layer of country manager.
    /// </summary>
    public class CountryManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        ICountryRepository countryRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for country manager, which initialise
        /// country repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public CountryManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.countryRepository = this.unityContainer.Resolve<ICountryRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add Country

        /// <summary>
        /// This method will call add country method of repository.
        /// </summary>
        /// <param name="countries">Country are to be add.</param>
        /// <param name="tenantCode">Tenant code of country.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddCountries(IList<Country> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidCountry(countries, tenantCode);
                this.IsDuplicateCountry(countries, tenantCode);
                result = this.countryRepository.AddCountries(countries, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update Country

        /// <summary>
        /// This method reference helps to update details about countries.
        /// </summary>
        /// <param name="countries">
        /// The list of countries.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if list of scene updates scus=ccessfully otherwise false
        /// </returns>
        public bool UpdateCountries(IList<Country> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidCountry(countries, tenantCode);
                this.IsDuplicateCountry(countries, tenantCode);
                result = this.countryRepository.UpdateCountries(countries, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete Country

        /// <summary>
        /// This method reference helps to delete details about country.
        /// </summary>
        /// <param name="countries">
        /// The list of countries.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        public bool DeleteCountries(IList<Country> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.countryRepository.DeleteCountries(countries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Get Countries

        /// <summary>
        /// This method will call get countries method of repository.
        /// </summary>
        /// <param name="countrySearchParameter">The country search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<Country> GetCountries(CountrySearchParameter countrySearchParameter, string tenantCode)
        {
            IList<Country> countries = new List<Country>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    countrySearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                countries = this.countryRepository.GetCountries(countrySearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return countries;
        }

        #endregion

        #region Get Country Count
        /// <summary>
        /// This method helps to get count of country.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetCountryCount(CountrySearchParameter countrySearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.countryRepository.GetCountryCount(countrySearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }
        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate country.
        /// </summary>
        /// <param name="countries"></param>
        /// <param name="tenantCode"></param>
        private void IsValidCountry(IList<Country> countries, string tenantCode)
        {
            try
            {
                if (countries?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidCountryException invalidCountryException = new InvalidCountryException(tenantCode);
                countries.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidCountryException.Data.Add(item.CountryName, ex.Data);
                    }
                });

                if (invalidCountryException.Data.Count > 0)
                {
                    throw invalidCountryException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate country in the list
        /// </summary>
        /// <param name="countries">countries</param>
        /// <param name="tenantCode">tenant code</param>
        private void IsDuplicateCountry(IList<Country> countries, string tenantCode)
        {
            try
            {
                int isDuplicateCountry = countries.GroupBy(p => p.CountryName).Where(g => g.Count() > 1).Count();
                if (isDuplicateCountry > 0)
                {
                    throw new DuplicateCountryFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}