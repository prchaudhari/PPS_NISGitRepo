// <copyright file="StateSearchParameter.cs" company="Websym Solutions Pvt Ltd">
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
    /// This class represents state search parameter
    /// </summary>
    public class StateSearchParameter : BaseSearchEntity
    {
        #region Private members

        /// <summary>
        /// The state identifier
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The state name
        /// </summary>
        private string stateName = string.Empty;

        /// <summary>
        /// The country Identifier
        /// </summary>
        private string countryIdentifier = string.Empty;

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
        /// Gets or sets state identifier
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
        /// Gets or sets state name
        /// </summary>
        [Description("State Name")]
        public string StateName
        {
            get
            {
                return this.stateName;
            }
            set
            {
                this.stateName = value;
            }
        }


        /// <summary>
        /// Gets or sets country identifier
        /// </summary>
        [Description("Country Identifier")]
        public string CountryIdentifier
        {
            get
            {
                return this.countryIdentifier;
            }
            set
            {
                this.countryIdentifier = value;
            }
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether this instance of state search parameter is valid.
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();

                if (string.IsNullOrEmpty(this.stateName))
                {
                    exception.Data.Add(this.utility.GetDescription("State Nmae", typeof(State)), ModelConstant.STATE_MODEL_SECTION + "~" + ModelConstant.INVALID_STATE_NAME);
                }                
                if (!this.PagingParameter.IsValid())
                {
                    exception.Data.Add(this.utility.GetDescription("Paging parameter", typeof(State)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
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
