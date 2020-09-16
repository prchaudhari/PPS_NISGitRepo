// <copyright file="IStateRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Represents interface that defines methods to access state repository.
    /// </summary>
    public interface IStateRepository
    {
        /// <summary>
        /// This method helps to adds the specified list of state in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are added successfully, else false.
        /// </returns>
        bool AddStates(IList<State> states, string tenantCode);

        /// <summary>
        /// This method helps to update the specified list of states in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are updated successfully, else false.
        /// </returns>
        bool UpdateStates(IList<State> states, string tenantCode);

        /// <summary>
        /// This method helps to delete the specified list of states in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are deleted successfully, else false.
        /// </returns>        
        bool DeleteStates(IList<State> states, string tenantCode);

        /// <summary>
        /// This method helps to retrieve states based on specified search condition from repository.
        /// </summary>
        /// <param name="stateSearchParameter">The state search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of states based on given search criteria
        /// </returns>
        IList<State> GetStates(StateSearchParameter stateSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get state count
        /// </summary>
        /// <param name="stateSearchParameter">The state search parameter</param>
        /// <param name="tenantCode"></param>
        /// <returns>Certifications count</returns>
        int GetStatesCount(StateSearchParameter stateSearchParameter, string tenantCode);


        /// <summary>
        /// This method helps to activate the states
        /// </summary>
        /// <param name="stateIdentifier">The state identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if state activated successfully false otherwise</returns>
        bool ActivateState(long stateIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the states
        /// </summary>
        /// <param name="stateIdentifier">The state identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if state deactivated successfully false otherwise</returns>
        bool DeactivateState(long stateIdentifier, string tenantCode);
    }
}
