// <copyright file="DynamicWidgetSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents dynamicWidget search parameter
    /// </summary>
    public class DynamicWidgetSearchParameter : BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// The dynamicWidget identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The dynamicWidget name
        /// </summary>
        private string dynamicWidgetName = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string dynamicWidgetType = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string enityName = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string enityId = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string pageTypeName = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string status = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string createdBy = string.Empty;

        /// <summary>
        /// The dynamicWidget code
        /// </summary>
        private string pageTypeId = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public members

        /// <summary>
        /// Gets or sets dynamicWidget identifier
        /// </summary>
        [Description("Identifier")]
        public string Identifier
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
        /// Gets or sets dynamicWidget name
        /// </summary>
        [Description("DynamicWidget Name")]
        public string DynamicWidgetName
        {
            get
            {
                return this.dynamicWidgetName;
            }
            set
            {
                this.dynamicWidgetName = value;
            }
        }

        /// <summary>
        /// Gets or sets dynamicWidget code
        /// </summary>
        [Description("DynamicWidgetType")]
        public string DynamicWidgetType
        {
            get
            {
                return this.dynamicWidgetType;
            }
            set
            {
                this.dynamicWidgetType = value;
            }
        }

        /// <summary>
        /// Gets or sets EntityName code
        /// </summary>
        [Description("EntityName")]
        public string EntityName
        {
            get
            {
                return this.enityName;
            }
            set
            {
                this.enityName = value;
            }
        }

        /// <summary>
        /// Gets or sets PageTypeName code
        /// </summary>
        [Description("PageTypeName")]
        public string PageTypeName
        {
            get
            {
                return this.pageTypeName;
            }
            set
            {
                this.pageTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets CreatedBy code
        /// </summary>
        [Description("CreatedBy")]
        public string CreatedBy
        {
            get
            {
                return this.createdBy;
            }
            set
            {
                this.createdBy = value;
            }
        }

        /// <summary>
        /// Gets or sets PageTypeName code
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
        /// Gets or sets EntityId code
        /// </summary>
        [Description("EntityId")]
        public string EntityId
        {
            get
            {
                return this.enityId;
            }
            set
            {
                this.enityId = value;
            }
        }

        /// <summary>
        /// Gets or sets PageTypeId code
        /// </summary>
        [Description("PageTypeId")]
        public string PageTypeId
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
        /// The start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date
        /// </summary>
        public DateTime EndDate { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of dynamicWidget search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Paging parameter", typeof(DynamicWidget)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Sort parameter", typeof(DynamicWidget)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
