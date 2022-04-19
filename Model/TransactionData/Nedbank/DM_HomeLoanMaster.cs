// <copyright file="DM_HomeLoanMaster.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class DM_HomeLoanMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public string Currency { get; set; }
        public string LoanAmount { get; set; }
        public string Balance { get; set; }
        public string IntialDue { get; set; }
        public string ArrearStatus { get; set; }
        public string ChargeRate { get; set; }
        public string LoanTerm { get; set; }
        public string SecDescription1 { get; set; }
        public string SecDescription2 { get; set; }
        public string SecDescription3 { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string RegisteredAmount { get; set; }
        public DM_HomeLoanArrear LoanArrear { get; set; }
        public DM_HomeLoanSummary LoanSummary { get; set; }
        public List<DM_HomeLoanTransaction> LoanTransactions { get; set; }
        public string TenantCode { get; set; }
        public string SegmentType { get; set; }
    }

    public class DM_HomeLoanArrear
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public string CurrentDue { get; set; }
        public string ARREARS_30 { get; set; }
        public string ARREARS_60 { get; set; }
        public string ARREARS_90 { get; set; }
        public string ARREARS_120 { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_HomeLoanSummary
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public string Annual_Interest { get; set; }
        public string Annual_Insurance { get; set; }
        public string Annual_Service_Fee { get; set; }
        public string Annual_Legal_Costs { get; set; }
        public string Annual_Total_Recvd { get; set; }
        public string Basic_Instalment { get; set; }
        public string Houseowner_Ins { get; set; }
        public string Loan_Protection { get; set; }
        public string Recovery_Fee_Debit { get; set; }
        public string Capital_Redemption { get; set; }
        public string Total_Instalment { get; set; }
        public string Service_Fee { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_HomeLoanTransaction
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public DateTime Posting_Date { get; set; }
        public DateTime Effective_Date { get; set; }
        public string Description { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string RunningBalance { get; set; }
        public string TenantCode { get; set; }
    }
}
