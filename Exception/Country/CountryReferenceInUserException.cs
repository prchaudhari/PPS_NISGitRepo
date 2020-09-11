// <copyright file="CountryReferenceInUserException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent country not found exception Class.
    /// </summary>
    public class CountryReferenceInUserException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for country not found exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public CountryReferenceInUserException(string tenantCode)
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
                return ExceptionConstant.COUNTRY_EXCEPTION_SECTION + "~" + ExceptionConstant.COUNTRY_REFERENCEIN_USER_EXCEPTION;
            }
        }

        #endregion
    }
}
