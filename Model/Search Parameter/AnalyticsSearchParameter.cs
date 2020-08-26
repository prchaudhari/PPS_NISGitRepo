// <copyright file="AnalyticsSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//----------------------------------------------------------------------- 

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents Analytics search parameter model
    /// </summary>
    public class AnalyticsSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The Analytics library identifier.
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The Analytics library identifier
        /// </summary>
        private string customerName = string.Empty;

        /// <summary>
        /// The tenant code.
        /// </summary>
        private string pagename = string.Empty;
        /// <summary>
        /// The tenant code.
        /// </summary>
        private string pageTypeName = string.Empty;

        /// <summary>
        /// The image extension
        /// </summary>
        private string widget = string.Empty;

        /// <summary>
        /// the start date
        /// </summary>
        private DateTime? startDate;

        /// <summary>
        /// the end date
        /// </summary>
        private DateTime? endDate;

        /// <summary>
        /// The is active
        /// </summary>
        private bool? isDeleted = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        #endregion

        #region Public Members

        /// <summary>
        /// gets or sets the identifier.
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
        /// gets or sets the name.
        /// </summary>
        [Description("CustomerName")]
        public string CustomerName
        {
            get
            {
                return this.customerName;
            }

            set
            {
                this.customerName = value;
            }
        }

        /// <summary>
        /// gets or sets is delete mode.
        /// </summary>
        [Description("Is delete")]
        public bool? IsDeleted
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
        /// Gets or sets image extension
        /// </summary>
        public string Pagename
        {
            get
            {
                return pagename;
            }
            set
            {
                pagename = value;
            }
        }
        /// <summary>
        /// Gets or sets image extension
        /// </summary>
        public string PageTypeName
        {
            get
            {
                return pageTypeName;
            }
            set
            {
                pageTypeName = value;
            }
        }
        /// <summary>
        /// Gets or sets image extension
        /// </summary>
        public string WidgetName
        {
            get
            {
                return widget;
            }
            set
            {
                widget = value;
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

        #region Public Methods

        /// <summary>
        /// Returns true if Analytics library search parameter object is valid.
        /// </summary>
        public void IsValid()
        {
            Exception exception = new Exception();
            if (!PagingParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("PagingParameter", typeof(AnalyticsSearchParameter)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
            }

            if (!SortParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("SortParameter", typeof(AnalyticsSearchParameter)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
