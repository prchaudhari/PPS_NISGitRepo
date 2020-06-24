// <copyright file="DuplicateAssetException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents duplicate asset  exception which is raised when an attempt is made to add duplicate asset  in the repository
    /// </summary>
    public class DuplicateAssetException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateCustomerException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateAssetException(string tenantCode)
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
                return ExceptionConstant.ASSET_EXCEPTION_SECTION + "~" + ExceptionConstant.DUPLICATE_ASSET_EXCEPTION;
            }
        }

        #endregion
    }
}
