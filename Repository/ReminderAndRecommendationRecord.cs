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
    
    public partial class ReminderAndRecommendationRecord
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string Description { get; set; }
        public string LabelText { get; set; }
        public string TargetURL { get; set; }
        public string TenantCode { get; set; }
    }
}
