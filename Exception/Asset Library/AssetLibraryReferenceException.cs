// <copyright file="AssetLibraryReferenceException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    public class AssetLibraryReferenceException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetLibraryReferenceException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public AssetLibraryReferenceException(string tenantCode)
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
                return ExceptionConstant.ASSETLIBRARY_EXCEPTION_SECTION + "~" + ExceptionConstant.ASSETLIBRARY_REFERENCE_EXCEPTION;
            }
        }

        #endregion
    }
}
