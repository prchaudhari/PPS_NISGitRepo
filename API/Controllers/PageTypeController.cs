// <copyright file="PageTypeController.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represent api controller for pageType
    /// </summary>
    
    public class PageTypeController : ApiController
    {

        #region Private Members

        /// <summary>
        /// The pageType manager object.
        /// </summary>
        private PageTypeManager pageTypeManager = null;

        #endregion

        #region Constructor

        public PageTypeController(IUnityContainer unityContainer)
        {
            this.pageTypeManager = new PageTypeManager(unityContainer);
        }

        #endregion

        #region Public Method

        #region Add PageType

        /// <summary>
        /// This method helps to add pageType
        /// </summary>
        /// <param name="pageTypes"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<PageType> pageTypes)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageTypeManager.AddPageTypes(pageTypes, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update PageType

        /// <summary>
        /// This method helps to update pageType.
        /// </summary>
        /// <param name="pageTypes"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<PageType> pageTypes)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageTypeManager.UpdatePageTypes(pageTypes, tenantCode);
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
        /// This method helps to delete pageTypes.
        /// </summary>
        /// <param name="pageTypes"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<PageType> pageTypes)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageTypeManager.DeletePageTypes(pageTypes, tenantCode);
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
        /// This method helps to get pageType list based on the search parameters.
        /// </summary>
        /// <param name="pageTypeSearchParameter"></param>
        /// <returns>List of pageTypes</returns>
        [HttpPost]
        public IList<PageType> GetPageTypeList(PageTypeSearchParameter pageTypeSearchParameter)
        {
            IList<PageType> pageTypes = new List<PageType>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                pageTypes = this.pageTypeManager.GetPageTypes(pageTypeSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.pageTypeManager.GetPageTypeCount(pageTypeSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return pageTypes;
        }

        #endregion
        #endregion
    }
}