// <copyright file="InvalidDynamicWidgetException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents invalid dynamicWidget exception which is raised when invalid dynamicWidget object is passed
    /// </summary>
    public class InvalidDynamicWidgetException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidDynamicWidgetException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidDynamicWidgetException(string tenantCode)
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
                return ExceptionConstant.DYNAMICWIDGET_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_DYNAMICWIDGET_EXCEPTION;
            }
        }

        #endregion
    }
}
