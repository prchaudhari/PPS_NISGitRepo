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
    public class DashboardReportSearchParameter : BaseSearchEntity
    {
        #region Private Members


        /// <summary>
        /// The Analytics library identifier
        /// </summary>
        private string groupTenantCode = string.Empty;

        /// <summary>
        /// The tenant code.
        /// </summary>
        private string tenantTenantCode = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        #endregion

        #region Public Members


        /// <summary>
        /// gets or sets the groupTenantCode.
        /// </summary>
        [Description("GroupTenantCode")]
        public string GroupTenantCode
        {
            get
            {
                return this.groupTenantCode;
            }

            set
            {
                this.groupTenantCode = value;
            }
        }

        /// <summary>
        /// Gets or sets image extension
        /// </summary>
        public string TenantTenantCode
        {
            get
            {
                return this.tenantTenantCode;
            }
            set
            {
                this.tenantTenantCode = value;
            }
        }

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
