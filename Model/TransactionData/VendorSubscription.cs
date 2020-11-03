// <copyright file="VendorSubscription.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class VendorSubscription
    {
        public long Identifier { get; set; }
        public string VenderName { get; set; }
        public long CountOfSubscription { get; set; }
        public string TenantCode { get; set; }
    }
}
