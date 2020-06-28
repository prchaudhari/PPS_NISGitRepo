// <copyright file="PageNotFoundException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018  Websym Solutions Pvt. Ltd. 
// </copyright>


namespace nIS
{
    #region References
    using System;
    #endregion

    /// <summary>
    /// This class implements page not found exception which will be raised 
    /// when an atte
    public class PageNotFoundException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for role not found exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public PageNotFoundException(string tenantCode)
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
                return ExceptionConstant.PAGE_EXCEPTION + "~" + ExceptionConstant.PAGE_NOT_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
