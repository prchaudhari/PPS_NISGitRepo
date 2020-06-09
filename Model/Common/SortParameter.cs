// <copyright file="SortParameter.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// Represents the model for sort parameter.
    /// </summary>
    public class SortParameter
    {
        #region Private Members 

        /// <summary>
        /// Represents the sorting order.
        /// </summary>
        /// <value>The sort order.</value>
        private SortOrder sortOrder = SortOrder.Ascending;

        /// <summary>
        /// This parameter defines column name on which sorting will be performed.
        /// </summary>
        /// <value>The sort column.</value>
        private string sortColumn = string.Empty;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets and sets sort order.
        /// </summary>
        /// <value>
        /// The sort order
        /// </value>
        [Description("Sort order")]
        public SortOrder SortOrder
        {
            get
            {
                return this.sortOrder;
            }

            set
            {
                this.sortOrder = value;
            }
        }

        /// Gets and sets column name.
        /// </summary>
        /// <value>The sort column.</value>
        [Description("Sort column")]
        public string SortColumn
        {
            get
            {
                return this.sortColumn;
            }

            set
            {
                this.sortColumn = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///Validates orderby parameters.
        /// </summary>
        /// <returns>
        /// Returns true if sort column and sort order exists, false otherwise.
        /// </returns>
        public bool IsValid()
        {
            try
            {
                bool result = true;

                if (string.IsNullOrWhiteSpace(this.SortColumn))
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