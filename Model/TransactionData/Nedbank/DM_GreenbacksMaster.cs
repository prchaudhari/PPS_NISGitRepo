// <copyright file="DM_GreenbacksMaster.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class DM_GreenbacksMaster
    {
        public long Identifier { get; set; }
        public string JoinUsUrl { get; set; }
        public string UseUsUrl { get; set; }
        public string ContactNumber { get; set; }
        public string TenantCode { get; set; }

    }

    public class DM_GreenbacksRewardPoints
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string Month { get; set; }
        public decimal RewardPoint { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_GreenbacksRewardPointsRedeemed
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string Month { get; set; }
        public decimal RedeemedPoints { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_CustomerProductWiseRewardPoints
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string AccountType { get; set; }
        public List<DM_MonthwiseProductRewardPoints> MonthwiseRewardPoints { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_MonthwiseProductRewardPoints
    {
        public long Identifier { get; set; }
        public string Month { get; set; }
        public decimal RewardPoint { get; set; }
    }

    public class DM_CustomerRewardSpendByCategory
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string Category { get; set; }
        public decimal SpendReward { get; set; }
        public string TenantCode { get; set; }
    }
}
