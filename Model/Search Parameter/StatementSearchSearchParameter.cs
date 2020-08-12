// <copyright file="StatementSearchSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  


namespace nIS
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    /// <summary>
    /// This class represents page search parameter
    /// </summary>
    public class StatementSearchSearchParameter : BaseSearchEntity
    {
        #region Private members
        /// <summary>
        /// The validation engine object
        /// </summary>
        IValidationEngine validationEngine = null;
        #endregion

        #region Public Members

        /// <summary>
        /// The page identifier
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The page dispaly name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The page status
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// The page status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The start date
        /// </summary>
        public DateTime StatementStartDate { get; set; }

        /// <summary>
        /// The start date
        /// </summary>
        public DateTime StatementEndDate { get; set; }


        /// <summary>
        /// The page page owner
        /// </summary>
        public string StatementCustomer { get; set; }


        /// <summary>
        /// The page page owner
        /// </summary>
        public string StatementPeriod { get; set; }

        /// <summary>
        /// The page page owner
        /// </summary>
        public string StatementAccount { get; set; }


        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of pageSearchParameter is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the page object is valid, false otherwise.
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
