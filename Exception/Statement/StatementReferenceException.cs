// <copyright file="StatementReferenceException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class StatementReferenceException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementReferenceException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public StatementReferenceException(string tenantCode)
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
                return ExceptionConstant.STATEMENT_EXCEPTION + "~" + ExceptionConstant.STATEMENT_REFERENCE_EXIST_EXCEPTION;
            }
        }

        #endregion
    }
}
