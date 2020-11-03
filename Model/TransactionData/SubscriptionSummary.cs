// <copyright file="SubscriptionSummary.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class SubscriptionSummary
    {
        public long Identifier { get; set; }
        public string Vendor { get; set; }
        public string Subscription { get; set; }
        public decimal Total { get; set; }
        public decimal AverageSpend { get; set; }
        public string TenantCode { get; set; }
    }
}
