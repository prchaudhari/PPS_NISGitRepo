// <copyright file="StatementMetadataArchive.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace nIS
{
    public class StatementMetadataArchive
    {
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public long ScheduleLogArchiveId { get; set; }
        public long StatementId { get; set; }
        public DateTime? StatementDate { get; set; }
        public string StatementPeriod { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string StatementURL { get; set; }
        public DateTime ArchivalDate { get; set; }
        public string TenantCode { get; set; }
    }
}
