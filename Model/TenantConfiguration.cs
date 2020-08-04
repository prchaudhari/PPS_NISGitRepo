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
        public long Identifier { get; set; }
        public string Description { get; set; }
        public string InputDataSourcePath { get; set; }
        public string OutputHTMLPath { get; set; }
        public string OutputPDFPath { get; set; }
    }
}
