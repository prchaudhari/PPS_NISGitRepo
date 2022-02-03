// <copyright file="ModelConstant.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------
namespace NedbankModel
{
    /// <summary>
    /// Represents Model Constant Class
    /// </summary>
    public partial class ModelConstant
    {

        #region Common
        /// <summary>
        /// The common section key
        /// </summary>
        public const string COMMON_SECTION = "nedBank";

        /// <summary>
        /// Key for tenant code
        /// </summary>
        public const string TENANT_CODE_KEY = "TenantCode";

        /// <summary>
        /// Default identifier for the tenant
        /// </summary>
        public const string DEFAULT_TENANT_CODE = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The nVidYo connection string key
        /// </summary>
        public const string NIS_CONNECTION_STRING = "nISConnectionString";

        /// <summary>
        /// The key for configuration base URL
        /// </summary>
        public const string CONFIGURATON_BASE_URL = "ConfigurationBaseURL";

        #endregion
    }
}
