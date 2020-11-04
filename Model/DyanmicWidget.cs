// <copyright file="DynamicWidget.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion
    /// <summary>
    /// This class represents dynamicWidget model
    /// </summary>
    public class DynamicWidget
    {
        #region Public Members

        public long Identifier { get; set; }
        public string WidgetName { get; set; }
        public string WidgetType { get; set; }
        public long PageTypeId { get; set; }

        public string PageTypeName { get; set; }
        public long EntityId { get; set; }

        public string EntityName { get; set; }
        public string Title { get; set; }
        public string ThemeType { get; set; }
        public string ThemeCSS { get; set; }

        public string PreviewData = string.Empty;
        public string WidgetSettings { get; set; }
        public string WidgetFilterSettings { get; set; }
        public string Status { get; set; }
        public long CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public string Version { get; set; }

        public System.DateTime CreatedOn { get; set; }
        public long LastUpdatedBy { get; set; }
        public Nullable<long> PublishedBy { get; set; }
        public string PublishedByName { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantCode { get; set; }
        public string APIPath { get; set; }
        public string RequestType { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method validates current dynamicWidget model
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
               

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
