// <copyright file="SearchMode.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This enum defines searching should be equals or contains.
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Like operation.
        /// </summary>
        [Description("Contains")]
        Contains = 1,

        /// <summary>
        /// Exact operation.
        /// </summary>
        [Description("Equals")]
        Equals = 2,

        /// <summary>
        /// Exact operation.
        /// </summary>
        [Description("StartsWith")]
        StartsWith = 3,


    }
}