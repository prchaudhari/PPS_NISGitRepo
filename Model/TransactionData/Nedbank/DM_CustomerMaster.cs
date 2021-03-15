// <copyright file="DM_CustomerMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class DM_CustomerMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string DS_Investor_Name { get; set; }
        public string AddressLine0 { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string EmailAddress { get; set; }
        public string Mask_Cell_No { get; set; }
        public string Barcode { get; set; }
        public string TenantCode { get; set; }
    }
}
