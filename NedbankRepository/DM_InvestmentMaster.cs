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
    
    public partial class DM_InvestmentMaster
    {
        public long Id { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public long InvestmentId { get; set; }
        public long BranchId { get; set; }
        public long ProductId { get; set; }
        public string ProductType { get; set; }
        public string Currency { get; set; }
        public string ProductDesc { get; set; }
        public Nullable<System.DateTime> AccountOpenDate { get; set; }
        public string CurrentInterestRate { get; set; }
        public Nullable<System.DateTime> ArchiveDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<long> InterestDisposalId { get; set; }
        public string InterestDisposalDesc { get; set; }
        public string NoticePeriod { get; set; }
        public string AccuredInterest { get; set; }
        public Nullable<System.DateTime> StatementDate { get; set; }
        public string DayOfStatement { get; set; }
        public string StatementPeriod { get; set; }
        public string ClosingBalance { get; set; }
        public string TenantCode { get; set; }
    }
}
