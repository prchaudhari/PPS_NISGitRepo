// <copyright file="DM_ExplanatoryNote.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class DM_ExplanatoryNote
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long ParentId { get; set; } //InvestmentId, HomeLoanId, PersonalLoanId, etc..
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }
        public string Note5 { get; set; }
        public string TenantCode { get; set; }
    }
}
