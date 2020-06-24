// <copyright file="AssetLibraryManager.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity;

    #endregion

    /// <summary>
    /// This class implements manager layer of asset library manager.
    /// </summary>
    public class AssetLibraryManager
    {
        #region Private members
        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The asset library repository.
        /// </summary>
        IAssetLibraryRepository assetLibraryRepository = null;


        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for asset library manager, which initialise
        /// asset library repository.
        /// </summary>
        /// <param name="container">IUnity container implementation object.</param>
        public AssetLibraryManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.assetLibraryRepository = this.unityContainer.Resolve<IAssetLibraryRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Public Methods

        #region Asset Libraries Functions

        /// <summary>
        /// This method will call add asset library method of repository.
        /// </summary>
        /// <param name="assetLibraries">Asset library are to be add.</param>
        /// <param name="tenantCode">Tenant code of asset library.</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidAssetLibraries(assetLibraries, tenantCode);
                this.IsDuplicateAssetLibraries(assetLibraries, tenantCode);
                result = this.assetLibraryRepository.AddAssetLibraries(assetLibraries, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update asset library method of repository
        /// </summary>
        /// <param name="assetlibraries">asset library are to be update.</param>
        /// <param name="tenantCode">Tenant code of asset library.</param>
        /// <returns>
        /// Returns true if roles updated successfully, false otherwise.
        /// </returns>
        public bool UpdateAssetLibraries(IList<AssetLibrary> assetlibraries, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidAssetLibraries(assetlibraries, tenantCode);
                this.IsDuplicateAssetLibraries(assetlibraries, tenantCode);
                result = this.assetLibraryRepository.UpdateAssetLibraries(assetlibraries, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete asset library method of repository
        /// </summary>
        /// <param name="assetlibraries">AssetLibraries are to be delete.</param>
        /// <param name="tenantCode">Tenant code of asset library.</param>
        /// <returns>
        /// Returns true if roles deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteAssetLibraries(IList<AssetLibrary> assetlibraries, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.assetLibraryRepository.DeleteAssetLibraries(assetlibraries, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get asset library method of repository.
        /// </summary>
        /// <param name="assetLibrarySearchParameter">The asset library search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<AssetLibrary> GetAssetLibraries(AssetLibrarySearchParameter assetLibrarySearchParameter, string tenantCode)
        {
            IList<AssetLibrary> assetLibraries = new List<AssetLibrary>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    assetLibrarySearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                assetLibraries = this.assetLibraryRepository.GetAssetLibraries(assetLibrarySearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return assetLibraries;
        }

        /// <summary>
        /// This method helps to get count of asset libraries.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of asset libraries
        /// </returns>
        public long GetAssetLibraryCount(AssetLibrarySearchParameter assetLibrarySearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.assetLibraryRepository.GetAssetLibraryCount(assetLibrarySearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }

        #endregion

        #region Asset Functions

        /// <summary>
        /// This method will call add asset  method of repository.
        /// </summary>
        /// <param name="assets">Asset  are to be add.</param>
        /// <param name="tenantCode">Tenant code of asset .</param>
        /// <returns>
        /// Returns true if entities added successfully, false otherwise.
        /// </returns>
        public bool AddAssets(IList<Asset> assets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidAssets(assets, tenantCode);
                this.IsDuplicateAssets(assets, tenantCode);
                result = this.assetLibraryRepository.AddAssets(assets, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call update asset  method of repository
        /// </summary>
        /// <param name="assets">asset  are to be update.</param>
        /// <param name="tenantCode">Tenant code of asset .</param>
        /// <returns>
        /// Returns true if roles updated successfully, false otherwise.
        /// </returns>
        public bool UpdateAssets(IList<Asset> assets, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidAssets(assets, tenantCode);
                this.IsDuplicateAssets(assets, tenantCode);
                result = this.assetLibraryRepository.UpdateAssets(assets, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method will call delete asset  method of repository
        /// </summary>
        /// <param name="assets">Assets are to be delete.</param>
        /// <param name="tenantCode">Tenant code of asset .</param>
        /// <returns>
        /// Returns true if roles deleted successfully, false otherwise.
        /// </returns>
        public bool DeleteAssets(IList<Asset> assets, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.assetLibraryRepository.DeleteAssets(assets, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return result;
        }

        /// <summary>
        /// This method will call get asset  method of repository.
        /// </summary>
        /// <param name="assetSearchParameter">The asset  search parameters.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns roles if found for given parameters, else return null
        /// </returns>
        public IList<Asset> GetAssets(AssetSearchParameter assetSearchParameter, string tenantCode)
        {
            IList<Asset> assets = new List<Asset>();
            try
            {
                InvalidSearchParameterException invalidSearchParameterException = new InvalidSearchParameterException(tenantCode);
                try
                {
                    assetSearchParameter.IsValid();
                }
                catch (Exception exception)
                {
                    invalidSearchParameterException.Data.Add("InvalidPagingParameter", exception.Data);
                }

                if (invalidSearchParameterException.Data.Count > 0)
                {
                    throw invalidSearchParameterException;
                }

                assets = this.assetLibraryRepository.GetAssets(assetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return assets;
        }

        /// <summary>
        /// This method helps to get count of roles.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns>
        /// Returns count of roles
        /// </returns>
        public long GetAssetCount(AssetSearchParameter assetSearchParameter, string tenantCode)
        {
            long roleCount = 0;
            try
            {
                roleCount = this.assetLibraryRepository.GetAssetCount(assetSearchParameter, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return roleCount;
        }


        /// <summary>
        /// This method refence to add asset path
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public bool AddAssetPath(AssetPathSetting assetPathSetting, string tenantCode)
        {
            bool result = false;
            try
            {
                this.IsValidAssetPath(assetPathSetting, tenantCode);
                result = this.assetLibraryRepository.AddAssetPath(assetPathSetting, tenantCode);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get asset storage path
        /// </summary>
        /// <param name="tenantCode"></param>
        /// <returns></returns>
        public AssetPathSetting GetAssetPath(string tenantCode)
        {
            try
            {
                return this.assetLibraryRepository.GetAssetPath(tenantCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region Asset Libraries

        /// <summary>
        /// This method is responsible for validate asset libraries.
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <param name="tenantCode"></param>
        private void IsValidAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode)
        {
            try
            {
                if (assetLibraries?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidAssetLibraryException invalidAssetLibraryException = new InvalidAssetLibraryException(tenantCode);
                assetLibraries.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidAssetLibraryException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidAssetLibraryException.Data.Count > 0)
                {
                    throw invalidAssetLibraryException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate asset library in the list
        /// </summary>
        /// <param name="assetLibraries"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateAssetLibraries(IList<AssetLibrary> assetLibraries, string tenantCode)
        {
            try
            {
                int isDuplicateAssetLibrary = assetLibraries.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateAssetLibrary > 0)
                {
                    throw new DuplicateAssetLibraryException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        #endregion

        #region Assets

        /// <summary>
        /// This method is responsible for validate asset s.
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        private void IsValidAssets(IList<Asset> assets, string tenantCode)
        {
            try
            {
                if (assets?.Count <= 0)
                {
                    throw new NullArgumentException(tenantCode);
                }

                InvalidAssetException invalidAssetException = new InvalidAssetException(tenantCode);
                assets.ToList().ForEach(item =>
                {
                    try
                    {
                        item.IsValid();
                    }
                    catch (Exception ex)
                    {
                        invalidAssetException.Data.Add(item.Name, ex.Data);
                    }
                });

                if (invalidAssetException.Data.Count > 0)
                {
                    throw invalidAssetException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// This method helps to check duplicate asset  in the list
        /// </summary>
        /// <param name="assets"></param>
        /// <param name="tenantCode"></param>
        private void IsDuplicateAssets(IList<Asset> assets, string tenantCode)
        {
            try
            {
                int isDuplicateAsset = assets.GroupBy(p => p.Name).Where(g => g.Count() > 1).Count();
                if (isDuplicateAsset > 0)
                {
                    throw new DuplicateAssetException(tenantCode);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// This method is responsible for validate asset s.
        /// </summary>
        /// <param name="assetPathSetting"></param>
        /// <param name="tenantCode"></param>
        private void IsValidAssetPath(AssetPathSetting assetPathSetting, string tenantCode)
        {
            try
            {
                InvalidAssetPathException invalidAssetPathException = new InvalidAssetPathException(tenantCode);
                try
                {
                    assetPathSetting.IsValid();
                }
                catch (Exception ex)
                {
                    invalidAssetPathException.Data.Add(assetPathSetting.AssetPath, ex.Data);
                }

                if (invalidAssetPathException.Data.Count > 0)
                {
                    throw invalidAssetPathException;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion
    }
}
