// <copyright file="WidgetSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    #endregion

    /// <summary>
    /// This class represents widget search parameter
    /// </summary>
    public class WidgetSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The widget identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The widget name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The widget status
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// Is required widget privileges
        /// </summary>
        private string pageTypeId = string.Empty;

        /// <summary>
        /// The widget status
        /// </summary>
        private bool? isPageTypeDetailsRequired = null;

        /// <summary>
        /// The Laste uodated by
        /// </summary>
        private bool? isLastUpdatedByDeatilReauired = null;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets widget identifier
        /// </summary>
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
        /// Gets or sets widget name
        /// </summary>
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
        /// Get or sets the value of widget status
        /// </summary>
        public bool? IsActive
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
        /// Get or sets the value of widget status
        /// </summary>
        public bool? IsPageTypeDetailsRequired
        {
            get
            {
                return this.isPageTypeDetailsRequired;
            }
            set
            {
                this.isPageTypeDetailsRequired = value;
            }
        }
        /// <summary>
        /// Get or sets the value of widget status
        /// </summary>
        public bool? IsLastUpdatedByDeatilReauired
        {
            get
            {
                return this.isLastUpdatedByDeatilReauired;
            }
            set
            {
                this.isLastUpdatedByDeatilReauired = value;
            }
        }

        /// <summary>
        /// Gets or sets PageTypeId
        /// </summary>
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

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of widgetSearchParameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the widget object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(ModelConstant.INVALID_PAGING_PARAMETER, ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(ModelConstant.INVALID_SORT_PARAMETER, ModelConstant.INVALID_SORT_PARAMETER);
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
