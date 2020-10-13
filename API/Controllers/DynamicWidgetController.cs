// <copyright file="DynamicWidgetController.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represent api controller for dynamicWidget
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("DynamicWidget")]
    public class DynamicWidgetController : ApiController
    {

        #region Private Members

        /// <summary>
        /// The dynamicWidget manager object.
        /// </summary>
        private DynamicWidgetManager dynamicWidgetManager = null;

        #endregion

        #region Constructor

        public DynamicWidgetController(IUnityContainer unityContainer)
        {
            this.dynamicWidgetManager = new DynamicWidgetManager(unityContainer);
        }

        #endregion

        #region Public Method

        #region Add DynamicWidget

        /// <summary>
        /// This method helps to add dynamicWidget
        /// </summary>
        /// <param name="countries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<DynamicWidget> countries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.dynamicWidgetManager.AddDynamicWidgets(countries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update DynamicWidget

        /// <summary>
        /// This method helps to update dynamicWidget.
        /// </summary>
        /// <param name="countries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<DynamicWidget> countries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.dynamicWidgetManager.UpdateDynamicWidgets(countries, tenantCode);
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
        /// This method helps to delete countries.
        /// </summary>
        /// <param name="countries"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<DynamicWidget> countries)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.dynamicWidgetManager.DeleteDynamicWidgets(countries, tenantCode);
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
        /// This method helps to get dynamicWidget list based on the search parameters.
        /// </summary>
        /// <param name="dynamicWidgetSearchParameter"></param>
        /// <returns>List of countries</returns>
        [HttpPost]
        public IList<DynamicWidget> List(DynamicWidgetSearchParameter dynamicWidgetSearchParameter)
        {
            IList<DynamicWidget> countries = new List<DynamicWidget>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                countries = this.dynamicWidgetManager.GetDynamicWidgets(dynamicWidgetSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.dynamicWidgetManager.GetDynamicWidgetCount(dynamicWidgetSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return countries;
        }

        [HttpPost]
        public IList<TenantEntity> GetTenantEntities()
        {
            IList<TenantEntity> tenantEntities = new List<TenantEntity>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                tenantEntities = this.dynamicWidgetManager.GetTenantEntities(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return tenantEntities;
        }

        [HttpPost]
        public IList<EntityFieldMap> GetEntityFields(long entityIdentfier)
        {
            IList<EntityFieldMap> entityFieldMaps = new List<EntityFieldMap>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                entityFieldMaps = this.dynamicWidgetManager.GetEntityFields(entityIdentfier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return entityFieldMaps;
        }

        #endregion
        #endregion
    }
}