// <copyright file="ITenantUserRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface ITenantUserRepository
    {
        /// <summary>
        /// This method adds the specified list of users in user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are added successfully, else false.
        /// </returns>
        bool AddTenantUsers(IList<TenantUser> users, string tenantCode);

        /// <summary>
        /// This method updates the specified list of users in user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are updated successfully, else false.
        /// </returns>
        bool UpdateTenantUsers(IList<TenantUser> users, string tenantCode);


        /// <summary>
        /// This method deletes the specified list of users from user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are deleted successfully, else false.
        /// </returns>
        bool DeleteTenantUsers(IList<TenantUser> users, string tenantCode);

        /// <summary>
        /// This method gets the specified list of users from user repository.
        /// </summary>
        /// <param name="userSearchParameter">The user search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of TenantUsers
        /// </returns>
        IList<TenantUser> GetTenantUsers(TenantUserSearchParameter userSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to activate the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer activated successfully false otherwise</returns>
        bool ActivateTenantUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer deactivated successfully false otherwise</returns>
        bool DeactivateTenantUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to get user count
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        int GetTenantUserCount(TenantUserSearchParameter userSearchParameter, string tenantCode);

        /// <summary>
        /// This method determines uniqueness of elements in repository.
        /// </summary>
        /// <param name="roles">The roles to save.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns name="result">
        /// Returns true if all elements are not present in repository, false otherwise.
        /// </returns>
        bool IsDuplicateTenantUserEmailAndMobileNumber(IList<TenantUser> users, string operation, string tenantCode);

    }
}
