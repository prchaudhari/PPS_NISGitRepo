// <copyright file="ContactType.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2016 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------


namespace nIS
{

    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This enum indicates the types of contacts.
    /// </summary>
    public class ContactType
    {
        public long Identifier { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantCode { get; set; }

        /// <summary>
        /// The validation engine
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility
        /// </summary>
        private IUtility utility = new Utility();


        #region Public Methods

        /// <summary>
        /// This method validates current country model
        /// </summary>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.Name))
                {
                    exception.Data.Add(this.utility.GetDescription("CountryName", typeof(Country)), ModelConstant.COUNTRY_MODEL_SECTION + "~" + ModelConstant.INVALID_CONTACTTYPE_NAME);
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
