// <copyright file="DM_AccountsSummary.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class DM_NewsAndAlerts
    {
        public long Identifier { get; set; }
        public string Description { get; set; }
        public bool IsGeneric { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_CustomerNewsAndAlert
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public DM_NewsAndAlerts NewsAndAlert { get; set; }
        public string TenantCode { get; set; }
    }
}
