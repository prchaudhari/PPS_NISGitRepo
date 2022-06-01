// <copyright file="DM_PersonalLoanMaster.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class DM_PersonalLoanMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public long BranchId { get; set; }
        public string Currency { get; set; }
        public string ProductType { get; set; }
        public string CreditAdvance { get; set; }
        public string OutstandingBalance { get; set; }
        public string AmountDue { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string MonthlyInstallment { get; set; }
        public DateTime DueDate { get; set; }
        public string Arrears { get; set; }
        public string AnnualRate { get; set; }
        public string Term { get; set; }
        public string TenantCode { get; set; }
        public List<string> Messages { get; set; }
        public List<DM_PersonalLoanTransaction> LoanTransactions { get; set; }
        public DM_PersonalLoanArrears LoanArrears { get; set; }
    }
}
