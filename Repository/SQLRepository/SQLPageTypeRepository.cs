// <copyright file="SQLPageTypeRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// Represents pageType repository that defines methods to access the repository.
    /// </summary>
    public class SQLPageTypeRepository : IPageTypeRepository
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
        public SQLPageTypeRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

        #region Add PageType

        /// <summary>
        /// This method helps to adds the specified list of pageType in pageType repository.
        /// </summary>
        /// <param name="pageTypes">The list of pageTypes</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pageTypes are added successfully, else false.
        /// </returns>
        public bool AddPageTypes(IList<PageType> pageTypes, string tenantCode)
        {
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicatePageType(pageTypes, "AddOperation", tenantCode);

                IList<PageTypeRecord> pageTypeRecords = new List<PageTypeRecord>();
                pageTypes.ToList().ForEach(pageType =>
                {
                    pageTypeRecords.Add(new PageTypeRecord()
                    {
                        Name = pageType.PageTypeName,
                        Description = pageType.Description,
                        IsActive = true,
                        IsDeleted = false,
                        TenantCode= tenantCode
                    });
                });

                if (pageTypeRecords.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.PageTypeRecords.AddRange(pageTypeRecords);
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

        #region Update PageType

        /// <summary>
        /// This method helps to update the specified list of pageTypes in pageType repository.
        /// </summary>
        /// <param name="pageTypes">The list of pageTypes</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pageTypes are updated successfully, else false.
        /// </returns>
        public bool UpdatePageTypes(IList<PageType> pageTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicatePageType(pageTypes, ModelConstant.UPDATE_OPERATION, tenantCode);

                StringBuilder query = new StringBuilder();
                query.Append("(" + string.Join("or ", string.Join(",", pageTypes.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageTypeRecord> pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (pageTypeRecords == null || pageTypeRecords.Count <= 0 || pageTypeRecords.Count() != string.Join(",", pageTypes.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                    {
                        throw new PageTypeNotFoundException(tenantCode);
                    }

                    pageTypes.ToList().ForEach(pageType =>
                    {
                        PageTypeRecord pageTypeRecord = pageTypeRecords.Single(item => item.Id == pageType.Identifier);
                        pageTypeRecord.Name = pageType.PageTypeName;
                        pageTypeRecord.Description = pageType.Description;
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

        #region Delete PageType

        /// <summary>
        /// This method helps to delete the specified list of pageTypes in pageType repository.
        /// </summary>
        /// <param name="pageTypes">The list of pageTypes</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if pageTypes are deleted successfully, else false.
        /// </returns>        
        public bool DeletePageTypes(IList<PageType> pageTypes, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (PageType pageType in pageTypes)
                    {
                        PageTypeRecord pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(item => item.Id == pageType.Identifier).FirstOrDefault();
                        if (pageTypeRecords == null)
                        {
                            throw new PageTypeNotFoundException(tenantCode);
                        }
                        PageRecord user = nISEntitiesDataContext.PageRecords.Where(item => item.PageTypeId == pageType.Identifier && item.IsDeleted == false).FirstOrDefault();
                        if (user != null)
                        {
                            throw new PageTypeReferenceInTenantContactException(tenantCode);
                        }
                        pageTypeRecords.IsDeleted = true;
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

        #region Get PageTypes

        /// <summary>
        /// This method helps to retrieve pageTypes based on specified search condition from repository.
        /// </summary>
        /// <param name="pageTypeSearchParameter">The pageType search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of pageTypes based on given search criteria
        /// </returns>
        public IList<PageType> GetPageTypes(PageTypeSearchParameter pageTypeSearchParameter, string tenantCode)
        {
            IList<PageType> pageTypes = new List<PageType>();
            IList<PageTypeRecord> pageTypeRecords = new List<PageTypeRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(pageTypeSearchParameter, tenantCode);

                    if (pageTypeSearchParameter.PagingParameter.PageIndex != 0 && pageTypeSearchParameter.PagingParameter.PageSize != 0)
                    {
                        pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(result).
                        OrderBy(pageTypeSearchParameter.SortParameter.SortColumn + " " + pageTypeSearchParameter.SortParameter.SortOrder)
                       .Skip(pageTypeSearchParameter.PagingParameter.PageSize * (pageTypeSearchParameter.PagingParameter.PageIndex - 1))
                       .Take(pageTypeSearchParameter.PagingParameter.PageIndex * pageTypeSearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(result).
                        OrderBy(pageTypeSearchParameter.SortParameter.SortColumn + " " + pageTypeSearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (pageTypeRecords?.Count > 0)
                    {
                        pageTypeRecords.ToList().ForEach(item =>
                        {
                            pageTypes.Add(new PageType()
                            {
                                Identifier = item.Id,
                                PageTypeName = item.Name,
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

            return pageTypes;
        }

        #endregion

        #region Get PageType Count

        /// <summary>
        /// This method reference to get pageType count
        /// </summary>
        /// <param name="pageTypeSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>pageType count</returns>
        public int GetPageTypeCount(PageTypeSearchParameter pageTypeSearchParameter, string tenantCode)
        {
            int pageTypeCount = 0;
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(pageTypeSearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    pageTypeCount = nISEntitiesDataContext.PageTypeRecords
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

            return pageTypeCount;
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

        #region Check Duplicate PageType

        /// <summary>
        /// Thismethod will be used to check duplicate pageType 
        /// </summary>
        /// <param name="pageTypes">list of pageType object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicatePageType(IList<PageType> pageTypes, string operation, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                StringBuilder query = new StringBuilder();
                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pageTypes.Select(item => string.Format("Name.Equals(\"{0}\") ", item.PageTypeName)).ToList()) + ") and TenantCode.Equals(\""+tenantCode+"\") and IsDeleted.Equals(false)");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", pageTypes.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}))", item.PageTypeName, item.Identifier))) + ") and TenantCode.Equals(\"" + tenantCode + "\") and IsDeleted.Equals(false)");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<PageTypeRecord> pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (pageTypeRecords.Count > 0)
                    {
                        throw new DuplicatePageTypeFoundException(tenantCode);
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
        /// <param name="pageTypeSearchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(PageTypeSearchParameter pageTypeSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(pageTypeSearchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", pageTypeSearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(pageTypeSearchParameter.PageTypeName))
                {
                    if (pageTypeSearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("Name.Equals(\"{0}\") and ", pageTypeSearchParameter.PageTypeName));
                    }
                    else
                    {
                        queryString.Append(string.Format("Name.Contains(\"{0}\") and ", pageTypeSearchParameter.PageTypeName));
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
