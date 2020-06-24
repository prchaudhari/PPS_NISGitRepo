using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class Page
    {
        #region Private Member

        /// <summary>
        /// The Page Identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// The Page display name
        /// </summary>
        private string displayName = string.Empty;

        /// <summary>
        /// The Page(Product) type Identitifer
        /// </summary>
        private long pageTypeId;

        /// <summary>
        /// The Page published by User Identitifer
        /// </summary>
        private long publishedBy;

        /// <summary>
        /// The Page Owner User Identitifer
        /// </summary>
        private long owner;

        /// <summary>
        /// The Page version
        /// </summary>
        private string version = string.Empty;

        /// <summary>
        /// The Page status
        /// </summary>
        private string status = string.Empty;

        /// <summary>
        /// The Page created date.
        /// </summary>
        private DateTime createdDate = DateTime.MinValue;

        /// <summary>
        /// The Page published date.
        /// </summary>
        private DateTime publishedOn = DateTime.MinValue;

        /// <summary>
        ///Flag for deleted or not
        /// </summary>
        private bool isDeleted = false;

        /// <summary>
        ///Flag for isActive or not
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The Page last updated date.
        /// </summary>
        private DateTime lastUpdatedDate = DateTime.MinValue;

        /// <summary>
        /// The Update by User Identitifer
        /// </summary>
        private long updatedBy;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        #endregion

        #region Public Member

        /// <summary>
        /// gets or sets the page identifier.
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
        /// Gets or sets the page dispaly name.
        /// </summary>
        /// <value>
        /// The page display name .
        /// </value>
        [Description("DisplayName")]
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                this.displayName = value;
            }
        }

        /// <summary>
        /// gets or sets the page (product) type identifier.
        /// </summary>
        [Description("PageTypeId")]
        public long PageTypeId
        {
            get
            {
                return this.pageTypeId;
            }

            set
            {
                this.pageTypeId = value;
            }
        }

        /// <summary>
        /// gets or sets the page published by user identifier.
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
        /// gets or sets the page owner user identifier.
        /// </summary>
        [Description("PageOwner")]
        public long PageOwner
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
        /// Gets or sets is page version
        /// </summary>
        /// <value>
        /// Value indicates is page version
        /// </value>
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
        /// Gets or sets is page status
        /// </summary>
        /// <value>
        /// Value indicates is page status
        /// </value>
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
        /// Gets or sets is created date
        /// </summary>
        /// <value>
        /// Value indicates is created date
        /// </value>
        [Description("CreatedDate")]
        public DateTime CreatedDate
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
        /// Gets or sets is published date
        /// </summary>
        /// <value>
        /// Value indicates is published date
        /// </value>
        [Description("PublishedOn")]
        public DateTime PublishedOn
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
        /// gets or sets the isActive.
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
        /// gets or sets the IsDeleted.
        /// </summary>
        [Description("IsDeleted")]
        public bool IsDeleted
        {
            get
            {
                return this.isDeleted;
            }

            set
            {
                this.isDeleted = value;
            }
        }

        /// <summary>
        /// Gets or sets is page last updated date
        /// </summary>
        /// <value>
        /// Value indicates is page last updated date
        /// </value>
        [Description("PageLastUpdatedDate")]
        public DateTime LastUpdatedDate
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
        /// gets or sets the page last updated by user identifier.
        /// </summary>
        [Description("LastUpdatedBy")]
        public long UpdatedBy
        {
            get
            {
                return this.updatedBy;
            }

            set
            {
                this.updatedBy = value;
            }
        }

        /// <summary>
        /// The tenant code
        /// </summary>
        public string TenantCode = string.Empty;

        public IList<PageWidget> PageWidgets { get; set; }

        public string PageOwnerName { get; set; }

        public string PagePublishedByUserName { get; set; }

        public string PageTypeName { get; set; }

        public string PageUpdatedByUserName { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method helps to validate the model
        /// </summary>

        public void IsValid()
        {
            Exception exception = new Exception();

            IUtility utility = new Utility();
            ValidationEngine validationEngine = new ValidationEngine();

            if (!validationEngine.IsValidLong(this.Identifier, false))
            {
                exception.Data.Add(utility.GetDescription("Identifier", typeof(Page)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_ID);
            }
            if (!validationEngine.IsValidText(this.DisplayName, false))
            {
                exception.Data.Add(utility.GetDescription("DisplayName", typeof(Page)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_DISPLAY_NAME);
            }
            if (!validationEngine.IsValidLong(this.PageTypeId, false))
            {
                exception.Data.Add(utility.GetDescription("PageTypeId", typeof(Page)), ModelConstant.PAGE_SECTION + "~" + ModelConstant.INVALID_PAGE_TYPE_ID);
            }
            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
