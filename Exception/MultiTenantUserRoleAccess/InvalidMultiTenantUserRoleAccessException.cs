using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class InvalidMultiTenantUserRoleAccessException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for invalid role exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidMultiTenantUserRoleAccessException(string tenantCode)
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
                return ExceptionConstant.MULTI_TENANT_USER_ROLE_ACCESS_EXCEPTION + "~" + ExceptionConstant.INVALID_MULTI_TENANT_USER_ROLE_ACCESS_EXCEPTION;
            }
        }

        #endregion
    }
}
