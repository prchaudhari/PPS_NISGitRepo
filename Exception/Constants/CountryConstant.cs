// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References
    #endregion

    /// <summary>
    /// This class represents constants for role entity.
    /// </summary>
    public partial class ExceptionConstant
    {
        /// <summary>
        /// The country exception section key
        /// </summary>
        public const string COUNTRY_EXCEPTION_SECTION = "CountryException";

        /// <summary>
        /// The duplicate country found exception key
        /// </summary>
        //public const string DUPLICATE_COUNTRY_FOUND_EXCEPTION = "DuplicateCountryFoundException";
        public const string DUPLICATE_COUNTRY_FOUND_EXCEPTION = "Duplicate country found";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        //public const string COUNTRY_NOT_FOUND_EXCEPTION = "CountryNotFoundException";
        public const string COUNTRY_NOT_FOUND_EXCEPTION = "Country not found exception";


        /// <summary>
        /// The invalid country exception key
        /// </summary>
        //public const string INVALID_COUNTRY_EXCEPTION = "InvalidCountryLibraryException";
        public const string INVALID_COUNTRY_EXCEPTION = "Invalid country";
    }
}
