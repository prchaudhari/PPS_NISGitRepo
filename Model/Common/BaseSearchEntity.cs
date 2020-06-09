// <copyright file="BaseSearchEntity.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  


namespace nIS
{
    #region References
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents the base search entity
    /// </summary>
    public class BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// Search mode like Exact search or Contains search.
        /// </summary>
        private SearchMode searchMode = SearchMode.Equals;

        /// <summary>
        /// Paging Parameter like index and size
        /// </summary>
        private PagingParameter pagingParameter = new PagingParameter();

        /// <summary>
        /// Sort parameter like column name and sortorder
        /// </summary>
        private SortParameter sortParameter = new SortParameter();

        #endregion

        #region Public members
        /// <summary>
        /// Gets or sets search mode.
        /// </summary>
        /// <value>
        /// Value is search mode. 
        /// </value>
        [Description("Search mode")]
        public SearchMode SearchMode
        {
            get
            {
                return this.searchMode;
            }
            set
            {
                this.searchMode = value;
            }
        }

        /// <summary>
        /// Gets or sets paging parameter.
        /// </summary>
        /// <value>
        /// Value is paging parameter. 
        /// </value>
        [Description("Paging parameter")]
        public PagingParameter PagingParameter
        {
            get
            {
                return this.pagingParameter;
            }
            set
            {
                this.pagingParameter = value;
            }
        }

        /// <summary>
        /// Gets or sets sort parameter.
        /// </summary>
        /// <value>
        /// Value is sort parameter. 
        /// </value>
        [Description("Sort parameter")]
        public SortParameter SortParameter
        {
            get
            {
                return this.sortParameter;
            }
            set
            {
                this.sortParameter = value;
            }
        }

        #endregion
    }
}
