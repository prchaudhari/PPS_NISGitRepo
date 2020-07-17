// <copyright file="IRenderEngineRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Represents interface that defines methods to access render engine repository.
    /// </summary>
    public interface IRenderEngineRepository
    {
        /// <summary>
        /// This method helps to adds the specified list of render engine in render engine repository.
        /// </summary>
        /// <param name="renderEngines">The list of render engine</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if render engine are added successfully, else false.
        /// </returns>
        bool AddRenderEngine(IList<RenderEngine> renderEngines, string tenantCode);

        /// <summary>
        /// This method helps to update the specified list of render engine in render engine repository.
        /// </summary>
        /// <param name="renderEngines">The list of render engine</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if render engine are updated successfully, else false.
        /// </returns>
        bool UpdateRenderEngine(IList<RenderEngine> renderEngines, string tenantCode);

        /// <summary>
        /// Updates the render engine from job.
        /// </summary>
        /// <param name="renderEngines">The render engines.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool UpdateRenderEngineFromJob(IList<RenderEngine> renderEngines, string tenantCode);

        /// <summary>
        /// This method helps to delete the specified list of render engine in render engine repository.
        /// </summary>
        /// <param name="renderEngines">The list of render engine</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if render engine are deleted successfully, else false.
        /// </returns>        
        bool DeleteRenderEngine(IList<RenderEngine> renderEngines, string tenantCode);

        /// <summary>
        /// This method helps to retrieve render engine based on specified search condition from repository.
        /// </summary>
        /// <param name="renderEngineSearchParameter">The render engine search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of render engine based on given search criteria
        /// </returns>
        IList<RenderEngine> GetRenderEngine(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to activate the render engines
        /// </summary>
        /// <param name="renderEngineIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer activated successfully false otherwise</returns>
        bool ActivateRenderEngine(long renderEngineIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the render engines
        /// </summary>
        /// <param name="renderEngineIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer deactivated successfully false otherwise</returns>
        bool DeactivateRenderEngine(long renderEngineIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to get render engine count
        /// </summary>
        /// <param name="renderEngineSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>render engine count</returns>
        int GetRenderEngineCount(RenderEngineSearchParameter renderEngineSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to retrieve render engine from repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of render engine
        /// </returns>
        IList<RenderEngine> GetRenderEngine(string tenantCode);
    }
}
