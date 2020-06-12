// <copyright file="ExceptionConstants.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    #endregion

    /// <summary>
    /// This class contains all exception related constant
    /// </summary>
    public partial class ExceptionConstant
    {
        #region Common

        /// <summary>
        /// Common exception section
        /// </summary>
        public const string COMMON_EXCEPTION_SECTION = "CommonResourceException";

        /// <summary>
        /// The repository not accessible exception 
        /// </summary>
        public const string REPOSITORY_NOT_ACCESSIBLE = "RepositoryNotAccessibleException";

        /// <summary>
        /// The invalid search parameter exception
        /// </summary>
        public const string INVALID_SEARCHPARAMETER_EXCEPTION = "InvalidSearchParameterException";

        /// <summary>
        /// The tenant code not found exception
        /// </summary>
        public const string TENANT_NOT_FOUND_EXCEPTION = "TenantNotFoundException";

        /// <summary>
        /// The invalid tenant exception
        /// </summary>
        public const string INVALID_TENANT_EXCEPTION = "InvalidTenantException";

        /// <summary>
        /// The null argument exception
        /// </summary>
        public const string NULLARGUMENT_EXCEPTION = "NullArgumentException";

        /// <summary>
        /// The invalid connection string
        /// </summary>
        public const string INVALID_CONNECTIONSTRING_EXCEPTION = "msgConnectionStringNotFoundException";

        /// <summary>
        /// The invalid encrypted data exception
        /// </summary>
        public const string INVALID_ENCRYPTED_DATA_EXCEPTION = "InvalidEncryptedDataException";
        #endregion
    }
}