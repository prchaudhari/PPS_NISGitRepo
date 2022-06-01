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
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long ScheduleId { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsDataReady { get; set; }
        public DateTime DataExtractionDate { get; set; }
        public DateTime BatchExecutionDate { get; set; }
        public string Status { get; set; }
        public string LanguageCode { get; set; }

    }
}
