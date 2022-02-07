// <copyright file="IBranchRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace NedbankRepository
{
    #region References

    using NedbankModel;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// This interface represents reference to access accet library repository.
    /// </summary>
    public interface IBranchRepository
    {
        #region Customer Data
        /// <summary>
        /// Gets the branch by branch id.
        /// </summary>
        /// <param name="branchId">The branch identifier.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        BranchInformation GetBranchById(long branchId, string tenantCode);
        #endregion

    }
}

