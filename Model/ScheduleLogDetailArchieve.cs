// <copyright file="ScheduleLogDetailArchieve.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace nIS
{
    public class ScheduleLogDetailArchieve
    {
        public long Identifier { get; set; }
        public long ScheduleLogArchiveId { get; set; }
        public long ScheduleId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long RenderEngineId { get; set; }
        public string RenderEngineName { get; set; }
        public string RenderEngineURL { get; set; }
        public int NumberOfRetry { get; set; }
        public string Status { get; set; }
        public string LogMessage { get; set; }
        public DateTime LogDetailCreationDate { get; set; }
        public string PdfStatementPath { get; set; }
        public DateTime ArchivalDate { get; set; }
        public string TenantCode { get; set; }
    }
}
