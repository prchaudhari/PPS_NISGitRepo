// <copyright file="ScheduleLogArchive.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace nIS
{
    public class ScheduleLogArchive
    {
        public long Identifier { get; set; }
        public long ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public long BatchId { get; set; }
        public string BatchName { get; set; }
        public int NumberOfRetry { get; set; }
        public string LogFilePath { get; set; }
        public DateTime LogCreationDate { get; set; }
        public string Status { get; set; }
        public DateTime ArchivalDate { get; set; }
        public string TenantCode { get; set; }
    }
}
