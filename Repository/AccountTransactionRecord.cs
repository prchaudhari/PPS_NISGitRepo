//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace nIS
{
    using System;
    using System.Collections.Generic;
    
    public partial class AccountTransactionRecord
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string AccountType { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string Narration { get; set; }
        public string FCY { get; set; }
        public string CurrentRate { get; set; }
        public string LCY { get; set; }
        public string TenantCode { get; set; }
    }
}
