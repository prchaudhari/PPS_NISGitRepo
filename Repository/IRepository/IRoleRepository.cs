// <copyright file="IRoleRepository.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

using System.Collections.Generic;

namespace nIS
{
    public interface IRoleRepository
    {
        /// <summary>
        /// This method adds the specified list of roles in role repository.
        /// </summary>
        /// <param name="roles">The list of roles</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are added successfully, else false.
        /// </returns>
        bool AddRoles(IList<Role> roles, string tenantCode);

        /// <summary>
        /// This method updates the specified list of roles in role repository.
        /// </summary>
        /// <param name="roles">The list of roles</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are updated successfully, else false.
        /// </returns>
        bool UpdateRoles(IList<Role> roles, string tenantCode);

        /// <summary>
        /// This method deletes the specified list of roles from role repository.
        /// </summary>
        /// <param name="roles">The list of users</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if roles are deleted successfully, else false.
        /// </returns>
        bool DeleteRoles(IList<Role> roles, string tenantCode);

        /// <summary>
        /// This method gets the specified list of roles from role repository.
        /// </summary>
        /// <param name="roleSearchParameter">The role search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of roles
        /// </returns>
        IList<Role> GetRoles(RoleSearchParameter roleSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get role count
        /// </summary>
        /// <param name="roleSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        int GetRoleCount(RoleSearchParameter roleSearchParameter, string tenantCode);
    }
}
