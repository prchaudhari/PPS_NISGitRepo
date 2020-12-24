// <copyright file="InvalidPageTypeException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid pageType exception which is raised when invalid pageType object is passed
    /// </summary>
    public class InvalidPageTypeException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPageTypeException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidPageTypeException(string tenantCode)
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
                return ExceptionConstant.PAGETYPE_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_PAGETYPE_EXCEPTION;
            }
        }

        #endregion
    }
}
