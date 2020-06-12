// <copyright file="AlreadyUsedPasswordException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2015 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{

    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents the alreday password user exception which will be raised when an attempt is changed password with same as previous password 
	/// user in the repository
    /// </summary>
    public class AlreadyUsedPasswordException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Parameterized constructor for duplicate user exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        public AlreadyUsedPasswordException(string tenantCode)
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
                return ExceptionConstant.USER_EXCEPTION_SECTION + "~" + ExceptionConstant.ALREADY_PASSWORD_EXCEPTION;
            }
        }

        #endregion  
    }

}
