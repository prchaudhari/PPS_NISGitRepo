﻿// <copyright file="InvalidScheduleException.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;

    #endregion

    /// <summary>
    /// This class implements invalid role exception which will be raised 
    /// when an role is passed with wrong role credentials.
    /// </summary>
    public class InvalidScheduleException : Exception
    {
        #region Private Members

        /// <summary>
        /// The tenantCode Code
        /// </summary>
        public string tenantCode = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Parameterized constructor for invalid role exception.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        public InvalidScheduleException(string tenantCode)
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
                return ExceptionConstant.SCHEDULE_EXCEPTION + "~" + ExceptionConstant.INVALID_SCHEDULE_EXCEPTION;
            }
        }

        #endregion
    }
}
