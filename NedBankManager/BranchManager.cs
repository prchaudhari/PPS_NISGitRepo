// <copyright file="RoleManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedBankManager
{
    #region References

    using NedbankModel;
    using NedbankRepository;
    using System;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of branch manager.
    /// </summary>
    public class BranchManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The branch repository.
        /// </summary>
        IBranchRepository branchRepository = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public BranchManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.branchRepository = this.unityContainer.Resolve<IBranchRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Get

        /// <summary>
        /// This method helps to get specified branch.
        /// </summary>
        /// <param name="branchId">The investor id</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns a list of branch if any found otherwise it will return enpty list.
        /// </returns>
        public BranchInformation GetbranchsByBranchId(long branchId, string tenantCode)
        {
            try
            {
                BranchInformation branch = this.branchRepository.GetBranchById(branchId, tenantCode);

                return branch;
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
