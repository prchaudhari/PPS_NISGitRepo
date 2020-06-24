// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References
    #endregion

    /// <summary>
    /// This class represents constants for page entity.
    /// </summary>
    public partial class ExceptionConstant
    {
        /// <summary>
        /// Role exception
        /// </summary>
        public const string PAGE_EXCEPTION = "PageException";

        /// <summary>
        /// Duplicate Page found exception
        /// </summary>
        public const string DUPLICATE_PAGE_FOUND_EXCEPTION = "Page already exists";

        /// <summary>
        /// Page not found exception
        /// </summary>
        public const string PAGE_NOT_FOUND_EXCEPTION = "Page not found";

        /// <summary>
        /// The invalid page exception
        /// </summary>
        public const string INVALID_PAGE_EXCEPTION = "Invalid page";

        /// <summary>
        /// The invalid page paging parameter
        /// </summary>
        public const string INVALID_PAGE_PAGING_PARAMETER = "Invalid page paging parameter";

        /// <summary>
        /// The invalid page sort parameter
        /// </summary>
        public const string INVALID_PAGE_SORT_PARAMETER = "Invalid page sort parameter";
    }
}
