// <copyright file="DM_PersonalLoanTransaction.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class DM_PersonalLoanTransaction
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public long BranchId { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Description { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string OutstandingCapital { get; set; }
        public string TenantCode { get; set; }
    }
}
