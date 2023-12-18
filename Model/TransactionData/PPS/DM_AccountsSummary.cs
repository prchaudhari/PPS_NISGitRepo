// <copyright file="DM_AccountsSummary.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class DM_AccountsSummary
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string AccountType { get; set; }
        public string TotalAmount { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_AccountAnanlysis
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string AccountType { get; set; }
        public List<DM_MonthwiseAccountAnanlysis> MonthwiseAmount { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_MonthwiseAccountAnanlysis
    {
        public long Identifier { get; set; }
        public string Month { get; set; }
        public decimal Amount { get; set; }
    }

}
