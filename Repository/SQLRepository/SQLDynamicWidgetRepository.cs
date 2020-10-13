// <copyright file="SQLDynamicWidgetRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// Represents dynamicWidget repository that defines methods to access the repository.
    /// </summary>
    public class SQLDynamicWidgetRepository : IDynamicWidgetRepository
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
        public SQLDynamicWidgetRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
            this.utility = new Utility();
        }

        #endregion

        #region Public Methods

        #region Add DynamicWidget

        /// <summary>
        /// This method helps to adds the specified list of dynamicWidget in dynamicWidget repository.
        /// </summary>
        /// <param name="dynamicWidgets">The list of dynamicWidgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if dynamicWidgets are added successfully, else false.
        /// </returns>
        public bool AddDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode)
        {
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateDynamicWidget(dynamicWidgets, "AddOperation", tenantCode);

                IList<DynamicWidgetRecord> dynamicWidgetRecords = new List<DynamicWidgetRecord>();
                dynamicWidgets.ToList().ForEach(dynamicWidget =>
                {
                    dynamicWidgetRecords.Add(new DynamicWidgetRecord()
                    {
                        WidgetName = dynamicWidget.WidgetName,
                        WidgetType = dynamicWidget.WidgetType,
                        PageTypeId = dynamicWidget.PageTypeId,
                        EntityId = dynamicWidget.EntityId,
                        Title = dynamicWidget.Title,
                        ThemeType = dynamicWidget.ThemeType,
                        ThemeCSS = dynamicWidget.ThemeCSS,
                        WidgetSettings = dynamicWidget.WidgetSettings,
                        WidgetFilterSettings = dynamicWidget.WidgetFilterSettings,
                        Status = dynamicWidget.Status,
                        CreatedBy = dynamicWidget.CreatedBy,
                        CreatedOn = dynamicWidget.CreatedOn,
                        LastUpdatedBy = dynamicWidget.LastUpdatedBy,
                        PublishedBy = dynamicWidget.PublishedBy,
                        PublishedDate = dynamicWidget.PublishedDate,
                        IsActive = true,
                        IsDeleted = false,
                        TenantCode = tenantCode
                    });
                });

                if (dynamicWidgetRecords.Count > 0)
                {
                    using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                    {
                        nISEntitiesDataContext.DynamicWidgetRecords.AddRange(dynamicWidgetRecords);
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

        #region Update DynamicWidget

        /// <summary>
        /// This method helps to update the specified list of dynamicWidgets in dynamicWidget repository.
        /// </summary>
        /// <param name="dynamicWidgets">The list of dynamicWidgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if dynamicWidgets are updated successfully, else false.
        /// </returns>
        public bool UpdateDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode)
        {
            bool result = false;
            try
            {
                SetAndValidateConnectionString(tenantCode);
                IsDuplicateDynamicWidget(dynamicWidgets, ModelConstant.UPDATE_OPERATION, tenantCode);

                StringBuilder query = new StringBuilder();
                query.Append("(" + string.Join("or ", string.Join(",", dynamicWidgets.Select(item => item.Identifier).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<DynamicWidgetRecord> dynamicWidgetRecords = nISEntitiesDataContext.DynamicWidgetRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    if (dynamicWidgetRecords == null || dynamicWidgetRecords.Count <= 0 || dynamicWidgetRecords.Count() != string.Join(",", dynamicWidgets.Select(item => item.Identifier).Distinct()).ToString().Split(',').Length)
                    {
                        throw new DynamicWidgetNotFoundException(tenantCode);
                    }

                    dynamicWidgets.ToList().ForEach(dynamicWidget =>
                    {
                        DynamicWidgetRecord dynamicWidgetRecord = dynamicWidgetRecords.Single(item => item.Id == dynamicWidget.Identifier);
                        dynamicWidgetRecord.WidgetName = dynamicWidget.WidgetName;
                        dynamicWidgetRecord.WidgetType = dynamicWidget.WidgetType;
                        dynamicWidgetRecord.PageTypeId = dynamicWidget.PageTypeId;
                        dynamicWidgetRecord.EntityId = dynamicWidget.EntityId;
                        dynamicWidgetRecord.Title = dynamicWidget.Title;
                        dynamicWidgetRecord.ThemeType = dynamicWidget.ThemeType;
                        dynamicWidgetRecord.ThemeCSS = dynamicWidget.ThemeCSS;
                        dynamicWidgetRecord.WidgetSettings = dynamicWidget.WidgetSettings;
                        dynamicWidgetRecord.WidgetFilterSettings = dynamicWidget.WidgetFilterSettings;
                        dynamicWidgetRecord.Status = dynamicWidget.Status;
                        dynamicWidgetRecord.CreatedBy = dynamicWidget.CreatedBy;
                        dynamicWidgetRecord.CreatedOn = dynamicWidget.CreatedOn;
                        dynamicWidgetRecord.LastUpdatedBy = dynamicWidget.LastUpdatedBy;
                        dynamicWidgetRecord.PublishedBy = dynamicWidget.PublishedBy;
                        dynamicWidgetRecord.PublishedDate = dynamicWidget.PublishedDate;
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

        #region Delete DynamicWidget

        /// <summary>
        /// This method helps to delete the specified list of dynamicWidgets in dynamicWidget repository.
        /// </summary>
        /// <param name="dynamicWidgets">The list of dynamicWidgets</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if dynamicWidgets are deleted successfully, else false.
        /// </returns>        
        public bool DeleteDynamicWidgets(IList<DynamicWidget> dynamicWidgets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    foreach (DynamicWidget dynamicWidget in dynamicWidgets)
                    {
                        DynamicWidgetRecord dynamicWidgetRecords = nISEntitiesDataContext.DynamicWidgetRecords.Where(item => item.Id == dynamicWidget.Identifier).FirstOrDefault();
                        //if (dynamicWidgetRecords == null)
                        //{
                        //    throw new DynamicWidgetNotFoundException(tenantCode);
                        //}

                        //TenantRecord tenantRecord = nISEntitiesDataContext.TenantRecords.Where(item => item.TenantDynamicWidget == dynamicWidget.Identifier.ToString() && item.IsDeleted == false).FirstOrDefault();
                        //if (tenantRecord != null)
                        //{
                        //    throw new DynamicWidgetReferenceInTenantException(tenantCode);
                        //}

                        //TenantContactRecord tenantContactRecord = nISEntitiesDataContext.TenantContactRecords.Where(item => item.DynamicWidgetId == dynamicWidget.Identifier && item.IsDeleted == false).FirstOrDefault();
                        //if (tenantRecord != null)
                        //{
                        //    throw new DynamicWidgetReferenceInTenantContactException(tenantCode);
                        //}

                        //UserRecord user = nISEntitiesDataContext.UserRecords.Where(item => item.DynamicWidgetId == dynamicWidget.Identifier && item.IsDeleted == false).FirstOrDefault();
                        //if (user != null)
                        //{
                        //    throw new DynamicWidgetReferenceInUserException(tenantCode);
                        //}
                        //TenantUserRecord tenantUserRecord = nISEntitiesDataContext.TenantUserRecords.Where(item => item.DynamicWidgetId == dynamicWidget.Identifier && item.IsDeleted == false).FirstOrDefault();
                        //if (user != null)
                        //{
                        //    throw new DynamicWidgetReferenceInUserException(tenantCode);
                        //}
                        dynamicWidgetRecords.IsDeleted = true;
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

        #region Get DynamicWidgets

        /// <summary>
        /// This method helps to retrieve dynamicWidgets based on specified search condition from repository.
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter">The dynamicWidget search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of dynamicWidgets based on given search criteria
        /// </returns>
        public IList<DynamicWidget> GetDynamicWidgets(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            IList<DynamicWidget> dynamicWidgets = new List<DynamicWidget>();
            IList<DynamicWidgetRecord> dynamicWidgetRecords = new List<DynamicWidgetRecord>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    string result = this.WhereClauseGenerator(dynamicWidgetSearchParameter, tenantCode);

                    if (dynamicWidgetSearchParameter.PagingParameter.PageIndex != 0 && dynamicWidgetSearchParameter.PagingParameter.PageSize != 0)
                    {
                        dynamicWidgetRecords = nISEntitiesDataContext.DynamicWidgetRecords.Where(result).
                        OrderBy(dynamicWidgetSearchParameter.SortParameter.SortColumn + " " + dynamicWidgetSearchParameter.SortParameter.SortOrder)
                       .Skip(dynamicWidgetSearchParameter.PagingParameter.PageSize * (dynamicWidgetSearchParameter.PagingParameter.PageIndex - 1))
                       .Take(dynamicWidgetSearchParameter.PagingParameter.PageIndex * dynamicWidgetSearchParameter.PagingParameter.PageSize).ToList();
                    }
                    else
                    {
                        dynamicWidgetRecords = nISEntitiesDataContext.DynamicWidgetRecords.Where(result).
                        OrderBy(dynamicWidgetSearchParameter.SortParameter.SortColumn + " " + dynamicWidgetSearchParameter.SortParameter.SortOrder).ToList();
                    }

                    if (dynamicWidgetRecords?.Count > 0)
                    {
                        dynamicWidgetRecords.ToList().ForEach(item =>
                        {
                            dynamicWidgets.Add(new DynamicWidget()
                            {
                                Identifier = item.Id,
                                WidgetName = item.WidgetName,
                                WidgetType = item.WidgetType,
                                PageTypeId = item.PageTypeId,
                                EntityId = item.EntityId,
                                Title = item.Title,
                                ThemeType = item.ThemeType,
                                ThemeCSS = item.ThemeCSS,
                                WidgetSettings = item.WidgetSettings,
                                WidgetFilterSettings = item.WidgetFilterSettings,
                                Status = item.Status,
                                CreatedBy = item.CreatedBy,
                                CreatedOn = item.CreatedOn,
                                LastUpdatedBy = item.LastUpdatedBy,
                                PublishedBy = item.PublishedBy,
                                PublishedDate = item.PublishedDate,
                                IsActive = true,
                                IsDeleted = false,
                                TenantCode = tenantCode
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

            return dynamicWidgets;
        }

        #endregion

        #region Get DynamicWidget Count

        /// <summary>
        /// This method reference to get dynamicWidget count
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>dynamicWidget count</returns>
        public int GetDynamicWidgetCount(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            int dynamicWidgetCount = 0;
            string whereClause = string.Empty;
            try
            {
                whereClause = this.WhereClauseGenerator(dynamicWidgetSearchParameter, tenantCode);
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    dynamicWidgetCount = nISEntitiesDataContext.DynamicWidgetRecords
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

            return dynamicWidgetCount;
        }


        #endregion

        public IList<TenantEntity> GetTenantEntities(string tenantCode)
        {
            IList<TenantEntity> tenantEntities = new List<TenantEntity>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<TenantEntityRecord> tenantEntityRecords = new List<TenantEntityRecord>();
                    tenantEntityRecords = nISEntitiesDataContext.TenantEntityRecords.Where(item => item.TenantCode == tenantCode).ToList();
                    tenantEntities = tenantEntityRecords.Select(item => new TenantEntity
                    {
                        Identifier = item.Id,
                        Name = item.Name,
                        Description = item.Name,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedOn,
                        LastUpdatedBy = item.LastUpdatedBy,
                        IsActive = item.IsActive,
                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tenantEntities;
        }

        public IList<EntityFieldMap> GetEntityFields(long entityIdentifier, string tenantCode)
        {
            IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<EntityFieldMapRecord> entityFieldRecords = new List<EntityFieldMapRecord>();
                    entityFieldRecords = nISEntitiesDataContext.EntityFieldMapRecords.Where(item => item.TenantCode == tenantCode && item.EntityId==entityIdentifier).ToList();
                    entityFieldMaps = entityFieldRecords.Select(item => new EntityFieldMap
                    {
                        Identifier = item.Id,
                        Name = item.Name,
                        EntityIdentifier = item.Id,
                        DataType = item.DataType,
                        IsDeleted = item.IsDeleted,
                        IsActive = item.IsActive,
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return entityFieldMaps;
        }

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

        #region Check Duplicate DynamicWidget

        /// <summary>
        /// Thismethod will be used to check duplicate dynamicWidget 
        /// </summary>
        /// <param name="dynamicWidgets">list of dynamicWidget object</param>
        /// <param name="tenantCode"> the tenant code</param>
        private void IsDuplicateDynamicWidget(IList<DynamicWidget> dynamicWidgets, string operation, string tenantCode)
        {
            try
            {
                //this.SetAndValidateConnectionString(tenantCode);

                //StringBuilder query = new StringBuilder();
                //if (operation.Equals(ModelConstant.ADD_OPERATION))
                //{
                //    query.Append("(" + string.Join(" or ", dynamicWidgets.Select(item => string.Format("Name.Equals(\"{0}\") or Code.Equals(\"{1}\") or DialingCode.Equals(\"{2}\")", item.WidgetName, item.Code, item.DialingCode)).ToList()) + ") and TenantCode.Equals(\"" + tenantCode + "\") and IsDeleted.Equals(false)");
                //}

                //if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                //{
                //    query.Append("(" + string.Join(" or ", dynamicWidgets.Select(item => string.Format("((Name.Equals(\"{0}\") or Code.Equals(\"{1}\") or DialingCode.Equals(\"{2}\")) and !Id.Equals({3}))", item.DynamicWidgetName, item.Code, item.DialingCode, item.Identifier))) + ") and TenantCode.Equals(\"" + tenantCode + "\") and IsDeleted.Equals(false)");
                //}

                //using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                //{
                //    IList<DynamicWidgetRecord> dynamicWidgetRecords = nISEntitiesDataContext.DynamicWidgetRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                //    if (dynamicWidgetRecords.Count > 0)
                //    {
                //        throw new DuplicateDynamicWidgetFoundException(tenantCode);
                //    }
                //}
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
        /// <param name="dynamicWidgetSearchParameter">query will be done on search Parameter</param>
        /// <param name="tenantCode">the tenant code </param>
        /// <returns></returns>
        private string WhereClauseGenerator(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();
            try
            {
                if (validationEngine.IsValidText(dynamicWidgetSearchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", dynamicWidgetSearchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }

                if (validationEngine.IsValidText(dynamicWidgetSearchParameter.DynamicWidgetName))
                {
                    if (dynamicWidgetSearchParameter.SearchMode == SearchMode.Equals)
                    {
                        queryString.Append(string.Format("WidgetName.Equals(\"{0}\") and ", dynamicWidgetSearchParameter.DynamicWidgetName));
                    }
                    else
                    {
                        queryString.Append(string.Format("WidgetName.Contains(\"{0}\") and ", dynamicWidgetSearchParameter.DynamicWidgetName));
                    }
                }

                if (this.validationEngine.IsValidDate(dynamicWidgetSearchParameter.StartDate) && !this.validationEngine.IsValidDate(dynamicWidgetSearchParameter.EndDate))
                {
                    DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(dynamicWidgetSearchParameter.StartDate), DateTimeKind.Utc);
                    queryString.Append("PublishedOn >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") and ");
                }
                if (this.validationEngine.IsValidDate(dynamicWidgetSearchParameter.EndDate) && !this.validationEngine.IsValidDate(dynamicWidgetSearchParameter.StartDate))
                {
                    DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(dynamicWidgetSearchParameter.EndDate), DateTimeKind.Utc);
                    queryString.Append("PublishedOn <= DateTime(" + toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
                }
                if (this.validationEngine.IsValidDate(dynamicWidgetSearchParameter.StartDate) && this.validationEngine.IsValidDate(dynamicWidgetSearchParameter.EndDate))
                {
                    DateTime fromDateTime = DateTime.SpecifyKind(Convert.ToDateTime(dynamicWidgetSearchParameter.StartDate), DateTimeKind.Utc);
                    DateTime toDateTime = DateTime.SpecifyKind(Convert.ToDateTime(dynamicWidgetSearchParameter.EndDate), DateTimeKind.Utc);

                    queryString.Append("PublishedOn >= DateTime(" + fromDateTime.Year + "," + fromDateTime.Month + "," + fromDateTime.Day + "," + fromDateTime.Hour + "," + fromDateTime.Minute + "," + fromDateTime.Second + ") " +
                                   "and PublishedOn <= DateTime(" + +toDateTime.Year + "," + toDateTime.Month + "," + toDateTime.Day + "," + toDateTime.Hour + "," + toDateTime.Minute + "," + toDateTime.Second + ") and ");
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
