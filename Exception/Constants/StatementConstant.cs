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
        public const string STATEMENT_EXCEPTION = "StatementException";

        /// <summary>
        /// Duplicate Statement found exception
        /// </summary>
        public const string DUPLICATE_STATEMENT_FOUND_EXCEPTION = "Statement already exists";

        /// <summary>
        /// Statement not found exception
        /// </summary>
        public const string STATEMENT_NOT_FOUND_EXCEPTION = "Statement not found";

        /// <summary>
        /// The invalid page exception
        /// </summary>
        public const string INVALID_STATEMENT_EXCEPTION = "Invalid page";

        /// <summary>
        /// The invalid page paging parameter
        /// </summary>
        public const string INVALID_STATEMENT_PAGING_PARAMETER = "Invalid page paging parameter";

        /// <summary>
        /// The invalid page sort parameter
        /// </summary>
        public const string INVALID_STATEMENT_SORT_PARAMETER = "Invalid page sort parameter";

        /// <summary>
        /// Duplicate Statement widget found exception
        /// </summary>
        public const string DUPLICATE_STATEMENT_WIDGET_FOUND_EXCEPTION = "Statement widget already exists";

        /// <summary>
        /// Duplicate Statement page reference exit exception
        /// </summary>
        public const string STATEMENT_REFERENCE_EXIST_EXCEPTION = "This statement is used in schedule";

        /// <summary>
        /// Duplicate Page widget found exception
        /// </summary>
        public const string DUPLICATE_STATEMENT_PAGE_FOUND_EXCEPTION = "Page widget already exists";
    }
}
