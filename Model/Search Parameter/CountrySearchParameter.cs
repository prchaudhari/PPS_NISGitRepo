// <copyright file="CountrySearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents country search parameter
    /// </summary>
    public class CountrySearchParameter : BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// The country identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The country name
        /// </summary>
        private string countryName = string.Empty;

        /// <summary>
        /// The country code
        /// </summary>
        private string countryCode = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public members

        /// <summary>
        /// Gets or sets country identifier
        /// </summary>
        [Description("Identifier")]
        public string Identifier
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

        #region Public methods

        /// <summary>
        /// Determines whether this instance of country search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Paging parameter", typeof(Country)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Sort parameter", typeof(Country)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion
    }
}
