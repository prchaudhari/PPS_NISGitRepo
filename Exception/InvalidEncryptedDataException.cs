// <copyright file="InvalidEncryptedDataException.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region Reference

    using System;

    #endregion

    /// <summary>
    /// This class represents error description for invalid encrypted data exception.
    /// </summary>
    public class InvalidEncryptedDataException : Exception
    {
        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        #region Constructor

        /// <summary>
        /// Initializing instance this class.
        /// </summary>
        /// <param name="tenantCode">The tenant code</param>
        public InvalidEncryptedDataException(string tenantCode)
        {
            this.tenantCode = tenantCode;
        }

        #endregion

        #region Public Property

        /// <summary>
        /// Gets the exception message.
        /// </summary>
        /// <value>Exception message.</value>
        public override string Message
        {
            get
            {
                return ExceptionConstant.COMMON_EXCEPTION_SECTION + "~" + ExceptionConstant.INVALID_ENCRYPTED_DATA_EXCEPTION;
            }
        }

        #endregion
    }
}
