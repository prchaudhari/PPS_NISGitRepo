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
        public const string COMMON_SECTION = "nIS";

        /// <summary>
        /// Invalid Pagin Parameter
        /// </summary>
        public const string INVALID_PAGING_PARAMETER = "InvalidPagingParameter";

        /// <summary>
        ///Invalid sort parameter
        /// </summary>
        public const string INVALID_SORT_PARAMETER = "InvalidSortParameter";

        /// <summary>
        /// Key for tenant code
        /// </summary>
        public const string TENANT_CODE_KEY = "TenantCode";

        /// <summary>
        /// Default identifier for the tenant
        /// </summary>
        public const string DEFAULT_TENANT_CODE = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The time zone key
        /// </summary>
        public const string TIME_ZONE_KEY = "Timezone";

        /// <summary>
        /// The off set key
        /// </summary>
        public const string OFF_SET_KEY = "Offset";

        /// <summary>
        /// The nVidYo connection string key
        /// </summary>
        public const string NIS_CONNECTION_STRING = "nISConnectionString";

        /// <summary>
        /// The key for configuration base URL
        /// </summary>
        public const string CONFIGURATON_BASE_URL = "ConfigurationBaseURL";

        /// <summary>
        /// The key for resource base URL
        /// </summary>
        public const string RESOURCE_BASE_URL = "ResourceBaseURL";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string TENANT_BASE_URL = "TenantManagerAPIURL";

        /// <summary>
        /// The entity base URL
        /// </summary>
        public const string ENTITY_BASE_URL = "EntityBaseURL";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string SORT_COLUMN = "Id";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string TENANT_ADMIN_ROLE = "Tenant Admin";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string GROUP_MANAGER_ROLE = "Group Manager";

        /// <summary>
        /// The tenant base URL
        /// </summary>
        public const string TENANT_PRIMARY_CONTACT = "Primary";

        /// <summary>
        /// The entity sort column
        /// </summary>
        public const string ENTITYSORTCOLUMN = "Id";

        /// <summary>
        /// The component code
        /// </summary>
        public const string COMPONENTCODE = "nIS";

        public const string CHILD_TENANT = "Tenant";

        public const string TENANT_GROUP = "Group";

        public const string TENANT_SELF_MANAGE_TYPE = "Self";

        public const string WEB_API_BASE_URL = "WebApiBaseUrl";

        #endregion

        #region Operations

        /// <summary>
        /// Indicates add operation constant value.
        /// </summary>
        public const string ADD_OPERATION = "AddOperation";

        /// <summary>
        /// Indicates update operation constant value..
        /// </summary>
        public const string UPDATE_OPERATION = "UpdateOperation";

        /// <summary>
        /// Indicates delete operation constant value.
        /// </summary>
        public const string DELETE_OPERATION = "DeleteOperation";

        /// <summary>
        /// Indicates change activation status operation constant value.
        /// </summary>
        public const string CHANGE_ACTIVATION_STATUS = "ChangeActivationStatus";

        /// <summary>
        /// Separation by
        /// </summary>
        public const string SEPARATION_BY = ",";


        #endregion

        #region Widget

        /// <summary>
        /// Widget section
        /// </summary>
        public const string WIDGET_SECTION = "Widget";

        /// <summary>
        /// Invalid widget id
        /// </summary>
        public const string INVALID_WIDGET_ID = "InvalidWidgetId";

        /// <summary>
        /// Invalid widget name
        /// </summary>
        public const string INVALID_WIDGET_NAME = "InvalidWidgetName";

        #endregion

        #region Product Type

        /// <summary>
        /// Product type section
        /// </summary>
        public const string PRODUCTTYPE_SECTION = "ProductType";

        /// <summary>
        /// Invalid product type id
        /// </summary>
        public const string INVALID_PRODUCTTYPE_ID = "InvalidProductTypeId";

        /// <summary>
        /// Invalid product name
        /// </summary>
        public const string INVALID_PRODUCTTYPE_NAME = "InvalidProductTypeName";

        #endregion

        #region Page

        /// <summary>
        /// Page section
        /// </summary>
        public const string PAGE_SECTION = "Page";

        /// <summary>
        /// Invalid page id
        /// </summary>
        public const string INVALID_PAGE_ID = "InvalidPageId";

        /// <summary>
        /// Invalid page display name
        /// </summary>
        public const string INVALID_PAGE_DISPLAY_NAME = "InvalidageDisplayName";

        /// <summary>
        /// Invalid page(product) type id
        /// </summary>
        public const string INVALID_PAGE_TYPE_ID = "InvalidPageTypeId";

        #endregion

        #region User

        /// <summary>
        /// The user model section
        /// </summary>
        public const string USER_MODEL_SECTION = "UserModelSection";

        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_NAME = "InvalidUserName";

        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_CONTACT_NUMBER = "InvalidUserContactNumber";
        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_EMAIL = "InvalidUserEmailAdress";
        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_IMAGE = "InvalidUserImage";

        /// <summary>
        /// The invalid user name
        /// </summary>
        public const string INVALID_USER_ROLE = "InvalidUserRole";

        #endregion

        #region Role

        /// <summary>
        /// The role model section
        /// </summary>
        public const string ROLE_MODEL_SECTION = "RoleModel";

        public const string SUPER_ADMIN_ROLE = "Super Admin";

        /// <summary>
        /// The invalid role name
        /// </summary>
        public const string INVALID_ROLE_NAME = "InvalidRoleName";

        #endregion

        #region Schdeule

        /// <summary>
        /// The schedule model section
        /// </summary>
        public const string SCHEDULE_MODEL_SECTION = "ScheduleModel";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_NAME = "InvalidScheduleName";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_STARTDATE = "InvalidScheduleStartDate";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_ENDDATE = "InvalidScheduleEndDate";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_DAYOFMONTH = "InvalidScheduleDayOfMonth";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_HOUROFDAY = "InvalidScheduleHourOfDay";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_MINOFDAY = "InvalidScheduleMinuteOfDay";

        /// <summary>
        /// The invalid schedule name
        /// </summary>
        public const string INVALID_SCHEDULE_STATUS = "InvalidScheduleStatus";

        /// <summary>
        /// The schedule recurrence patterns
        /// </summary>
        public const string DOES_NOT_REPEAT = "DoesNotRepeat";
        public const string DAILY = "Daily";
        public const string CUSTOM_DAY = "Custom-Day";
        public const string WEEKLY = "Weekly";
        public const string CUSTOM_WEEK = "Custom-Week";
        public const string WEEKDAY = "Weekday";
        public const string MONTHLY = "Monthly";
        public const string CUSTOM_MONTH = "Custom-Month";
        public const string YEARLY = "Yearly";
        public const string CUSTOM_YEAR = "Custom-Year";

        #endregion

        #region Role Privilege

        /// <summary>
        /// The role privilege model
        /// </summary>
        public const string ROLEPRIVILEGEMODELSECTION = "RolePrivilegeModel";

        /// <summary>
        /// Invalid entity name
        /// </summary>
        public const string INVALIDENTITYNAME = "InvalidEntityName";

        /// <summary>
        /// Invalid role privilege operations
        /// </summary>
        public const string INVALIDROLEPRIVILEGEOPERATIONS = "InvalidRolePrivilegeoOperations";

        #endregion

        #region Role Privilege Operation

        /// <summary>
        /// Role privilege operation model section
        /// </summary>
        public const string ROLEPRIVILEGEOPERATIONMODELSECTION = "RolePrivilegeOperationModel";

        /// <summary>
        /// Invalid operation name
        /// </summary>
        public const string INVALIDOPERATIONNAME = "InvalidOperationName";

        #endregion

        #region Mail contents

        /// <summary>
        /// Indicates mail subject when user will be added.
        /// </summary>
        public const string NEWLYADDEDUSERMAILSUBJECT = "NewlyAddedUserMailSubject";

        /// <summary>
        /// Indicates mail message when user will be added.
        /// </summary>
        public const string NEWLYADDEDUSERMAILMESSAGE = "NewlyAddedUserMailMessage";

        /// <summary>
        /// Indicates the link which will attach with mail for reset password.
        /// </summary>
        public const string CHANGEPASSWORDLINK = "ChangePasswordLink";

        /// <summary>
        /// Indicates mail subject when user is going to change password.
        /// </summary>
        public const string USERFORGOTPASSWORDSUBJECT = "UserForgotPasswordSubject";

        /// <summary>
        /// Indicates mail message when user is going to change password.
        /// </summary>
        public const string USERFORGOTPASSWORDMESSAGE = "UserForgotPasswordMessage";

        /// <summary>
        /// Indicates the new password is sent to user by mail.
        /// </summary>
        public const string SENDPASSWORDMAILTOUSERMESSAGE = "SendPasswordMailToUserMessage";

        #endregion

        #region nIS engine

        public const string DEFAULT_NIS_ENGINE_BASE_URL = "DefaultNisEngineBaseUrl";

        public const string CREATE_CUSTOMER_STATEMENT_API_URL = "GenerateStatement/CreateCustomerStatement";

        public const string CREATE_NEDBANK_CUSTOMER_STATEMENT_API_URL = "GenerateStatement/CreateNedbankCustomerStatement";

        public const string RETRY_TO_CREATE_FAILED_CUSTOMER_STATEMENTS_API_URL = "GenerateStatement/RetryToCreateFailedCustomerStatements";

        public const string RUN_ARCHIVAL_PROCESS_FOR_CUSTOMER_RECORD = "GenerateStatement/RunArchivalForCustomerRecord";

        public const string APPLICATION_JSON_MEDIA_TYPE = "application/json";

        public const string BATCH_ID = "BatchId";

        public const string CUSTOEMR_ID = "CustomerId";

        public const string WIDGET_FILTER_SETTING = "WidgetFilterSetting";

        public const string PARALLEL_THREAD_COUNT = "ParallelThreadCount";

        public const string MINIMUM_ARCHIVAL_PROCESS_PERIOD_IN_DAYS = "MinimumArchivalPeriodDays";

        public const string IS_WANT_TO_USE_NIS_ENGINES = "IsWantToUseNisEngines";

        public const string PREVIEW_FINANCIAL_STATEMENT_FUNCTION_NAME = "PreviewFinancialStatement";
        
        public const string PREVIEW_NEDBANK_STATEMENT_FUNCTION_NAME = "PreviewNedbankStatement";

        public const string GENERATE_FINANCIAL_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW = "GenerateFinancialTenantCustomerStatementsByScheduleRunNow";

        public const string GENERATE_FINANCIAL_CUSTOEMR_STATEMENT_BY_SCHEDULE_TIME = "GenerateFinancialTenantCustomerStatementsByScheduleRunTime";

        public const string GENERATE_NEDBANK_CUSTOEMR_STATEMENT_BY_SCHEDULE_RUN_NOW = "GenerateNedbankCustomerStatementsByScheduleRunNow";

        public const string GENERATE_NEDBANK_CUSTOEMR_STATEMENT_BY_SCHEDULE_TIME = "GenerateNedbankCustomerStatementsByScheduleRunTime";

        public const string GENERATE_HTML_FORMAT_OF_FINANCIAL_TENANT_STATEMENT = "GenerateHtmlFormatOfStatement";

        public const string GENERATE_HTML_FORMAT_OF_NEDBANK_TENANT_STATEMENT = "GenerateHtmlFormatOfNedbankStatement";

        public const string RETRY_FAILED_FINANCIAL_TENANT_CUSTOMER_STATEMENT_API_NAME = "RetryToCreateFailedCustomerStatements";

        public const string FINANCIAL_TENANT_ARCHIVAL_PROCESS_API_NAME = "RunArchivalForCustomerRecord";

        public const string RETRY_FAILED_NEDBANK_TENANT_CUSTOMER_STATEMENT_API_NAME = "RetryToCreateFailedNedbankCustomerStatements";

        public const string NEDBANK_TENANT_ARCHIVAL_PROCESS_API_NAME = "RunArchivalForNedbankCustomerRecord";

        #endregion

        #region Statement constants

        public const string SA_COUNTRY_CULTURE_INFO_CODE = "en-ZA";

        public const string DOT_AS_CURERNCY_DECIMAL_SEPARATOR = ".";

        public const string CURRENCY_FORMAT_VALUE = "C";

        public const string BALANCE_CARRIED_FORWARD_TRANSACTION_DESC = "balance carried forward";

        public const string BALANCE_BROUGHT_FORWARD_TRANSACTION_DESC = "balance brought forward";

        public const string PAYMENT_THANK_YOU_TRANSACTION_DESC = "Payment - Thank you";

        public const string DATE_FORMAT_dd_MM_yyyy = "dd'/'MM'/'yyyy";

        public const string DATE_FORMAT_dd_MMM_yyyy = "dd MMM yyyy";

        public const string DATE_FORMAT_yyyy_MM_dd = "yyyy-MM-dd";
        #endregion
    }
}
