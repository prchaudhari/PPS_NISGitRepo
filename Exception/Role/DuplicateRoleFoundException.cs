// <copyright file="DuplicateRoleFoundException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class implements duplicate role exception which will be raised 
    /// when an attempt is made to save duplicate role.
    /// </summary>
    public class DuplicateRoleFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for duplicate role exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public DuplicateRoleFoundException(string tenantCode)
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
                return  ExceptionConstant.DUPLICATE_ROLE_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
