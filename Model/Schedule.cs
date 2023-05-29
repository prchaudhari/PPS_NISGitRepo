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

    /// <summary>
    /// 
    /// </summary>
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
        /// This ProductBatchName
        /// </summary>
        private string productBatchName;

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
        /// The languages
        /// </summary>
        private IList<string> languages;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The product id
        /// </summary>
        private int productId;

        #endregion

        #region public members

        /// <summary>
        /// Gets or sets user Identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
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
        /// Gets or sets Product Batch Name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
       /*   [Description("ProductBatchName")]
      public string ProductBatchName
        {
            get
            {
                return this.ProductBatchName;
            }
            set
            {
                this.ProductBatchName = value;
            }
        }*/

        /// <summary>
        /// Gets or sets user Identifier.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
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
        /// <value>
        /// The description.
        /// </value>
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
        /// <value>
        /// The day of month.
        /// </value>
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
        /// <value>
        /// The hour of day.
        /// </value>
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
        /// <value>
        /// The minute of day.
        /// </value>
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
        /// <value>
        /// The start date.
        /// </value>
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
        /// <value>
        /// The end date.
        /// </value>
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
        /// <value>
        /// The status.
        /// </value>
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
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
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
        /// <value>
        /// The statement.
        /// </value>
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
        /// <value>
        ///   <c>true</c> if this instance is export to PDF; otherwise, <c>false</c>.
        /// </value>
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
        /// <value>
        /// The last updated date.
        /// </value>
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
        /// <value>
        /// The update by.
        /// </value>
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

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        [Description("Languages")]
        public IList<string> Languages
        {
            get
            {
                return this.languages;
            }
            set
            {
                this.languages = value;
            }
        }

        /// <summary>
        /// Gets or sets the recurrance pattern.
        /// </summary>
        /// <value>
        /// The recurrance pattern.
        /// </value>
        public string RecurrancePattern { get; set; }
        /// <summary>
        /// Gets or sets the repeat every day mon week year.
        /// </summary>
        /// <value>
        /// The repeat every day mon week year.
        /// </value>
        public Nullable<long> RepeatEveryDayMonWeekYear { get; set; }
        /// <summary>
        /// Gets or sets the week days.
        /// </summary>
        /// <value>
        /// The week days.
        /// </value>
        public string WeekDays { get; set; }
        /// <summary>
        /// Gets or sets the is every week day.
        /// </summary>
        /// <value>
        /// The is every week day.
        /// </value>
        public Nullable<bool> IsEveryWeekDay { get; set; }
        /// <summary>
        /// Gets or sets the month of year.
        /// </summary>
        /// <value>
        /// The month of year.
        /// </value>
        public string MonthOfYear { get; set; }
        /// <summary>
        /// Gets or sets the is ends after no of occurrences.
        /// </summary>
        /// <value>
        /// The is ends after no of occurrences.
        /// </value>
        public Nullable<bool> IsEndsAfterNoOfOccurrences { get; set; }
        /// <summary>
        /// Gets or sets the no of occurrences.
        /// </summary>
        /// <value>
        /// The no of occurrences.
        /// </value>
        public Nullable<long> NoOfOccurrences { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is recurrance pattern change.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is recurrance pattern change; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecurrancePatternChange { get; set; }
        /// <summary>
        /// Gets or sets the executed batch count.
        /// </summary>
        /// <value>
        /// The executed batch count.
        /// </value>
        public int ExecutedBatchCount { get; set; }

        /// <summary>
        /// Gets or sets the tenant code.
        /// </summary>
        /// <value>
        /// The tenant code.
        /// </value>
        public string TenantCode { get; set; }

        /// <summary>
        /// Gets or sets user Product Id.
        /// </summary>
        /// <value>
        /// The product id.
        /// </value>
        [Description("Product Id")]
        public int ProductId
        {
            get
            {
                return this.productId;
            }
            set
            {
                this.productId = value;
            }
        }
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
