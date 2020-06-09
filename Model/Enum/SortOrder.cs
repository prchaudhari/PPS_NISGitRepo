// <copyright file="SortOrder.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// Represents the sort order.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Ascending order.
        /// </summary>
        [Description("Ascending")]
        Ascending = 1,

        /// <summary>
        /// Descending order.
        /// </summary>
        [Description("Descending")]
        Descending = 2,
    }
}