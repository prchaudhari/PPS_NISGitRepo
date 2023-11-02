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
    /// This class represents ETL Schedule search parameter
    /// </summary>
    public class ETLScheduleSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The ETL Schedule identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The product
        /// </summary>
        private long productBatchId = 0;


        /// <summary>
        /// The product id
        /// </summary>
        private long productId = 0;

        /// <summary>
        /// The name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// the schedule date
        /// </summary>
        private DateTime? scheduleDate;

        /// <summary>
        /// the start date
        /// </summary>
        private DateTime? startDate;

        /// <summary>
        /// the end date
        /// </summary>
        private DateTime? endDate;

        /// <summary>
        /// The etl schedule status
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// The etl schedule Deleted
        /// </summary>
        private bool? isDeleted = null;
        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets ETL Schedule identifier
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
        /// Gets or sets Product
        /// </summary>
        public long ProductBatchId
        {
            get
            {
                return this.productBatchId;
            }
            set
            {
                this.productBatchId = value;
            }
        }

        /// <summary>
        /// Gets or sets Product Id
        /// </summary>
        public long ProductId
        {
            get
            {
                return this.productId;
            }
            set
            {
                this.productId = value;
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
        /// The Schedule Date
        /// </summary>
        public DateTime ScheduleDate { get; set; }

        /// <summary>
        /// The start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Get or sets the value of etl schedule status
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
        /// Get or sets the value of etl schedule Deleted
        /// </summary>
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
        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of ETLScheduleSearchParameter is valid.
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
                throw;
            }
        }

        #endregion
    }
}
