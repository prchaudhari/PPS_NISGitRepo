// <copyright file="SQLAzureStateRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// Represents state repository that defines methods to access the repository.
    /// </summary>
    public class SQLAzureStateRepository : IStateRepository
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
        /// The state repository constructor
        /// </summary>
        public SQLAzureStateRepository()
        {
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

        #region Add State

        /// <summary>
        /// This method helps to adds the specified list of state in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are added successfully, else false.
        /// </returns>
        public bool AddStates(IList<State> states, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                this.IsDuplicateState(states, ModelConstant.ADD_OPERATION, tenantCode);
                IList<StateRecord> stateRecords = new List<StateRecord>();
                states.ToList().ForEach(item =>
                {
                    stateRecords.Add(new StateRecord()
                    {
                        Name = item.Name,
                        CountryId = item.Country.Identifier,
                        IsActive = true,
                        IsDeleted = false,                       
                    });
                });

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    nISEntitiesDataContext.StateRecords.AddRange(stateRecords);
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

        #region Update State

        /// <summary>
        /// This method helps to update the specified list of states in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are updated successfully, else false.
        /// </returns>
        public bool UpdateStates(IList<State> states, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                this.IsDuplicateState(states, ModelConstant.UPDATE_OPERATION, tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (State state in states)
                    {
                        StateRecord stateRecords = nISEntitiesDataContext.StateRecords.Where(item => item.Id == state.Identifier && item.IsDeleted == false).FirstOrDefault();
                        if (stateRecords == null)
                        {
                            throw new StateNotFoundException(tenantCode);
                        }
                        stateRecords.Name = state.Name;
                        stateRecords.CountryId = state.Country.Identifier;
                        stateRecords.IsActive = true;
                        stateRecords.IsDeleted = false;
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

        #region Delete State

        /// <summary>
        /// This method helps to delete the specified list of states in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are deleted successfully, else false.
        /// </returns>        
        public bool DeleteStates(IList<State> states, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (State state in states)
                    {
                        StateRecord stateRecords = nISEntitiesDataContext.StateRecords.Where(item => item.Id == state.Identifier).FirstOrDefault();
                        if (stateRecords == null)
                        {
                            throw new CountryNotFoundException(tenantCode);
                        }
                        stateRecords.IsDeleted = true;
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

        #region Get State

        /// <summary>
        /// This method helps to retrieve states based on specified search condition from repository.
        /// </summary>
        /// <param name="stateSearchParameter">The state search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of states based on given search criteria
        /// </returns>
        public IList<State> GetStates(StateSearchParameter stateSearchParameter, string tenantCode)
        {
            IList<State> states = new List<State>();
            IList<StateRecord> stateRecords = new List<StateRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(stateSearchParameter, tenantCode);

                    if (stateSearchParameter.PagingParameter.PageIndex != 0 && stateSearchParameter.PagingParameter.PageSize != 0)
                    {
                        stateRecords = nISEntitiesDataContext.StateRecords.Where(result).
                        OrderBy(stateSearchParameter.SortParameter.SortColumn + " " + stateSearchParameter.SortParameter.SortOrder)
                       .Skip(stateSearchParameter.PagingParameter.PageSize * (stateSearchParameter.PagingParameter.PageIndex - 1))
                       .Take(stateSearchParameter.PagingParameter.PageIndex * stateSearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        stateRecords = nISEntitiesDataContext.StateRecords.Where(result).
                        OrderBy(stateSearchParameter.SortParameter.SortColumn + " " + stateSearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (stateRecords?.Count > 0)
                    {

                        string countryquery = string.Join(" or ", stateRecords.Select(item => string.Format("Id.Equals({0})", item.CountryId.ToString())).ToList());
                        IList<CountryRecord> countryRecords = new List<CountryRecord>();
                        IList<Country> countries = new List<Country>();

                        ///Read country                           
                        countryRecords = nISEntitiesDataContext.CountryRecords.Where(countryquery).ToList();
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

                        stateRecords.ToList().ForEach(item =>
                        {
                            states.Add(new State()
                            {
                                Identifier = item.Id,
                                Name = item.Name,
                                //Country=nISEntitiesDataContext.CountryRecords.Where(x=>x.Id==item.CountryId).Select(x=>new Country {Identifier=x.Id }).FirstOrDefault(),
                                Country = countries.Where(x => x.Identifier == item.CountryId).FirstOrDefault(),
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

            return states;
        }

        #endregion

        #region Get State Count

        /// <summary>
        /// This method reference to get state count
        /// </summary>
        /// <param name="stateSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Certifications count</returns>
        public int GetStatesCount(StateSearchParameter stateSearchParameter, string tenantCode)
        {
            int stateCount = 0;
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(stateSearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    stateCount = nISEntitiesDataContext.CountryRecords
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

            return stateCount;
        }

        #endregion

        #region Activate State

        /// <summary>
        /// This method helps to activate the states
        /// </summary>
        /// <param name="countryIdentifier">The state identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if state activated successfully false otherwise</returns>
        public bool ActivateState(long stateIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StateRecord stateRecord = nISEntitiesDataContext.StateRecords.Where(item => item.Id == stateIdentifier && item.IsDeleted == false).FirstOrDefault();
                    if (stateRecord == null)
                    {
                        throw new StateNotFoundException(tenantCode);
                    }
                    stateRecord.IsActive = true;
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

        #region DeActivate State

        /// <summary>
        /// This method helps to deactivate the states
        /// </summary>
        /// <param name="stateIdentifier">The state identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if state deactivated successfully false otherwise</returns>
        public bool DeactivateState(long stateIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StateRecord stateRecord = nISEntitiesDataContext.StateRecords.Where(item => item.Id == stateIdentifier && item.IsDeleted == false).FirstOrDefault();
                    if (stateRecord == null)
                    {
                        throw new StateNotFoundException(tenantCode);
                    }
                    stateRecord.IsActive = false;
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
        /// <param name="stateSearchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(StateSearchParameter stateSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(stateSearchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", stateSearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(stateSearchParameter.CountryIdentifier))
                {
                    queryString.Append("(" + string.Join("or ", stateSearchParameter.CountryIdentifier.ToString().Split(',').Select(item => string.Format("CountryId.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(stateSearchParameter.StateName))
                {
                    if (stateSearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", stateSearchParameter.StateName));
                    }
                    else
                    {
                        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", stateSearchParameter.StateName));
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

        #region Check Duplicate states

        /// <summary>
        /// This method will be used to check duplicate state 
        /// </summary>
        /// <param name="states">list of state object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicateState(IList<State> states, string operation, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", states.Select(item => string.Format("Name.Equals(\"{0}\")", item.Name)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", states.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals(\"{1}\"))", item.Name, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<StateRecord> stateRecords = nISEntitiesDataContext.StateRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (stateRecords != null)
                    {
                        throw new DuplicateStateFoundException(tenantCode);
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
