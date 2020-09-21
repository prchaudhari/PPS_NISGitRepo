// <copyright file="SQLCountryRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;

    #endregion

    /// <summary>
    /// Represents country repository that defines methods to access the repository.
    /// </summary>
    public class SQLCountryRepository : ICountryRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        public SQLCountryRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

        #region Add Country

        /// <summary>
        /// This method helps to adds the specified list of country in country repository.
        /// </summary>
        /// <param name="countries">The list of countries</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if countries are added successfully, else false.
        /// </returns>
        public bool AddCountries(IList<Country> countries, string tenantCode)
        {
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateCountry(countries, "AddOperation", tenantCode);

                IList<CountryRecord> countryRecords = new List<CountryRecord>();
                countries.ToList().ForEach(country =>
                {
                    countryRecords.Add(new CountryRecord()
                    {
                        Name = country.CountryName,
                        Code = country.Code,
                        DialingCode = country.DialingCode,
                        IsActive = true,
                        IsDeleted = false,
                    });
                });

                if (countryRecords.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.CountryRecords.AddRange(countryRecords);
                        nISEntitiesDataContext.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Update Country

        /// <summary>
        /// This method helps to update the specified list of countries in country repository.
        /// </summary>
        /// <param name="countries">The list of countries</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if countries are updated successfully, else false.
        /// </returns>
        public bool UpdateCountries(IList<Country> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateCountry(countries, ModelConstant.UPDATE_OPERATION, tenantCode);

                StringBuilder query = new StringBuilder();
                query.Append("(" + string.Join("or ", string.Join(",", countries.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<CountryRecord> countryRecords = nISEntitiesDataContext.CountryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (countryRecords == null || countryRecords.Count <= 0 || countryRecords.Count() != string.Join(",", countries.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                    {
                        throw new CountryNotFoundException(tenantCode);
                    }

                    countries.ToList().ForEach(country =>
                    {
                        CountryRecord countryRecord = countryRecords.Single(item => item.Id == country.Identifier);
                        countryRecord.Name = country.CountryName;
                        countryRecord.Code = country.Code;
                        countryRecord.DialingCode = country.DialingCode;
                    });

                    nISEntitiesDataContext.SaveChanges();
                }

                result = true;
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
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
        /// This method helps to delete the specified list of countries in country repository.
        /// </summary>
        /// <param name="countries">The list of countries</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if countries are deleted successfully, else false.
        /// </returns>        
        public bool DeleteCountries(IList<Country> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (Country country in countries)
                    {
                        CountryRecord countryRecords = nISEntitiesDataContext.CountryRecords.Where(item => item.Id == country.Identifier).FirstOrDefault();
                        if (countryRecords == null)
                        {
                            throw new CountryNotFoundException(tenantCode);
                        }

                        TenantRecord tenantRecord = nISEntitiesDataContext.TenantRecords.Where(item => item.TenantCountry == country.Identifier.ToString() && item.IsDeleted == false).FirstOrDefault();
                        if (tenantRecord != null)
                        {
                            throw new CountryReferenceInTenantException(tenantCode);
                        }

                        TenantContactRecord tenantContactRecord = nISEntitiesDataContext.TenantContactRecords.Where(item => item.CountryId == country.Identifier && item.IsDeleted == false).FirstOrDefault();
                        if (tenantRecord != null)
                        {
                            throw new CountryReferenceInTenantContactException(tenantCode);
                        }

                        UserRecord user = nISEntitiesDataContext.UserRecords.Where(item => item.CountryId == country.Identifier && item.IsDeleted == false).FirstOrDefault();
                        if (user != null)
                        {
                            throw new CountryReferenceInUserException(tenantCode);
                        }
                        countryRecords.IsDeleted = true;
                        nISEntitiesDataContext.SaveChanges();
                    }

                    result = true;
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
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
        /// This method helps to retrieve countries based on specified search condition from repository.
        /// </summary>
        /// <param name="countrySearchParameter">The country search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of countries based on given search criteria
        /// </returns>
        public IList<Country> GetCountries(CountrySearchParameter countrySearchParameter, string tenantCode)
        {
            IList<Country> countries = new List<Country>();
            IList<CountryRecord> countryRecords = new List<CountryRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(countrySearchParameter, tenantCode);

                    if (countrySearchParameter.PagingParameter.PageIndex != 0 && countrySearchParameter.PagingParameter.PageSize != 0)
                    {
                        countryRecords = nISEntitiesDataContext.CountryRecords.Where(result).
                        OrderBy(countrySearchParameter.SortParameter.SortColumn + " " + countrySearchParameter.SortParameter.SortOrder)
                       .Skip(countrySearchParameter.PagingParameter.PageSize * (countrySearchParameter.PagingParameter.PageIndex - 1))
                       .Take(countrySearchParameter.PagingParameter.PageIndex * countrySearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        countryRecords = nISEntitiesDataContext.CountryRecords.Where(result).
                        OrderBy(countrySearchParameter.SortParameter.SortColumn + " " + countrySearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (countryRecords?.Count > 0)
                    {
                        countryRecords.ToList().ForEach(item =>
                        {
                            countries.Add(new Country()
                            {
                                Identifier = item.Id,
                                CountryName = item.Name,
                                Code = item.Code,
                                DialingCode = item.DialingCode,
                                IsActive = item.IsActive,
                                IsDeleted = item.IsDeleted,
                            });
                        });
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return countries;
        }

        #endregion

        #region Get Country Count

        /// <summary>
        /// This method reference to get country count
        /// </summary>
        /// <param name="countrySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>country count</returns>
        public int GetCountryCount(CountrySearchParameter countrySearchParameter, string tenantCode)
        {
            int countryCount = 0;
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(countrySearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    countryCount = nISEntitiesDataContext.CountryRecords
                                                      .Where(whereClause.ToString())
                                                      .Count();
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception)
            {
                throw;
            }

            return countryCount;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Get Connection String

        /// <summary>
        /// This method help to set and validate connection string
        /// </summary>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Check Duplicate Country

        /// <summary>
        /// Thismethod will be used to check duplicate country 
        /// </summary>
        /// <param name="countries">list of country object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicateCountry(IList<Country> countries, string operation, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", countries.Select(item => string.Format("Name.Equals(\"{0}\") or Code.Equals(\"{1}\") or DialingCode.Equals(\"{2}\")", item.CountryName, item.Code, item.DialingCode)).ToList()) + ") and IsDeleted.Equals(false)");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", countries.Select(item => string.Format("((Name.Equals(\"{0}\") or Code.Equals(\"{1}\") or DialingCode.Equals(\"{2}\")) and !Id.Equals({3}))", item.CountryName, item.Code, item.DialingCode, item.Identifier))) + ") and IsDeleted.Equals(false)");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<CountryRecord> countryRecords = nISEntitiesDataContext.CountryRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (countryRecords.Count > 0)
                    {
                        throw new DuplicateCountryFoundException(tenantCode);
                    }
                }
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        #endregion

        #region Where Clause Generator

        /// <summary>
        /// This method will be used to generate Where Clause
        /// </summary>
        /// <param name="countrySearchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(CountrySearchParameter countrySearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(countrySearchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", countrySearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(countrySearchParameter.CountryName))
                {
                    if (countrySearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", countrySearchParameter.CountryName));
                    }
                    else
                    {
                        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", countrySearchParameter.CountryName));
                    }
                }

                if (validationEngine.IsValidText(countrySearchParameter.CountryCode))
                {
                    if (countrySearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Code.Equals(\"{0}\") and ", countrySearchParameter.CountryCode));
                    }
                    else
                    {
                        queryString.Append(string.Format("Code.Contains(\"{0}\") and ", countrySearchParameter.CountryCode));
                    }
                }

                queryString.Append("IsDeleted.Equals(false) ");
                return queryString.ToString();
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
