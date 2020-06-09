// <copyright file="IUserRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface IUserRepository
    {
        /// <summary>
        /// This method adds the specified list of users in user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are added successfully, else false.
        /// </returns>
        bool AddUsers(IList<User> users, string tenantCode);

        /// <summary>
        /// This method updates the specified list of users in user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are updated successfully, else false.
        /// </returns>
        bool UpdateUsers(IList<User> users, string tenantCode);


        /// <summary>
        /// This method deletes the specified list of users from user repository.
        /// </summary>
        /// <param name="users">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if users are deleted successfully, else false.
        /// </returns>
        bool DeleteUsers(IList<User> users, string tenantCode);

        /// <summary>
        /// This method gets the specified list of users from user repository.
        /// </summary>
        /// <param name="userSearchParameter">The user search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of Users
        /// </returns>
        IList<User> GetUsers(UserSearchParameter userSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to activate the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer activated successfully false otherwise</returns>
        bool ActivateUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the users
        /// </summary>
        /// <param name="userIdentifier">The customer identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if customer deactivated successfully false otherwise</returns>
        bool DeactivateUser(long userIdentifier, string tenantCode);

        /// <summary>
        /// This method reference to get user count
        /// </summary>
        /// <param name="userSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        int GetUserCount(UserSearchParameter userSearchParameter, string tenantCode);




    }
}
