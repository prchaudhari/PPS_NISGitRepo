﻿// <copyright file="Widget.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents the widget entity
    /// </summary>
    public class Widget : BaseEntity
    {
        /// <summary>
        /// The Widget Icon
        /// </summary>
        public string WidgetIcon { get; set; }

        #region Private member

        /// <summary>
        /// The Widget Identifier
        /// </summary>
        private long identifier;

        /// <summary>
        /// The widget settings
        /// </summary>
        private object widgetSetting = new object();

        /// <summary>
        /// The widget name
        /// </summary>
        private string widgetName = string.Empty;

        /// <summary>
        /// The Product Types
        /// </summary>
        private IList <ProductType> productTypes= new List<ProductType>();

        /// <summary>
        /// The Last Updated Date
        /// </summary>
        private DateTime lastUpdatedDate = DateTime.UtcNow;

        /// <summary>
        /// The UpdatedBy
        /// </summary>
        private User updatedBy = new User();

        /// <summary>
        ///Flag for isConfigurable or not
        /// </summary>
        private bool isConfigurable = true;

        /// <summary>
        /// The validation engine object
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        ///Flag for isActive or not
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// The tenent code.
        /// </summary>
        string tenantCode = string.Empty;

        #endregion

        #region Public member        

        /// <summary>
        /// gets or sets the widget identifier.
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
        /// Gets or sets the widget settings.
        /// </summary>
        /// <value>
        /// The widget settings.
        /// </value>
        [Description("WidgetSetting")]
        public object WidgetSetting
        {
            get
            {
                return this.widgetSetting;
            }

            set
            {
                this.widgetSetting = value;
            }
        }

        /// <summary>
        /// Gets or sets the widget name.
        /// </summary>
        /// <value>
        /// The widget name .
        /// </value>
        [Description("WidgetName")]
        public string WidgetName
        {
            get
            {
                return this.widgetName;
            }

            set
            {
                this.widgetName = value;
            }
        }

        /// <summary>
        /// Gets or sets the product type.
        /// </summary>
        /// <value>
        /// The product type.
        /// </value>
        [Description("ProductTypes")]
        public IList<ProductType> ProductTypes
        {
            get
            {
                return this.productTypes;
            }

            set
            {
                this.productTypes = value;
            }
        }

        /// <summary>
        /// gets or sets the tenant code.
        /// </summary>
        [Description("TenantCode")]
        public string TenantCode
        {
            get
            {
                return this.tenantCode;
            }

            set
            {
                this.tenantCode = value;
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
        /// gets or sets the isConfigurable.
        /// </summary>
        [Description("IsConfigurable")]
        public bool IsConfigurable
        {
            get
            {
                return this.isConfigurable;
            }

            set
            {
                this.isConfigurable = value;
            }
        }

        /// <summary>
        /// gets or sets the LastUpdatedDate.
        /// </summary>
        [Description("LastUpdatedDate")]
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
        /// gets or sets the UpdatedBy.
        /// </summary>
        [Description("UpdatedBy")]
        public User UpdatedBy
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
                exception.Data.Add(utility.GetDescription("Identifier", typeof(Widget)), ModelConstant.WIDGET_SECTION + "~" + ModelConstant.INVALID_WIDGET_ID);
            }
            if (!validationEngine.IsValidText(this.WidgetName, false))
            {
                exception.Data.Add(utility.GetDescription("WidgetName", typeof(Widget)), ModelConstant.WIDGET_SECTION + "~" + ModelConstant.INVALID_WIDGET_NAME);
            }
            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
