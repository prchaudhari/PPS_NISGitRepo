// <copyright file="IAssetSettingRepository.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This interface represents reference to access accet library repository.
    /// </summary>
    public interface IAssetSettingRepository
    {
        #region Asset Setting

        /// <summary>
        /// This method reference to get roles based on serach parameter.
        /// </summary>
        /// <param name="assetLibrarySearchParameter">
        /// The search parameter for product
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It returns list of roles.
        /// </returns>
        IList<AssetSetting> GetAssetSettings(string tenantCode);

        /// <summary>
        /// This method reference to get roles based on serach parameter.
        /// </summary>
        /// <param name="assetLibrarySearchParameter">
        /// The search parameter for product
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It returns list of roles.
        /// </returns>
       bool Save(AssetSetting setting,string tenantCode);


        #endregion

    }
}

