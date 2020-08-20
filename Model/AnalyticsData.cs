// <copyright file="AnalyticsData.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.ComponentModel;
    using System.Collections.Generic;
    using System;

    #endregion

    /// <summary>
    /// This class represents AnalyticsData model.
    /// </summary>
    public class AnalyticsData
    {
        #region

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
        /// Gets or sets identifier.
        /// </summary>
        [Description("Identifier")]
        public long Identifier { get; set; }

        /// <summary>
        /// Gets or sets StatementId.
        /// </summary>
        [Description("StatementId")]
        public long StatementId { get; set; }

        /// <summary>
        /// Gets or sets AccountId.
        /// </summary>
        [Description("AccountId")]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets PageWidgetId.
        /// </summary>
        [Description("PageWidgetId")]
        public long PageWidgetId { get; set; }

        /// <summary>
        /// Gets or sets CustomerId.
        /// </summary>
        [Description("CustomerId")]
        public long CustomerId { get; set; }

        /// <summary>
        /// Gets or sets CustomerId.
        /// </summary>
        [Description("CustomerName")]
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets PageId.
        /// </summary>
        [Description("PageId")]
        public long PageId { get; set; }

        /// <summary>
        /// Gets or sets CustomerId.
        /// </summary>
        [Description("PageName")]
        public string PageName { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("WidgetId")]
        public long WidgetId { get; set; }

        /// <summary>
        /// Gets or sets Widgetname.
        /// </summary>
        [Description("Widgetname")]
        public string Widgetname { get; set; }

        /// <summary>
        /// Gets or sets EventDate.
        /// </summary>
        [Description("EventDate")]
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets EventType.
        /// </summary>
        [Description("EventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets TenantCode.
        /// </summary>
        [Description("TenantCode")]
        public string TenantCode { get; set; }


        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of asset library is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the asset library object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.EventType))
                {
                    exception.Data.Add(this.utility.GetDescription("EventType", typeof(AssetLibrary)), ModelConstant.ANALYTICSDATA_MODEL_SECTION + "~" + ModelConstant.INVALID_ANALYTICS_EVENTTYPE);
                }
                if (!this.validationEngine.IsValidDate(this.EventDate))
                {
                    exception.Data.Add(this.utility.GetDescription("EventDate", typeof(AssetLibrary)), ModelConstant.ANALYTICSDATA_MODEL_SECTION + "~" + ModelConstant.INVALID_ANALYTICS_EVENTDATE);
                }
                if (!this.validationEngine.IsValidLong(this.StatementId))
                {
                    exception.Data.Add(this.utility.GetDescription("StatementId", typeof(AssetLibrary)), ModelConstant.ANALYTICSDATA_MODEL_SECTION + "~" + ModelConstant.INVALID_ANALYTICS_STMTID);
                }
                if (!this.validationEngine.IsValidLong(this.CustomerId))
                {
                    exception.Data.Add(this.utility.GetDescription("CustomerId", typeof(AssetLibrary)), ModelConstant.ANALYTICSDATA_MODEL_SECTION + "~" + ModelConstant.INVALID_ANALYTICS_CUSTID);
                }

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
