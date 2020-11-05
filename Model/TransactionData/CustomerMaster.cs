// <copyright file="SubscriptionMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace nIS
{
    public class CustomerMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public string CustomerCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public DateTime? StatementDate { get; set; }
        public string StatementPeriod { get; set; }
        public string TenantCode { get; set; }
    }
}
