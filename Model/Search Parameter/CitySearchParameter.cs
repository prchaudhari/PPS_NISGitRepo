// <copyright file="CitySearchParameter.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represents city search parameter
    /// </summary>
    public class CitySearchParameter : BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// The city identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The city name
        /// </summary>
        private string cityName = string.Empty;

        /// <summary>
        /// The state id
        /// </summary>
        private string stateIdentifier = string.Empty;

        /// <summary>
        /// The utility object
        /// </summary>
        private ModelUtility utility = new ModelUtility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public members

        /// <summary>
        /// Gets or sets city identifier
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
        /// Gets or sets city name
        /// </summary>
        [Description("City Name")]
        public string CityName
        {
            get
            {
                return this.cityName;
            }
            set
            {
                this.cityName = value;
            }
        }

        /// <summary>
        /// Gets or sets state id
        /// </summary>
        [Description("State Identifier")]
        public string StateIdentifier
        {
            get
            {
                return this.stateIdentifier;
            }
            set
            {
                this.stateIdentifier = value;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of city search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (string.IsNullOrEmpty(this.cityName))
                {
                    exception.Data.Add(this.utility.GetDescription("Country Name", typeof(City)), ModelConstant.CITY_MODEL_SECTION + "~" + ModelConstant.INVALID_City_NAME);
                }
                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Paging parameter", typeof(City)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
                }

                if (!this.SortParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Sort parameter", typeof(City)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
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
