// <copyright file="RenderEngineController.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Unity;
    #endregion

    /// <summary>
    /// This class represent api controller for render engine
    /// </summary>
    [EnableCors("*", "*", "*", "*")]
    [RoutePrefix("RenderEngine")]
    public class RenderEngineController : ApiController
    {

        #region Private Members

        /// <summary>
        /// The render engine manager object.
        /// </summary>
        private RenderEngineManager renderEngineManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        public RenderEngineController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.renderEngineManager = new RenderEngineManager(this.unityContainer);
        }

        #endregion

        #region Public Method

        #region Add Render Engine

        /// <summary>
        /// This method helps to add render engine
        /// </summary>
        /// <param name="renderEngines"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Add(IList<RenderEngine> renderEngines)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.renderEngineManager.AddRenderEngine(renderEngines, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update Render Engine

        /// <summary>
        /// This method helps to update render engine.
        /// </summary>
        /// <param name="renderEngines"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Update(IList<RenderEngine> renderEngines)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.renderEngineManager.UpdateRenderEngine(renderEngines, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion
            
        #region Delete Render Engine

        /// <summary>
        /// This method helps to delete render engine.
        /// </summary>
        /// <param name="renderEngines"></param>
        /// <returns>boolean value</returns>
        [HttpPost]
        public bool Delete(IList<RenderEngine> renderEngines)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.renderEngineManager.DeleteRenderEngine(renderEngines, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Get Render Engines

        /// <summary>
        /// This method helps to get render engine list based on the search parameters.
        /// </summary>
        /// <param name="renderEngineSearchParameter"></param>
        /// <returns>List of renderEngines</returns>
        [HttpPost]
        public IList<RenderEngine> List(RenderEngineSearchParameter renderEngineSearchParameter)
        {
            IList<RenderEngine> renderEngines = new List<RenderEngine>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                renderEngines = this.renderEngineManager.GetRenderEngine(renderEngineSearchParameter, tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", this.renderEngineManager.GetRenderEngineCount(renderEngineSearchParameter, tenantCode).ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return renderEngines;
        }

        /// <summary>
        /// This method helps to get render engine list
        /// </summary>
        /// <returns>List of renderEngines</returns>
        [HttpGet]
        public IList<RenderEngine> List()
        {
            IList<RenderEngine> renderEngines = new List<RenderEngine>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                renderEngines = this.renderEngineManager.GetRenderEngine(tenantCode);
                HttpContext.Current.Response.AppendHeader("recordCount", renderEngines.Count.ToString());
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return renderEngines;
        }

        #endregion

        #region Activate

        /// <summary>
        /// This method used to activate render engine
        /// </summary>
        /// <param name="renderEngineIdentifier">The render engine identifier</param>
        /// <returns>Returns true if activated successfully otherwise false</returns>
        [HttpGet]
        public bool Activate(long renderEngineIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.renderEngineManager.ActivateRenderEngine(renderEngineIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This method used to deactivate render engine
        /// </summary>
        /// <param name="renderEngineIdentifier">The render engine identifier</param>
        /// <returns>Returns true if deactivated successfully otherwise false</returns>
        [HttpGet]
        public bool DeActivate(long renderEngineIdentifier)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.renderEngineManager.DeActivateRenderEngine(renderEngineIdentifier, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion
        #endregion
    }
}