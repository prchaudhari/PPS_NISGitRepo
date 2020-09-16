// <copyright file="DuplicateStateFoundException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent duplicate state found exception class.
    /// </summary>
    public class DuplicateStateFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for duplicate state exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateStateFoundException(string tenantCode)
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
                return ExceptionConstant.STATE_EXCEPTION_SECTION + "~" + ExceptionConstant.DUPLICATE_STATE_FOUND_EXCEPTION ;
            }
        }

        #endregion
    }
}
