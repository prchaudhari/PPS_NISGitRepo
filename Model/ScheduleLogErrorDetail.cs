// <copyright file="ScheduleLogErrorDetail.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{

    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class ScheduleLogErrorDetail
    {
        public long StatementId { get; set; }
        public string StatementName { get; set; }
        public long ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public long ScheduleLogId { get; set; }
        public long ScheduleLogDetailId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string ErrorLogMessage { get; set; }
    }
}
