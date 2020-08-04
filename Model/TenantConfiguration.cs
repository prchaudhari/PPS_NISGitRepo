// <copyright file="TenantConfiguration.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    using System;
    #region References

    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents Tenant Configuration model.
    /// </summary>
    public class TenantConfiguration
    {
        #region Public Member
        [Description("Identifier")]
        public long Identifier { get; set; }

        [Description("Description")]
        public string Description { get; set; }

        [Description("InputDataSourcePath")]
        public string InputDataSourcePath { get; set; }

        [Description("OutputHTMLPath")]
        public string OutputHTMLPath { get; set; }

        [Description("OutputPDFPath")]
        public string OutputPDFPath { get; set; }

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.InputDataSourcePath))
                {
                    exception.Data.Add(this.utility.GetDescription("InputDataSourcePath", typeof(TenantConfiguration)), ModelConstant.TENANTCONFIG_MODEL_SECTION + "~" + ModelConstant.INVALID_TENANTCONFIGS_IMAGEDATASOURCEPATH);
                }
                if (!this.validationEngine.IsValidText(this.OutputHTMLPath))
                {
                    exception.Data.Add(this.utility.GetDescription("OutputHTMLPath", typeof(TenantConfiguration)), ModelConstant.TENANTCONFIG_MODEL_SECTION + "~" + ModelConstant.INVALID_TENANTCONFIGS_OUTPUTHTMLPATH);
                }
                if (!this.validationEngine.IsValidText(this.OutputPDFPath))
                {
                    exception.Data.Add(this.utility.GetDescription("OutputPDFPath", typeof(TenantConfiguration)), ModelConstant.TENANTCONFIG_MODEL_SECTION + "~" + ModelConstant.INVALID_TENANTCONFIGS_OUTPUTPDFPATH);
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
    }
}
