// <copyright file="AssetPathNotFoundException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid asset path exception which is raised when invalid asset path object is passed
    /// </summary>
    public class AssetPathNotFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetPathNotFoundException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public AssetPathNotFoundException(string tenantCode)
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
                return ExceptionConstant.ASSETPATH_EXCEPTION_SECTION + "~" + ExceptionConstant.ASSETPATH_NOT_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
