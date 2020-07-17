// <copyright file="DuplicateRenderEngineFoundException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent duplicate render engine found exception class.
    /// </summary>
    public class DuplicateRenderEngineFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for duplicate render engine exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateRenderEngineFoundException(string tenantCode)
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
                return ExceptionConstant.RENDERENGINE_EXCEPTION_SECTION + "~" + ExceptionConstant.DUPLICATE_RENDERENGINE_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
