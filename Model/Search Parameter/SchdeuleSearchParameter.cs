// <copyright file="ScheduleSearchParameter.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represents role search parameter
    /// </summary>
    public class ScheduleSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The role identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The role name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The role status
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// the start date
        /// </summary>
        private DateTime? startDate;

        /// <summary>
        /// the end date
        /// </summary>
        private DateTime? endDate;

        /// <summary>
        /// isStatementDefinitionRequired
        /// </summary>
        private bool? isStatementDefinitionRequired;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets role identifier
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
        /// Gets or sets role name
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
        /// Gets or sets IsStatementDefinitionRequired
        /// </summary>
        public bool? IsStatementDefinitionRequired
        {
            get
            {
                return this.isStatementDefinitionRequired;
            }
            set
            {
                this.isStatementDefinitionRequired = value;
            }
        }

        /// <summary>
        /// Get or sets the value of role status
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
        /// The start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets user Status.
        /// </summary>
        [Description("StatementDefinitionId")]
        public string StatementDefinitionName { get; set; }

        /// <summary>
        /// Gets or sets ScheduleName Status.
        /// </summary>
        [Description("ScheduleName")]
        public string ScheduleName { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of roleSearchParameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the role object is valid, false otherwise.
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
