// <copyright file="SQLAzureTenantContactRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{

    #region References

    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Security.Cryptography;
    using System.Text;
    using Unity;

    #endregion

    public class SQLTenantContactRepository : ITenantContactRepository
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
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        #endregion

        #region Constructor

        public SQLTenantContactRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);

        }

        #endregion

        #region Public Methods

        #region Add 

        /// <summary>
        /// This method helps to adds the specified list of tenantContacts in tenantContact entity.
        /// </summary>
        /// <param name="tenantContacts"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddTenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                //this.connectionString = "metadata=res://*/nVidYoDataContext.csdl|res://*/nVidYoDataContext.ssdl|res://*/nVidYoDataContext.msl;provider=System.Data.SqlClient;provider connection string=';Data Source=192.168.100.7;Initial Catalog=nvidyo;TenantContact ID=sa;Password=Admin@123;multipleactiveresultsets=True;application name=EntityFramework';";

                this.SetAndValidateConnectionString(tenantCode);
                if (this.IsDuplicateTenantContactEmailAndMobileNumber(tenantContacts, ModelConstant.ADD_OPERATION, tenantCode))
                {
                    throw new DuplicateTenantContactFoundException(tenantCode);
                }

                IList<TenantContactRecord> tenantContactRecords = new List<TenantContactRecord>();

                tenantContacts.ToList().ForEach(tenantContact =>
                {
                    tenantContactRecords.Add(new TenantContactRecord()
                    {
                        FirstName = tenantContact.FirstName,
                        LastName = tenantContact.LastName,
                        ContactNumber = tenantContact.ContactNumber,
                        EmailAddress = tenantContact.EmailAddress,
                        Image = tenantContact.Image,
                        IsActive = tenantContact.IsActive,
                        IsDeleted = false,
                        TenantCode = tenantCode,
                        CountryId = tenantContact.CountryId
                    });
                });

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {

                    nVidYoEntitiesDataContext.TenantContactRecords.AddRange(tenantContactRecords);
                    nVidYoEntitiesDataContext.SaveChanges();
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

        #region Update

        /// <summary>
        /// This method helps to updates the specified list of tenantContacts in uaer entity.
        /// </summary>
        /// <param name="tenantContacts"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool UpdateTenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                if (this.IsDuplicateTenantContactEmailAndMobileNumber(tenantContacts, ModelConstant.UPDATE_OPERATION, tenantCode))
                {
                    throw new DuplicateTenantContactFoundException(tenantCode);
                }


                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", tenantContacts.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");

                    IList<TenantContactRecord> tenantContactRecords = nVidYoEntitiesDataContext.TenantContactRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (tenantContactRecords == null || tenantContactRecords.Count <= 0 || tenantContactRecords.Count() != string.Join(",", tenantContactRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new TenantContactNotFoundException(tenantCode);
                    }

                    tenantContacts.ToList().ForEach(tenantContact =>
                    {
                        TenantContactRecord tenantContactRecord = nVidYoEntitiesDataContext.TenantContactRecords.FirstOrDefault(data => data.Id == tenantContact.Identifier && data.TenantCode == tenantCode && data.IsDeleted == false);
                        tenantContactRecord.FirstName = tenantContact.FirstName;
                        tenantContactRecord.LastName = tenantContact.LastName;
                        tenantContactRecord.ContactNumber = tenantContact.ContactNumber;
                        tenantContactRecord.EmailAddress = tenantContact.EmailAddress;
                        tenantContactRecord.Image = tenantContact.Image;
                        tenantContactRecord.IsActive = tenantContact.IsActive;
                        tenantContactRecord.TenantCode = tenantCode;
                        tenantContactRecord.CountryId = tenantContact.CountryId;
                    });

                    nVidYoEntitiesDataContext.SaveChanges();
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

        #region Delete

        /// <summary>
        /// This method helps to deletes the specified list of tenantContacts in tenantContact repository. 
        /// </summary>
        /// <param name="tenantContacts"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool DeleteTenantContacts(IList<TenantContact> tenantContacts, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", tenantContacts.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");
                    IList<TenantContactRecord> tenantContactRecords = nVidYoEntitiesDataContext.TenantContactRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (tenantContactRecords == null || tenantContactRecords.Count <= 0)
                    {
                        throw new TenantContactNotFoundException(tenantCode);
                    }

                    tenantContactRecords.ToList().ForEach(item =>
                    {
                        item.IsDeleted = true;
                    });

                    nVidYoEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
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

        #region Get

        /// <summary>
        /// This method helps to retrieve list of tenantContact based on specified search paramters.
        /// </summary>
        /// <param name="tenantContactSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantContact> GetTenantContacts(TenantContactSearchParameter tenantContactSearchParameter, string tenantCode)
        {
            IList<TenantContact> tenantContacts = new List<TenantContact>();
            IList<TenantContactRecord> tenantContactRecords = null;
            IList<CountryRecord> countires = new List<CountryRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntities = new NISEntities(this.connectionString))
                {
                    string whereClause = this.WhereClauseGenerator(tenantContactSearchParameter, tenantCode);
                    tenantContactRecords = new List<TenantContactRecord>();

                    if (tenantContactSearchParameter.PagingParameter.PageIndex > 0 && tenantContactSearchParameter.PagingParameter.PageSize > 0)
                    {
                        tenantContactRecords = nISEntities.TenantContactRecords
                        .OrderBy(tenantContactSearchParameter.SortParameter.SortColumn + " " + tenantContactSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((tenantContactSearchParameter.PagingParameter.PageIndex - 1) * tenantContactSearchParameter.PagingParameter.PageSize)
                        .Take(tenantContactSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        tenantContactRecords = nISEntities.TenantContactRecords
                        .Where(whereClause)
                        .OrderBy(tenantContactSearchParameter.SortParameter.SortColumn + " " + tenantContactSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }
                    if (tenantContactRecords?.Count() > 0)
                    {
                        StringBuilder countryIdQuery = new StringBuilder();
                        countryIdQuery = countryIdQuery.Append("(" + string.Join("or ", tenantContactRecords.Select(item => string.Format("Id.Equals({0}) ", item.CountryId))) + ")");
                        countires = nISEntities.CountryRecords.Where(countryIdQuery.ToString()).ToList();
                    }
                }
                IList<TenantContact> tempTenantContacts = new List<TenantContact>();
                tenantContactRecords?.ToList().ForEach(tenantContactRecord =>
                {
                    //Country country = null;

                    ///Get roles
                    string contactnumber = countires.Where(i => i.Id == tenantContactRecord.CountryId).FirstOrDefault().DialingCode + "-" + tenantContactRecord.ContactNumber;
                    //Get country
                    //country = this.GetCountry(tenantContactRecord.CountryId, tenantCode);
                    tempTenantContacts.Add(new TenantContact()
                    {
                        Identifier = tenantContactRecord.Id,
                        FirstName = tenantContactRecord.FirstName,
                        LastName = tenantContactRecord.LastName,
                        EmailAddress = tenantContactRecord.EmailAddress,
                        ContactNumber = contactnumber,
                        CountryId = (long)tenantContactRecord.CountryId,
                        Image = tenantContactRecord.Image,
                        IsActive = tenantContactRecord.IsActive,
                        TenantCode = tenantCode.Equals(ModelConstant.DEFAULT_TENANT_CODE) ? tenantContactRecord.TenantCode : tenantCode,
                    });

                });
                tenantContacts = tempTenantContacts;

            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return tenantContacts;
        }

        /// <summary>
        /// This method helps to retrieve list of tenantContact based on specified search paramters.
        /// </summary>
        /// <param name="tenantContactSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public IList<TenantContact> GetTenantContactToAuthentication(TenantContactSearchParameter tenantContactSearchParameter, string tenantCode)
        {
            IList<TenantContact> tenantContacts = null;
            IList<TenantContactRecord> tenantContactRecords = null;

            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntities = new NISEntities(this.connectionString))
                {
                    tenantContactRecords = nISEntities.TenantContactRecords.Where(item => item.EmailAddress == tenantContactSearchParameter.EmailAddress).ToList();
                }
                tenantContacts = new List<TenantContact>();
                tenantContactRecords?.ToList().ForEach(tenantContactRecord =>
                {
                    tenantContacts.Add(new TenantContact()
                    {
                        Identifier = tenantContactRecord.Id,
                        CountryId = tenantContactRecord.CountryId,
                        FirstName = tenantContactRecord.FirstName,
                        LastName = tenantContactRecord.LastName,
                        EmailAddress = tenantContactRecord.EmailAddress,
                        ContactNumber = tenantContactRecord.ContactNumber,
                        Image = tenantContactRecord.Image,
                        IsActive = tenantContactRecord.IsActive,
                        TenantCode = tenantCode.Equals(ModelConstant.DEFAULT_TENANT_CODE) ? tenantContactRecord.TenantCode : tenantCode,
                    });

                });
            }
            catch (SqlException)
            {
                throw new RepositoryStoreNotAccessibleException(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return tenantContacts;
        }

        #endregion



        #region Get Count

        /// <summary>
        /// this method helps to get tenantContact count
        /// </summary>
        /// <param name="tenantContactSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetTenantContactCount(TenantContactSearchParameter tenantContactSearchParameter, string tenantCode)
        {
            int tenantContactCount = 0;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                string whereClause = this.WhereClauseGenerator(tenantContactSearchParameter, tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    tenantContactCount = nVidYoEntitiesDataContext.TenantContactRecords
                                                      .Where(whereClause.ToString())
                                                      .Count();
                }
                return tenantContactCount;
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

        #region Activate

        /// <summary>
        /// This method will be used to activate tenantContact
        /// </summary>
        /// <param name="tenantContactIdentifier">The tenantContact identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated tenantContact successfully otherwise false</returns>
        public bool ActivateTenantContact(long tenantContactIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    TenantContactRecord tenantContactRecord = nVidYoEntitiesDataContext.TenantContactRecords.FirstOrDefault(x => x.Id == tenantContactIdentifier);
                    tenantContactRecord.IsActive = true;
                    nVidYoEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
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

        #region DeActivate

        /// <summary>
        /// This method will be used to deactivate tenantContact
        /// </summary>
        /// <param name="tenantContactIdentifier">The tenantContact identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>Returns true is activated tenantContact successfully otherwise false</returns>
        public bool DeactivateTenantContact(long tenantContactIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    TenantContactRecord tenantContactRecord = nVidYoEntitiesDataContext.TenantContactRecords.FirstOrDefault(x => x.Id == tenantContactIdentifier);
                    tenantContactRecord.IsActive = false;
                    nVidYoEntitiesDataContext.SaveChanges();
                }
                result = true;
                return result;
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

        #endregion

        #region Private Methods

        #region Check Duplicate TenantContact

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        public bool IsDuplicateTenantContactEmailAndMobileNumber(IList<TenantContact> tenantContacts, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", tenantContacts.Select(item => string.Format("EmailAddress.Equals(\"{0}\")",
                  item.EmailAddress))) + "");
                }
                else
                {
                    query.Append("(" + string.Join(" or ", tenantContacts.Select(item => string.Format("EmailAddress.Equals(\"{0}\") and !Id.Equals({1})",
                      item.EmailAddress, item.Identifier))) + "");
                }
                query.Append(string.Format(" and TenantCode.Equals(\"{0}\")", tenantCode));
                query.Append(" and IsDeleted.Equals(false))");

                using (NISEntities nVidYoEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<TenantContactRecord> tenantContactRecords = nVidYoEntitiesDataContext.TenantContactRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (tenantContactRecords.Count > 0)
                    {
                        result = true;
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

            return result;
        }
        #endregion

        #region WhereClause Generator

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">TenantContact search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(TenantContactSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            try
            {
                if (!string.IsNullOrWhiteSpace(searchParameter.TenantCode))
                {
                    tenantCode = searchParameter.TenantCode;
                }

                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(searchParameter.FirstName))
                {
                    if (searchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append("(" + string.Join("or ", searchParameter.FirstName.ToString().Split(',').Select(item => string.Format("(FirstName+\" \"+LastName).Contains(\"{0}\")", item))) + ") and ");
                    }
                    else
                    {
                        queryString.Append("(" + string.Join("or ", searchParameter.FirstName.ToString().Split(',').Select(item => string.Format("(FirstName+\" \"+LastName).Contains(\"{0}\")", item))) + ") and ");
                    }
                }

                if (validationEngine.IsValidText(searchParameter.EmailAddress))
                {
                    if (searchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("EmailAddress.Equals(\"{0}\") and ", searchParameter.EmailAddress));
                    }
                    else
                    {
                        queryString.Append(string.Format("EmailAddress.Contains(\"{0}\") and ", searchParameter.EmailAddress));
                    }
                }

                if (tenantCode != ModelConstant.DEFAULT_TENANT_CODE)
                {
                    queryString.Append(string.Format(" TenantCode.Equals(\"{0}\") and ", tenantCode));
                }

                if (searchParameter.IsActive != null)
                {
                    queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.IsActive));
                }
                if (searchParameter.ActivationStatus != null)
                {
                    queryString.Append(string.Format("IsActive.Equals({0}) and ", searchParameter.ActivationStatus));
                }
                if (searchParameter.LockStatus != null)
                {
                    queryString.Append(string.Format("IsLocked.Equals({0}) and ", searchParameter.LockStatus));
                }
                queryString.Append("IsDeleted.Equals(false)");
            }

            catch (Exception exception)
            {
                throw exception;
            }

            return queryString.ToString();
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
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
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
