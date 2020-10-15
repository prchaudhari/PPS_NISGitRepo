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

    public class Schedule
    {
        #region Private Members
        /// <summary>
        /// The identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// The Schedule Name
        /// </summary>
        private string name;

        /// <summary>
        /// The Schdeule description
        /// </summary>
        private string description;

        /// <summary>
        /// The day of month
        /// </summary>
        private long dayOfMonth;

        /// <summary>
        /// the hour of day
        /// </summary>
        private long hourOfDay;

        /// <summary>
        /// the minnute of day
        /// </summary>
        private long minuteOfDay;

        /// <summary>
        /// the start date
        /// </summary>
        private DateTime? startDate;

        /// <summary>
        /// the end date
        /// </summary>
        private DateTime? endDate;

        /// <summary>
        /// the status
        /// </summary>
        private string status;

        /// <summary>
        /// the is active flag
        /// </summary>
        private bool isActive;


        /// <summary>
        /// the is active flag
        /// </summary>
        private Statement statement;

        /// <summary>
        /// the is isExportToPDF flag
        /// </summary>
        private bool isExportToPDF;

        /// <summary>
        /// the laste updated date
        /// </summary>
        private DateTime? lastUpdatedDate;

        /// <summary>
        /// the laste updated by
        /// </summary>
        private User updateBy;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

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
        /// Gets or sets user Identifier.
        /// </summary>
        [Description("Name")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }
        /// <summary>
        /// Gets or sets user Description.
        /// </summary>
        [Description("Description")]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        /// <summary>
        /// Gets or sets user DayOfMonth.
        /// </summary>
        [Description("DayOfMonth")]
        public long DayOfMonth
        {
            get
            {
                return this.dayOfMonth;
            }
            set
            {
                this.dayOfMonth = value;
            }
        }
        /// <summary>
        /// Gets or sets user HourOfDay.
        /// </summary>
        [Description("HourOfDay")]
        public long HourOfDay
        {
            get
            { return this.hourOfDay; }
            set
            {
                this.hourOfDay = value;
            }
        }
        /// <summary>
        /// Gets or sets user MinuteOfDay.
        /// </summary>
        [Description("MinuteOfDay")]
        public long MinuteOfDay
        {
            get
            {
                return this.minuteOfDay;
            }
            set
            {
                this.minuteOfDay = value;
            }
        }

        /// <summary>
        /// Gets or sets user StartDate.
        /// </summary>
        [Description("StartDate")]
        public DateTime? StartDate
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
        public DateTime? EndDate
        {
            get
            { return this.endDate; }
            set
            {
                this.endDate = value;
            }
        }
        /// <summary>
        /// Gets or sets user Status.
        /// </summary>
        [Description("Status")]
        public string Status
        {
            get
            { return this.status; }
            set
            {
                this.status = value;
            }
        }

        /// <summary>
        /// Gets or sets user IsActive.
        /// </summary>
        [Description("IsActive")]
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;
            }
        }

        /// <summary>
        /// Gets or sets Statement.
        /// </summary>
        [Description("Statement")]
        public Statement Statement
        {
            get
            {
                return this.statement;
            }
            set
            {
                this.statement = value;
            }
        }

        /// <summary>
        /// Gets or sets user IsExportToPDF.
        /// </summary>
        [Description("IsExportToPDF")]
        public bool IsExportToPDF
        {
            get
            {
                return this.isExportToPDF;
            }
            set
            {
                this.isExportToPDF = value;
            }
        }

        /// <summary>
        /// Gets or sets user Identifier.
        /// </summary>
        [Description("LastUpdatedDate")]
        public DateTime? LastUpdatedDate
        {
            get
            {
                return this.lastUpdatedDate;
            }
            set
            {
                this.lastUpdatedDate = value;
            }
        }
        /// <summary>
        /// Gets or sets user Identifier.
        /// </summary>
        [Description("UpdateBy")]
        public User UpdateBy
        {
            get
            {
                return this.updateBy;
            }
            set
            {
                this.updateBy = value;
            }
        }

        public string RecurrancePattern { get; set; }
        public Nullable<long> RepeatEveryDayMonWeekYear { get; set; }
        public string WeekDays { get; set; }
        public Nullable<bool> IsEveryWeekDay { get; set; }
        public string MonthOfYear { get; set; }
        public Nullable<bool> IsEndsAfterNoOfOccurrences { get; set; }
        public Nullable<long> NoOfOccurrences { get; set; }
        public bool IsRecurrancePatternChange { get; set; }
        public int ExecutedBatchCount { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of skill is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the skill object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.name))
                {
                    exception.Data.Add(this.utility.GetDescription("Name", typeof(Schedule)), ModelConstant.SCHEDULE_MODEL_SECTION + "~" + ModelConstant.INVALID_SCHEDULE_NAME);
                }
                if (!this.validationEngine.IsValidText(this.status))
                {
                    exception.Data.Add(this.utility.GetDescription("Status", typeof(Schedule)), ModelConstant.SCHEDULE_MODEL_SECTION + "~" + ModelConstant.INVALID_SCHEDULE_STATUS);
                }
                if (!this.validationEngine.IsValidLong(this.DayOfMonth, true))
                {
                    exception.Data.Add(this.utility.GetDescription("DayOfMonth", typeof(Schedule)), ModelConstant.SCHEDULE_MODEL_SECTION + "~" + ModelConstant.INVALID_SCHEDULE_DAYOFMONTH);
                }

                if (this.StartDate == null)
                {
                    exception.Data.Add(this.utility.GetDescription("StartDate", typeof(Schedule)), ModelConstant.SCHEDULE_MODEL_SECTION + "~" + ModelConstant.INVALID_SCHEDULE_ENDDATE);
                }
                //if (this.EndDate == null)
                //{
                //    exception.Data.Add(this.utility.GetDescription("EndDate", typeof(Schedule)), ModelConstant.SCHEDULE_MODEL_SECTION + "~" + ModelConstant.INVALID_SCHEDULE_STARTDATE);
                //}
                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}
