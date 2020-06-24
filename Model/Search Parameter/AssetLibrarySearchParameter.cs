// <copyright file="AssetLibrarySearchParameter.cs" company="Websym Solutions Pvt Ltd">
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
    public class AssetLibrarySearchParameter : BaseSearchEntity
    {
        #region Private Members

        /// <summary>
        /// The asset library identifier.
        /// </summary>
        private string identifier = string.Empty;

        /// <summary>
        /// The tenant code.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The is active
        /// </summary>
        private bool? isActive = null;

        /// <summary>
        /// The is asset data required
        /// </summary>
        private bool isAssetDataRequired = false;

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
        /// gets or sets is active mode.
        /// </summary>
        [Description("Is active")]
        public bool? IsActive
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
        /// gets or sets isassetdatarequired mode.
        /// </summary>
        [Description("Is asset data required")]
        public bool IsAssetDataRequired
        {
            get
            {
                return this.isAssetDataRequired;
            }

            set
            {
                this.isAssetDataRequired = value;
            }
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
                exception.Data.Add(this.utility.GetDescription("PagingParameter", typeof(AssetLibrarySearchParameter)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_PAGING_PARAMETER);
            }

            if (!SortParameter.IsValid())
            {
                exception.Data.Add(this.utility.GetDescription("SortParameter", typeof(AssetLibrarySearchParameter)), ModelConstant.COMMON_SECTION + "~" + ModelConstant.INVALID_SORT_PARAMETER);
            }

            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }

        #endregion
    }
}
