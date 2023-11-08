// <copyright file="ScheduleSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    using nIS.Models;
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents ETL Schedule Batch Log search parameter
    /// </summary>
    public class ETLScheduleBatchLogSearchParameter
    {
        #region Private Members

        /// <summary>
        /// the batch id
        /// </summary>
        private long eTLBatchId;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets batch id
        /// </summary>
        public long ETLBatchId
        {
            get
            {
                return this.eTLBatchId;
            }
            set
            {
                this.eTLBatchId = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of ETLScheduleBatchLogSearchParameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the role object is valid, false otherwise.
        /// </returns>
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
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
