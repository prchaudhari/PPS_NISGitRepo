// <copyright file="AssetSearchParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>
//----------------------------------------------------------------------- 

namespace nIS
{
    #region References

    using System;
    using System.ComponentModel;

    #endregion

    /// <summary>
    /// This class represents asset search parameter model
    /// </summary>
    public class AssetSearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The asset library identifier.
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The asset library identifier
        /// </summary>
        private string assetLibraryIdentifier = string.Empty;       

        /// <summary>
        /// The tenant code.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The image extension
        /// </summary>
        private string extension = string.Empty;

        /// <summary>
        /// scene identifier
        /// </summary>
        private string sceneIdentifier;
            

        /// <summary>
        /// The is active
        /// </summary>
        private bool? isDeleted = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        #endregion

        #region Public Members

        /// <summary>
        /// gets or sets the identifier.
        /// </summary>
        [Description("Identifier")]
        public string Identifier
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
        /// Gets or sets asset library identifier
        /// </summary>
        public string AssetLibraryIdentifier
        {
            get
            {
                return assetLibraryIdentifier;
            }
            set
            {
                assetLibraryIdentifier = value;
            }
        }

        /// <summary>
        /// gets or sets the name.
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
        /// gets or sets is delete mode.
        /// </summary>
        [Description("Is delete")]
        public bool? IsDeleted
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

        /// <summary>
        /// Gets or sets image extension
        /// </summary>
        public string Extension
        {
            get
            {
                return extension;
            }
            set
            {
                extension = value;
            }
        }

        /// <summary>
        /// Gets or sets scene identifier
        /// </summary>
        public string SceneIdentifier
        {
            get { return sceneIdentifier; }
            set { sceneIdentifier = value; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if asset library search parameter object is valid.
        /// </summary>
        public void IsValid()
        {
            Exception exception = new Exception();
            if (!PagingParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("PagingParameter", typeof(AssetSearchParameter)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
            }

            if (!SortParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("SortParameter", typeof(AssetSearchParameter)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
