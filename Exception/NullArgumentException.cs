// <copyright file="NullArgumentException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent NullArgumentException Class.
    /// </summary>
    public class NullArgumentException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        public NullArgumentException(string tenantCode)
        {
            this.tenantCode = tenantCode;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// This method overrides exception message.
        /// </summary>
        public override string Message
        {
            get
            {
                return ExceptionConstant.COMMON_EXCEPTION_SECTION + "~" + ExceptionConstant.NULLARGUMENT_EXCEPTION;
            }
        }

        #endregion
    }
}
