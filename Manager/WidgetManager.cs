// <copyright file="WidgetManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of widget manager.
    /// </summary>
    public class WidgetManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The widget repository.
        /// </summary>
        IWidgetRepository widgetRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for widget manager, which initialise
        /// widget repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public WidgetManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.widgetRepository = this.unityContainer.Resolve<IWidgetRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will call add widgets method of repository.
        /// </summary>
        /// <param name="widgets">Widgets are to be add.</param>
        /// <param name="tenantCode">Tenant code of widget.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddWidgets(IList<Widget> widgets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidWidgets(widgets, tenantCode);
                this.IsDuplicateWidget(widgets, tenantCode);
                result = this.widgetRepository.AddWidgets(widgets, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update widgets method of repository
        /// </summary>
        /// <param name="widgets">Widgets are to be update.</param>
        /// <param name="tenantCode">Tenant code of widget.</param>
        /// <returns>
        /// Returns true if widgets updated successfully, false otherwise.
        /// </returns>
        public bool UpdateWidgets(IList<Widget> widgets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidWidgets(widgets, tenantCode);
                this.IsDuplicateWidget(widgets, tenantCode);
                result = this.widgetRepository.UpdateWidgets(widgets, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete widgets method of repository
        /// </summary>
        /// <param name="widgets">Widgets are to be delete.</param>
        /// <param name="tenantCode">Tenant code of widget.</param>
        /// <returns>
        /// Returns true if widgets deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteWidgets(IList<Widget> widgets, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.widgetRepository.DeleteWidgets(widgets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get widgets method of repository.
        /// </summary>
        /// <param name="widgetSearchParameter">The widget search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns widgets if found for given parameters, else return null
        /// </returns>
        public IList<Widget> GetWidgets(WidgetSearchParameter widgetSearchParameter, string tenantCode)
        {
            IList<Widget> widgets = new List<Widget>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    widgetSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                widgets = this.widgetRepository.GetWidgets(widgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return widgets;
        }

        /// <summary>
        /// This method helps to get count of widgets.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of widgets
        /// </returns>
        public int GetWidgetCount(WidgetSearchParameter widgetSearchParameter, string tenantCode)
        {
            int widgetCount = 0;
            try
            {
                widgetCount = this.widgetRepository.GetWidgetCount(widgetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return widgetCount;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate widgets.
        /// </summary>
        /// <param name="widgets"></param>
        /// <param name="tenantCode"></param>
        private void IsValidWidgets(IList<Widget> widgets, string tenantCode)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate widget in the list
        /// </summary>
        /// <param name="widgets"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateWidget(IList<Widget> widgets, string tenantCode)
        {
            try
            {

            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        #endregion
    }
}
