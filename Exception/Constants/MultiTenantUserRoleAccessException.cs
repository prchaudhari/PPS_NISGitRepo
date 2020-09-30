// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References
    #endregion

    public partial class ExceptionConstant
    {
        /// <summary>
        /// Multi-Tenant User Role Access exception
        /// </summary>
        public const string MULTI_TENANT_USER_ROLE_ACCESS_EXCEPTION = "MultiTenantUserRoleAccessException";

        /// <summary>
        /// Multi-Tenant User Role Access not found exception
        /// </summary>
        public const string MULTI_TENANT_USER_ROLE_ACCESS_NOT_FOUND_EXCEPTION = "Multi-Tenant User Role Access not found";

        /// <summary>
        /// The invalid Multi-Tenant User Role Access exception
        /// </summary>
        public const string INVALID_MULTI_TENANT_USER_ROLE_ACCESS_EXCEPTION = "Invalid Multi-Tenant User Role Access";

        /// <summary>
        /// Duplicate Tenant user role access mapping found exception
        /// </summary>
        public const string DUPLICATE_TENANT_USER_ROLE_ACCESS_MAPPING_FOUND_EXCEPTION = "Tenant user role access mapping already exists";
    }
}
