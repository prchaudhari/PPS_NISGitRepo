// <copyright file="AssetPageReferenceException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    public class AssetPageReferenceException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetReferenceException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public AssetPageReferenceException(string tenantCode)
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
                return ExceptionConstant.ASSET_EXCEPTION_SECTION + "~" + ExceptionConstant.ASSET_PAGE_REFERENCE_EXCEPTION;
            }
        }

        #endregion
    }
}
