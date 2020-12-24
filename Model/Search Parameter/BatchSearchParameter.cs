// <copyright file="BatchSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents batch search parameter
    /// </summary>
    /// 
    public class BatchSearchParameter: BaseSearchEntity
    {
        /// <summary>
        /// The batch identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The schedule identifier
        /// </summary>
        public string ScheduleId { get; set; }

        /// <summary>
        /// The from date
        /// </summary>
        public DateTime FromDate { get; set; }
        
        /// <summary>
        /// The to date
        /// </summary>
        public DateTime ToDate { get; set; }

        public string Status { get; set; }
        public bool? IsExecuted { get; set; }
    }
}
