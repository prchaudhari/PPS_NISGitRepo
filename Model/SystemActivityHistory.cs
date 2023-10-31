// <copyright file="SystemActivityHistory.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2021 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    #endregion

    public class SystemActivityHistory
    {
        [Key]
        public long Identifier { get; set; }
        public string Module { get; set; }
        public long EntityId { get; set; }
        public string EntityName { get; set; }
        public long? SubEntityId { get; set; }
        public string SubEntityName { get; set; }
        public string ActionTaken { get; set; }
        public long ActionTakenBy { get; set; }
        public string ActionTakenByUserName { get; set; }
        public DateTime ActionTakenDate { get; set; }
        public string TenantCode { get; set; }
    }
}
