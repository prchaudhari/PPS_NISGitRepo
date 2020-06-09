// <copyright file="ConnectionStringNotFoundException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class InvalidSearchParameterException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for invalid search parameter exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidSearchParameterException(string tenantCode)
        {
            this.tenantCode = tenantCode;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// This method overrides exception message.
        /// </summary>
        public override string Message
        {
            get
            {
                return ExceptionConstant.COMMON_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_SEARCHPARAMETER_EXCEPTION;
            }
        }

        #endregion
    }
}
