// <copyright file="SavingTrend.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class SavingTrend
    {
        public long Identifier { get; set; }
        public long AccountId { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string Month { get; set; }
        public int NumericMonth { get; set; }
        public decimal SpendAmount { get; set; }
        public decimal SpendPercentage { get; set; }
        public decimal Income { get; set; }
        public decimal IncomePercentage { get; set; }
    }
}
