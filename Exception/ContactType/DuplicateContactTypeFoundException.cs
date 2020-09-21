// <copyright file="DuplicateContactTypeFoundException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent duplicate contactType found exception class.
    /// </summary>
    public class DuplicateContactTypeFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for duplicate contactType exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateContactTypeFoundException(string tenantCode)
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
                return ExceptionConstant.COUNTRY_EXCEPTION_SECTION + "~" + ExceptionConstant.DUPLICATE_COUNTRY_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
