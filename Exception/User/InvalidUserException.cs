// <copyright file="InvalidUserException.cs" company="Websym Solutions Pvt. Ltd.">
//     Copyright (c) 2015 Precimetrix Technologies Pvt. Ltd. All rights reserved. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents the invalid user exception which will be raised when invalid user object is passed in the application
    /// </summary>
    public class InvalidUserException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor for invalid user exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        public InvalidUserException(string tenantCode)
        {
            this.tenantCode = tenantCode;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method overrides the base exception message, in order to display custom message.
        /// </summary>
        public override string Message
        {
            get
            {
                return ExceptionConstant.USER_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_USER_EXCEPTION;
            }
        }

        #endregion  
    }
}
