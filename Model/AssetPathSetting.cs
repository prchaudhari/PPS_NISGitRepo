// <copyright file="AssetPathSetting.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// This class represents AssetPathSetting model.
    /// </summary>
    public class AssetPathSetting
    {
        #region Private Members

        /// <summary>
        /// The AssetPathSetting identifier.
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The asset path.
        /// </summary>
        private string assetPath = string.Empty;
        
        /// <summary>
        /// The isactive.
        /// </summary>
        private bool isActive = false;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets identifier.
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
        /// Gets or Asset Path.
        /// </summary>
        [Description("AssetPath")]
        public string AssetPath
        {
            get
            {
                return this.assetPath;
            }

            set
            {
                this.assetPath = value;
            }
        }     

        /// <summary>
        /// Gets or sets the isactive.
        /// </summary>
        [Description("Is Active")]
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of asset path is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the asset path setting object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.AssetPath))
                {
                    exception.Data.Add(this.utility.GetDescription("Name", typeof(AssetPathSetting)), ModelConstant.ASSETPATHSETTING_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETPATH);
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
