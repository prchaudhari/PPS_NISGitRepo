// <copyright file="SQLContactTypeRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// Represents contactType repository that defines methods to access the repository.
    /// </summary>
    public class SQLContactTypeRepository : IContactTypeRepository
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
        public SQLContactTypeRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

        #region Add ContactType

        /// <summary>
        /// This method helps to adds the specified list of contactType in contactType repository.
        /// </summary>
        /// <param name="contactTypes">The list of contactTypes</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if contactTypes are added successfully, else false.
        /// </returns>
        public bool AddContactTypes(IList<ContactType> contactTypes, string tenantCode)
        {
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateContactType(contactTypes, "AddOperation", tenantCode);

                IList<ContactTypeRecord> contactTypeRecords = new List<ContactTypeRecord>();
                contactTypes.ToList().ForEach(contactType =>
                {
                    contactTypeRecords.Add(new ContactTypeRecord()
                    {
                        Name = contactType.Name,
                        Description = contactType.Description,
                        IsActive = true,
                        IsDeleted = false,
                        TenantCode= tenantCode
                    });
                });

                if (contactTypeRecords.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.ContactTypeRecords.AddRange(contactTypeRecords);
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

        #region Update ContactType

        /// <summary>
        /// This method helps to update the specified list of contactTypes in contactType repository.
        /// </summary>
        /// <param name="contactTypes">The list of contactTypes</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if contactTypes are updated successfully, else false.
        /// </returns>
        public bool UpdateContactTypes(IList<ContactType> contactTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateContactType(contactTypes, ModelConstant.UPDATE_OPERATION, tenantCode);

                StringBuilder query = new StringBuilder();
                query.Append("(" + string.Join("or ", string.Join(",", contactTypes.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ContactTypeRecord> contactTypeRecords = nISEntitiesDataContext.ContactTypeRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (contactTypeRecords == null || contactTypeRecords.Count <= 0 || contactTypeRecords.Count() != string.Join(",", contactTypes.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                    {
                        throw new ContactTypeNotFoundException(tenantCode);
                    }

                    contactTypes.ToList().ForEach(contactType =>
                    {
                        ContactTypeRecord contactTypeRecord = contactTypeRecords.Single(item => item.Id == contactType.Identifier);
                        contactTypeRecord.Name = contactType.Name;
                        contactTypeRecord.Description = contactType.Description;
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

        #region Delete ContactType

        /// <summary>
        /// This method helps to delete the specified list of contactTypes in contactType repository.
        /// </summary>
        /// <param name="contactTypes">The list of contactTypes</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if contactTypes are deleted successfully, else false.
        /// </returns>        
        public bool DeleteContactTypes(IList<ContactType> contactTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (ContactType contactType in contactTypes)
                    {
                        ContactTypeRecord contactTypeRecords = nISEntitiesDataContext.ContactTypeRecords.Where(item => item.Id == contactType.Identifier).FirstOrDefault();
                        if (contactTypeRecords == null)
                        {
                            throw new ContactTypeNotFoundException(tenantCode);
                        }
                        TenantContactRecord user = nISEntitiesDataContext.TenantContactRecords.Where(item => item.ContactType == contactType.Identifier.ToString() && item.IsDeleted == false).FirstOrDefault();
                        if (user != null)
                        {
                            throw new ContactTypeReferenceInTenantContactException(tenantCode);
                        }
                        contactTypeRecords.IsDeleted = true;
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

        #region Get ContactTypes

        /// <summary>
        /// This method helps to retrieve contactTypes based on specified search condition from repository.
        /// </summary>
        /// <param name="contactTypeSearchParameter">The contactType search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of contactTypes based on given search criteria
        /// </returns>
        public IList<ContactType> GetContactTypes(ContactTypeSearchParameter contactTypeSearchParameter, string tenantCode)
        {
            IList<ContactType> contactTypes = new List<ContactType>();
            IList<ContactTypeRecord> contactTypeRecords = new List<ContactTypeRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(contactTypeSearchParameter, tenantCode);

                    if (contactTypeSearchParameter.PagingParameter.PageIndex != 0 && contactTypeSearchParameter.PagingParameter.PageSize != 0)
                    {
                        contactTypeRecords = nISEntitiesDataContext.ContactTypeRecords.Where(result).
                        OrderBy(contactTypeSearchParameter.SortParameter.SortColumn + " " + contactTypeSearchParameter.SortParameter.SortOrder)
                       .Skip(contactTypeSearchParameter.PagingParameter.PageSize * (contactTypeSearchParameter.PagingParameter.PageIndex - 1))
                       .Take(contactTypeSearchParameter.PagingParameter.PageIndex * contactTypeSearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        contactTypeRecords = nISEntitiesDataContext.ContactTypeRecords.Where(result).
                        OrderBy(contactTypeSearchParameter.SortParameter.SortColumn + " " + contactTypeSearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (contactTypeRecords?.Count > 0)
                    {
                        contactTypeRecords.ToList().ForEach(item =>
                        {
                            contactTypes.Add(new ContactType()
                            {
                                Identifier = item.Id,
                                Name = item.Name,
                                Description = item.Description,
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

            return contactTypes;
        }

        #endregion

        #region Get ContactType Count

        /// <summary>
        /// This method reference to get contactType count
        /// </summary>
        /// <param name="contactTypeSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>contactType count</returns>
        public int GetContactTypeCount(ContactTypeSearchParameter contactTypeSearchParameter, string tenantCode)
        {
            int contactTypeCount = 0;
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(contactTypeSearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    contactTypeCount = nISEntitiesDataContext.ContactTypeRecords
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

            return contactTypeCount;
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

        #region Check Duplicate ContactType

        /// <summary>
        /// Thismethod will be used to check duplicate contactType 
        /// </summary>
        /// <param name="contactTypes">list of contactType object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicateContactType(IList<ContactType> contactTypes, string operation, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", contactTypes.Select(item => string.Format("Name.Equals(\"{0}\") ", item.Name)).ToList()) + ") and TenantCode.Equals(\""+tenantCode+"\") and IsDeleted.Equals(false)");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", contactTypes.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}))", item.Name, item.Identifier))) + ") and TenantCode.Equals(\"" + tenantCode + "\") and IsDeleted.Equals(false)");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<ContactTypeRecord> contactTypeRecords = nISEntitiesDataContext.ContactTypeRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (contactTypeRecords.Count > 0)
                    {
                        throw new DuplicateContactTypeFoundException(tenantCode);
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
        /// <param name="contactTypeSearchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(ContactTypeSearchParameter contactTypeSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(contactTypeSearchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", contactTypeSearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(contactTypeSearchParameter.ContactTypeName))
                {
                    if (contactTypeSearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", contactTypeSearchParameter.ContactTypeName));
                    }
                    else
                    {
                        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", contactTypeSearchParameter.ContactTypeName));
                    }
                }

                queryString.Append(string.Format("TenantCode.Equals(\"{0}\") and IsDeleted.Equals(false)", tenantCode));
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
