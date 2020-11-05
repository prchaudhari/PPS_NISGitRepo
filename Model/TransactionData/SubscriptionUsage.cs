// <copyright file="SubscriptionUsage.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class SubscriptionUsage
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public string Subscription { get; set; }
        public string VendorName { get; set; }
        public string Email { get; set; }
        public decimal Usage { get; set; }
        public long Emails { get; set; }
        public long Meetings { get; set; }
        public string TenantCode { get; set; }
    }
}
