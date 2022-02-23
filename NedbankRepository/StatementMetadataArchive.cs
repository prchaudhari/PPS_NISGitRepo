//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NedbankRepository
{
    using System;
    using System.Collections.Generic;
    
    public partial class StatementMetadataArchive
    {
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public long ScheduleLogArchiveId { get; set; }
        public long StatementId { get; set; }
        public Nullable<System.DateTime> StatementDate { get; set; }
        public string StatementPeriod { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string StatementURL { get; set; }
        public System.DateTime ArchivalDate { get; set; }
        public string TenantCode { get; set; }
        public Nullable<bool> IsPasswordGenerated { get; set; }
        public string Password { get; set; }
    }
}
