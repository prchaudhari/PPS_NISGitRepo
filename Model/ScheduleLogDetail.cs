// <copyright file="ScheduleLogDetail.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public class ScheduleLogDetail
    {
        /// <summary>
        /// The schedule log detail identifier
        /// </summary>
        public long Identifier { get; set; }

        /// <summary>
        /// The schedule log identifier
        /// </summary>
        public long ScheduleLogId { get; set; }

        /// <summary>
        /// The schedule identifier
        /// </summary>
        public long ScheduleId { get; set; }

        /// <summary>
        /// The customer identifier
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        /// The customer name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// The render engine identifier
        /// </summary>
        public long RenderEngineId { get; set; }

        /// <summary>
        /// The render engine name
        /// </summary>
        public string RenderEngineName { get; set; }

        /// <summary>
        /// The render engine url
        /// </summary>
        public string RenderEngineURL { get; set; }

        /// <summary>
        /// The number of retry of schedule run attempt
        /// </summary>
        public int NumberOfRetry { get; set; }

        /// <summary>
        /// The schedule log created date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The Log message
        /// </summary>
        public string LogMessage { get; set; }

        public string StatementFilePath { get; set; }

        public IList<StatementMetadata> statementMetadata { get; set; }
    }
}
