// <copyright file="InvalidUserPasswordException.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------


namespace nIS
{

    #region Reference

    using System;

    #endregion

    /// <summary>
    /// This class represents error description about invalid user password exception.
    /// </summary>
    public class InvalidUserPasswordException : Exception
    {
        #region Private Member

        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        #endregion

        /// <summary>
        /// Initailzing instance of class.
        /// </summary>
        /// <param name="tenantCode"></param>
        public InvalidUserPasswordException(string tenantCode)
        {
            this.tenantCode = tenantCode;
        }

        #region Public Property

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        /// <value>Exception message.</value>
        public override string Message
        {
            get
            {
                return ExceptionConstant.USER_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_USER_PASSWORD_EXCEPTION;
            }
        }

        #endregion
    }
}
