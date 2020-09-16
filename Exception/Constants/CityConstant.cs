// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    /// <summary>
    /// This class represents city constants
    /// </summary>
    public partial class ExceptionConstant
    {
        /// <summary>
        /// The city exception section key
        /// </summary>
        public const string CITY_EXCEPTION_SECTION = "CityException";

        /// <summary>
        /// The invalid city exception key
        /// </summary>
        public const string INVALID_CITY_EXCEPTION = "InvalidCityException";

        /// <summary>
        /// The invalid city search parameter exception key
        /// </summary>
        public const string INVALID_CITY_SEARCH_PARAMTER_EXCEPTION = "InvalidCitySearchParameterException";

        /// <summary>
        /// The city not found exception key
        /// </summary>
        public const string CITY_NOT_FOUND_EXCEPTION = "CityNotFoundException";

        /// <summary>
        /// The duplicate city found exception key
        /// </summary>
        public const string DUPLICATE_CITY_FOUND_EXCEPTION = "DuplicateCityFoundException";
    }
}