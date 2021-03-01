// <copyright file="SQLWidgetRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for widget entity.
    /// </summary>
    /// 
    public class SQLWidgetRepository : IWidgetRepository
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

        public SQLWidgetRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new ValidationEngine();
            this.utility = new Utility();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method adds the specified list of widget in the repository.
        /// </summary>
        /// <param name="widgets"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the widget values are added successfully, false otherwise
        /// </returns>
        public bool AddWidgets(IList<Widget> widgets, string tenantCode)
        {
            bool result = false;
            try
            {

            }

            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update already added widgets entry to database.
        /// </summary>
        /// <param name="widgets"></param>
        /// <param name="tenantCode"></param>
        /// <returns>
        /// True, if the widget values are updated successfully,otherwise false
        /// </returns>
        public bool UpdateWidgets(IList<Widget> widgets, string tenantCode)
        {
            bool result = false;
            try
            {
            }

            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Delete widgets from database
        /// </summary>
        /// <param name="widgets"></param>
        /// <param name="tenantCode"></param>
        /// <returns>True, if the widget values are deleted successfully(soft delete), 
        /// otherwise false</returns>
        public bool DeleteWidgets(IList<Widget> widgets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method used to get the rolse based on search paramter.
        /// </summary>
        /// <param name="widgetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>List of widgets</returns>
        public IList<Widget> GetWidgets(WidgetSearchParameter widgetSearchParameter, string tenantCode)
        {
            IList<Widget> widgets = new List<Widget>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    if (validationEngine.IsValidText(widgetSearchParameter.PageTypeId))
                    {
                        IList<WidgetPageTypeMap> widgetPageTypeMaps = new List<WidgetPageTypeMap>();
                        widgetPageTypeMaps = nISEntitiesDataContext.WidgetPageTypeMaps.Where(item => item.PageTypeId.ToString() == widgetSearchParameter.PageTypeId
                         && item.IsDynamicWidget == false && item.TenantCode == tenantCode).ToList();
                        if (widgetPageTypeMaps.Count > 0)
                        {
                            widgetSearchParameter.Identifier = string.Join(",", widgetPageTypeMaps.Select(item => item.WidgetId).ToList());
                            //return widgets;
                        }
                    }
                    string whereClause = this.WhereClauseGenerator(widgetSearchParameter, tenantCode);
                    IList<WidgetRecord> widgetRecords = new List<WidgetRecord>();
                    if (widgetSearchParameter.PagingParameter.PageIndex > 0 && widgetSearchParameter.PagingParameter.PageSize > 0)
                    {
                        widgetRecords = nISEntitiesDataContext.WidgetRecords
                        .OrderBy(widgetSearchParameter.SortParameter.SortColumn + " " + widgetSearchParameter.SortParameter.SortOrder.ToString())
                        .Where(whereClause)
                        .Skip((widgetSearchParameter.PagingParameter.PageIndex - 1) * widgetSearchParameter.PagingParameter.PageSize)
                        .Take(widgetSearchParameter.PagingParameter.PageSize)
                        .ToList();
                    }
                    else
                    {
                        widgetRecords = nISEntitiesDataContext.WidgetRecords
                        .Where(whereClause)
                        .OrderBy(widgetSearchParameter.SortParameter.SortColumn + " " + widgetSearchParameter.SortParameter.SortOrder.ToString().ToLower())
                        .ToList();
                    }

                    if (widgetRecords != null && widgetRecords.Count > 0)
                    {

                        widgets = widgetRecords.Select(widgetRecord => new Widget()
                        {
                            Identifier = widgetRecord.Id,
                            WidgetName = widgetRecord.WidgetName,
                            WidgetDescription = widgetRecord.Description,
                            DisplayName = widgetRecord.DisplayName,
                            IsActive = widgetRecord.IsActive,
                            IsConfigurable = widgetRecord.IsConfigurable,
                            Instantiable = widgetRecord.Instantiable,
                            LastUpdatedDate = widgetRecord.LastUpdatedDate,
                            PageTypeIds = widgetRecord.PageTypeId,
                            UpdatedBy = new User { Identifier = (long)widgetRecord.UpdateBy }
                        }).ToList();

                        if (widgetSearchParameter.IsLastUpdatedByDeatilReauired != null && widgetSearchParameter.IsLastUpdatedByDeatilReauired == true)
                        {
                            StringBuilder userIdentifier = new StringBuilder();
                            userIdentifier.Append("(" + string.Join(" or ", widgetRecords.Select(item => string.Format("Id.Equals({0})", item.UpdateBy))) + ")");
                            List<UserRecord> userRecords = new List<UserRecord>();
                            userRecords = nISEntitiesDataContext.UserRecords.Where(userIdentifier.ToString()).ToList();
                            widgets.ToList().ForEach(widget =>
                            {
                                UserRecord record = userRecords.Where(item => item.Id == widget.UpdatedBy.Identifier)?.ToList().FirstOrDefault();
                                widget.UpdatedBy = new User
                                {
                                    FirstName = record.FirstName,
                                    LastName = record.LastName,
                                };
                            });
                        }

                        if (widgetSearchParameter.IsPageTypeDetailsRequired != null && widgetSearchParameter.IsPageTypeDetailsRequired == true)
                        {
                            StringBuilder queryString = new StringBuilder();
                            queryString.Append("(" + string.Join("or ", widgets.Select(item => string.Format("WidgetId.Equals({0}) ", item.Identifier))) + ")");
                            queryString.Append(" and IsDynamicWidget.Equals(false)");
                            queryString.Append(string.Format("and TenantCode.Equals(\"{0}\") ", tenantCode));
                            List<WidgetPageTypeMap> pageWidgetMapRecords = new List<WidgetPageTypeMap>();
                            pageWidgetMapRecords = nISEntitiesDataContext.WidgetPageTypeMaps.Where(queryString.ToString()).ToList();

                            List<PageTypeRecord> pageTypeRecords = new List<PageTypeRecord>();
                            queryString = new StringBuilder();
                            queryString.Append("(" + string.Join("or ", pageWidgetMapRecords.Select(item => string.Format("Id.Equals({0}) ", item.PageTypeId))) + ")");
                            queryString.Append(" and IsDeleted.Equals(false)");
                            queryString.Append(string.Format("and TenantCode.Equals(\"{0}\") ", tenantCode));
                            pageTypeRecords = nISEntitiesDataContext.PageTypeRecords.Where(queryString.ToString()).ToList();

                            widgets.ToList().ForEach(item =>
                            {
                                IList<PageType> pageTypes = new List<PageType>();

                                IList<WidgetPageTypeMap> currentWidgetPageTypeMaps = pageWidgetMapRecords.Where(map => map.WidgetId == item.Identifier).ToList();
                                currentWidgetPageTypeMaps.ToList().ForEach(map =>
                                {
                                    pageTypes.Add(new PageType
                                    {
                                        Identifier = map.PageTypeId,
                                        PageTypeName = pageTypeRecords.Where(pg => pg.Id == map.PageTypeId).FirstOrDefault().Name,
                                    });
                                });
                                item.PageTypes = pageTypes;
                                item.PageTypeNames = string.Join(",", pageTypes.Select(pg => pg.PageTypeName));
                            });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return widgets;
        }

        /// <summary>
        /// This method helps to get count of widgets.
        /// </summary>
        /// <param name="widgetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public int GetWidgetCount(WidgetSearchParameter widgetSearchParameter, string tenantCode)
        {
            int widgetCount = 0;
            string whereClause = this.WhereClauseGenerator(widgetSearchParameter, tenantCode);
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    widgetCount = nISEntitiesDataContext.WidgetRecords.Where(whereClause.ToString()).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return widgetCount;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Generate string for dynamic linq.
        /// </summary>
        /// <param name="searchParameter">Widget search Parameters</param>
        /// <returns>
        /// Returns a string.
        /// </returns>
        private string WhereClauseGenerator(WidgetSearchParameter searchParameter, string tenantCode)
        {
            StringBuilder queryString = new StringBuilder();

            if (searchParameter.SearchMode == SearchMode.Equals)
            {
                if (validationEngine.IsValidText(searchParameter.Identifier))
                {
                    queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
                }
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Equals(\"{0}\") and ", searchParameter.Name));
                }
            }
            if (searchParameter.SearchMode == SearchMode.Contains)
            {
                if (validationEngine.IsValidText(searchParameter.Name))
                {
                    queryString.Append(string.Format("Name.Contains(\"{0}\") and ", searchParameter.Name));
                }
            }
            if (validationEngine.IsValidText(searchParameter.Identifier))
            {
                queryString.Append("(" + string.Join("or ", searchParameter.Identifier.ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") and ");
            }
            if (searchParameter.IsActive == null || searchParameter.IsActive == true)
            {
                queryString.Append(string.Format("IsDeleted.Equals(false) and "));
            }
            else if (searchParameter.IsActive != null && searchParameter.IsActive == false)
            {
                queryString.Append(string.Format("IsDeleted.Equals(true) and "));
            }

            queryString.Append(string.Format("TenantCode.Equals(\"{0}\") ", tenantCode));
            return queryString.ToString();
        }

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="widgets">The widgets to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        private bool IsDuplicateWidget(IList<Widget> widgets, string operation, string tenantCode)
        {
            bool result = false;
            try
            {
                this.SetAndValidateConnectionString(tenantCode);

                StringBuilder query = new StringBuilder();

                if (operation.Equals(ModelConstant.ADD_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", widgets.Select(item => string.Format("Name.Equals(\"{0}\")", item.WidgetName)).ToList()) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                if (operation.Equals(ModelConstant.UPDATE_OPERATION))
                {
                    query.Append("(" + string.Join(" or ", widgets.Select(item => string.Format("(Name.Equals(\"{0}\") and !Id.Equals({1}))", item.WidgetName, item.Identifier))) + ") and IsDeleted.Equals(false) and TenantCode.Equals(\"" + tenantCode + "\")");
                }

                using (NISEntities nISEntitiesDataContext = new NISEntities(this.connectionString))
                {
                    IList<WidgetRecord> widgetRecords = nISEntitiesDataContext.WidgetRecords.Where(query.ToString()).Select(item => item).AsQueryable().ToList();
                    if (widgetRecords.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

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
