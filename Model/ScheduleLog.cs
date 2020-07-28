// <copyright file="ScheduleLog.cs" company="Websym Solutions Pvt Ltd">
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

    public class ScheduleLog
    {
        /// <summary>
        /// The schedule log identifier
        /// </summary>
        public long Identifier { get; set; }

        /// <summary>
        /// The schedule identifier
        /// </summary>
        public long ScheduleId { get; set; }

        /// <summary>
        /// The schedule name
        /// </summary>
        public string ScheduleName { get; set; }

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
        /// The schedule log file path
        /// </summary>
        public string LogFilePath { get; set; }

        /// <summary>
        /// The schedule log created date
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The complete schedule run processing time
        /// </summary>
        public string ProcessingTime { get; set; }

        /// <summary>
        /// The number of records proccessed in schedule run
        /// </summary>
        public string RecordProcessed { get; set; }

        /// <summary>
        /// The schedule run status
        /// </summary>
        public string ScheduleStatus { get; set; }
    }
}
