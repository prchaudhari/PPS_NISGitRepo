// <copyright file="EmailsBySubscription.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class EmailsBySubscription
    {
        public long Identifier { get; set; }
        public string Subscription { get; set; }
        public long Emails { get; set; }
        public string TenantCode { get; set; }
    }
}
