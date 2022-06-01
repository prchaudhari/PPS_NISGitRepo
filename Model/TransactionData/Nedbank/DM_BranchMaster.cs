// <copyright file="DM_BranchMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class DM_BranchMaster
    {
        public long Identifier { get; set; }
        public string BranchName { get; set; }
        public string AddressLine0 { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string VatRegNo { get; set; }
        public string ContactNo { get; set; }
        public string TenantCode { get; set; }
    }
}
