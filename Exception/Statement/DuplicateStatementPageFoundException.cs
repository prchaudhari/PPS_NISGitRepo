
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class DuplicateStatementPageFoundException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for duplicate role exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateStatementPageFoundException(string tenantCode)
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
                return ExceptionConstant.DUPLICATE_PAGE_WIDGET_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
