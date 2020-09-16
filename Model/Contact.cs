// <copyright file="Contact.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2016 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents contact model.
    /// </summary>
    public class Contact
    {
        #region Private Members

        /// <summary>
        /// The firstName.
        /// </summary>
        private string firstName = string.Empty;

        /// <summary>
        /// The lastName.
        /// </summary>
        private string lastName = string.Empty;

        /// <summary>
        /// The email address.
        /// </summary>
        private string emailAddress = string.Empty;

        /// <summary>
        /// The contact number.
        /// </summary>
        private string contactNumber = string.Empty;

        /// <summary>
        /// The contcat type.
        /// </summary>
        private string contactType = string.Empty;

        /// <summary>
        /// The country code.
        /// </summary>
        private string countryCode = string.Empty;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets firstName.
        /// </summary>
        [Description("First Name")]
        public string FirstName
        {
            get
            {
                return this.firstName;
            }

            set
            {
                this.firstName = value;
            }
        }

        /// <summary>
        /// The lastName.
        /// </summary>
        [Description("Last Name")]
        public string LastName
        {
            get
            {
                return this.lastName;
            }

            set
            {
                this.lastName = value;
            }
        }

        /// <summary>
        /// The email address.
        /// </summary>
        [Description("Email Address")]
        public string EmailAddress
        {
            get
            {
                return this.emailAddress;
            }

            set
            {
                this.emailAddress = value;
            }
        }

        /// <summary>
        /// The contact number.
        /// </summary>
        [Description("Contact Number")]
        public string ContactNumber
        {
            get
            {
                return this.contactNumber;
            }

            set
            {
                this.contactNumber = value;
            }
        }

        /// <summary>
        /// The contcat type.
        /// </summary>
        [Description("Contact Type")]
        public string ContactType
        {
            get
            {
                return this.contactType;
            }

            set
            {
                this.contactType = value;
            }
        }

        /// <summary>
        /// The country code.
        /// </summary>
        [Description("CountryCode")]
        public string CountryCode
        {
            get
            {
                return this.countryCode;
            }

            set
            {
                this.countryCode = value;
            }
        } 

        #endregion
    }
}
