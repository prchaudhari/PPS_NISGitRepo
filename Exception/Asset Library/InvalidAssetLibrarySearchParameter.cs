// <copyright file="InvalidAssetLibrarySearchParameter.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid asset library search parameter exception which is raised when invalid search parameter is passed for the asset library object
    /// </summary>
    public class InvalidAssetLibrarySearchParameter : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCustomerSearchParameter"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidAssetLibrarySearchParameter(string tenantCode)
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
                return ExceptionConstant.ASSETLIBRARY_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_ASSETLIBRARY_SEARCH_PARAMETER_EXCEPTION;
            }
        }

        #endregion
    }
}
