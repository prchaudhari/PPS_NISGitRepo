// <copyright file="DuplicateClientException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents duplicate client exception which is raised when an attempt is made to add duplicate client in the repository
    /// </summary>
    public class DuplicateClientException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateClientException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateClientException(string tenantCode)
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
                return ExceptionConstant.CLIENT_EXCEPTION_SECTION + "~" + ExceptionConstant.DUPLICATE_CLIENT_EXCEPTION;
            }
        }

        #endregion
    }
}
