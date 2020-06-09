// <copyright file="PagingParameter.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// Represents the model for paging parameter.
    /// </summary>
    public class PagingParameter
    {
        #region Private members

        /// <summary>
        /// Represents the current page index number.
        /// </summary>
        private int pageIndex = 0;

        /// <summary>
        /// Represents the number of elements to be displayed in a page.
        /// </summary>
        private int pageSize = 0;

        #endregion

        #region Public members

        /// <summary>
        /// Gets or sets page index.
        /// </summary>
        /// <value>
        /// The page index.
        /// </value>
        [Description("Page index")]
        public int PageIndex
        {
            get
            {
                return this.pageIndex;
            }

            set
            {
                this.pageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets page size.
        /// </summary>
        /// <value>
        /// The page size.
        /// </value>
        [Description("Page size")]
        public int PageSize
        {
            get
            {
                return this.pageSize;
            }

            set
            {
                this.pageSize = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates paging parameters.
        /// </summary>
        /// <returns value="result">Returns true if valid parameters, false otherwise.</returns>
        public bool IsValid()
        {
            try
            {
                bool result = true;
                if ((this.PageIndex > 0 && this.PageSize == 0) || (this.PageSize > 0 && this.PageIndex == 0))
                {
                    result = false;
                }

                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}
