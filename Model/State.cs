// <copyright file="State.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace nIS
{
    #region References
    #endregion

    /// <summary>
    /// This class represents State model
    /// </summary>
    public class State
    {
        #region Private Members

        /// <summary>
        /// The state identifier
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The state name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The country
        /// </summary>
        private Country country = new Country();

        /// <summary>
        /// The validation engine
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility
        /// </summary>
        private ModelUtility utility = new ModelUtility();

        /// <summary>
        /// IsActive for State  
        /// </summary>
        bool isActive = true;

        /// <summary>
        /// IsDeleted for State  
        /// </summary>
        bool isDeleted = true;


        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets state identifier
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
        /// Gets or sets state name
        /// </summary>
        [Description("Name")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets country
        /// </summary>
        [Description("Country")]
        public Country Country
        {
            get
            {
                return this.country;
            }
            set
            {
                this.country = value;
            }
        }

        /// <summary>
        /// Gets or sets  IsActive for state.
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
        /// Gets or sets  IsDeleted for state.
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
        /// This method validates current state model
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.name))
                {
                    exception.Data.Add(this.utility.GetDescription("CountryName", typeof(State)), ModelConstant.STATE_MODEL_SECTION + "~" + ModelConstant.INVALID_STATE_NAME);
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
