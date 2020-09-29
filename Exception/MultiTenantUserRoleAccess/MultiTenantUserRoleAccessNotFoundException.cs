// <copyright file="MultiTenantUserRoleAccessNotFoundException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2020  Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References
    using System;
    #endregion


    public class MultiTenantUserRoleAccessNotFoundException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for role not found exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public MultiTenantUserRoleAccessNotFoundException(string tenantCode)
        {
            this.tenantCode = tenantCode;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// This method overrides the exception message.
        /// </summary>    

        public override string Message
        {
            get
            {
                return ExceptionConstant.MULTI_TENANT_USER_ROLE_ACCESS_EXCEPTION + "~" + ExceptionConstant.MULTI_TENANT_USER_ROLE_ACCESS_NOT_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
