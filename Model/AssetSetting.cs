// <copyright file="AssetSetting.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.ComponentModel;
    using System.Collections.Generic;
    using System;

    #endregion

    /// <summary>
    /// This class represents AssetSetting model.
    /// </summary>
    public class AssetSetting
    {
        #region

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region public members
        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("Identifier")]
        public long Identifier { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("ImageHeight")]
        public decimal ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("ImageWidth")]
        public decimal ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("ImageSize")]
        public decimal ImageSize { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("ImageFileExtension")]
        public string ImageFileExtension { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("VideoSize")]
        public decimal VideoSize { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("VideoFileExtension")]
        public string VideoFileExtension { get; set; }

        /// <summary>
        /// Gets or sets identifier.
        /// </summary>
        [Description("TenantCode")]
        public string TenantCode { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of asset library is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the asset library object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.ImageFileExtension))
                {
                    exception.Data.Add(this.utility.GetDescription("ImageFileExtension", typeof(AssetLibrary)), ModelConstant.ASSETSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETSETTINGS_IMAGEFILEEXTENSION);
                }
                if (!this.validationEngine.IsValidText(this.VideoFileExtension))
                {
                    exception.Data.Add(this.utility.GetDescription("VideoFileExtension", typeof(AssetLibrary)), ModelConstant.ASSETSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETSETTINGS_VIDEOFILEEXTENSION);
                }
                if (this.ImageHeight>0)
                {
                    exception.Data.Add(this.utility.GetDescription("ImageHeight", typeof(AssetLibrary)), ModelConstant.ASSETSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETSETTINGS_IMAGEHEIGHT);
                }
                if (this.ImageWidth > 0)
                {
                    exception.Data.Add(this.utility.GetDescription("ImageWidth", typeof(AssetLibrary)), ModelConstant.ASSETSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETSETTINGS_IMAGEWIDTH);
                }
                if (this.ImageSize > 0)
                {
                    exception.Data.Add(this.utility.GetDescription("ImageSize", typeof(AssetLibrary)), ModelConstant.ASSETSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETSETTINGS_IMAGESIZE);
                }
                if (this.VideoSize > 0)
                {
                    exception.Data.Add(this.utility.GetDescription("VideoSize", typeof(AssetLibrary)), ModelConstant.ASSETSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETSETTINGS_VIDEOSIZE);
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
