// <copyright file="RepositoryStoreNotAccessibleException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2016 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region references

    using System;

    #endregion

    /// <summary>
    /// This class implements role store not accessible exception which will 
    /// be raised when an attempt is made to access repository with wrong credentials.
    /// </summary>
    public class RepositoryStoreNotAccessibleException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenant code
        /// </summary>
        private string tenantCode = string.Empty;

        #endregion

        #region Contructor

        /// <summary>
        /// Initializing instance of class.
        /// </summary>
        /// <param name="tenantCode"></param>
        public RepositoryStoreNotAccessibleException(string tenantCode)
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
                return ExceptionConstant.COMMON_EXCEPTION_SECTION + "~" + ExceptionConstant.REPOSITORY_NOT_ACCESSIBLE;
            }
        }

        #endregion
    }
}