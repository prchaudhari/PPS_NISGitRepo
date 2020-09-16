// <copyright file="City.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace nIS
{
    /// <summary>
    /// This class represents city model
    /// </summary>
    public class City
    {
        #region Private Members

        /// <summary>
        /// The city identifier
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The city name
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The state
        /// </summary>
        private State state = new State();

        /// <summary>
        /// IsActive for city  
        /// </summary>
        bool isActive = true;

        /// <summary>
        /// IsDeleted for city  
        /// </summary>
        bool isDeleted = true;

        /// <summary>
        /// The validation engine
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility
        /// </summary>
        private ModelUtility utility = new ModelUtility();

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets city identifier
        /// </summary>
        [Description("City Identifier")]
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
        /// Gets or sets city name
        /// </summary>
        [Description("City Name")]
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
        /// Gets or sets state
        /// </summary>
        [Description("State")]
        public State State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        /// <summary>
        /// Gets or sets  IsActive for city.
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
        /// Gets or sets  IsDeleted for city.
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
        /// This method validates current city model
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.name))
                {
                    exception.Data.Add(this.utility.GetDescription("City Name", typeof(City)), ModelConstant.CITY_MODEL_SECTION + "~" + ModelConstant.INVALID_City_NAME);
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