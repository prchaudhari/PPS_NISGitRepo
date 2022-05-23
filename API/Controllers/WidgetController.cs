// <copyright file="WidgetController.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represent api controller for widget
    /// </summary>
    public class WidgetController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The widget manager object.
        /// </summary>
        private WidgetManager widgetManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public WidgetController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.widgetManager = new WidgetManager(this.unityContainer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to add widgets
        /// </summary>
        /// <param name="widgets"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<Widget> widgets)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.widgetManager.AddWidgets(widgets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to update widgets.
        /// </summary>
        /// <param name="widgets"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<Widget> widgets)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.widgetManager.UpdateWidgets(widgets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to delete widgets.
        /// </summary>
        /// <param name="widgets"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<Widget> widgets)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.widgetManager.DeleteWidgets(widgets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// This method helps to get widgets list based on the search parameters.
        /// </summary>
        /// <param name="widgetSearchParameter"></param>
        /// <returns>List of widgets</returns>
        [HttpPost]
        public IList<Widget> List(WidgetSearchParameter widgetSearchParameter)
        {
            IList<Widget> widgets = new List<Widget>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                widgets = this.widgetManager.GetWidgets(widgetSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.widgetManager.GetWidgetCount(widgetSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception; 
            }

            return widgets;
        }

        /// <summary>
        /// This method helps to get widget based on given identifier.
        /// </summary>
        /// <param name="widgetSearchParameter"></param>
        /// <returns>widget record</returns>
        [HttpGet]
        public Widget Detail(long widgetIdentifier)
        {
            IList<Widget> widgets = new List<Widget>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                WidgetSearchParameter widgetSearchParameter = new WidgetSearchParameter();
                widgetSearchParameter.Identifier = widgetIdentifier.ToString();
                widgetSearchParameter.SortParameter.SortColumn = "Id";
                widgets = this.widgetManager.GetWidgets(widgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return widgets.First();
        }

        #endregion

    }
}
