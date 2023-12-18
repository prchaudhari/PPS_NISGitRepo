// <copyright file="DM_MarketingMessage.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class DM_MarketingMessage
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long ParentId { get; set; } //InvestmentId, HomeLoanId, PersonLoanId, etc...
        public string Header { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string Message3 { get; set; }
        public string Message4 { get; set; }
        public string Message5 { get; set; }
        public string Type { get; set; }
        public string TenantCode { get; set; }
    }
}
