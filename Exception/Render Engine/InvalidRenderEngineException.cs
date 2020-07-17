// <copyright file="InvalidRenderEngineException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid render engine exception which is raised when invalid render engine object is passed
    /// </summary>
    public class InvalidRenderEngineException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidRenderEngineException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidRenderEngineException(string tenantCode)
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
                return ExceptionConstant.RENDERENGINE_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_RENDERENGINE_EXCEPTION;
            }
        }

        #endregion
    }
}
