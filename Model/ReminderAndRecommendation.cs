// <copyright file="ReminderAndRecommendation.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class ReminderAndRecommendation
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public string TenantCode { get; set; }
    }

    public class NewsAlert
    {
        public long Identifier { get; set; }
        public long BatchId { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string Message3 { get; set; }
        public string Message4 { get; set; }
        public string Message5 { get; set; }
        public string TenantCode { get; set; }
    }
}
