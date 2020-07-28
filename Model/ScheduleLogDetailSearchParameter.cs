// <copyright file="ScheduleLogDetailSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    public class ScheduleLogDetailSearchParameter: BaseSearchEntity
    {
        /// <summary>
        /// The schedule log identifier
        /// </summary>
        public string ScheduleLogId { get; set; }

        /// <summary>
        /// The schedule log detail identifier
        /// </summary>
        public string ScheduleLogDetailId { get; set; }

        /// <summary>
        /// The customer identifier
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The customer name
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Determines whether this instance of schedule log detail search parameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the schedule log detail search parameter object is valid, false otherwise.
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
    }
}
