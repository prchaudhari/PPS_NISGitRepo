using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public interface IMultiTenantUserRoleAccessRepository
    {
        /// <summary>
        /// This method adds the specified list of multi-tenant user role access in multi tenant user role access repository.
        /// </summary>
        /// <param name="multiTenantUserRoles">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if list of multi-tenant user role access are added successfully, else false.
        /// </returns>
        bool AddMultiTenantUserRoleAccess(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode);

        /// <summary>
        /// This method updates the specified list of multi-tenant user role access in multi tenant user role access repository.
        /// </summary>
        /// <param name="multiTenantUserRoles">The list of statements</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns true if list of multi-tenant user role access are updated successfully, else false.
        /// </returns>
        bool UpdateMultiTenantUserRoleAccess(IList<MultiTenantUserRoleAccess> lstMultiTenantUserRoleAccess, string tenantCode);

        /// <summary>
        /// This method gets the specified list of multi-tenant user role access from multi tenant user role access repository.
        /// </summary>
        /// <param name="multiTenantUserRoleAccessSearchParameter">The multi-tenant user role access search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of multi-tenant user role access
        /// </returns>
        IList<MultiTenantUserRoleAccess> GetMultiTenantUserRoleAccessList(MultiTenantUserRoleAccessSearchParameter multiTenantUserRoleAccessSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get multi-tenant user role access list count
        /// </summary>
        /// <param name="multiTenantUserRoleAccessSearchParameter">The multi-tenant user role access search parameter</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>multi-tenant user role access list count</returns>
        int GetMultiTenantUserRoleAcessListCount(MultiTenantUserRoleAccessSearchParameter multiTenantUserRoleAccessSearchParameter, string tenantCode);

        /// <summary>
        /// This method helps to activate the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access activated successfully false otherwise</returns>
        bool ActivateMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to deactivate the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access deactivated successfully false otherwise</returns>
        bool DeactivateMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode);

        /// <summary>
        /// This method helps to delete the multi-tenant user role access
        /// </summary>
        /// <param name="multiTenantUserRoleAccessIdentifier">The multi tenant user role access identifier</param>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>True if Multi-tenant user role access deleted successfully false otherwise</returns>
        bool DeletedMultiTenantUserRoleAccess(long multiTenantUserRoleAccessIdentifier, string tenantCode);

        /// <summary>
        /// This method gets the specified list of users by tenant code from user repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of users of specific tenant code
        /// </returns>
        IList<User> GetUsersByTenantCode(string tenantCode);

        /// <summary>
        /// This method gets the specified list of roles by tenant code from role repository.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        /// <returns>
        /// Returns the list of roles of specific tenant code
        /// </returns>
        IList<Role> GetRolesByTenantCode(string tenantCode);

    }

}
