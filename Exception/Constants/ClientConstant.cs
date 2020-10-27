// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    /// <summary>
    /// This class contains all client exception related constant
    /// </summary>
    public partial class ExceptionConstant
    {
        #region Client

        /// <summary>
        /// The client exception section key
        /// </summary>
        public const string CLIENT_EXCEPTION_SECTION = "ClientException";

        /// <summary>
        /// The invalid client search parameter exception key
        /// </summary>
        public const string INVALID_CLIENT_SEARCH_PARAMETER_EXCEPTION = "InvalidClientSearchParameterException";

        /// <summary>
        /// The invalid client exception key
        /// </summary>
        public const string INVALID_CLIENT_EXCEPTION = "InvalidClientException";

        /// <summary>
        /// The duplicate client exception key
        /// </summary>
        public const string DUPLICATE_CLIENT_EXCEPTION = "DuplicateClientException";

        /// <summary>
        /// The client not found exception key
        /// </summary>
        public const string CLIENT_NOT_FOUND_EXCEPTION = "ClientNotFoundException";

        /// <summary>
        /// The client subscription failed exception key
        /// </summary>
        public const string CLIENT_SUBSCRIPTION_FAILED_EXCEPTION = "ClientSubscriptionFailedException";

        /// <summary>
        /// The client not found exception key
        /// </summary>
        public const string CLIENT_CONFIGURATION_IS_IN_PROCESS_EXCEPTION = "Tenant on boarding is in process, till then please wait or contact Admin.";

        #endregion
    }
}