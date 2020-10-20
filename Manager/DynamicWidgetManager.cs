// <copyright file="DynamicWidgetManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of dynamicWidget manager.
    /// </summary>
    public class DynamicWidgetManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IDynamicWidgetRepository dynamicWidgetRepository = null;

        /// <summary>
        /// The tenant configuration manager object.
        /// </summary>
        private TenantConfigurationManager tenantConfigurationManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for dynamicWidget manager, which initialise
        /// dynamicWidget repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public DynamicWidgetManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.dynamicWidgetRepository = this.unityContainer.Resolve<IDynamicWidgetRepository>();
                this.tenantConfigurationManager = new TenantConfigurationManager(unityContainer);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add DynamicWidget

        /// <summary>
        /// This method will call add dynamicWidget method of repository.
        /// </summary>
        /// <param name="countries">DynamicWidget are to be add.</param>
        /// <param name="tenantCode">Tenant code of dynamicWidget.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddDynamicWidgets(IList<DynamicWidget> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidDynamicWidget(countries, tenantCode);
                this.IsDuplicateDynamicWidget(countries, tenantCode);
                result = this.dynamicWidgetRepository.AddDynamicWidgets(countries, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update DynamicWidget

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
        public bool UpdateDynamicWidgets(IList<DynamicWidget> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidDynamicWidget(countries, tenantCode);
                this.IsDuplicateDynamicWidget(countries, tenantCode);
                result = this.dynamicWidgetRepository.UpdateDynamicWidgets(countries, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete DynamicWidget

        /// <summary>
        /// This method reference helps to delete details about dynamicWidget.
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
        public bool DeleteDynamicWidgets(IList<DynamicWidget> countries, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.dynamicWidgetRepository.DeleteDynamicWidgets(countries, tenantCode);
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
        /// This method will call get countries method of repository.
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter">The dynamicWidget search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<DynamicWidget> GetDynamicWidgets(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            IList<DynamicWidget> countries = new List<DynamicWidget>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    dynamicWidgetSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                countries = this.dynamicWidgetRepository.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return countries;
        }

        #endregion

        public IList<TenantEntity> GetTenantEntities(string tenantCode)
        {
            IList<TenantEntity> tenantEntities = new List<TenantEntity>();
            try
            {

                tenantEntities = this.dynamicWidgetRepository.GetTenantEntities(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return tenantEntities;
        }


        public IList<EntityFieldMap> GetEntityFields(long entityIdentfier, string tenantCode)
        {
            IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
            try
            {

                entityFieldMaps = this.dynamicWidgetRepository.GetEntityFields(entityIdentfier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return entityFieldMaps;
        }

        #region Get DynamicWidget Count
        /// <summary>
        /// This method helps to get count of dynamicWidget.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetDynamicWidgetCount(DynamicWidgetSearchParameter dynamicWidgetSearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.dynamicWidgetRepository.GetDynamicWidgetCount(dynamicWidgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }
        #endregion

        public string PreviewDynamicWidget(long widgetIdentifier, string baseURL, string tenantCode)
        {
            StringBuilder htmlString = new StringBuilder();

            try
            {
                var tenantConfiguration = this.tenantConfigurationManager.GetTenantConfigurations(tenantCode)?.FirstOrDefault();
                DynamicWidgetSearchParameter pageSearchParameter = new DynamicWidgetSearchParameter
                {
                    Identifier = widgetIdentifier.ToString(),
                    SortParameter = new SortParameter()
                    {
                        SortOrder = SortOrder.Ascending,
                        SortColumn = ModelConstant.SORT_COLUMN,
                    },
                };
                IList<DynamicWidget> dynamicWidgets = this.dynamicWidgetRepository.GetDynamicWidgets(pageSearchParameter, tenantCode);
                if (dynamicWidgets.Count != 0)
                {
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_HEADER);
                    for (int y = 0; y < dynamicWidgets.Count; y++)
                    {


                    }
                    htmlString.Append(HtmlConstants.CONTAINER_DIV_HTML_FOOTER);
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlString.ToString();
        }

        /// <summary>
        /// This method will call publish page method of repository
        /// </summary>
        /// <param name="pageIdentifier">Page identifier</param>
        /// <param name="tenantCode">Tenant code of page.</param>
        /// <returns>
        /// Returns true if pages publish successfully, false otherwise.
        /// </returns>
        public bool PublishDynamicWidget(long widgetIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.dynamicWidgetRepository.PublishDynamicWidget(widgetIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate dynamicWidget.
        /// </summary>
        /// <param name="countries"></param>
        /// <param name="tenantCode"></param>
        private void IsValidDynamicWidget(IList<DynamicWidget> countries, string tenantCode)
        {
            try
            {
                if (countries?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidDynamicWidgetException invalidDynamicWidgetException = new InvalidDynamicWidgetException(tenantCode);
                countries.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidDynamicWidgetException.Data.Add(item.WidgetName, ex.Data);
                    }
                });

                if (invalidDynamicWidgetException.Data.Count > 0)
                {
                    throw invalidDynamicWidgetException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate dynamicWidget in the list
        /// </summary>
        /// <param name="countries">countries</param>
        /// <param name="tenantCode">tenant code</param>
        private void IsDuplicateDynamicWidget(IList<DynamicWidget> countries, string tenantCode)
        {
            try
            {
                int isDuplicateDynamicWidget = countries.GroupBy(p => p.WidgetName).Where(g => g.Count() > 1).Count();
                if (isDuplicateDynamicWidget > 0)
                {
                    throw new DuplicateDynamicWidgetFoundException(tenantCode);
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