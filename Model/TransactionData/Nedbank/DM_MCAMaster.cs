// <copyright file="DM_MCAMaster.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class DM_MCAMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public string VatNo { get; set; }
        public decimal? OverdraftLimit { get; set; }
        public string Currency { get; set; }
        public decimal? FreeBalance { get; set; }
        public string StatementNo { get; set; }
        public DateTime StatementDate { get; set; }
        public string StatementFrequency { get; set; }
        public List<DM_MCATransaction> MCATransactions { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_MCATransaction
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public DateTime Transaction_Date { get; set; }
        public string Description { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Credit { get; set; }
        public long? Days { get; set; }
        public decimal? AccuredInterest { get; set; }
        public string TenantCode { get; set; }
    }
}
