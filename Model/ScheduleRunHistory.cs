// <copyright file="User.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------


namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class ScheduleRunHistory
    {
        #region Private Members
        /// <summary>
        /// The identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// the start date
        /// </summary>
        private DateTime startDate;

        /// <summary>
        /// the end date
        /// </summary>
        private DateTime endDate;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        private string filePath = string.Empty;
        #endregion
        #region public members

        /// <summary>
        /// Gets or sets user Identifier.
        /// </summary>
        [Description("Identifier")]
        public long Identifier
        {
            get
            {
                return this.identifier;
            }
            set
            {
                this.identifier = value;
            }
        }

        /// <summary>
        /// Gets or sets user Schedule.
        /// </summary>
        [Description("Schedule")]
        public Schedule Schedule { get; set; }

        /// <summary>
        /// Gets or sets user StartDate.
        /// </summary>
        [Description("StartDate")]
        public DateTime StartDate
        {
            get
            { return this.startDate; }
            set
            {
                this.startDate = value;
            }
        }

        /// <summary>
        /// Gets or sets user EndDate.
        /// </summary>
        [Description("EndDate")]
        public DateTime EndDate
        {
            get
            { return this.endDate; }
            set
            {
                this.endDate = value;
            }
        }

        public string StatementFilePath
        {
            get
            { return this.filePath; }
            set
            {
                this.filePath = value;
            }
        }
        #endregion

    }
}
