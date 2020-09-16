// <copyright file="ClientSubscriptionHistoryUpdatationFailedException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class represents client subscription history failed exception which is raised when the specified client already has higher subscription and updated subscription is low object is not found in repository
    /// </summary>
    public class ClientSubscriptionHistoryUpdatationFailedException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSubscriptionHistoryUpdatationFailedException"/> class.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public ClientSubscriptionHistoryUpdatationFailedException(string tenantCode)
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
                return ExceptionConstant.CLIENT_EXCEPTION_SECTION + "~" + ExceptionConstant.CLIENT_SUBSCRIPTION_FAILED_EXCEPTION;
            }
        }

        #endregion
    }
}
