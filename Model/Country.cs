// <copyright file="Country.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion
    /// <summary>
    /// This class represents country model
    /// </summary>
    public class Country
    {
        #region Private Members

        /// <summary>
        /// The country identifier
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The country name
        /// </summary>
        private string countryName = string.Empty;

        /// <summary>
        /// The country code
        /// </summary>
        private string code = string.Empty;

        /// <summary>
        /// The country calling Code
        /// </summary>
        private string dialingCode = string.Empty;

        /// <summary>
        /// The validation engine
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// IsActive for country  
        /// </summary>
        bool isActive = true;

        /// <summary>
        /// IsDeleted for country  
        /// </summary>
        bool isDeleted = false;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets country identifier
        /// </summary>
        [Description("Identifier")]
        public long Identifier
        {
            get
            {
                return this.identifier;
            }
            set
            {
                this.identifier = value;
            }
        }

        /// <summary>
        /// Gets or sets country name
        /// </summary>
        [Description("Country Name")]
        public string CountryName
        {
            get
            {
                return this.countryName;
            }
            set
            {
                this.countryName = value;
            }
        }

        /// <summary>
        /// Gets or sets country code
        /// </summary>
        [Description("Country Code")]
        public string Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }

        /// <summary>
        /// Gets or sets country calling code
        /// </summary>
        [Description("Dialing Code")]
        public string DialingCode
        {
            get
            {
                return this.dialingCode;
            }
            set
            {
                this.dialingCode = value;
            }
        }

        /// <summary>
        /// Gets or sets  IsActive for country.
        /// </summary>
        [Description("IsActive")]
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;
            }
        }

        /// <summary>
        /// Gets or sets  IsDeleted for country.
        /// </summary>
        [Description("IsDeleted")]
        public bool IsDeleted
        {
            get
            {
                return this.isDeleted;
            }
            set
            {
                this.isDeleted = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method validates current country model
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.countryName))
                {
                    exception.Data.Add(this.utility.GetDescription("CountryName", typeof(Country)), ModelConstant.COUNTRY_MODEL_SECTION + "~" + ModelConstant.INVALID_COUNTRY_NAME);
                }

                if (!this.validationEngine.IsValidText(this.code))
                {
                    exception.Data.Add(this.utility.GetDescription("CountryCode", typeof(Country)), ModelConstant.COUNTRY_MODEL_SECTION + "~" + ModelConstant.INVALID_COUNTRY_CODE);
                }

                if (!this.validationEngine.IsValidText(this.dialingCode))
                {
                    exception.Data.Add(this.utility.GetDescription("DialingCode", typeof(Country)), ModelConstant.COUNTRY_MODEL_SECTION + "~" + ModelConstant.INVALID_COUNTRY_DIALING_CODE);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion
    }
}
