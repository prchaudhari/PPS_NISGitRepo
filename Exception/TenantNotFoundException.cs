// <copyright file="TenantNotFoundException.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//-----------------------------------------------------------------------  

namespace nIS
{
    #region references
    using System;
    #endregion

    /// <summary>
    /// This class represents tenant identifier not found exception which 
    /// will be raised when an attempt is made to 
    /// call method of API without passing tenant identifier as a parameter
    /// </summary>
    public class TenantNotFoundException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        public TenantNotFoundException(string tenantCode)
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
                return ExceptionConstant.COMMON_EXCEPTION_SECTION + "~" + ExceptionConstant.TENANT_NOT_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}
