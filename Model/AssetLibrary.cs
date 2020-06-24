// <copyright file="AssetLibrary.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// This class represents AssetLibrary model.
    /// </summary>
    public class AssetLibrary
    {
        #region Private Members

        /// <summary>
        /// The asset libray identifier.
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The asset libray name.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The asset libray description.
        /// </summary>
        private string description = string.Empty;

        /// <summary>
        /// The assets
        /// </summary>
        private IList<Asset> assets = new List<Asset>();

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
        /// Gets or sets Name.
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
        /// Gets or sets description.
        /// </summary>
        [Description("description")]
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }


        /// <summary>
        /// Gets or sets assets.
        /// </summary>
        [Description("Assets")]
        public IList<Asset> Assets
        {
            get
            {
                return this.assets;
            }

            set
            {
                this.assets = value;
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
                if (!this.validationEngine.IsValidText(this.Name))
                {
                    exception.Data.Add(this.utility.GetDescription("Name", typeof(AssetLibrary)), ModelConstant.ASSETLIBRARY_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSETLIBRARY_NAME);
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
