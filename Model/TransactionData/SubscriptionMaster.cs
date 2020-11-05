// <copyright file="SubscriptionMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace nIS
{
    public class SubscriptionMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string VendorName { get; set; }
        public string Subscription { get; set; }
        public string EmailId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TenantCode { get; set; }
    }
}
