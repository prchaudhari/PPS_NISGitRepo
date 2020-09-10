// <copyright file="CountryController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;

    #endregion

    /// <summary>
    /// This class represent api controller for country
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("Country")]
    public class CountryController : ApiController
    {

        #region Private Members

        /// <summary>
        /// The country manager object.
        /// </summary>
        private CountryManager countryManager = null;

        #endregion

        #region Constructor

        public CountryController(IUnityContainer unityContainer)
        {
            this.countryManager = new CountryManager(unityContainer);
        }

        #endregion

        #region Public Method

        #region Add Country

        /// <summary>
        /// This method helps to add country
        /// </summary>
        /// <param name="countries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<Country> countries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.countryManager.AddCountries(countries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update Country

        /// <summary>
        /// This method helps to update country.
        /// </summary>
        /// <param name="countries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<Country> countries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.countryManager.UpdateCountries(countries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Delete Country

        /// <summary>
        /// This method helps to delete countries.
        /// </summary>
        /// <param name="countries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<Country> countries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.countryManager.DeleteCountries(countries, tenantCode);
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
        /// This method helps to get country list based on the search parameters.
        /// </summary>
        /// <param name="countrySearchParameter"></param>
        /// <returns>List of countries</returns>
        [HttpPost]
        public IList<Country> List(CountrySearchParameter countrySearchParameter)
        {
            IList<Country> countries = new List<Country>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                countries = this.countryManager.GetCountries(countrySearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.countryManager.GetCountryCount(countrySearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return countries;
        }

        #endregion
        #endregion
    }
}