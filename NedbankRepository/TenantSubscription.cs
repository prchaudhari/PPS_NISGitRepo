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
    
    public partial class TenantSubscription
    {
        public long Id { get; set; }
        public System.Guid TenantCode { get; set; }
        public System.DateTime SubscriptionStartDate { get; set; }
        public System.DateTime SubscriptionEndDate { get; set; }
        public long LastModifiedBy { get; set; }
        public System.DateTime LastModifiedOn { get; set; }
        public string SubscriptionKey { get; set; }
    }
}
