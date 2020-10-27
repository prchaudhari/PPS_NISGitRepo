// <copyright file="ClientConfigurationIsInProcessException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents client configuration is in-process exception which is raised when the specified client widget schema is not configured yet.
    /// </summary>
    public class ClientConfigurationIsInProcessException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConfigurationIsInProcessException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public ClientConfigurationIsInProcessException(string tenantCode)
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
                return ExceptionConstant.CLIENT_EXCEPTION_SECTION + "~" + ExceptionConstant.CLIENT_CONFIGURATION_IS_IN_PROCESS_EXCEPTION;
            }
        }

        #endregion
    }
}
