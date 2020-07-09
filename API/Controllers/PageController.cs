// <copyright file="PageController.cs" company="Websym Solutions Pvt Ltd">
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
    using Unity;
    #endregion

    /// <summary>
    /// This class represent api controller for page
    /// </summary>
    public class PageController: ApiController
    {
        #region Private Members

        /// <summary>
        /// The page manager object.
        /// </summary>
        private PageManager pageManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public PageController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.pageManager = new PageManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to add pages
        /// </summary>
        /// <param name="pages"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<Page> pages)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageManager.AddPages(pages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update pages.
        /// </summary>
        /// <param name="pages"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<Page> pages)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageManager.UpdatePages(pages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete pages.
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(long pageIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageManager.DeletePages(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get roles list based on the search parameters.
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <returns>List of roles</returns>
        [HttpPost]
        public IList<Page> List(PageSearchParameter pageSearchParameter)
        {
            IList<Page> pages = new List<Page>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                pages = this.pageManager.GetPages(pageSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.pageManager.GetPageCount(pageSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return pages;
        }

        /// <summary>
        /// This method helps to publish page.
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Publish(long pageIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageManager.PublishPage(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to preview page.
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public string Preview(long pageIdentifier)
        {
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                return this.pageManager.PreviewPage(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method helps to clone page.
        /// </summary>
        /// <param name="pageIdentifier"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Clone(long pageIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.pageManager.ClonePage(pageIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion
    }
}