using System;
// <copyright file="DuplicateTenantContactEmailAddressFoundException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2015 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents the duplicate user exception which will be raised when an attempt is made to add duplicate 
	/// user in the repository
    /// </summary>
    public class DuplicateTenantContactEmailAddressFoundException : Exception
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
        public DuplicateTenantContactEmailAddressFoundException(string tenantCode)
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
                return ExceptionConstant.DUPLICATE_USER_EMAIL_FOUND_EXCEPTION;
            }
        }

        #endregion  
    }
}
