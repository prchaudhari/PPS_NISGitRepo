// <copyright file="ScheduleLogDetailNotFoundException.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018  Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References

    using System;

    #endregion

    public class ScheduleLogDetailNotFoundException: Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for schedule log detail not found exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public ScheduleLogDetailNotFoundException(string tenantCode)
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
                return ExceptionConstant.SCHEDULE_LOG_DETAIL_EXCEPTION + "~" + ExceptionConstant.SCHEDULE_LOG_DETAIL_NOT_FOUND_EXCEPTION;
            }
        }

        #endregion
    }
}

