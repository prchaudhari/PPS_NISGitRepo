// <copyright file="IRoleRepository.cs" company="Websym Solutions Pvt. Ltd.">
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
    public interface IAssetLibraryRepository
    {
        #region Asset Library

        /// <summary>
        /// This method reference to add roles.
        /// </summary>
        /// <param name="assetLibraries">
        /// List of product object.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully added or it will throw an exception.
        /// </returns>
        bool AddAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode);

        /// <summary>
        /// This method reference to update roles.
        /// </summary>
        /// <param name="roles">
        /// The list of products.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        bool UpdateAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode);

        /// <summary>
        /// This method reference to delete asset libraries.
        /// </summary>
        /// <param name="assetLibraries">
        /// The list of assetLibraries.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully deleted or it will throw an exception.
        /// </returns>
        bool DeleteAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode);

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
        IList<AssetLibrary> GetAssetLibraries(AssetLibrarySearchParameter assetLibrarySearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get accet library count
        /// </summary>
        /// <param name="assetLibrarySearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        long GetAssetLibraryCount(AssetLibrarySearchParameter assetLibrarySearchParameter, string tenantCode);

        #endregion

        #region Assets

        /// <summary>
        /// This method reference to add assets.
        /// </summary>
        /// <param name="assets">
        /// List of product object.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully added or it will throw an exception.
        /// </returns>
        bool AddAssets(IList<Asset> assets, string tenantCode);

        /// <summary>
        /// This method reference to update assets.
        /// </summary>
        /// <param name="assets">
        /// The list of products.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully updated or it will throw an exception.
        /// </returns>
        bool UpdateAssets(IList<Asset> assets, string tenantCode);

        /// <summary>
        /// This method reference to delete assets.
        /// </summary>
        /// <param name="assets">
        /// The list of product.
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It will return true if successfully deleted or it will throw an exception.
        /// </returns>
        bool DeleteAssets(IList<Asset> assets, string tenantCode);

        /// <summary>
        /// This method reference to get assets based on serach parameter.
        /// </summary>
        /// <param name="assetSearchParameter">
        /// The search parameter for product
        /// </param>
        /// <param name="tenantCode">
        /// The tenant code
        /// </param>
        /// <returns>
        /// It returns list of assets.
        /// </returns>
        IList<Asset> GetAssets(AssetSearchParameter assetSearchParameter, string tenantCode);

        /// <summary>
        /// This method reference to get accet library count
        /// </summary>
        /// <param name="assetSearchParameter"></param>
        /// <param name="tenantCode"></param>
        /// <returns>Role count</returns>
        long GetAssetCount(AssetSearchParameter assetSearchParameter, string tenantCode);

        /// <summary>
        /// This method refence to add asset path
        /// </summary>
        /// <param name="assetPathSetting"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        bool AddAssetPath(AssetPathSetting assetPathSetting, string tenantCode);

        /// <summary>
        /// Get the asset storage path
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        AssetPathSetting GetAssetPath(string tenantCode);

        #endregion
    }
}

