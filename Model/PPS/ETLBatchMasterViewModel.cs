// <copyright file="BatchMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS.Models.PPS
{
    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class ETLBatchMasterViewModel
    {
        public long Identifier { get; set; }
        public string TenantCode { get; set; }
        public string BatchName { get; set; }
        public long ETLScheduleId { get; set; }
        public string ETLScheduleName { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? DataExtractionDate { get; set; }
        public DateTime BatchExecutionDate { get; set; }
        public string Status { get; set; }
        public long ScheduleId { get; set; }
        public string Ids { get; set; }
        public int RecordProcess { get; set; }
        public int RecordRecieved { get; set; }
    }
}
