// <copyright file="DM_InvestmentMaster.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
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

    public class DM_InvestmentMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public long InvestmentId { get; set; }
        public long BranchId { get; set; }
        public long ProductId { get; set; }
        public string ProductType { get; set; }
        public string ProductDesc { get; set; }
        public string Currenacy { get; set; }
        public DateTime? AccountOpenDate { get; set; }
        public string CurrentInterestRate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string InterestDisposalDesc { get; set; }
        public string NoticePeriod { get; set; }
        public string AccuredInterest { get; set; }
        public DateTime? StatementDate { get; set; }
        public string DayOfStatement { get; set; }
        public string StatementPeriod { get; set; }
        public string ClosingBalance { get; set; }
        public string TenantCode { get; set; }
        public List<DM_InvestmentTransaction> investmentTransactions { get; set; }
        public string BonusInterest { get; set; }
}
}
