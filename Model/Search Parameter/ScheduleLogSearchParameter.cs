// <copyright file="ScheduleLogSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2020 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    public class ScheduleLogSearchParameter : BaseSearchEntity
    {
        /// <summary>
        /// The schedule log identifier
        /// </summary>
        public string ScheduleLogId { get; set; }

        /// <summary>
        /// The schedule identifier
        /// </summary>
        public string ScheduleId { get; set; }

        /// <summary>
        /// The schedule name
        /// </summary>
        public string ScheduleName { get; set; }

        /// <summary>
        /// The schedule status
        /// </summary>
        public string ScheduleStatus { get; set; }

        /// <summary>
        /// The render engine id
        /// </summary>
        public string RenderEngineId { get; set; }

        /// <summary>
        /// The render engine name
        /// </summary>
        public string RenderEngineName { get; set; }

        /// <summary>
        /// The start date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Determines whether this instance of schedule log search parameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the schedule log search parameter object is valid, false otherwise.
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
