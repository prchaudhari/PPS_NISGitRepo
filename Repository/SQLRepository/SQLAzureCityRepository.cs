// <copyright file="SQLAzureCityRepository.cs" company="Websym Solutions Pvt. Ltd.">
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

    #endregion

    /// <summary>
    /// Represents city repository that defines methods to access the repository.
    /// </summary>
    public class SQLAzureCityRepository : ICityRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// The city repository constructor
        /// </summary>
        public SQLAzureCityRepository()
        {
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

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
                this.SetAndValidateConnectionString(tenantCode);
                this.IsDuplicateCity(cities,ModelConstant.ADD_OPERATION, tenantCode);
                IList<CityRecord> cityRecords = new List<CityRecord>();
                cities.ToList().ForEach(item =>
                {
                    cityRecords.Add(new CityRecord()
                    {
                        Name = item.Name,
                        StateId =item.State.Identifier,
                        IsActive = true,
                        IsDeleted = false,                       
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.CityRecords.AddRange(cityRecords);
                    nISEntitiesDataContext.SaveChanges();

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
                this.SetAndValidateConnectionString(tenantCode);
                this.IsDuplicateCity(cities,ModelConstant.UPDATE_OPERATION, tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (City City in cities)
                    {
                        CityRecord cityRecords = nISEntitiesDataContext.CityRecords.Where(item => item.Id == City.Identifier && item.IsDeleted == false).FirstOrDefault();
                        if (cityRecords == null)
                        {
                            throw new CityNotFoundException(tenantCode);
                        }
                        cityRecords.Name = City.Name;
                        cityRecords.IsActive = City.IsActive;
                        cityRecords.StateId = City.State.Identifier;
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

        #region Delete City

        /// <summary>
        /// This method helps to delete the specified list of cities in city repository.
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
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (City city in cities)
                    {
                        CityRecord cityRecords = nISEntitiesDataContext.CityRecords.Where(item => item.Id == city.Identifier).FirstOrDefault();
                        if (cityRecords == null)
                        {
                            throw new CityNotFoundException(tenantCode);
                        }
                        cityRecords.IsDeleted = true;
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
            IList<City> cities = new List<City>();
            IList<CityRecord> cityRecords = new List<CityRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(citySearchParameter, tenantCode);

                    if (citySearchParameter.PagingParameter.PageIndex != 0 && citySearchParameter.PagingParameter.PageSize != 0)
                    {
                        cityRecords = nISEntitiesDataContext.CityRecords.Where(result).
                        OrderBy(citySearchParameter.SortParameter.SortColumn + " " + citySearchParameter.SortParameter.SortOrder)
                       .Skip(citySearchParameter.PagingParameter.PageSize * (citySearchParameter.PagingParameter.PageIndex - 1))
                       .Take(citySearchParameter.PagingParameter.PageIndex * citySearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        cityRecords = nISEntitiesDataContext.CityRecords.Where(result).
                        OrderBy(citySearchParameter.SortParameter.SortColumn + " " + citySearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (cityRecords?.Count > 0)
                    {
                        string statequery = string.Join(" or ", cityRecords.Select(item => string.Format("Id.Equals({0})", item.StateId.ToString())).ToList());
                        IList<StateRecord> stateRecords = new List<StateRecord>();
                        IList<State> states = new List<State>();
                        Country country = new Country();
                        //Read states                           
                        stateRecords = nISEntitiesDataContext.StateRecords.Where(statequery).ToList();
                        stateRecords?.ToList().ForEach(stateRecord =>
                        {
                            if (stateRecords?.Count > 0)
                            {
                                //Read country 
                                string countryquery = string.Join(" or ", stateRecords.Select(item => string.Format("Id.Equals({0})", item.CountryId.ToString())).ToList());
                                IList<CountryRecord> countryRecords = new List<CountryRecord>();
                                IList<Country> countries = new List<Country>();

                                //Map country                           
                                countryRecords = nISEntitiesDataContext.CountryRecords.Where(countryquery).ToList();
                                if (countryRecords?.Count > 0)
                                {
                                    countryRecords?.ToList().ForEach(countryRecord =>
                                    {
                                        countries.Add(new Country
                                        {
                                            Identifier = countryRecord.Id,
                                            Code = countryRecord.Code,
                                            CountryName = countryRecord.Name,
                                            DialingCode = countryRecord.DialingCode,
                                            IsActive = countryRecord.IsActive,
                                            IsDeleted = countryRecord.IsDeleted
                                        });
                                    });

                                }

                                //Map State
                                stateRecords.ToList().ForEach(item =>
                                {
                                    states.Add(new State()
                                    {
                                        Identifier = item.Id,
                                        Name = item.Name,
                                        Country = countries.Where(x => x.Identifier == item.Id).FirstOrDefault(),
                                        IsActive = item.IsActive,
                                        IsDeleted = item.IsDeleted,
                                    });
                                });

                                //Map City
                                bool flag = false;
                                cityRecords.ToList().ForEach(item =>
                                {
                                    
                                    foreach(City c in cities)
                                    {
                                        if(c.Identifier==item.Id)
                                        {
                                            flag = true;
                                        }

                                    }
                                    if(!flag)
                                    {
                                        cities.Add(new City()
                                        {
                                            Identifier = item.Id,
                                            Name = item.Name,
                                            State = states.Where(x => x.Identifier == item.StateId).FirstOrDefault(),
                                            IsActive = item.IsActive,
                                            IsDeleted = item.IsDeleted,
                                        });
                                    }
                                });
                            }
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

            return cities;
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
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(citySearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    cityCount = nISEntitiesDataContext.CityRecords
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
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    CityRecord cityRecord = nISEntitiesDataContext.CityRecords.Where(item => item.Id == cityIdentifier && item.IsDeleted == false).FirstOrDefault();
                    if (cityRecord == null)
                    {
                        throw new CityNotFoundException(tenantCode);
                    }

                    cityRecord.IsActive = true;
                    nISEntitiesDataContext.SaveChanges();
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

        #region DeActivate City

        /// <summary>
        /// This method helps to deactivate the cities
        /// </summary>
        /// <param name="cityIdentifier">The city identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if city deactivated successfully false otherwise</returns>
        public bool DeactivateCity(long cityIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    CityRecord cityRecord = nISEntitiesDataContext.CityRecords.Where(item => item.Id == cityIdentifier && item.IsDeleted == false).FirstOrDefault();
                    if (cityRecord == null)
                    {
                        throw new CityNotFoundException(tenantCode);
                    }

                    cityRecord.IsActive = false;
                    nISEntitiesDataContext.SaveChanges();

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

        #endregion

        #region Private Method

        #region Where Clause Generator

        /// <summary>
        /// This method will be used to generate Where Clause
        /// </summary>
        /// <param name="searchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(CitySearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(searchParameter.StateIdentifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.StateIdentifier.ToString().Split(',').Select(item => string.Format("StateId.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(searchParameter.CityName))
                {
                    if (searchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", searchParameter.CityName));
                    }
                    else
                    {
                        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", searchParameter.CityName));
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

        #region Check Duplicate City

        /// <summary>
        /// Thismethod will be used to check duplicate city 
        /// </summary>
        /// <param name="cities">list of city object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicateCity(IList<City> cities,string operation, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", cities.Select(item => string.Format("Name.Equals(\"{0}\")", item.Name)).ToList()) + ")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", cities.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals(\"{1}\"))", item.Name, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<CityRecord> cityRecords = nISEntitiesDataContext.CityRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (cityRecords?.Count>0)
                        {
                            throw new DuplicateCityFoundException(tenantCode);
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
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.utility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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

        #endregion
    }
}
