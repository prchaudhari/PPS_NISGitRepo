// <copyright file="Asset.cs" company="Websym Solutions Pvt. Ltd.">
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
    /// This class represents Asset model.
    /// </summary>
    public class Asset
    {
        #region Private Members

        /// <summary>
        /// The asset  id.
        /// </summary>
        private long identifier = 0;

        /// <summary>
        /// The asset library id.
        /// </summary>
        private long assetLibraryidentifier = 0;

        /// <summary>
        /// The asset  name.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The asset  file path.
        /// </summary>
        private string filePath = string.Empty;

        /// <summary>
        /// The file content
        /// </summary>
        private string fileContent = string.Empty;

        /// <summary>
        /// The isdeleted
        /// </summary>
        private bool isDeleted;

        /// <summary>
        /// The isdeleted
        /// </summary>
        private User lastUpdatedBy;
        /// <summary>
        /// The isdeleted
        /// </summary>
        private DateTime? lastUpdatedDate=DateTime.UtcNow;

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
        /// Gets or sets Identifier.
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
        /// Gets or sets assetLibraryidentifier.
        /// </summary>
        [Description("Identifier")]
        public long AssetLibraryIdentifier
        {
            get
            {
                return this.assetLibraryidentifier;
            }

            set
            {
                this.assetLibraryidentifier = value;
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
        /// Gets or sets FilePath.
        /// </summary>
        [Description("FilePath")]
        public string FilePath
        {
            get
            {
                return this.filePath;
            }

            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// Gets or sets FilePath.
        /// </summary>
        [Description("FilePath")]
        public User LastUpdatedBy
        {
            get
            {
                return this.lastUpdatedBy;
            }

            set
            {
                this.lastUpdatedBy = value;
            }
        }
        /// <summary>
        /// Gets or sets FilePath.
        /// </summary>
        [Description("FilePath")]
        public DateTime? LastUpdatedDate
        {
            get
            {
                return this.lastUpdatedDate;
            }

            set
            {
                this.lastUpdatedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the content of the file.
        /// </summary>
        /// <value>
        /// The content of the file.
        /// </value>
        [Description("FileContent")]
        public string FileContent
        {
            get
            {
                return this.fileContent;
            }

            set
            {
                this.fileContent = value;
            }
        }

        /// <summary>
        /// Gets or sets isDeleted
        /// </summary>
        [Description("IsDeleted")]
        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }
            set
            {
                isDeleted = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this instance of asset is valid.
        /// </summary>
        /// <returns>
        /// Returns true if the asset object is valid, false otherwise.
        /// </returns>
        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.FilePath))
                {
                    exception.Data.Add(this.utility.GetDescription("FilePath", typeof(Asset)), ModelConstant.ASSET_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSET_NAME);
                }
                if (this.AssetLibraryIdentifier == 0)
                {
                    exception.Data.Add(this.utility.GetDescription("AssetLibraryIdentifier", typeof(Asset)), ModelConstant.ASSET_MODEL_SECTION + "~" + ModelConstant.INVALID_ASSET_LIBRARYIDENTIFIER);
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
