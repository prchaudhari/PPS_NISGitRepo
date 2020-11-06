// <copyright file="AccountMaster.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class AccountMaster
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public long CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }
        public string Balance { get; set; }
        public string TotalDeposit { get; set; }
        public string TotalSpend { get; set; }
        public string ProfitEarned { get; set; }
        public string Indicator { get; set; }
        public string FeesPaid { get; set; }
        public string GrandTotal { get; set; }
        public string Percentage { get; set; }
        public string TenantCode { get; set; }
    }
}
