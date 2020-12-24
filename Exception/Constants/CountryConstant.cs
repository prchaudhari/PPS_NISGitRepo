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
        /// The country not found exception key
        /// </summary>
        //public const string COUNTRY_NOT_FOUND_EXCEPTION = "CountryNotFoundException";
        public const string COUNTRY_REFERENCEIN_USER_EXCEPTION = "Country is used in user";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        //public const string COUNTRY_NOT_FOUND_EXCEPTION = "CountryNotFoundException";
        public const string COUNTRY_REFERENCEIN_TENANT_EXCEPTION = "Country is used in tenant";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        //public const string COUNTRY_NOT_FOUND_EXCEPTION = "CountryNotFoundException";
        public const string COUNTRY_REFERENCEIN_TENANTCONTACT_EXCEPTION = "Country is used in tenant contact";


        /// <summary>
        /// The invalid country exception key
        /// </summary>
        //public const string INVALID_COUNTRY_EXCEPTION = "InvalidCountryLibraryException";
        public const string INVALID_COUNTRY_EXCEPTION = "Invalid country";

        /// <summary>
        /// The country exception section key
        /// </summary>
        public const string CONTACTTYPE_EXCEPTION_SECTION = "ContactType";

        /// <summary>
        /// The duplicate country found exception key
        /// </summary>
        public const string DUPLICATE_CONTACT_TYPE_FOUND_EXCEPTION = "Duplicate contact type found";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        public const string CONTACT_TYPE_NOT_FOUND_EXCEPTION = "Contact type not found exception";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        public const string CONTACTTYPE_REFERENCEIN_CONTACT_EXCEPTION = "Contact type is used in tenant contact";

        /// <summary>
        /// The country exception section key
        /// </summary>
        public const string PAGETYPE_EXCEPTION_SECTION = "PageType";

        /// <summary>
        /// The duplicate country found exception key
        /// </summary>
        public const string DUPLICATE_PAGETYPE_FOUND_EXCEPTION = "Duplicate Page type found";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        public const string PAGETYPE_NOT_FOUND_EXCEPTION = "Page type not found exception";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        public const string INVALID_PAGETYPE_EXCEPTION = "Invalid page type exception";

        /// <summary>
        /// The country not found exception key
        /// </summary>
        public const string PAGETYPE_REFERENCEIN_PAGE_EXCEPTION = "Page type is used in tenant contact";
    }
}
