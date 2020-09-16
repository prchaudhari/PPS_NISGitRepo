// <copyright file="StateManager .cs" company="Websym Solutions Pvt. Ltd.">
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
    /// This class represents the manager layer for state.
    /// </summary>
    public class StateManager
    {
        #region Private Members

        /// <summary>
        /// The state repository
        /// </summary>
        private IStateRepository stateRepository = null;

        #endregion

        #region Constructor

        public StateManager(IUnityContainer unityContainer)
        {
            this.stateRepository = unityContainer.Resolve<IStateRepository>();
        }

        #endregion

        #region public Methods

        #region Add State

        /// <summary>
        /// This method helps to adds the specified list of state in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are added successfully, else false.
        /// </returns>
        public bool AddStates(IList<State> states, string tenantCode)
        {
            bool result = false;
            try
            {

                this.IsDuplicateState(states, tenantCode);
                this.IsValidStates(states, tenantCode);
                result = this.stateRepository.AddStates(states, tenantCode);

            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }


        #endregion

        #region Update State

        /// <summary>
        /// This method helps to update the specified list of states in state repository.
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are updated successfully, else false.
        /// </returns>
        public bool UpdateStates(IList<State> states, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsDuplicateState(states, tenantCode);
                this.IsValidStates(states, tenantCode);
                result = this.stateRepository.UpdateStates(states, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Delete State

        /// <summary>
        /// This method helps to delete the specified list of states .
        /// </summary>
        /// <param name="states">The list of states</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if states are deleted successfully, else false.
        /// </returns>        
        public bool DeleteStates(IList<State> states, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsDuplicateState(states, tenantCode);
                this.IsValidStates(states, tenantCode);
                result = this.stateRepository.DeleteStates(states, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Get State

        /// <summary>
        /// This method helps to retrieve states based on specified search condition from repository.
        /// </summary>
        /// <param name="stateSearchParameter">The state search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of states based on given search criteria
        /// </returns>
        public IList<State> GetStates(StateSearchParameter stateSearchParameter, string tenantCode)
        {
            IList<State> Countries = new List<State>();
            try
            {
                //InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                //try
                //{
                //    stateSearchParameter.IsValid();
                //}
                //catch (Exception exception)
                //{
                //    invalidSearchParameterException.Data.Add(ModelConstant.INVALID_PAGING_PARAMETER, exception.Data);
                //}

                //if (invalidSearchParameterException.Data.Count > 0)
                //{
                //    throw invalidSearchParameterException;
                //}

                Countries = this.stateRepository.GetStates(stateSearchParameter, tenantCode);

            }
            catch (Exception exception)
            {
                throw exception;
            }

            return Countries;
        }

        #endregion

        #region Get State Count

        /// <summary>
        /// This method reference to get state count
        /// </summary>
        /// <param name="stateSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Certifications count</returns>
        public int GetStatesCount(StateSearchParameter stateSearchParameter, string tenantCode)
        {
            int stateCount = 0;
            try
            {
                stateCount = this.stateRepository.GetStatesCount(stateSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return stateCount;
        }

        #endregion

        #region Activate State

        /// <summary>
        /// This method helps to activate the states
        /// </summary>
        /// <param name="stateIdentifier">The state identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if state activated successfully false otherwise</returns>
        public bool ActivateState(long stateIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.stateRepository.ActivateState(stateIdentifier, tenantCode); ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #region DeActivate State

        /// <summary>
        /// This method helps to deactivate the states
        /// </summary>
        /// <param name="stateIdentifier">The state identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if state deactivated successfully false otherwise</returns>
        public bool DeactivateState(long stateIdentifier, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.stateRepository.DeactivateState(stateIdentifier, tenantCode); ;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        #endregion

        #endregion

        #region Private Methods

        #region Validate States

        /// <summary>
        /// This method will be used to validate state.
        /// </summary>
        /// <param name=" states">The list of states</param>        
        private void IsValidStates(IList<State> states, string tenantCode)
        {
            try
            {
                if (states?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidStateException invalidStateException = new InvalidStateException(tenantCode);
                states.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidStateException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidStateException.Data.Count > 0)
                {
                    throw invalidStateException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Check Duplicate States

        /// <summary>
        /// This method will be used to check duplicate states in the list
        /// </summary>
        /// <param name="states">The list of states</param>        
        private void IsDuplicateState(IList<State> states, string tenantCode)
        {
            try
            {
                int duplicateStateCount = states.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (duplicateStateCount > 0)
                {
                    throw new DuplicateStateFoundException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #endregion


    }
}
