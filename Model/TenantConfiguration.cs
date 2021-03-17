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
        [Description("Name")]
        public string Name { get; set; }
        [Description("Description")]
        public string Description { get; set; }

        [Description("InputDataSourcePath")]
        public string InputDataSourcePath { get; set; }

        [Description("OutputHTMLPath")]
        public string OutputHTMLPath { get; set; }

        [Description("OutputPDFPath")]
        public string OutputPDFPath { get; set; }

        [Description("AssetPath")]
        public string AssetPath { get; set; }

        [Description("ArchivalPath")]
        public string ArchivalPath { get; set; }

        [Description("IsAssetPathEditable")]
        public bool IsAssetPathEditable { get; set; }

        [Description("IsAssetPathEditable")]
        public bool IsOutputHTMLPathEditable { get; set; }

        [Description("ArchivalPeriodUnit")]
        public ArchivalPeriod ArchivalPeriodUnit { get; set; }

        [Description("ArchivalPeriod")]
        public int? ArchivalPeriod { get; set; }

        [Description("DateFormat")]
        public string DateFormat { get; set; }

        [Description("ApplicationTheme")]
        public string ApplicationTheme { get; set; }

        [Description("WidgetThemeSetting")]
        public string WidgetThemeSetting { get; set; }

        [Description("BaseUrlForTransactionData")]
        public string BaseUrlForTransactionData { get; set; }

        [Description("TenantCode")]
        public string TenantCode { get; set; }

        public string PreviewStatementFunctionName { get; set; }
        public string GenerateStatementRunNowScheduleFunctionName { get; set; }
        public string GenerateStatementScheduleTimeFunctionName { get; set; }
        public string GenerateHtmlFormatForStatementFunctionName { get; set; }
        public string RetryFailedCustomerStatementApiName { get; set; }
        public string ArchivalProcessApiName { get; set; }

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
                if (!this.validationEngine.IsValidText(this.Name))
                {
                    exception.Data.Add(this.utility.GetDescription("Name", typeof(TenantConfiguration)), ModelConstant.TENANTCONFIG_MODEL_SECTION + "~" + ModelConstant.INVALID_TENANTCONFIGS_NAME);
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
