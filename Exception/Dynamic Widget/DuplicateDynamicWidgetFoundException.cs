// <copyright file="DuplicateDynamicWidgetFoundException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent duplicate dynamicWidget found exception class.
    /// </summary>
    public class DuplicateDynamicWidgetFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for duplicate dynamicWidget exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateDynamicWidgetFoundException(string tenantCode)
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
                return ExceptionConstant.DYNAMICWIDGET_EXCEPTION_SECTION + "~" + ExceptionConstant.DUPLICATE_DYNAMICWIDGET_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
