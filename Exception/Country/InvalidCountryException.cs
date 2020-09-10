// <copyright file="InvalidCountryException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid country exception which is raised when invalid country object is passed
    /// </summary>
    public class InvalidCountryException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCountryException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidCountryException(string tenantCode)
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
                return ExceptionConstant.COUNTRY_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_COUNTRY_EXCEPTION;
            }
        }

        #endregion
    }
}
