// <copyright file="BatchMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
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

    public class BatchMaster
    {
        public long Identifier { get; set; }
        public string TenantCode { get; set; }
        public string BatchName { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> ScheduleId { get; set; }
        public Nullable<bool> IsExecuted { get; set; }
        public Nullable<bool> IsDataReady { get; set; }
        public Nullable<int> DataExtractionDay { get; set; }
        public Nullable<int> BatchExecutionDayOfMonth { get; set; }
    }
}
