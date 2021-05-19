// <copyright file="DM_ReminderAndRecommendation.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    public class DM_ReminderAndRecommendation
    {
        public long Identifier { get; set; }
        public string Description { get; set; }
        public bool IsGeneric { get; set; }
        public bool IsActionable { get; set; }
        public string ActionTitle { get; set; }
        public string ActionUrl { get; set; }
        public string TenantCode { get; set; }
    }

    public class DM_CustomerReminderAndRecommendation
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public DM_ReminderAndRecommendation reminderAndRecommendation { get; set; }
        public string TenantCode { get; set; }
    }
}
