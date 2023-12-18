// <copyright file="DM_InvestmentTransaction.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2020 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    #endregion

    public class DM_InvestmentTransaction
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public long InvestorId { get; set; }
        public long InvestmentId { get; set; }
        public long ProductId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionDesc { get; set; }
        public string WJXBFS1 { get; set; }
        public string WJXBFS2_Debit { get; set; }
        public string WJXBFS3_Credit { get; set; }
        public string WJXBFS4_Balance { get; set; }
        public string WJXBFS5_TransId { get; set; }
        public string TenantCode { get; set; }
    }
}
