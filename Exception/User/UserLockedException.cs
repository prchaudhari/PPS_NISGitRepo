// <copyright file="UserLockedException.cs" company="Websym Solutions Pvt. Ltd.">
//     Copyright (c) 2015 Precimetrix Technologies Pvt. Ltd. All rights reserved. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents the user locked found exception which will be raised when an log in attempt is made for more than three times
    /// which is not present in the repository.
    /// </summary>
    public class UserLockedException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor for user locked exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        public UserLockedException(string tenantCode)
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
                return ExceptionConstant.USER_EXCEPTION_SECTION + "~" + ExceptionConstant.USER_LOCKED_EXCEPTION;
            }
        }

        #endregion  
    }
}
