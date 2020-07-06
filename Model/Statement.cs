// <copyright file="Statement.cs" company="Websym Solutions Pvt. Ltd.">
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

    public class Statement
    {
        #region Private Member
        /// <summary>
        /// The Identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// The name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The description
        /// </summary>
        private string description = string.Empty;

        /// <summary>
        /// The publishedBy
        /// </summary>
        private long publishedBy = new long();

        /// <summary>
        /// The owner
        /// </summary>
        private long owner = new long();

        /// <summary>
        /// The version
        /// </summary>
        private string version = string.Empty;

        /// <summary>
        /// The status
        /// </summary>
        private string status = string.Empty;

        /// <summary>
        /// The createdDate
        /// </summary>
        private DateTime? createdDate;

        /// <summary>
        /// The publishedOn
        /// </summary>
        private DateTime? publishedOn;

        /// <summary>
        /// The isActive
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The lastUpdatedDate
        /// </summary>
        private DateTime? lastUpdatedDate;

        /// <summary>
        /// The updateBy
        /// </summary>
        private long updateBy;

        /// <summary>
        /// The Statement Pages
        /// </summary>
        private IList<StatementPage> statementPages = new List<StatementPage>();

        #endregion

        #region Public Member

        /// <summary>
        /// Gets or sets user first name.
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
        /// Gets or sets user first name.
        /// </summary>
        [Description("Name")]
        public string Name
        {
            get
            {
                return
                  this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets user first name.
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
        /// Gets or sets user first name.
        /// </summary>
        [Description("PublishedBy")]
        public long PublishedBy
        {
            get
            {
                return this.publishedBy;
            }
            set
            {
                this.publishedBy = value;
            }
        }

        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        [Description("Owner")]
        public long Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        [Description("Version")]
        public string Version
        {
            get
            {
                return this.version;
            }
            set
            {
                this.version = value;
            }
        }

        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        [Description("Status")]
        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }
        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        [Description("CreatedDate")]
        public DateTime? CreatedDate
        {
            get
            {
                return this.createdDate;
            }
            set
            {
                this.createdDate = value;
            }
        }

        /// <summary>
        /// Gets or sets user first name.
        /// </summary>
        [Description("PublishedOn")]
        public DateTime? PublishedOn
        {
            get
            {
                return this.publishedOn;
            }
            set
            {
                this.publishedOn = value;
            }
        }

        /// <summary>
        /// Gets or sets user first name.
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
        /// Gets or sets user first name.
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
        /// Gets or sets user first name.
        /// </summary>
        [Description("UpdateBy")]
        public long UpdateBy
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
        /// Gets or sets user first name.
        /// </summary>
        [Description("StatementPages")]
        public IList<StatementPage> StatementPages
        {
            get
            {
                return this.statementPages;
            }
            set
            {
                this.statementPages = value;
            }
        }
        /// <summary>
        /// The StatementOwnerName
        /// </summary>
        public string StatementOwnerName { get; set; }

        /// <summary>
        /// StatementPublishedByUserName
        /// </summary>
        public string StatementPublishedByUserName { get; set; }

        /// <summary>
        /// StatementTypeName
        /// </summary>
        public string StatementTypeName { get; set; }

        /// <summary>
        /// StatementUpdatedByUserName
        /// </summary>
        public string StatementUpdatedByUserName { get; set; }
        #endregion

        #region

        public void IsValid()
        {


            Exception exception = new Exception();

            IUtility utility = new Utility();
            ValidationEngine validationEngine = new ValidationEngine();
            if (!validationEngine.IsValidText(this.Name, false))
            {
                exception.Data.Add(utility.GetDescription("Name", typeof(Statement)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_DISPLAY_NAME);
            }
            if (this.statementPages.Count==0)
            {
                exception.Data.Add(utility.GetDescription("StatementPages", typeof(Statement)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_TYPE_ID);
            }
            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }
        #endregion
    }
}

