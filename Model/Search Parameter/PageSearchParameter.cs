// <copyright file="PageSearchParameter.cs" company="Websym Solutions Pvt Ltd">
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
    public class PageSearchParameter : BaseSearchEntity
    {
        #region Public Members

        /// <summary>
        /// The page identifier
        /// </summary>
        public long Identifier { get; set; }

        /// <summary>
        /// The page dispaly name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The page status
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// The page type identifier
        /// </summary>
        public long PageTypeId { get; set; }

        public bool IsPageWidgetsRequired { get; set; }

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
