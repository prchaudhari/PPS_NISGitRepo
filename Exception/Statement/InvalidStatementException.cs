using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class InvalidStatementException: Exception
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
        public InvalidStatementException(string tenantCode)
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
                return ExceptionConstant.STATEMENT_EXCEPTION + "~" + ExceptionConstant.INVALID_STATEMENT_EXCEPTION;
            }
        }

        #endregion
    }
}
