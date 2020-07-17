// <copyright file="RenderEngineManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
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
    /// This class implements manager layer of render engine manager.
    /// </summary>
    public class RenderEngineManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The render engine repository.
        /// </summary>
        IRenderEngineRepository renderEngineRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for render engine manager, which initialise
        /// render engine repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public RenderEngineManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.renderEngineRepository = this.unityContainer.Resolve<IRenderEngineRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Add RenderEngine

        /// <summary>
        /// This method will call add render engine method of repository.
        /// </summary>
        /// <param name="renderEngines">RenderEngine are to be add.</param>
        /// <param name="tenantCode">Tenant code of render engine.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidRenderEngine(renderEngines, tenantCode);
                this.IsDuplicateRenderEngine(renderEngines, tenantCode);
                result = this.renderEngineRepository.AddRenderEngine(renderEngines, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Update RenderEngine

        /// <summary>
        /// This method reference helps to update details about render engines.
        /// </summary>
        /// <param name="renderEngines">
        /// The list of renderEngines.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if list of scene updates scus=ccessfully otherwise false
        /// </returns>
        public bool UpdateRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidRenderEngine(renderEngines, tenantCode);
                this.IsDuplicateRenderEngine(renderEngines, tenantCode);
                result = this.renderEngineRepository.UpdateRenderEngine(renderEngines, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Delete RenderEngine

        /// <summary>
        /// This method reference helps to delete details about render engine.
        /// </summary>
        /// <param name="renderEngines">
        /// The list of renderEngines.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        public bool DeleteRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.renderEngineRepository.DeleteRenderEngine(renderEngines, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region Get RenderEngine

        /// <summary>
        /// This method will call get render engines method of repository.
        /// </summary>
        /// <param name="renderEngineSearchParameter">The render engine search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<RenderEngine> GetRenderEngine(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode)
        {
            IList<RenderEngine> renderEngines = new List<RenderEngine>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    renderEngineSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                renderEngines = this.renderEngineRepository.GetRenderEngine(renderEngineSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return renderEngines;
        }

        /// <summary>
        /// This method will call get render engines method of repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public IList<RenderEngine> GetRenderEngine(string tenantCode)
        {
            IList<RenderEngine> renderEngines = new List<RenderEngine>();
            try
            {
                renderEngines = this.renderEngineRepository.GetRenderEngine(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return renderEngines;
        }

        #endregion

        #region Get RenderEngine Count
        /// <summary>
        /// This method helps to get count of render engine.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetRenderEngineCount(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.renderEngineRepository.GetRenderEngineCount(renderEngineSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }
        #endregion

        #region Activate

        /// <summary>
        /// This method helps to active render engine.
        /// </summary>
        /// <param name="renderEngineIdentifier"></param>
        /// <returns></returns>
        public bool ActivateRenderEngine(long renderEngineIdentifier, string tenantCode)
        {
            try
            {
                return this.renderEngineRepository.ActivateRenderEngine(renderEngineIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region DeActivate

        /// <summary>
        /// This mehod helps to deactive render engine.
        /// </summary>
        /// <param name="renderEngineIdentifier"></param>
        /// <returns></returns>
        public bool DeActivateRenderEngine(long renderEngineIdentifier, string tenantCode)
        {
            try
            {
                return this.renderEngineRepository.DeactivateRenderEngine(renderEngineIdentifier, tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is responsible for validate render engine.
        /// </summary>
        /// <param name="renderEngines"></param>
        /// <param name="tenantCode"></param>
        private void IsValidRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            try
            {
                if (renderEngines?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidRenderEngineException invalidRenderEngineException = new InvalidRenderEngineException(tenantCode);
                renderEngines.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidRenderEngineException.Data.Add(item.RenderEngineName, ex.Data);
                    }
                });

                if (invalidRenderEngineException.Data.Count > 0)
                {
                    throw invalidRenderEngineException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate render engine in the list
        /// </summary>
        /// <param name="renderEngines">renderEngines</param>
        /// <param name="tenantCode">tenant code</param>
        private void IsDuplicateRenderEngine(IList<RenderEngine> renderEngines, string tenantCode)
        {
            try
            {
                int isDuplicateRenderEngine = renderEngines.GroupBy(p => p.URL).Where(g => g.Count() > 1).Count();
                if (isDuplicateRenderEngine > 0)
                {
                    throw new DuplicateRenderEngineFoundException(tenantCode);
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