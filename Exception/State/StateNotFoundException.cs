// <copyright file="StateNotFoundException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------
namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represent state not found exception Class.
    /// </summary>
    public class StateNotFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for state not found exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public StateNotFoundException(string tenantCode)
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
                return ExceptionConstant.STATE_EXCEPTION_SECTION + "~" + ExceptionConstant.STATE_NOT_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
