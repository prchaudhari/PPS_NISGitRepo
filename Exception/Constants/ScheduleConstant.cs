// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References
    #endregion

    /// <summary>
    /// This class represents constants for role entity.
    /// </summary>
    public partial class ExceptionConstant
    {
        /// <summary>
        /// Schedule exception
        /// </summary>
        public const string SCHEDULE_EXCEPTION = "ScheduleException";

        /// <summary>
        /// Duplicate Schedule found exception
        /// </summary>
        //public const string DUPLICATE_SCHEDULE_FOUND_EXCEPTION = "DuplicateScheduleFoundException";
        public const string DUPLICATE_SCHEDULE_FOUND_EXCEPTION = "Schedule already exists";

        /// <summary>
        /// Duplicate Schedule found exception
        /// </summary>
        //public const string DUPLICATE_SCHEDULE_FOUND_EXCEPTION = "DuplicateScheduleFoundException";
        public const string SCHEDULE_REFERENCE_EXCEPTION = "Schedule is running";

        /// <summary>
        /// Schedule not found exception
        /// </summary>
        //public const string SCHEDULE_NOT_FOUND_EXCEPTION = "ScheduleNotFoundException";
        public const string SCHEDULE_NOT_FOUND_EXCEPTION = "Schedule not found";

        /// <summary>
        /// The invalid role exception
        /// </summary>
        //public const string INVALID_SCHEDULE_EXCEPTION = "InvalidSchedule";
        public const string INVALID_SCHEDULE_EXCEPTION = "Invalid schedule";

        /// <summary>
        /// The invalid role exception
        /// </summary>
        //public const string TENANT_SECURITYCODE_NOTAVAILABLE = "TENANT_SECURITYCODE_NOTAVAILABLE";
        public const string TENANT_SECURITYCODEFORMAT_NOTAVAILABLE = "Tenant security code format not available";

        /// <summary>
        /// The  TENANT_SECURITYCODEFIELDDATA_NOTAVAILABLE exception
        /// </summary>
        //public const string TENANT_SECURITYCODEFIELDDATA_NOTAVAILABLE = "TENANT_SECURITYCODEFIELDDATA_NOTAVAILABLE";
        public const string TENANT_SECURITYCODEFIELDDATA_NOTAVAILABLE = "Tenant security code field data not available";

        /// <summary>
        /// The invalid role paging parameter
        /// </summary>
        //public const string INVALID_SCHEDULE_PAGING_PARAMETER = "InvalidSchedulePagingParameter";
        public const string INVALID_SCHEDULE_PAGING_PARAMETER = "Invalid role paging parameter";

        /// <summary>
        /// The invalid role sort parameter
        /// </summary>
        //public const string INVALID_SCHEDULE_SORT_PARAMETER = "InvalidScheduleSortParameter";
        public const string INVALID_SCHEDULE_SORT_PARAMETER = "Invalid role sort parameter";
    }
}
