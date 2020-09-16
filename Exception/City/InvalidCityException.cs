// <copyright file="InvalidCityException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent invalid city exception Class.
    /// </summary>
    public class InvalidCityException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for invalid city exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidCityException(string tenantCode)
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
                return ExceptionConstant.CITY_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_CITY_EXCEPTION;
            }
        }

        #endregion

    }
}
