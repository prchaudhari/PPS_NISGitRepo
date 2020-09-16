// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    /// <summary>
    /// This class represents state constants
    /// </summary>
    public partial class ExceptionConstant
    {
        /// <summary>
        /// The state exception section key
        /// </summary>
        public const string STATE_EXCEPTION_SECTION = "StateException";

        /// <summary>
        /// The invalid state exception key
        /// </summary>
        public const string INVALID_STATE_EXCEPTION = "InvalidStateException";

        /// <summary>
        /// The invalid state search parameter exception key
        /// </summary>
        public const string INVALID_STATE_SEARCH_PARAMTER_EXCEPTION = "InvalidStateSearchParameterException";

        /// <summary>
        /// The state not found exception key
        /// </summary>
        public const string STATE_NOT_FOUND_EXCEPTION = "StateNotFoundException";

        /// <summary>
        /// The duplicate state found exception key
        /// </summary>
        public const string DUPLICATE_STATE_FOUND_EXCEPTION = "DuplicateStateFoundException";
    }
}