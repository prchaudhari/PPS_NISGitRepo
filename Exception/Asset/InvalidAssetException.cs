// <copyright file="InvalidAssetException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid asset  exception which is raised when invalid asset  object is passed
    /// </summary>
    public class InvalidAssetException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCustomerException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidAssetException(string tenantCode)
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
                return ExceptionConstant.ASSET_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_ASSET_EXCEPTION;
            }
        }

        #endregion
    }
}
